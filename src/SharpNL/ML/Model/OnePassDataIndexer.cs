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

using System.Collections.Generic;
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// An indexer for maxent model data which handles cutoffs for uncommon contextual 
    /// predicates and provides a unique integer index for each of the predicates.
    /// </summary>
    public class OnePassDataIndexer : AbstractDataIndexer {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePassDataIndexer"/> class using a event stream and a cutoff value.
        /// </summary>
        /// <param name="eventStream">The event stream.</param>
        /// <param name="cutoff">The cutoff.</param>
        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff) 
            : this(eventStream, cutoff, true) {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="OnePassDataIndexer"/> class, using a event stream, a cutoff value and a evaluation <see cref="Monitor"/>.
        /// </summary>
        /// <param name="eventStream">The event stream.</param>
        /// <param name="cutoff">The cutoff value.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff, Monitor monitor)
            : this(eventStream, cutoff, true, monitor) {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePassDataIndexer"/> class, using a event stream, a cutoff value and a value that indicates if the events should be sorted.
        /// </summary>
        /// <param name="eventStream">The event stream.</param>
        /// <param name="cutoff">The cutoff.</param>
        /// <param name="sort">if set to <c>true</c> the events will be sorted during the indexing.</param>
        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff, bool sort)
            : this(eventStream, cutoff, sort, null) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePassDataIndexer"/> class, using a event stream, a cutoff value and a value that indicates if the events should be sorted.
        /// </summary>
        /// <param name="eventStream">The event stream.</param>
        /// <param name="cutoff">The cutoff.</param>
        /// <param name="sort">if set to <c>true</c> the events will be sorted during the indexing.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        public OnePassDataIndexer(IObjectStream<Event> eventStream, int cutoff, bool sort, Monitor monitor)
            : base(monitor) {

            EventStream = eventStream;
            Cutoff = cutoff;
            Sort = sort;
        }

        #endregion

        #region + Properties .

        #region . Cutoff .
        /// <summary>
        /// Gets the cutoff value.
        /// </summary>
        /// <value>The cutoff value.</value>
        protected int Cutoff { get; private set; }
        #endregion

        #region . EventStream .
        /// <summary>
        /// Gets the event stream.
        /// </summary>
        /// <value>The event stream.</value>
        protected IObjectStream<Event> EventStream { get; private set; }
        #endregion

        #region . Sort .
        /// <summary>
        /// Gets a value indicating whether data should be sorted.
        /// </summary>
        /// <value><c>true</c> if data should be sorted; otherwise, <c>false</c>.</value>
        protected bool Sort { get; private set; }
        #endregion

        #endregion

        #region . PerformIndexing .
        /// <summary>
        /// Performs the data indexing.
        /// </summary>
        protected override void PerformIndexing() {

            var predicateIndex = new Dictionary<string, int>();

            Display("Indexing events using cutoff of " + Cutoff);
            Display("\tComputing event counts...");

            var events = ComputeEventCounts(predicateIndex);

            Display("done. " + events.Count + " events");

            if (Monitor != null && Monitor.Token.CanBeCanceled)
                Monitor.Token.ThrowIfCancellationRequested();

            Display("\tIndexing...");

            var eventsToCompare = Index(events, predicateIndex);

            events.Clear();
            predicateIndex.Clear();

            Display("done.");

            Display("Sorting and merging events...");

            SortAndMerge(eventsToCompare, Sort);

            Display("Done indexing.");
        }

        #endregion

        #region . ComputeEventCounts .

        /// <summary>
        /// Reads events into a linked list. The predicates associated with each event are
        /// counted and any which occur at least cutoff times are added to the 
        /// <see cref="predicatesInOut"/> map along with a unique integer index.
        /// </summary>
        /// <param name="predicatesInOut">The predicates.</param>
        /// <returns>The events</returns>
        private LinkedList<Event> ComputeEventCounts(Dictionary<string, int> predicatesInOut) {

            var predicateSet = new HashSet<string>();
            var counter = new Dictionary<string, int>();
            var events = new LinkedList<Event>();

            Event ev;
            while ((ev = EventStream.Read()) != null) {

                if (Monitor != null && Monitor.Token.CanBeCanceled)
                    Monitor.Token.ThrowIfCancellationRequested();

                events.AddLast(ev);
                Update(ev.Context, predicateSet, counter, Cutoff);
            }
            predCounts = new int[predicateSet.Count];

            int index = 0;
            for (var e = predicateSet.GetEnumerator(); e.MoveNext(); index++) {
                if (e.Current != null) {
                    predCounts[index] = counter[e.Current];
                    predicatesInOut[e.Current] = index;
                }
            }

            return events;
        }

        #endregion

        #region . Index .
        /// <summary>
        /// Indexes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <param name="predicateIndex">Index of the predicate.</param>
        /// <returns>List&lt;ComparableEvent&gt;.</returns>
        protected virtual List<ComparableEvent> Index(LinkedList<Event> events, Dictionary<string, int> predicateIndex) {
            var map = new Dictionary<string, int>();

            var numEvents = events.Count;
            var outcomeCount = 0;
           
            var eventsToCompare = new List<ComparableEvent>();
            var indexedContext = new List<int>();

            for (var eventIndex = 0; eventIndex < numEvents; eventIndex++) {
                var ev = events.First.Value;
                events.RemoveFirst();

                if (Monitor != null && Monitor.Token.CanBeCanceled)
                    Monitor.Token.ThrowIfCancellationRequested();

                int ocID;

                if (map.ContainsKey(ev.Outcome)) {
                    ocID = map[ev.Outcome];
                } else {
                    ocID = outcomeCount++;
                    map[ev.Outcome] = ocID;
                }

                foreach (var pred in ev.Context) {
                    if (predicateIndex.ContainsKey(pred)) {
                        indexedContext.Add(predicateIndex[pred]);
                    }
                }

                // drop events with no active features
                if (indexedContext.Count > 0) {
                    eventsToCompare.Add(new ComparableEvent(ocID, indexedContext.ToArray()));
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