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
using System.Diagnostics;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Abstract class for collecting event and context counts used in training.
    /// </summary>
    public abstract class AbstractDataIndexer : IDataIndexer {
        /** The integer contexts associated with each unique event. */
        protected int[][] contexts;
        private int numEvents;
        /** The integer outcome associated with each unique event. */
        /** The number of times an event occurred in the training data. */
        protected int[] numTimesEventsSeen;
        /** The predicate/context names. */
        /** The names of the outcomes. */
        protected string[] outcomeLabels;
        protected int[] outcomeList;
        /** The number of times each predicate occurred. */
        protected int[] predCounts;
        protected string[] predLabels;
        private bool indexed;

        #region + Properties .

        #region . Values .
        /// <summary>
        /// Gets the values associated with each event context or null if integer values are to be used.
        /// </summary>
        /// <returns>The values associated with each event context.</returns>
        public float[][] Values { get; protected set; }
        #endregion

        #endregion

        #region . Execute .
        /// <summary>
        /// Execute the data indexing.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The data indexing has already been performed.
        /// </exception>
        /// <remarks>
        /// Do NOT call this method in the constructor of the data indexer, otherwise it will not work in C#!
        /// </remarks>
        public void Execute() {
            if (indexed)
                throw new InvalidOperationException("The data indexing has already been performed.");

            try {
                PerformIndexing();
            } finally {
                indexed = true;
            }
        }
        #endregion

        #region . PerformIndexing .
        /// <summary>
        /// Performs the data indexing.
        /// </summary>
        protected abstract void PerformIndexing();
        #endregion

        #region . GetContexts .
        /// <summary>
        /// Gets the predicates seen in each event.
        /// </summary>
        /// <returns>A 2-D array whose first dimension is the event index and array this refers to contains.</returns>
        public int[][] GetContexts() {
            return contexts;
        }

        #endregion

        #region . GetNumTimesEventsSeen .

        /// <summary>
        /// Gets the number of times a particular event was seen.
        /// </summary>
        /// <returns>An array indexed by the event index indicating the number of times a particular event was seen.</returns>
        public int[] GetNumTimesEventsSeen() {
            return numTimesEventsSeen;
        }

        #endregion

        #region . GetOutcomeList .

        /// <summary>
        /// Gets the outcome index for each event.
        /// </summary>
        /// <returns>The outcome index for each event.</returns>
        public int[] GetOutcomeList() {
            return outcomeList;
        }

        #endregion

        #region . GetOutcomeLabels .

        /// <summary>
        /// Gets the outcome names.
        /// </summary>
        /// <returns>The outcome names indexed by outcome index.</returns>
        public string[] GetOutcomeLabels() {
            return outcomeLabels;
        }

        #endregion

        #region . GetPredLabels .

        /// <summary>
        /// Gets the predicate/context names.
        /// </summary>
        /// <returns>
        /// The predicate/context names indexed by context index.
        /// These indices are the value of the array returned by <see cref="M:GetContexts"/>.
        /// </returns>
        public string[] GetPredLabels() {
            return predLabels;
        }

        #endregion

        #region . GetPredCounts .

        /// <summary>
        /// Gets the count of each predicate in the events.
        /// </summary>
        /// <returns>The count of each predicate in the events.</returns>
        public int[] GetPredCounts() {
            return predCounts;
        }

        #endregion

        #region . GetValues .


        #endregion

        #region . GetNumEvents .

        /// <summary>
        /// Gets the number of total events indexed.
        /// </summary>
        /// <returns>The number of total events indexed.</returns>
        public int GetNumEvents() {
            return numEvents;
        }

        #endregion

        #region . SortAndMerge .

        protected virtual int SortAndMerge(List<ComparableEvent> eventsToCompare, bool sort) {
            var numUniqueEvents = 1;

            numEvents = eventsToCompare.Count;
            if (sort) {
                eventsToCompare.Sort();
                if (numEvents <= 1) {
                    return numUniqueEvents;
                }

                var ce = eventsToCompare[0];
                for (var i = 1; i < numEvents; i++) {
                    var ce2 = eventsToCompare[i];

                    if (ce.CompareTo(ce2) == 0) {
                        ce.seen++; // increment the seen count
                        eventsToCompare[i] = null; // kill the duplicate
                    } else {
                        ce = ce2; // a new champion emerges...
                        numUniqueEvents++; // increment the # of unique events
                    }
                }
            } else {
                numUniqueEvents = eventsToCompare.Count;
            }

            if (sort) {
                Debug.WriteLine("done. Reduced " + numEvents + " events to " + numUniqueEvents + ".");
            }

            contexts = new int[numUniqueEvents][];
            outcomeList = new int[numUniqueEvents];
            numTimesEventsSeen = new int[numUniqueEvents];

            for (int i = 0, j = 0; i < numEvents; i++) {
                var evt = eventsToCompare[i];
                if (evt == null) {
                    continue; // this was a dupe, skip over it.
                }
                numTimesEventsSeen[j] = evt.seen;
                outcomeList[j] = evt.outcome;
                contexts[j] = evt.predIndexes;
                ++j;
            }

            return numUniqueEvents;
        }

        #endregion

        #region . ToIndexedStringArray .

        /// <summary>
        /// Utility method for creating a string[] array from a map whose
        /// keys are labels (strings) to be stored in the array and whose
        /// values are the indices (integers) at which the corresponding
        /// labels should be inserted.
        /// </summary>
        /// <param name="labelToIndexMap">The label to index map.</param>
        /// <returns>System.String[].</returns>
        protected static string[] ToIndexedStringArray(Dictionary<string, int> labelToIndexMap) {
            var array = new string[labelToIndexMap.Count];
            foreach (var pair in labelToIndexMap) {
                array[pair.Value] = pair.Key;
            }
            return array;
        }

        #endregion

        #region . Update .

        /// <summary>
        /// Updates the set of predicated and counter with the specified event contexts and cutoff.
        /// </summary>
        /// <param name="ec">The contexts/features which occur in a event.</param>
        /// <param name="predicateSet">The set of predicates which will be used for model building.</param>
        /// <param name="counter">The predicate counters.</param>
        /// <param name="cutoff">The cutoff which determines whether a predicate is included.</param>
        protected static void Update(string[] ec, HashSet<string> predicateSet, Dictionary<string, int> counter, int cutoff) {
            foreach (var s in ec) {
                if (!counter.ContainsKey(s)) {
                    counter[s] = 1;
                } else {
                    counter[s]++;
                }
                if (!predicateSet.Contains(s) && counter[s] >= cutoff) {
                    predicateSet.Add(s);
                }
            }
        }

        #endregion
    }
}