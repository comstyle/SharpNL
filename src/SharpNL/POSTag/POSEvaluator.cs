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
using SharpNL.Utility.Evaluation;

namespace SharpNL.POSTag {
    /// <summary>
    /// The <see cref="POSEvaluator"/> measures the performance of the given <see cref="IPOSTagger"/> 
    /// with the provided reference <see cref="POSSample"/>s.
    /// </summary>
    public class POSEvaluator : Evaluator<POSSample> {
        private readonly IPOSTagger tagger;

        private readonly Mean wordAccuracy = new Mean();


        /// <summary>
        /// Initializes a new instance of the <see cref="POSEvaluator"/>.
        /// </summary>
        /// <param name="tagger">The tagger.</param>
        /// <param name="listeners">Any listeners.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="tagger"/>
        /// </exception>
        public POSEvaluator(IPOSTagger tagger, params IEvaluationMonitor<POSSample>[] listeners) : base(listeners) {
            if (tagger == null) {
                throw new ArgumentNullException("tagger");
            }

            this.tagger = tagger;
        }

        /// <summary>
        /// Evaluates the given reference <see cref="POSSample"/> object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference <see cref="POSSample"/>.</param>
        /// <returns>The predicted <see cref="POSSample"/>.</returns>
        protected override POSSample ProcessSample(POSSample reference) {
            var predictedTags = tagger.Tag(reference.Sentence,
                Array.ConvertAll(reference.AdditionalContext, input => (object) input));
            var referenceTags = reference.Tags;

            for (var i = 0; i < referenceTags.Length; i++) {
                wordAccuracy.Add(referenceTags[i].Equals(predictedTags[i]) ? 1 : 0);
            }

            return new POSSample(reference.Sentence, predictedTags);
        }

        #region . WordAccuracy .

        /// <summary>
        /// Gets the word accuracy.
        /// </summary>
        /// <value>The word accuracy.</value>
        /// <remarks>
        /// This is defined as: word accuracy = correctly detected tags / total words
        /// </remarks>
        public double WordAccuracy {
            get { return wordAccuracy.Value; }
        }

        #endregion

        #region . WordCount .

        /// <summary>
        /// Gets the total number of words considered in the evaluation.
        /// </summary>
        /// <value>The word count.</value>
        public long WordCount {
            get { return wordAccuracy.Count; }
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("POSEvaluator (Accuracy: {0}, Number of Samples: {1})", WordAccuracy, WordCount);
        }

        #endregion
    }
}