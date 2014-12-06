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
using SharpNL.WordNet;

namespace SharpNL.Text {
    /// <summary>
    /// Represents a token, which is a word, its lemma, its morphological posTag and the 
    /// position of it in the sentence.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Token : IToken {

        public Token(int start, int end, string lexeme) : this(start, end, lexeme, null, null, null) { }

        public Token(int start, int end, string lexeme, string[] lemmas, string tag, string features) {
            Start = start;
            End = end;
            Lexeme = lexeme;
            Lemmas = lemmas;
            POSTag = tag;
            Features = features;
        }

        #region + Properties .

        #region . ChunkTag .
        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <value>The chunk tag.</value>
        public string ChunkTag { get; set; }
        #endregion

        #region . End .
        /// <summary>
        /// Gets the token end position.
        /// </summary>
        /// <value>The token end position.</value>
        public int End { get; internal set; }
        #endregion

        #region . Features .
        /// <summary>
        /// Gets the token features.
        /// </summary>
        /// <value>The token features.</value>
        public string Features { get; internal set; }
        #endregion

        #region . IsChunkHead .
        /// <summary>
        /// Gets a value indicating whether this token is a chunk head.
        /// </summary>
        /// <value><c>true</c> if this token is a chunk head; otherwise, <c>false</c>.</value>
        public bool IsChunkHead { get; internal set; }
        #endregion

        #region . Lemmas .
        /// <summary>
        /// Gets the token lemmas.
        /// </summary>
        /// <value>The token lemmas.</value>
        public string[] Lemmas { get; internal set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the token length.
        /// </summary>
        /// <value>The token length.</value>
        public int Length {
            get { return End - Start; }
        }
        #endregion

        #region . Lexeme .
        /// <summary>
        /// Gets the lexeme.
        /// </summary>
        /// <value>The lexeme.</value>
        public string Lexeme { get; internal set; }
        #endregion

        #region . POSTag .
        /// <summary>
        /// Gets the POSTag.
        /// </summary>
        /// <value>The POSTag.</value>
        public string POSTag { get; set; }
        #endregion

        #region . POSTagProbability .
        /// <summary>
        /// Gets the POSTag probability.
        /// </summary>
        /// <value>The POSTag probability.</value>
        public double POSTagProbability { get; set; }
        #endregion

        #region . Probability .
        /// <summary>
        /// Gets the token probability.
        /// </summary>
        /// <value>The token probability.</value>
        public double Probability { get; set; }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets the token start position.
        /// </summary>
        /// <value>The token start position.</value>
        public int Start { get; internal set; }
        #endregion

        #region . SyntSets .
        private List<SynSet> synSets;
        /// <summary>
        /// Gets the WordNet synsets list.
        /// </summary>
        /// <value>
        /// The synonym ring or synset if the <see cref="DefaultTextFactory"/> provides a WordNet instance.
        /// </value>
        public List<SynSet> SynSets {
            get {
                if (synSets != null)
                    return synSets;

                if (WordNet == null)
                    return null;

                return synSets = WordNet.GetSynSets(Lexeme);
            }
        }

        #endregion

        #region . SyntacticTag .
        /// <summary>
        /// Gets the syntactic tag.
        /// </summary>
        /// <value>The syntactic tag.</value>
        public string SyntacticTag { get; internal set; }
        #endregion

        #region . WordNet .
        /// <summary>
        /// Gets or sets the WordNet instance.
        /// </summary>
        /// <value>The WordNet instance.</value>
        internal WordNet.WordNet WordNet { get; set; }
        #endregion

        #endregion

        #region + Equals .
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:Token"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Token)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Token"/> is equal to the current <see cref="T:Token"/>.
        /// </summary>
        /// <param name="other">The token to compare with the current token.</param>
        /// <returns><c>true</c> if the specified token is equal to the current token, <c>false</c> otherwise.</returns>
        public bool Equals(Token other) {
            return
                End == other.End &&
                Start == other.Start &&
                string.Equals(Lexeme, other.Lexeme) &&
                Equals(Lemmas, other.Lemmas);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:IToken"/> is equal to the current <see cref="T:Token"/>.
        /// </summary>
        /// <param name="other">The token to compare with the current token.</param>
        /// <returns><c>true</c> if the specified token is equal to the current token, <c>false</c> otherwise.</returns>
        public bool Equals(IToken other) {
            return
                End == other.End &&
                Start == other.Start &&
                string.Equals(Lexeme, other.Lexeme) &&
                Equals(Lemmas, other.Lemmas);
        }

        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Returns a hash code for this token.
        /// </summary>
        /// <returns>A hash code for this token, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() {
            unchecked {
                int hashCode = End;
                hashCode = (hashCode * 397) ^ Start;
                hashCode = (hashCode * 397) ^ (Lexeme != null ? Lexeme.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Lemmas != null ? Lemmas.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

    }
}