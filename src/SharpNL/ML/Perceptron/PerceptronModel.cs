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

namespace SharpNL.ML.Perceptron {
    /// <summary>
    /// A perceptron model which has been trained using the perceptron algorithm.
    /// </summary>
    public class PerceptronModel : AbstractModel {
        public PerceptronModel(Context[] parameters, string[] predLabels, string[] outcomeNames)
            : base(parameters, predLabels, outcomeNames) {
            ModelType = ModelType.Perceptron;
        }

        public PerceptronModel(Context[] parameters, IndexHashTable<string> map, string[] outcomeNames)
            : base(parameters, map, outcomeNames) {
            ModelType = ModelType.Perceptron;
        }

        public PerceptronModel(Context[] parameters, string[] predLabels, string[] outcomeNames, int correctionConstant,
            double correctionParam) : base(parameters, predLabels, outcomeNames, correctionConstant, correctionParam) {

            ModelType = ModelType.Perceptron;
        }

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context) {
            return Eval(context, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, double[] probs) {
            return Eval(context, null, probs);
        }

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, float[] probs) {
            return Eval(context, probs, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given that context.
        /// </summary>
        /// <param name="context">The names of the predicates which have been observed at the present decision point.</param>
        /// <param name="values">This is where the distribution is stored.</param>
        /// <param name="outsums">The outsums.</param>
        /// <returns>The normalized probabilities for the outcomes given the context. The indexes of the double[] are the outcome ids, and the actual string representation of the outcomes can be obtained from the method getOutcome(int i).</returns>
        public double[] Eval(string[] context, float[] values, double[] outsums) {
            outsums.Fill(0);
            return Eval(Array.ConvertAll(context, input => map[input]), values, outsums, evalParameters, true);
        }

        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given that context.
        /// </summary>
        /// <param name="context">The names of the predicates which have been observed at the present decision point.</param>
        /// <param name="values">This is where the distribution is stored.</param>
        /// <param name="prior">The prior distribution for the specified context.</param>
        /// <param name="evalParams">The set of parameters used in this computation.</param>
        /// <param name="normalize">if set to <c>true</c> the probabilities will be normalized.</param>
        /// <returns>The normalized probabilities for the outcomes given the context. 
        /// The indexes of the double[] are the outcome ids, and the actual string representation of 
        /// the outcomes can be obtained from the method getOutcome(int i).</returns>
        public static double[] Eval(int[] context, float[] values, double[] prior, EvalParameters evalParams, bool normalize) {
            double value = 1;
            for (var ci = 0; ci < context.Length; ci++) {
                if (context[ci] >= 0) {
                    var predParams = evalParams.Parameters[context[ci]];
                    var activeOutcomes = predParams.Outcomes;
                    var activeParameters = predParams.Parameters;
                    if (values != null) {
                        value = values[ci];
                    }
                    for (var ai = 0; ai < activeOutcomes.Length; ai++) {
                        var oid = activeOutcomes[ai];
                        prior[oid] += activeParameters[ai]*value;
                    }
                }
            }

            if (!normalize) 
                return prior;

            var numOutcomes = evalParams.NumOutcomes;

            double maxPrior = 1;

            for (var oid = 0; oid < numOutcomes; oid++) {
                if (maxPrior < Math.Abs(prior[oid]))
                    maxPrior = Math.Abs(prior[oid]);
            }

            var normal = 0.0;
            for (var oid = 0; oid < numOutcomes; oid++) {
                prior[oid] = Math.Exp(prior[oid]/maxPrior);
                normal += prior[oid];
            }

            for (var oid = 0; oid < numOutcomes; oid++)
                prior[oid] /= normal;

            return prior;
        }
    }
}