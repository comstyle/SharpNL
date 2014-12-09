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
using System.Globalization;
using System.Text;
using SharpNL.Extensions;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Represents a document with begin indexes of the individual sentences.
    /// </summary>
    public class SentenceSample : IEquatable<SentenceSample> {

        #region + Constructors .

        /// <summary>Initializes a new instance of the <see cref="SentenceSample" /> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="sentences">The sentences.</param>
        public SentenceSample(string document, params Span[] sentences) {
            Document = document;
            Sentences = sentences;
        }


        /// <summary>Initializes a new instance of the <see cref="SentenceSample" /> with de specified detokenizer and sentences.</summary>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="sentences">The sentences.</param>
        public SentenceSample(IDetokenizer detokenizer, string[][] sentences) {
            Sentences = new Span[sentences.Length];

            var sb = new StringBuilder();

            for (var i = 0; i < sentences.Length; i++) {
                var sentence = detokenizer.Detokenize(sentences[i], null);
                var beginIndex = sb.Length;

                sb.Append(sentence);

                Sentences[i] = new Span(beginIndex, sb.Length);
            }

            Document = sb.ToString();
        }

        #endregion

        #region + Properties .

        /// <summary>Gets the document.</summary>
        /// <value>The document.</value>
        public string Document { get; private set; }

        /// <summary>Gets the sentences.</summary>
        /// <value>The the begin indexes of the sentences in the document.</value>
        public Span[] Sentences { get; private set; }

        #endregion

        #region . ToString .

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            var sb = new StringBuilder();

            foreach (var sentence in Sentences) {
                sb.Append(
                    sentence.GetCoveredText(Document)
                        .ToString(CultureInfo.InvariantCulture)
                        .Replace("\r", "<CR>")
                        .Replace("\n", "<LF>"));
                sb.Append("\n");
            }
            return sb.ToString();
        }

        #endregion

        #region + Equals .

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current
        /// <see cref="T:SentenceSample" />.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj.GetType() == typeof (SentenceSample) && Equals((SentenceSample) obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(SentenceSample other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Document.Equals(other.Document) &&
                   Sentences.SequenceEqual(other.Sentences);
        }


        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return (
                    (Document != null ? Document.GetHashCode() : 0) * 397) ^
                    (Sentences != null ? Sentences.GetHashCode() : 0);
            }
        }
        #endregion

    }
}