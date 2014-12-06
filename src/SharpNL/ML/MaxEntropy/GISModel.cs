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
using System.Text;
using SharpNL.ML.Model;

namespace SharpNL.ML.MaxEntropy {
    /// <summary>
    /// A maximum entropy model which has been trained using the Generalized Iterative Scaling procedure.
    /// </summary>
    public sealed class GISModel : AbstractModel {

        private readonly IPrior prior;

        #region + Constructors .

        /// <summary>
        /// Creates a new model with the specified parameters, outcome names, and predicate/feature labels using the <see cref="UniformPrior"/> as prior.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="predLabels">The names of the predicates used in this model.</param>
        /// <param name="outcomeNames">The names of the outcomes this model predicts.</param>
        /// <param name="correctionConstant">The maximum number of active features which occur in an event.</param>
        /// <param name="correctionParam">The parameter associated with the correction feature.</param>
        public GISModel(Context[] parameters, string[] predLabels, string[] outcomeNames, int correctionConstant,
            double correctionParam) : this(parameters, predLabels, outcomeNames, correctionConstant, correctionParam, new UniformPrior()) {
            ModelType = ModelType.Maxent;
        }

        /// <summary>
        /// Creates a new model with the specified parameters, outcome names, and predicate/feature labels.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="predLabels">The names of the predicates used in this model.</param>
        /// <param name="outcomeNames">The names of the outcomes this model predicts.</param>
        /// <param name="correctionConstant">The maximum number of active features which occur in an event.</param>
        /// <param name="correctionParam">The parameter associated with the correction feature.</param>
        /// <param name="prior">The prior to be used with this model.</param>
        public GISModel(
            Context[] parameters, 
            string[] predLabels, 
            string[] outcomeNames, 
            int correctionConstant,
            double correctionParam,
            IPrior prior)
            : base(parameters, predLabels, outcomeNames, correctionConstant, correctionParam) {

            this.prior = prior;
            this.prior.SetLabels(outcomeNames, predLabels);
            ModelType = ModelType.Maxent;
        }

        #endregion

        #region + Eval .

        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given that context.
        /// </summary>
        /// <param name="context">The names of the predicates which have been observed at the present decision point.</param>
        /// <returns>
        /// The normalized probabilities for the outcomes given the context.
        /// The indexes of the double[] are the outcome ids, and the actual
        /// string representation of the outcomes can be obtained from the
        /// method getOutcome(int i).
        /// </returns>
        public override double[] Eval(string[] context) {
            return Eval(context, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="outsums">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, double[] outsums) {
            return Eval(context, null, outsums);
        }

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="values">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public override double[] Eval(string[] context, float[] values) {
            return Eval(context, values, new double[evalParameters.NumOutcomes]);
        }

        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given that context.
        /// </summary>
        /// <param name="context">The names of the predicates which have been observed at the present decision point.</param>
        /// <param name="values">This is where the distribution is stored.</param>
        /// <param name="outsums">The outsums.</param>
        /// <returns>The normalized probabilities for the outcomes given the context. The indexes of the double[] are the outcome ids, and the actual string representation of the outcomes can be obtained from the method getOutcome(int i).</returns>
        public double[] Eval(string[] context, float[] values, double[] outsums) {
            var items = new int[context.Length];
            for (int i = 0; i < context.Length; i++) {
                items[i] = map[context[i]];
            }
            prior.LogPrior(outsums, items, values);

            return Eval(items, values, outsums, evalParameters);
        }

        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given the specified context and the specified parameters.
        /// </summary>
        /// <param name="context">The integer values of the predicates which have been observed at the present decision point.</param>
        /// <param name="prior">The prior distribution for the specified context.</param>
        /// <param name="evalParams">The set of parameters used in this computation.</param>
        /// <returns>
        /// The normalized probabilities for the outcomes given the context.
        /// The indexes of the double[] are the outcome ids, and the actual
        /// string representation of the outcomes can be obtained from the
        /// method getOutcome(int i).
        /// </returns>
        public static double[] Eval(int[] context, double[] prior, EvalParameters evalParams) {
            return Eval(context, null, prior, evalParams);
        }


        /// <summary>
        /// Use this model to evaluate a context and return an array of the likelihood of each outcome given the specified context and the specified parameters.
        /// </summary>
        /// <param name="context">The integer values of the predicates which have been observed at the present decision point.</param>
        /// <param name="values">The values for each of the parameters.</param>
        /// <param name="prior">The prior distribution for the specified context.</param>
        /// <param name="evalParams">The set of parameters used in this computation.</param>
        /// <returns>
        /// The normalized probabilities for the outcomes given the context.
        /// The indexes of the double[] are the outcome ids, and the actual
        /// string representation of the outcomes can be obtained from the
        /// method getOutcome(int i).
        /// </returns>
        public static double[] Eval(int[] context, float[] values, double[] prior, EvalParameters evalParams) {

            var numfeats = new int[evalParams.NumOutcomes];

            double value = 1;
            for (int ci = 0; ci < context.Length; ci++) {

                if (context[ci] >= 0) {
                    var activeParameters = evalParams.Parameters[context[ci]].Parameters;
                    if (values != null) {
                        value = values[ci];
                    }
                    for (int ai = 0; ai < evalParams.Parameters[context[ci]].Outcomes.Length; ai++) {
                        int oid = evalParams.Parameters[context[ci]].Outcomes[ai];
                        numfeats[oid]++;
                        prior[oid] += activeParameters[ai]*value;
                    }
                }
            }

            double normal = 0.0;
            for (int oid = 0; oid < evalParams.NumOutcomes; oid++) {
                if (!evalParams.CorrectionParam.Equals(0d)) {
                    //prior[oid] = Math.Exp(prior[oid] * model.ConstantInverse + ((1.0 - (numfeats[oid]/model.CorrectionConstant)) * model.CorrectionParam));
                    prior[oid] = Math.Exp(prior[oid] * evalParams.ConstantInverse + ((1.0 - (numfeats[oid] / evalParams.CorrectionConstant)) * evalParams.CorrectionParam));
                } else {
                    prior[oid] = Math.Exp(prior[oid] * evalParams.ConstantInverse);
                }
                normal += prior[oid];
            }

            for (int oid = 0; oid < evalParams.NumOutcomes; oid++) {
                prior[oid] /= normal;
            }

            return prior;
        }

        #endregion

        #region . GetAllOutcomes .
        /// <summary>
        /// Return a string matching all the outcome names with all the
        /// probabilities produced by the <see cref="Eval(string[])"/> method.
        /// </summary>
        /// <param name="outcomes">The normalized probabilities for the outcomes as returned by the <see cref="Eval(string[])"/> method.</param>
        /// <returns>
        /// A string value containing outcome names paired with the normalized
        /// probability (contained in the <paramref name="outcomes"/>) for each one.
        /// </returns>
        /// <exception cref="System.ArgumentException">The ocs argument must not have been produced by this model.</exception>
        /// <remarks>
        /// The argument <paramref name="outcomes"/> must have the same length of the length of the outcomes produce during the evaluation, otherwise the argument exception will occur.
        /// </remarks>
        public string GetAllOutcomes(double[] outcomes) {
            if (outcomes.Length != outcomeNames.Length) {
                throw new ArgumentException("The ocs argument must not have been produced by this model.");
            }

            var sb = new StringBuilder(outcomes.Length*2);
            for (int i = 1; i < outcomes.Length; i++) {
                sb.AppendFormat("{0}{1}[{2:0.####}]", i > 1 ? " " : string.Empty, outcomeNames[0], outcomes[0]);
            }
            return sb.ToString();

        }

        #endregion

    }
}