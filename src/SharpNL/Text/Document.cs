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
using System.ComponentModel;
using System.Linq;

namespace SharpNL.Text {
    /// <summary>
    /// Represents a document with its texts and sentences.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Document : IDocument {

        #region + Constructors .

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

        #endregion

        #region + Properties .

        #region . Language .
        /// <summary>
        /// Gets the language of this document.
        /// </summary>
        /// <value>The language of this document.</value>
        [Description("The document language.")]
        public string Language { get; private set; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets the document text.
        /// </summary>
        /// <value>The document text.</value>
        [Description("The document text.")]
        public string Text { get; private set; }
        #endregion

        #region . Tokenized .
        /// <summary>
        /// Gets a value indicating whether this <see cref="Document"/> is tokenized.
        /// </summary>
        /// <value><c>true</c> if tokenized; otherwise, <c>false</c>.</value>
        public bool Tokenized { get; protected set; }
        #endregion

        #region . Sentences .
        /// <summary>
        /// Gets the document sentences.
        /// </summary>
        /// <value>The document sentences.</value>
        [Description("The document sentences.")]
        public IReadOnlyList<Sentence> Sentences { get; internal set; }

        IReadOnlyList<ISentence> IDocument.Sentences {
            get { return Sentences; }
        }
        #endregion

        #endregion

        #region . GetEntities .
        /// <summary>
        /// Gets the all the entities in the sentences.
        /// </summary>
        /// <returns>All the entities in the sentences.</returns>
        public IEntity[] GetEntities() {
            var list = new List<IEntity>();
            foreach (var sentence in Sentences) {
                list.AddRange(sentence.Entities);
            }
            return list.ToArray();
        }
        #endregion

        #region . GetEntityCount .
        /// <summary>
        /// Gets the get entity count.
        /// </summary>
        /// <value>The get entity count.</value>
        public int GetEntityCount {
            get {
                return Sentences
                    .Where(sentence => sentence.Entities != null)
                    .Sum(sentence => sentence.Entities.Count);
            }
        }
        #endregion

        #region . SetTokenized .
        internal void SetTokenized(bool value) {
            Tokenized = value;
        }
        #endregion



    }
}