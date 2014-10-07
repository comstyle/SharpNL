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

namespace SharpNL.ML.Model {
    /// <summary>
    /// Object which compresses events in memory and performs feature selection.
    /// </summary>
    public interface IDataIndexer {


        /// <summary>
        /// Execute the data indexing.
        /// </summary>
        void Execute();

        /// <summary>
        /// Gets the predicates seen in each event.
        /// </summary>
        /// <returns>A 2-D array whose first dimension is the event index and array this refers to contains.</returns>
        int[][] GetContexts();

        /// <summary>
        /// Gets the number of times a particular event was seen.
        /// </summary>
        /// <returns>An array indexed by the event index indicating the number of times a particular event was seen.</returns>
        int[] GetNumTimesEventsSeen();

        /// <summary>
        /// Gets the outcome index for each event.
        /// </summary>
        /// <returns>The outcome index for each event.</returns>
        int[] GetOutcomeList();

        /// <summary>
        /// Gets the outcome names.
        /// </summary>
        /// <returns>The outcome names indexed by outcome index.</returns>
        string[] GetOutcomeLabels();

        /// <summary>
        /// Gets the predicate/context names.
        /// </summary>
        /// <returns>
        /// The predicate/context names indexed by context index.
        /// These indices are the value of the array returned by <see cref="M:GetContexts"/>.
        /// </returns>
        string[] GetPredLabels();

        /// <summary>
        /// Gets the count of each predicate in the events.
        /// </summary>
        /// <returns>The count of each predicate in the events.</returns>
        int[] GetPredCounts();

        /// <summary>
        /// Gets the values associated with each event context or null if integer values are to be used.
        /// </summary>
        /// <returns>The values associated with each event context.</returns>
        float[][] Values { get; }

        /// <summary>
        /// Gets the number of total events indexed.
        /// </summary>
        /// <returns>The number of total events indexed.</returns>
        int GetNumEvents();
    }
}