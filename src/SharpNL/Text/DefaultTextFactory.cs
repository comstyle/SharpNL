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
using SharpNL.Utility;

namespace SharpNL.Text {

    /// <summary>
    /// The factory that provides the default implementations and resources for the SharpNL text objects. 
    /// This class cannot be inherited.
    /// </summary>
    public sealed class DefaultTextFactory : ITextFactory {

        #region + Properties .

        #region . DefaultWordNet .
        /// <summary>
        /// Gets or sets the default WordNet instance.
        /// </summary>
        /// <value>The default WordNet instance. The default value is <c>null</c>.</value>
        public static WordNet.WordNet DefaultWordNet { get; set; }
        #endregion

        #region . Instance .
        /// <summary>
        /// Gets the <see cref="DefaultTextFactory"/> instance.
        /// </summary>
        /// <value>The <see cref="DefaultTextFactory"/> instance.</value>
        public static DefaultTextFactory Instance { get; private set; }
        #endregion

        #region . WordNet .
        /// <summary>
        /// Gets the WordNet instance.
        /// </summary>
        /// <returns>Always return the value of the <see cref="DefaultWordNet"/> property.</returns>
        public WordNet.WordNet WordNet {
            get {
                return DefaultWordNet;
            }
        }
        #endregion

        #endregion

        #region + Constructors .
        /// <summary>
        /// Initializes static members of the <see cref="DefaultTextFactory"/> class.
        /// </summary>
        static DefaultTextFactory() {
            Instance = new DefaultTextFactory();
        }
        /// <summary>
        /// Prevents a default instance of the <see cref="DefaultTextFactory"/> class from being created.
        /// </summary>
        private DefaultTextFactory() { }
        #endregion

        #region . CreateCategory .
        /// <summary>
        /// Creates the <see cref="ICategory"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="dict">The scored dictionary of categories.</param>
        /// <returns>The new <see cref="ICategory"/> object or a <c>null</c> value if the category is invalid or should be ignored.</returns>
        public ICategory CreateCategory(ISentence sentence, Dictionary<string, double> dict) {
            var key = string.Empty;
            var prob = double.MinValue;
            foreach (var pair in dict) {
                if (prob >= pair.Value)
                    continue;

                prob = pair.Value;
                key = pair.Key;
            }

            // returns the category with the highest probability.
            return new Category {
                Name = key,
                Probability = dict[key]
            };
        }
        #endregion

        #region . CreateChunk .
        /// <summary>
        /// Creates the <see cref="IChunk"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The chunk span.</param>
        /// <returns>The <see cref="IChunk"/> representation.</returns>
        public Chunk CreateChunk(Sentence sentence, Span span) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (span == null)
                throw new ArgumentNullException("span");

            return new Chunk(sentence, span);
        }
        IChunk ITextFactory.CreateChunk(ISentence sentence, Span span) {
            var s = sentence as Sentence;
            if (s != null)
                return CreateChunk(s, span);

            throw new NotSupportedException("The sentence type " + sentence.GetType().Name + " is not supported by " + GetType().Name + ".");
        }
        #endregion

        #region . CreateDocument .
        /// <summary>
        /// Creates the document object.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="text">The text.</param>
        /// <returns>The created <see cref="IDocument"/> object.</returns>
        public Document CreateDocument(string language, string text) {
            return new Document(language, text);
        }

        IDocument ITextFactory.CreateDocument(string language, string text) {
            return CreateDocument(language, text);
        }
        #endregion

        #region . CreateEntity .
        /// <summary>
        /// Creates an entity object.
        /// </summary>
        /// <param name="sentence">The sentence where the entity is present.</param>
        /// <param name="span">The entity span.</param>
        /// <returns>The new <see cref="IEntity"/> object.</returns>
        public Entity CreateEntity(Sentence sentence, Span span) {
            return new Entity(span, sentence);
        }
        IEntity ITextFactory.CreateEntity(ISentence sentence, Span span) {
            var s = sentence as Sentence;
            if (s != null)
                return CreateEntity(s, span);

            throw new NotSupportedException("The sentence type " + sentence.GetType().Name + " is not supported.");
        }
        #endregion

        #region . CreateSentence .
        /// <summary>
        /// Creates a sentence object.
        /// </summary>
        /// <param name="span">The sentence span</param>
        /// <param name="document">The document.</param>
        /// <returns>The created <see cref="ISentence" /> object.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="span"/>
        /// or
        /// <paramref name="document"/>
        /// </exception>
        public Sentence CreateSentence(Span span, Document document) {
            if (span == null)
                throw new ArgumentNullException("span");

            if (document == null)
                throw new ArgumentNullException("document");

            return new Sentence(span.Start, span.End, document);
        }
        ISentence ITextFactory.CreateSentence(Span span, IDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            var d = document as Document;
            if (d != null)
                return CreateSentence(span, (Document)document);

            throw new NotSupportedException("The document type " + document.GetType().Name + " is not supported.");
        }
        #endregion

        #region . CreateToken .
        /// <summary>
        /// Creates an token object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The token span.</param>
        /// <param name="lexeme">The lexeme.</param>
        /// <returns>The created <see cref="IToken" /> object.</returns>
        public Token CreateToken(Sentence sentence, Span span, string lexeme) {
            return new Token(sentence, span, lexeme) {
                WordNet = WordNet
            };
        }

        IToken ITextFactory.CreateToken(ISentence sentence, Span span, string lexeme) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (span == null)
                throw new ArgumentNullException("span");

            var s = sentence as Sentence;
            if (s != null)
                return CreateToken(s, span, lexeme);

            throw new NotSupportedException("The sentence type " + sentence.GetType().Name + " is not supported.");
        }
        #endregion

    }
}