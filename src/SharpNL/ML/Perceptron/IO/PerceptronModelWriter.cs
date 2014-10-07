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


namespace SharpNL.ML.Perceptron.IO {
    using Model;
    /// <summary>
    /// Abstract parent class for Perceptron writers. It provides the persist method
    /// which takes care of the structure of a stored document, and requires an
    /// extending class to define precisely how the data should be stored.
    /// </summary>
    public abstract class PerceptronModelWriter : AbstractModelWriter {
        protected readonly Context[] PARAMS;
        protected readonly string[] OUTCOME_LABELS;
        protected readonly string[] PRED_LABELS;

        private readonly int numOutcomes;

        protected PerceptronModelWriter(AbstractModel model) {
            var data = model.GetDataStructures();
            numOutcomes = model.GetNumOutcomes();
            PARAMS = (Context[])data[0];
            OUTCOME_LABELS = (string[])data[2];

            var map = (IndexHashTable<string>)data[1];
            PRED_LABELS = new string[map.Size];
            map.ToArray(PRED_LABELS);
        }

        #region . ComputeOutcomePatterns .
        protected List<List<ComparablePredicate>> ComputeOutcomePatterns(ComparablePredicate[] sorted) {
            ComparablePredicate cp = sorted[0];
            var newGroup = new List<ComparablePredicate>();
            var outcomePatterns = new List<List<ComparablePredicate>>();

            foreach (var predicate in sorted) {
                if (cp.CompareTo(predicate) == 0) {
                    newGroup.Add(predicate);
                } else {
                    cp = predicate;
                    outcomePatterns.Add(newGroup);
                    newGroup = new List<ComparablePredicate> { predicate };
                }
            }

            outcomePatterns.Add(newGroup);
#if DEBUG
            Debug.Print(outcomePatterns.Count + " outcome patterns.");
#endif
            return outcomePatterns;
        }
        #endregion

        #region . SortValues .

        protected ComparablePredicate[] SortValues() {
            var tmpPreds = new ComparablePredicate[PARAMS.Length];
            var tmpOutcomes = new int[numOutcomes];
            var tmpParams = new double[numOutcomes];
            int numPreds = 0;

            //remove parameters with 0 weight and predicates with no parameters
            for (int pid = 0; pid < PARAMS.Length; pid++) {
                var numParams = 0;
                var predParams = PARAMS[pid].Parameters;
                var outcomePattern = PARAMS[pid].Outcomes;
                for (int pi = 0; pi < predParams.Length; pi++) {
                    if (!predParams[pi].Equals(0d)) {
                        tmpOutcomes[numParams] = outcomePattern[pi];
                        tmpParams[numParams] = predParams[pi];
                        numParams++;
                    }
                }

                var activeOutcomes = new int[numParams];
                var activeParams = new double[numParams];

                for (int pi = 0; pi < numParams; pi++) {
                    activeOutcomes[pi] = tmpOutcomes[pi];
                    activeParams[pi] = tmpParams[pi];
                }

                if (numParams != 0) {
                    tmpPreds[numPreds] = new ComparablePredicate(PRED_LABELS[pid], activeOutcomes, activeParams);
                    numPreds++;
                }
            }

#if DEBUG
            Debug.Print("Compressed " + PARAMS.Length + " parameters to " + numPreds);
#endif  

            var sortPreds = new ComparablePredicate[numPreds];
            Array.Copy(tmpPreds, 0, sortPreds, 0, numPreds);
            Array.Sort(sortPreds);
            return sortPreds;
        }

        #endregion

        public override void Persist() {

            // the type of model (Perceptron)
            Write("Perceptron");

            // the mapping from outcomes to their integer indexes
            Write(OUTCOME_LABELS.Length);

            foreach (string label in OUTCOME_LABELS) {
                Write(label);
            }

            // the mapping from predicates to the outcomes they contributed to.
            // The sorting is done so that we actually can write this out more
            // compactly than as the entire list.
            var sorted = SortValues();
            var compressed = ComputeOutcomePatterns(sorted);

            Write(compressed.Count);

            foreach (var item in compressed) {
                Write(item.Count + item[0].ToString());
            }

            // the mapping from predicate names to their integer indexes
            Write(sorted.Length);

            foreach (var item in sorted) {
                Write(item.Name);
            }

            // write out the parameters
            foreach (var pred in sorted)
                foreach (var value in pred.Parameters)
                    Write(value);

            Close();
        }
    }
}