// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

using System.IO;
using System.Text;
using System.Collections.Generic;

using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Collecting event and context counts by making two passes over the events. The
    /// first pass determines which contexts will be used by the model, and the 
    /// second pass creates the events in memory containing only the contexts which
    /// will be used. This greatly reduces the amount of memory required for storing
    /// the events. During the first pass a temporary event file is created which
    /// is read during the second pass.
    /// </summary>
    public class TwoPassDataIndexer : AbstractDataIndexer {

        protected readonly IObjectStream<Event> eventStream;
        protected readonly int cutoff;
        protected readonly bool sort;

        /// <summary>
        /// One argument constructor for DataIndexer which calls the two argument constructor assuming no cutoff.
        /// </summary>
        /// <param name="eventStream">An event stream which contains the a list of all the Events seen in the training data.</param>
        public TwoPassDataIndexer(IObjectStream<Event> eventStream) : this(eventStream, 0) { }

        /// <summary>
        /// Two argument constructor for DataIndexer.
        /// </summary>
        /// <param name="eventStream">An event stream which contains the a list of all the Events seen in the training data.</param>
        /// <param name="cutoff">The minimum number of times a predicate must have been observed in order to be included in the model.</param>
        public TwoPassDataIndexer(IObjectStream<Event> eventStream, int cutoff) : this(null, eventStream, cutoff, true) { }

        /// <summary>
        /// Two argument constructor for DataIndexer.
        /// </summary>
        /// <param name="eventStream">An event stream which contains the a list of all the Events seen in the training data.</param>
        /// <param name="cutoff">The minimum number of times a predicate must have been observed in order to be included in the model.</param>
        /// <param name="sort">if set to <c>true</c> [sort].</param>
        public TwoPassDataIndexer(IObjectStream<Event> eventStream, int cutoff, bool sort)
            : this(null, eventStream, cutoff, sort) {
            
        }

        /// <summary>
        /// Two argument constructor for DataIndexer.
        /// </summary>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the evaluation messages or it can cancel the indexing operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <param name="eventStream">An event stream which contains the a list of all the Events seen in the training data.</param>
        /// <param name="cutoff">The minimum number of times a predicate must have been observed in order to be included in the model.</param>
        /// <param name="sort">if set to <c>true</c> [sort].</param>
        public TwoPassDataIndexer(Monitor monitor, IObjectStream<Event> eventStream, int cutoff, bool sort) : base(monitor) {
            this.eventStream = eventStream;
            this.cutoff = cutoff;
            this.sort = sort;           
        }

        protected override void PerformIndexing() {

            Display("Indexing events using cutoff of " + cutoff);
            Display("\tComputing event counts...");

            var fileName = Path.GetTempFileName();
            var predicateIndex = new Dictionary<string, int>();

            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.DeleteOnClose)) {

                var writer = new StreamWriter(new UnclosableStream(file), Encoding.UTF8);
                int numEvents = ComputeEventCounts(writer, predicateIndex);

                Display("done. " + numEvents + " events");

                file.Seek(0, SeekOrigin.Begin);

                Display("\tIndexing...");
                var fes = new FileEventStream(file);

                var eventsToCompare = Index(fes, predicateIndex);

                file.Close();

                Display("done.");

                Display(sort ? "Sorting and merging events..." : "Collecting events...");

                SortAndMerge(eventsToCompare, sort);

                Display("Done indexing.");
            }
        }

        #region . ComputeEventCounts .

        /// <summary>
        /// Reads events into a linked list. The predicates associated with each event are 
        /// counted and any which occur at least N times (cutoff) are added to the
        /// <paramref name="predicatesInOut"/> map along with a unique integer index.
        /// </summary>
        /// <param name="eventStore">A writer to which the events are written to for later processing.</param>
        /// <param name="predicatesInOut">The predicates in out.</param>
        /// <returns>The number of events readed from the event stream.</returns>
        private int ComputeEventCounts(StreamWriter eventStore, Dictionary<string, int> predicatesInOut) {

            int eventCount = 0;
            var counter = new Dictionary<string, int>();
            //var predicateSet = new Java.HashSet<string>();

            var predicateSet = new HashSet<string>();

            Event ev;
            while ((ev = eventStream.Read()) != null) {

                if (Monitor != null && Monitor.Token.CanBeCanceled)
                    Monitor.Token.ThrowIfCancellationRequested();

                eventCount++;

                eventStore.Write(FileEventStream.ToLine(ev));

                Update(ev.Context, predicateSet, counter, cutoff);
            }

            predCounts = new int[predicateSet.Count];
            int index = 0;
            for (var e = predicateSet.GetEnumerator(); e.MoveNext(); index++) {
                if (e.Current != null) {
                    predCounts[index] = counter[e.Current];
                    predicatesInOut.Add(e.Current, index);
                } 
            }
            eventStore.Flush();
            eventStore.Close();

            return eventCount;
        }

        #endregion

        #region . Index .

        private List<ComparableEvent> Index(
            IObjectStream<Event> indexEventStream,
            Dictionary<string, int> predicateIndex) {

            var map = new Dictionary<string, int>();
            var indexedContext = new List<int>();
            var eventsToCompare = new List<ComparableEvent>();
            int outcomeCount = 0;

            Event ev;
            while ((ev = indexEventStream.Read()) != null) {
                int ocID;

                if (Monitor != null && Monitor.Token.CanBeCanceled)
                    Monitor.Token.ThrowIfCancellationRequested();

                if (map.ContainsKey(ev.Outcome)) {
                    ocID = map[ev.Outcome];
                } else {
                    ocID = outcomeCount++;
                    map[ev.Outcome] = ocID;
                }

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var pred in ev.Context) {
                    if (predicateIndex.ContainsKey(pred)) {
                        indexedContext.Add(predicateIndex[pred]);
                    }
                }

                // drop events with no active features
                if (indexedContext.Count > 0) {
                    var cons = new int[indexedContext.Count];
                    for (int ci = 0; ci < cons.Length; ci++) {
                        cons[ci] = indexedContext[ci];
                    }
                    eventsToCompare.Add(new ComparableEvent(ocID, cons));
                } else {
                    if (Monitor != null)
                        Monitor.OnWarning(string.Format("Dropped event {0}:{1}", ev.Outcome, ev.Context.ToDisplay()));
                }
                indexedContext.Clear();
            }

            outcomeLabels = ToIndexedStringArray(map);
            predLabels = ToIndexedStringArray(predicateIndex);
            return eventsToCompare;
        }
        #endregion

    }
}