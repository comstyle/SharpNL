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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SharpNL.Utility.Evaluation {
    /// <summary>
    /// Represents an abstract base class for evaluators.
    /// Evaluation results are the arithmetic mean of the scores calculated for each reference sample.
    /// </summary>
    /// <typeparam name="T">The object type to be evaluated.</typeparam>
    public abstract class Evaluator<T> where T : class {
        protected readonly ReadOnlyCollection<IEvaluationMonitor<T>> listeners;

        /// <summary>
        /// Initializes a new instance of the <see cref="Evaluator{T}"/> class.
        /// </summary>
        /// <param name="listeners">The evaluation listeners.</param>
        protected Evaluator(params IEvaluationMonitor<T>[] listeners) {
            this.listeners = listeners == null ? 
                new List<IEvaluationMonitor<T>>().AsReadOnly():
                new List<IEvaluationMonitor<T>>(listeners).AsReadOnly();
        }

        #region . FMeasure .
        /// <summary>
        /// Gets or sets the f-measure.
        /// </summary>
        /// <value>The f-measure.</value>
        public FMeasure FMeasure { get; protected set; }
        #endregion

        #region . ProcessSample .
        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected abstract T ProcessSample(T reference);
        #endregion

        #region . EvaluateSample .
        /// <summary>
        /// Evaluates the given reference object. The default implementation calls <see cref="M:ProcessSample"/>.
        /// </summary>
        /// <param name="sample">The sample to be evaluated.</param>
        /// <remarks>
        /// This method will be changed to private in the future.
        /// Implementations should override <see cref="M:ProcessSample"/> instead.
        /// If this method is override, the implementation has to update the score after every invocation.
        /// </remarks>
        private void EvaluateSample(T sample) {
            var predicted = ProcessSample(sample);
            foreach (var listener in listeners) {
                if (sample.Equals(predicted)) {
                    listener.CorrectlyClassified(predicted, predicted);
                } else {
                    listener.Misclassified(sample, predicted);
                }
            }
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Reads all sample objects from the stream and evaluates each sample object with <see cref="M:ProcessSample"/> method.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public void Evaluate(IObjectStream<T> samples) {
            T sample;
            while ((sample = samples.Read()) != null) {
                EvaluateSample(sample);
            }
        }
        #endregion

    }
}