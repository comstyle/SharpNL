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

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Represents a document categorizer evaluator.
    /// </summary>
    public class DocumentCategorizerEvaluator : Evaluator<DocumentSample, string> {
        private readonly IDocumentCategorizer documentCategorizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerEvaluator"/> class.
        /// </summary>
        /// <param name="documentCategorizer">The document categorizer.</param>
        /// <param name="listeners">The listeners.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="documentCategorizer"/>.</exception>
        public DocumentCategorizerEvaluator(IDocumentCategorizer documentCategorizer,
            params IEvaluationMonitor<DocumentSample>[] listeners) : base(listeners) {

            if (documentCategorizer == null)
                throw new ArgumentNullException("documentCategorizer");

            this.documentCategorizer = documentCategorizer;
        }

        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override DocumentSample ProcessSample(DocumentSample reference) {
            var document = reference.Text;

            var probs = documentCategorizer.Categorize(document, reference.ExtraInformation);
            var cat = documentCategorizer.GetBestCategory(probs);

            FMeasure.UpdateScores(new[] {reference.Category}, new[] {cat});

            return new DocumentSample(cat, reference.Text);
        }
    }
}