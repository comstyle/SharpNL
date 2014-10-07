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
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.NameFind {
    /// <summary>
    /// The <see cref="TokenNameFinderEvaluator"/> measures the performance of the given 
    /// <see cref="ITokenNameFinder"/> with the provided reference <see cref="NameSample"/>s.
    /// </summary>
    /// <seealso cref="NameSample"/>
    /// <seealso cref="Evaluator{T,F}"/>
    /// <seealso cref="ITokenNameFinder"/>    
    public class TokenNameFinderEvaluator : Evaluator<NameSample, Span> {
        private readonly ITokenNameFinder nameFinder;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderEvaluator"/> with the given <paramref name="nameFinder"/>.
        /// </summary>
        /// <param name="nameFinder">The <see cref="ITokenNameFinder"/> to evaluate.</param>
        /// <param name="listeners">The evaluation sample listeners.</param>
        /// <exception cref="System.ArgumentNullException">listeners</exception>
        public TokenNameFinderEvaluator(ITokenNameFinder nameFinder, params IEvaluationMonitor<NameSample>[] listeners)
            : base(listeners) {
            if (nameFinder == null)
                throw new ArgumentNullException("listeners");

            this.nameFinder = nameFinder;

            FMeasure = new FMeasure<Span>();
        }
        #endregion

        #region . ProcessSample .
        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override NameSample ProcessSample(NameSample reference) {
            if (reference.ClearAdaptiveData) {
                nameFinder.ClearAdaptiveData();
            }

            var predictedNames = nameFinder.Find(reference.Sentence);
            var references = reference.Names;

            // OPENNLP-396 When evaluating with a file in the old format
            // the type of the span is null, but must be set to default to match
            // the output of the name finder.
            for (var i = 0; i < references.Length; i++) {
                if (references[i].Type == null) {
                    references[i] = new Span(references[i].Start, references[i].End, "default");
                }
            }

            FMeasure.UpdateScores(references, predictedNames);

            return new NameSample(reference.Sentence, predictedNames, reference.ClearAdaptiveData);
        }
        #endregion

    }
}