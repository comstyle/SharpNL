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

namespace SharpNL.Sentence {
    /// <summary>
    /// The SentenceDetectorEvaluator measures the performance of the given <see cref="ISentenceDetector"/>
    /// with the provided reference <see cref="SentenceSample"/>s.
    /// </summary>
    /// <seealso cref="Evaluator{T}"/>
    /// <seealso cref="ISentenceDetector"/>
    /// <seealso cref="SentenceSample"/>
    public class SentenceDetectorEvaluator : Evaluator<SentenceSample> {
        private readonly ISentenceDetector sentenceDetector;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceDetectorEvaluator"/> class.
        /// </summary>
        /// <param name="sentenceDetector">The sentence detector.</param>
        /// <param name="listeners">The evaluation sample listeners.</param>
        public SentenceDetectorEvaluator(
            ISentenceDetector sentenceDetector,
            params IEvaluationMonitor<SentenceSample>[] listeners) : base(listeners) {

            this.sentenceDetector = sentenceDetector;
            FMeasure = new FMeasure();
        }
        #endregion

        #region . ProcessSample .
        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override SentenceSample ProcessSample(SentenceSample reference) {
            var doc = reference.Document;
            var predictions = TrimSpans(doc, sentenceDetector.SentPosDetect(doc));
            var references = TrimSpans(doc, reference.Sentences);

            FMeasure.UpdateScores(references, predictions);

            return new SentenceSample(reference.Document, Array.ConvertAll(predictions, input => (Span)input));
        }

        private static object[] TrimSpans(string document, Span[] spans) {
            var trimmedSpans = new object[spans.Length];
            for (int i = 0; i < spans.Length; i++) {
                trimmedSpans[i] = spans[i].Trim(document);
            }

            return trimmedSpans;
        }
        #endregion

    }
}