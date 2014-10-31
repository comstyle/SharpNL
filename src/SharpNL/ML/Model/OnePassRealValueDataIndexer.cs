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

using System;
using System.Collections.Generic;
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// An indexer for maxent model data which handles cutoffs for uncommon
    /// contextual predicates and provides a unique integer index for each of the
    /// predicates and maintains event values.
    /// </summary>
    public class OnePassRealValueDataIndexer : OnePassDataIndexer {
        public OnePassRealValueDataIndexer(IObjectStream<Event> eventStream, int cutoff)
            : base(eventStream, cutoff) {
            
        }

        public OnePassRealValueDataIndexer(IObjectStream<Event> eventStream, int cutoff, bool sort)
            : base(eventStream, cutoff, sort) {
            
        }

        #region . Index .

        protected override List<ComparableEvent> Index(LinkedList<Event> events, Dictionary<string, int> predicateIndex) {
            var map = new Dictionary<string, int>();

            var numEvents = events.Count;
            var outcomeCount = 0;
            var eventsToCompare = new List<ComparableEvent>(numEvents);
            var indexedContext = new List<int>();

            for (var eventIndex = 0; eventIndex < numEvents; eventIndex++) {
                var ev = events.First.Value;
                var ec = ev.Context;

                events.RemoveFirst();

                int ocID;
                var oc = ev.Outcome;

                if (map.ContainsKey(oc)) {
                    ocID = map[oc];
                } else {
                    ocID = outcomeCount++;
                    map[oc] = ocID;
                }

                foreach (var pred in ec) {
                    if (predicateIndex.ContainsKey(pred)) {
                        indexedContext.Add(predicateIndex[pred]);
                    }
                }

                //drop events with no active features
                if (indexedContext.Count > 0) {
                    var cons = new int[indexedContext.Count];
                    for (var ci = 0; ci < cons.Length; ci++) {
                        cons[ci] = indexedContext[ci];
                    }
                    eventsToCompare.Add(new ComparableEvent(ocID, cons, ev.Values));
                } else {
                    Display("Dropped event " + ev.Outcome + ":" + ev.Context.ToDisplay());
                }

                // indexedContext.Clear();
                // Knuppe: its more fast to recreate the object instead cleaning it.
                //indexedContext = new List<int>();
                indexedContext.Clear();
            }
            outcomeLabels = ToIndexedStringArray(map);
            predLabels = ToIndexedStringArray(predicateIndex);
            return eventsToCompare;
        }

        #endregion

        #region . SortAndMerge .
        protected override int SortAndMerge(List<ComparableEvent> eventsToCompare, bool sort) {
            var numUniqueEvents = base.SortAndMerge(eventsToCompare, sort);
            Values = new float[numUniqueEvents][];
            var numEvents = eventsToCompare.Count;
            for (int i = 0, j = 0; i < numEvents; i++) {
                var evt = eventsToCompare[i];
                if (null == evt) {
                    continue; // this was a dupe, skip over it.
                }
                Values[j++] = evt.values;
            }
            return numUniqueEvents;
        }

        #endregion

    }
}