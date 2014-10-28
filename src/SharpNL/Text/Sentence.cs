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
using SharpNL.Parser;


namespace SharpNL.Text {
    /// <summary>
    /// Represents a sentence.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Sentence : ISentence {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sentence"/> class.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <param name="document">The document where the sentence is located.</param>
        public Sentence(int start, int end, IDocument document) : this(start, end, null, document) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sentence"/> class.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <param name="tokens">The sentence tokens.</param>
        /// <param name="document">The document where the sentence is located.</param>
        public Sentence(int start, int end, List<Token> tokens, IDocument document) {
            Start = start;
            End = end;

            if (tokens != null)
                Tokens = tokens.AsReadOnly();

            Document = document;
        }

        #region + Chunks .
        /// <summary>
        /// Gets the sentence chunks.
        /// </summary>
        /// <value>The sentence chunks.</value>
        public IReadOnlyList<Chunk> Chunks { get; protected set; }
        IReadOnlyList<IChunk> ISentence.Chunks {
            get { return Chunks; }
            set {
                Chunks = value != null
                    ? value.Cast<Chunk>().ToList().AsReadOnly()
                    : null;
            }
        }

        #endregion

        #region . Document .
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public IDocument Document { get; private set; }
        #endregion

        #region . End .
        /// <summary>
        /// Gets the sentence end position.
        /// </summary>
        /// <value>The sentence end position.</value>
        public int End { get; private set; }
        #endregion

        #region . Entities .
        /// <summary>
        /// Gets the sentence entities.
        /// </summary>
        /// <value>The sentence entities.</value>
        public IReadOnlyList<IEntity> Entities { get; internal set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the sentence length.
        /// </summary>
        /// <value>The sentence length.</value>
        public int Length {
            get { return End - Start; }
            
        }
        #endregion

        #region . Parse .
        /// <summary>
        /// Gets the parsed sentence.
        /// </summary>
        /// <value>The parsed sentence.</value>
        public Parse Parse { get; set; }
        #endregion

        #region . Start .

        /// <summary>
        /// Gets the sentence start position.
        /// </summary>
        /// <value>The sentence start position.</value>
        public int Start { get; private set; }

        #endregion

        #region . Text .

        private string text;

        /// <summary>
        /// Gets the sentence text.
        /// </summary>
        /// <value>The sentence text.</value>
        public string Text {
            get {
                if (text != null)
                    return text;

                return (text = Document.Text.Substring(Start, Length));
            }
        }

        #endregion

        #region + Tokens .

        /// <summary>
        /// Gets the sentence tokens.
        /// </summary>
        /// <value>The sentence tokens.</value>
        public IReadOnlyList<Token> Tokens { get; protected set; }
        IReadOnlyList<IToken> ISentence.Tokens {
            get { return Tokens; }
            set {
                Tokens = value != null
                    ? value.Cast<Token>().ToList().AsReadOnly()
                    : null;
            }
        }

        #endregion

        #region . TokensProbability .

        /// <summary>
        /// Gets the tokens probability.
        /// </summary>
        /// <value>The tokens probability.</value>
        public double TokensProbability { get; set; }

        #endregion

        #region . SubString .
        /// <summary>
        /// Retrieves a substring from the current <see cref="Sentence"/> object. 
        /// The substring starts at a specified index number, or character position, in the <see cref="Sentence"/> object and has a specified length.
        /// </summary>
        /// <param name="startIndex">The starting index number in the substring.</param>
        /// <param name="length">The number of characters in the substring.</param>
        /// <returns>A <see cref="string"/> object equivalent to the substring of the length specified in the length parameter, beginning at the starting index number in the current sentence object.</returns>
        /// <remarks>The startIndex parameter is zero-based.</remarks>
        public string Substring(int startIndex, int length) {
            return Text.Substring(startIndex, length);
        }
        #endregion

        #region . SetParse .
        internal void SetParse(Parse parse) {
            Parse = parse;
        }
        #endregion

    }
}