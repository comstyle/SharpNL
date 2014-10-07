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
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy.QuasiNewton {
    public class QNModel : AbstractModel {
        public QNModel(Context[] parameters, string[] predLabels, string[] outcomeNames)
            : base(parameters, predLabels, outcomeNames) {}

        public QNModel(Context[] parameters, IndexHashTable<string> map, string[] outcomeNames)
            : base(parameters, map, outcomeNames) {}

        public QNModel(Context[] parameters, string[] predLabels, string[] outcomeNames, int correctionConstant,
            double correctionParam) : base(parameters, predLabels, outcomeNames, correctionConstant, correctionParam) {}

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, double[] probs) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, float[] probs) {
            throw new NotImplementedException();
        }
    }
}