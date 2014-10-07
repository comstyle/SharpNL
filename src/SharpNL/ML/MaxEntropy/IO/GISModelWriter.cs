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
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.IO {
    /// <summary>
    /// Generalized Iterative Scaling model writer.
    /// </summary>
    public abstract class GISModelWriter : AbstractModelWriter {
        protected int CORRECTION_CONSTANT;
        protected double CORRECTION_PARAM;
        protected string[] OUTCOME_LABELS;
        protected Context[] PARAMS;
        protected string[] PRED_LABELS;

        /// <para>
        ///   <c>index 0</c>: <see cref="T:Context[]"/> containing the model parameters.
        ///   <c>index 1</c>: <see cref="T:IndexHashTable{string}"/> containing the mapping of model predicates to unique integers.
        ///   <c>index 2</c>: <see cref="T:string[]"/> containing the names of the outcomes, stored in the index of the array which represents their unique ids in the model.
        ///   <c>index 3</c>: <see cref="T:double"/> containing the value of the models correction constant.
        ///   <c>index 4</c>: <see cref="T:double"/> containing the value of the models correction parameter.
        /// </para>
        /// 
        protected GISModelWriter(AbstractModel model) {
            var data = model.GetDataStructures();

            PARAMS = (Context[]) data[0];
            var map = (IndexHashTable<string>) data[1];
            OUTCOME_LABELS = (string[]) data[2];
            CORRECTION_CONSTANT = (int) data[3]; // string[] ??????????
            CORRECTION_PARAM = (Double) data[4];

            PRED_LABELS = new String[map.Size];
            map.ToArray(PRED_LABELS);
        }

        #region . CompressOutcomes .

        protected List<List<ComparablePredicate>> CompressOutcomes(ComparablePredicate[] sorted) {
            var cp = sorted[0];
            var outcomePatterns = new List<List<ComparablePredicate>>();
            var newGroup = new List<ComparablePredicate>();
            foreach (var t in sorted) {
                if (cp.CompareTo(t) == 0) {
                    newGroup.Add(t);
                } else {
                    cp = t;
                    outcomePatterns.Add(newGroup);
                    newGroup = new List<ComparablePredicate> {t};
                }
            }
            outcomePatterns.Add(newGroup);
            return outcomePatterns;
        }

        #endregion


        public override void Persist() {
            // the type of model (GIS)
            Write("GIS");

            // the value of the correction constant
            Write(CORRECTION_CONSTANT);

            // the value of the correction constant
            Write(CORRECTION_PARAM);

            // the mapping from outcomes to their integer indexes
            Write(OUTCOME_LABELS.Length);

            for (var i = 0; i < OUTCOME_LABELS.Length; i++)
                Write(OUTCOME_LABELS[i]);

            // the mapping from predicates to the outcomes they contributed to.
            // The sorting is done so that we actually can write this out more
            // compactly than as the entire list.
            var sorted = SortValues();
            var compressed = CompressOutcomes(sorted);

            Write(compressed.Count);

            for (var i = 0; i < compressed.Count; i++) {
                var a = compressed[i];
                Write(a.Count + a[0].ToString());
            }

            // the mapping from predicate names to their integer indexes
            Write(PARAMS.Length);

            for (var i = 0; i < sorted.Length; i++)
                Write(sorted[i].Name);

            // write out the parameters
            for (var i = 0; i < sorted.Length; i++)
                for (var j = 0; j < sorted[i].Parameters.Length; j++)
                    Write(sorted[i].Parameters[j]);

            Close();
        }

        #region . SortValues .

        protected ComparablePredicate[] SortValues() {
            var sortPreds = new ComparablePredicate[PARAMS.Length];
            for (var pid = 0; pid < PARAMS.Length; pid++) {
                sortPreds[pid] = new ComparablePredicate(
                    PRED_LABELS[pid],
                    PARAMS[pid].Outcomes,
                    PARAMS[pid].Parameters);
            }

            Array.Sort(sortPreds);
            return sortPreds;
        }

        #endregion
    }
}