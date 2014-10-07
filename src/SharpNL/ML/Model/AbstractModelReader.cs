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

using SharpNL.Java;

namespace SharpNL.ML.Model {
    public abstract class AbstractModelReader {
        protected readonly IDataReader reader;
        protected int NUM_PREDS;

        protected AbstractModelReader(IDataReader reader) {
            this.reader = reader;
        }

        protected int ReadInt() {
            return reader.ReadInt();
        }

        protected double ReadDouble() {
            return reader.ReadDouble();
        }

        protected string ReadString() {
            return reader.ReadString();
        }

        public AbstractModel GetModel() {
            CheckModelType();
            return ConstructModel();
        }

        protected abstract void CheckModelType();

        internal abstract AbstractModel ConstructModel();

        protected string[] GetOutcomes() {
            var numOutcomes = ReadInt();
            var outcomeLabels = new string[numOutcomes];

            for (var i = 0; i < numOutcomes; i++)
                outcomeLabels[i] = ReadString();

            return outcomeLabels;
        }

        protected int[][] GetOutcomePatterns() {
            var numOCTypes = ReadInt();
            var outcomePatterns = new int[numOCTypes][];
            for (var i = 0; i < numOCTypes; i++) {
                var tok = new StringTokenizer(ReadString(), " ");
                var infoInts = new int[tok.CountTokens];
                for (var j = 0; tok.HasMoreTokens; j++) {
                    infoInts[j] = int.Parse(tok.NextToken);
                }
                outcomePatterns[i] = infoInts;
            }
            return outcomePatterns;
        }

        protected string[] GetPredicates() {
            NUM_PREDS = ReadInt();
            var predLabels = new string[NUM_PREDS];
            for (var i = 0; i < NUM_PREDS; i++)
                predLabels[i] = ReadString();

            return predLabels;
        }

        /// <summary>
        /// Reads the parameters from a file and populates an array of context objects.
        /// </summary>
        /// <param name="outcomePatterns">The outcomes patterns for the model. The first index refers to which
        /// outcome pattern (a set of outcomes that occurs with a context) is being specified. The
        /// second index specifies the number of contexts which use this pattern at index 0, and the
        /// index of each outcomes which make up this pattern in indices 1-n.</param>
        /// <returns>An array of context objects.</returns>
        protected Context[] GetParameters(int[][] outcomePatterns) {
            var par = new Context[NUM_PREDS];
            var pid = 0;
            for (var i = 0; i < outcomePatterns.Length; i++) {
                //construct outcome pattern
                var outcomePattern = new int[outcomePatterns[i].Length - 1];
                for (var k = 1; k < outcomePatterns[i].Length; k++) {
                    outcomePattern[k - 1] = outcomePatterns[i][k];
                }

                //populate parameters for each context which uses this outcome pattern.
                for (var j = 0; j < outcomePatterns[i][0]; j++) {
                    var contextParameters = new double[outcomePatterns[i].Length - 1];
                    for (var k = 1; k < outcomePatterns[i].Length; k++) {
                        contextParameters[k - 1] = ReadDouble();
                    }
                    par[pid] = new Context(outcomePattern, contextParameters);
                    pid++;
                }
            }
            return par;
        }
    }
}