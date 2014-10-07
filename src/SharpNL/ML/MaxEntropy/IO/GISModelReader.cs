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
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.IO {
    /// <summary>
    /// Generalized Iterative Scaling model reader.
    /// </summary>
    public class GISModelReader : AbstractModelReader {
        public GISModelReader(IDataReader reader) : base(reader) {}

        protected override void CheckModelType() {
            var modelType = ReadString();

            if (!modelType.Equals("GIS"))
                throw new InvalidDataException("Invalid model type.");
        }

        /// <summary>
        /// Constructs the model.
        /// <para>
        ///  GIS (model type identifier)
        ///   1. the correction constant (int)
        ///   2. the correction constant parameter (double)
        ///   3. # of outcomes (int)
        ///      list of outcome names (string) 
        ///   4. # of different types of outcome patterns (int)
        ///      list of (int int[])
        ///      [# of predicates for which outcome pattern is true] [outcome pattern]
        ///   5. # of predicates (int)
        ///      list of predicate names (String)
        /// </para>
        /// </summary>
        /// <returns><see cref="GISModel"/></returns>
        internal override AbstractModel ConstructModel() {
            var correctionConstant = ReadInt();
            var correctionParam = ReadDouble();
            var outcomeLabels = GetOutcomes();
            var outcomePatterns = GetOutcomePatterns();
            var predLabels = GetPredicates();
            var parameters = GetParameters(outcomePatterns);

            return new GISModel(parameters, predLabels, outcomeLabels, correctionConstant, correctionParam);
        }
    }
}