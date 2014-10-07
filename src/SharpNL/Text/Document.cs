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

namespace SharpNL.Text {
    /// <summary>
    /// Represents a document with its texts and sentences.
    /// </summary>
    public class Document : IDocument {
        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="language">The language of this document</param>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="text"/></exception>
        public Document(string language, string text) {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");

            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            Language = language;
            Text = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="sentences">The sentences.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="text"/>
        /// or
        /// <paramref name="sentences"/>
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="sentences"/></exception>
        public Document(string text, List<Sentence> sentences) {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (sentences == null)
                throw new ArgumentNullException("sentences");

            if (sentences.Count == 0)
                throw new ArgumentOutOfRangeException("sentences");

            Text = text;
            Sentences = sentences.AsReadOnly();
        }

        #region . Language .
        /// <summary>
        /// Gets the language of this document.
        /// </summary>
        /// <value>The language of this document.</value>
        public string Language { get; private set; }
        #endregion


        #region . Text .
        /// <summary>
        /// Gets the document text.
        /// </summary>
        /// <value>The document text.</value>
        public string Text { get; private set; }
        #endregion

        #region + Sentences .
        /// <summary>
        /// Gets the document sentences.
        /// </summary>
        /// <value>The document sentences.</value>
        public IReadOnlyList<Sentence> Sentences { get; internal set; }

        IReadOnlyList<ISentence> IDocument.Sentences {
            get { return Sentences; }
        }
        #endregion

    }
}