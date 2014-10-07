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
using System.Linq;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a abstract model.
    /// </summary>
    public abstract class AbstractModel : IMaxentModel {
        protected readonly EvalParameters evalParameters;
        protected readonly IndexHashTable<string> map;

        protected readonly string[] outcomeNames;

        #region + Constructors .

        private AbstractModel(string[] predLabels, string[] outcomeNames) {
            map = new IndexHashTable<string>(predLabels, 0.7d);

            this.outcomeNames = outcomeNames;
        }

        protected AbstractModel(Context[] parameters, string[] predLabels, string[] outcomeNames)
            : this(predLabels, outcomeNames) {
            evalParameters = new EvalParameters(parameters, outcomeNames.Length);
        }

        protected AbstractModel(Context[] parameters, IndexHashTable<string> map, string[] outcomeNames) {
            this.map = map;
            this.outcomeNames = outcomeNames;

            evalParameters = new EvalParameters(parameters, outcomeNames.Length);
        }


        protected AbstractModel(
            Context[] parameters,
            string[] predLabels,
            string[] outcomeNames,
            int correctionConstant,
            double correctionParam) : this(predLabels, outcomeNames) {
            evalParameters = new EvalParameters(parameters, correctionParam, correctionConstant, outcomeNames.Length);
        }

#if DEBUG
        [Obsolete("predLabels argument are not used, so use the constructor (context, indexhashtable, string[])")]
        protected AbstractModel(Context[] parameters, string[] predLabels, IndexHashTable<string> map, string[] outcomeNames) {
            this.map = map;
            this.outcomeNames = outcomeNames;
            evalParameters = new EvalParameters(parameters, outcomeNames.Length);
        }
#endif

        #endregion

        #region + Properties .

        #region . ModelType .

        /// <summary>
        /// Gets the model type.
        /// </summary>
        /// <value>The type of the model.</value>
        public ModelType ModelType { get; protected set; }

        #endregion

        #endregion

        #region . ContainsOutcome .

        /// <summary>
        /// Determines whether the model contains the specified outcome.
        /// </summary>
        /// <param name="outcome">The outcome to locate in the model.</param>
        /// <returns><c>true</c> if the model contains the specified outcome; otherwise, <c>false</c>.</returns>
        public bool ContainsOutcome(string outcome) {
            return Array.IndexOf(outcomeNames, outcome) != -1;
        }
        #endregion

        #region . ContainsOutcomes .
        /// <summary>
        /// Determines whether the model contains all the specified outcomes.
        /// </summary>
        /// <param name="outcomes">The outcomes to locate in the model.</param>
        /// <returns><c>true</c> if the model contains ALL the specified outcome; otherwise, <c>false</c>.</returns>
        public bool ContainsOutcomes(IEnumerable<string> outcomes) {
            return outcomes.All(ContainsOutcome);
        }
        #endregion

        #region . Eval .

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public abstract double[] Eval(string[] context);

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public abstract double[] Eval(string[] context, double[] probs);

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public abstract double[] Eval(string[] context, float[] probs);

        #endregion

        #region . GetBestOutcome .

        /// <summary>
        /// Gets the name of the outcome corresponding to the highest likelihood in the parameter <paramref name="outcomes"/>.
        /// </summary>
        /// <param name="outcomes">The outcomes to be evaluated.</param>
        /// <returns>The string name of the best outcome.</returns>
        string IMaxentModel.GetBestOutcome(double[] outcomes) {
            var best = 0;
            for (var i = 1; i < outcomes.Length; i++)
                if (outcomes[i] > outcomes[best]) best = i;
            return outcomeNames[best];
        }

        #endregion

        #region . GetOutcome .

        /// <summary>
        /// Gets the string name of the outcome associated with the index,
        /// </summary>
        /// <param name="index">The index for which the name of the associated outcome is desired.</param>
        /// <returns>The string name of the outcome.</returns>
        public string GetOutcome(int index) {
            return outcomeNames[index];
        }

        #endregion

        #region . GetIndex .

        /// <summary>
        /// Gets the index associated with the String name of the given outcome.
        /// </summary>
        /// <param name="outcome">The string name of the outcome for which the index is desired.</param>
        /// <returns>The index if the given outcome label exists for this model, -1 if it does not.</returns>
        public int GetIndex(string outcome) {
            for (var i = 0; i < outcomeNames.Length; i++) {
                if (outcomeNames[i].Equals(outcome))
                    return i;
            }
            return -1;
        }

        #endregion

        #region . GetNumOutcomes .

        /// <summary>
        /// Gets the data structures relevant to storing the model.
        /// </summary>
        /// <returns>The data structures relevant to storing the model.</returns>
        public int GetNumOutcomes() {
            return evalParameters.NumOutcomes;
        }

        #endregion

        #region . GetDataStructures .

        /// <summary>
        /// Provides  the fundamental data structures which encode the maxent model information.  This method will usually only be needed by GISModelWriters.  The following values are held in the Object array which is returned by this method:
        /// <para>
        ///   <c>index 0</c>: <see cref="T:Context[]"/> containing the model parameters.
        ///   <c>index 1</c>: <see cref="T:IndexHashTable{string}"/> containing the mapping of model predicates to unique integers.
        ///   <c>index 2</c>: <see cref="T:string[]"/> containing the names of the outcomes, stored in the index of the array which represents their unique ids in the model.
        ///   <c>index 3</c>: <see cref="T:int"/> containing the value of the models correction constant.
        ///   <c>index 4</c>: <see cref="T:double"/> containing the value of the models correction parameter.
        /// </para>
        /// </summary>
        /// <returns>System.Object[].</returns>
        public object[] GetDataStructures() {
            return new object[] {
                evalParameters.Parameters,
                map,
                outcomeNames,
                Convert.ToInt32(evalParameters.CorrectionConstant),
                evalParameters.CorrectionParam
            };
        }

        #endregion
    }
}