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
using System.ComponentModel;
using System.Text;
using SharpNL.Utility;

namespace SharpNL.Text {
    /// <summary>
    /// Represents a chunk.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Chunk : IChunk {

        private readonly Sentence sentence;

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The chunk span.</param>
        public Chunk(Sentence sentence, Span span) {
            HeadIndex = -1;
            Span = span;
            this.sentence = sentence;
        }
        #endregion

        #region + Properties .

        #region . End .
        /// <summary>
        /// Gets the chunk end position.
        /// </summary>
        /// <value>The chunk end position.</value>
        public int End { get { return Span.End; } }
        #endregion

        #region . HeadIndex .
        /// <summary>
        /// Gets or sets the index of the head. Default value is -1.
        /// </summary>
        /// <value>The index of the head.</value>
        public int HeadIndex { get; set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the chunk length.
        /// </summary>
        /// <value>The chunk length.</value>
        public int Length {
            get { return End - Start; }
        }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets the chunk start position.
        /// </summary>
        /// <value>The chunk start position.</value>
        public int Start { get { return Span.Start; } }
        #endregion

        #region . Span .
        /// <summary>
        /// Gets the chunk span.
        /// </summary>
        /// <value>The chunk span.</value>
        public Span Span { get; private set; }
        #endregion

        #region . Tag .

        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <value>The chunk tag.</value>
        public string Tag {
            get { return Span.Type; }
        }

        #endregion

        #region . Tokens .

        private IReadOnlyList<Token> tokens;
        /// <summary>
        /// Gets the chunk tokens.
        /// </summary>
        /// <value>The chunk tokens.</value>
        public IReadOnlyList<Token> Tokens {
            get {
                if (tokens != null)
                    return tokens;

                return (tokens = sentence.Tokens.SubList(Start, End - Start));
            }
        }
        IReadOnlyList<IToken> IChunk.Tokens {
            get { return Tokens; }   
        }

        #endregion

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder(10 + Length + (Tokens.Count * 2));

            sb.AppendFormat("Chunk: {0} [ ", Tag);

            for (int i = Start; i < End; i++) {
                if (i == HeadIndex)
                    sb.Append('*');

                sb.Append(tokens[i - Start].Lexeme).Append(" ");
            }
            sb.AppendLine("]");

            return sb.ToString();
        }
        #endregion

    }
}