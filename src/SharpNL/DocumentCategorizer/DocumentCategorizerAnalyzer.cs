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
using System.Collections.ObjectModel;
using SharpNL.Analyzer;
using SharpNL.Text;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Represents a document categorizer which allows the easy abstraction of the document categorization. 
    /// This class is thread-safe.
    /// </summary>
    public class DocumentCategorizerAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Gets the document categorizer.
        /// </summary>
        /// <value>The document categorizer.</value>
        protected IDocumentCategorizer DocumentCategorizer { get; private set; }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerAnalyzer" /> using the default weight.
        /// </summary>
        /// <param name="documentCategorizer">The document categorizer.</param>
        /// <exception cref="System.ArgumentNullException">documentCategorizer</exception>
        public DocumentCategorizerAnalyzer(IDocumentCategorizer documentCategorizer) : this(documentCategorizer, 2f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerAnalyzer" /> with the specified weight.
        /// </summary>
        /// <param name="documentCategorizer">The document categorizer.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">documentCategorizer</exception>
        public DocumentCategorizerAnalyzer(IDocumentCategorizer documentCategorizer, float weight)
            : base(weight) {
            if (documentCategorizer == null)
                throw new ArgumentNullException("documentCategorizer");

            DocumentCategorizer = documentCategorizer;
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected override void Evaluate(ITextFactory factory, IDocument document) {
            if (document.Sentences == null)
                throw new AnalyzerException(this, "The document does not have the sentences detected.");

            foreach (var sentence in document.Sentences) {
                if (sentence.Tokens == null)
                    throw new AnalyzerException(this, "The document have a sentence without the tokenization.");

                var list = sentence.Categories != null
                    ? new List<ICategory>(sentence.Categories)
                    : new List<ICategory>();

                var dict = DocumentCategorizer.ScoreMap(sentence.GetTokens());
                var category = factory.CreateCategory(sentence, dict);
                if (category != null && !list.Contains(category))
                    list.Add(category);

                sentence.Categories = new ReadOnlyCollection<ICategory>(list);
            }
        }
        #endregion

    }
}