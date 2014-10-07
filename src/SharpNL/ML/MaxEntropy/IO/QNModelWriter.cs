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

namespace SharpNL.ML.MaxEntropy.IO {
    using Model;
    public abstract class QNModelWriter : GISModelWriter {
        protected QNModelWriter(AbstractModel model) : base(model) {}

        public override void Persist() {
            // the type of model (QN)
            Write("QN");

            // the mapping from outcomes to their integer indexes
            Write(OUTCOME_LABELS.Length);

            foreach (var label in OUTCOME_LABELS)
                Write(label);

            // the mapping from predicates to the outcomes they contributed to.
            // The sorting is done so that we actually can write this out more
            // compactly than as the entire list.
            var sorted = SortValues();
            var compressed = CompressOutcomes(sorted);

            Write(compressed.Count);

            foreach (var a in compressed) {
                Write(a.Count + a[0].ToString());
            }

            // the mapping from predicate names to their integer indexes
            Write(PARAMS.Length);

            foreach (var p in sorted)
                Write(p.Name);

            // write out the parameters
            foreach (var pred in sorted)
                foreach (var par in pred.Parameters)
                    Write(par);

            Close();
        }
    }
}