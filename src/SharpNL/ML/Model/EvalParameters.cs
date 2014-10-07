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


namespace SharpNL.ML.Model {
    /// <summary>
    /// Encapsulates the variables used in producing probabilities from a model and facilitates passing these variables to the eval method.
    /// </summary>
    public class EvalParameters {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvalParameters"/> which can be evaluated.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="numOutcomes">The number outcomes.</param>
        public EvalParameters(Context[] parameters, int numOutcomes) : this(parameters, 0, 0, numOutcomes) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="EvalParameters"/> which can be evaluated.
        /// </summary>
        /// <param name="parameters">The parameters of the model.</param>
        /// <param name="correctionParam">The correction parameter.</param>
        /// <param name="correctionConstant">The correction constant.</param>
        /// <param name="numOutcomes">The number outcomes.</param>
        public EvalParameters(Context[] parameters, double correctionParam, double correctionConstant, int numOutcomes) {
            Parameters = parameters;
            NumOutcomes = numOutcomes;
            CorrectionParam = correctionParam;
            CorrectionConstant = correctionConstant;
            ConstantInverse = 1/correctionConstant;
        }

        #region . Parameters .

        /// <summary>
        /// Gets the parameters of the model.
        /// </summary>
        /// <value>The parameters of the model.</value>
        public Context[] Parameters { get; private set; }

        #endregion

        #region . Outcomes .

        /// <summary>
        /// Gets the number outcomes.
        /// </summary>
        /// <value>The number outcomes.</value>
        public int NumOutcomes { get; private set; }

        #endregion

        /// <summary>
        /// Gets the correction constant.
        /// </summary>
        /// <value>The correction constant.</value>
        public double CorrectionConstant { get; private set; }

        /// <summary>
        /// Gets or sets the correction parameter.
        /// </summary>
        /// <value>The correction parameter.</value>
        public double CorrectionParam { get; set; }

        /// <summary>
        /// Gets the constant inverse.
        /// </summary>
        /// <value>The constant inverse.</value>
        public double ConstantInverse { get; private set; }
    }
}