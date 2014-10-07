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
using SharpNL.ML.MaxEntropy.QuasiNewton;
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.IO {
    public class QNModelReader : AbstractModelReader {
        public QNModelReader(IDataReader reader) : base(reader) {}

        protected override void CheckModelType() {
            var modelType = ReadString();

            if (!modelType.Equals("QN"))
                throw new InvalidDataException("Invalid model type.");
        }

        internal override AbstractModel ConstructModel() {
            var outcomeLabels = GetOutcomes();
            var outcomePatterns = GetOutcomePatterns();
            var predLabels = GetPredicates();
            var parameters = GetParameters(outcomePatterns);

            return new QNModel(parameters, predLabels, outcomeLabels);
        }
    }
}