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
using System.Text;
using SharpNL.Extensions;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.POSTag {
    /// <summary>
    /// Represents an pos-tagged sentence.
    /// </summary>
    public class POSSample : IEquatable<POSSample> {

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the pos-tagged sentence.
        /// </summary>
        /// <param name="sentence">The sentence tokens.</param>
        /// <param name="tags">The tags for each sentence token.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentence"/>
        /// or
        /// <paramref name="tags"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// null elements are not allowed in <paramref name="sentence"/> tokens!
        /// or
        /// null elements are not allowed in <paramref name="tags"/>!
        /// </exception>
        public POSSample(string[] sentence, string[] tags) : this (sentence, tags, null) {}

        /// <summary>
        /// Initializes a new instance of the pos-tagged sentence.
        /// </summary>
        /// <param name="sentence">The sentence tokens.</param>
        /// <param name="tags">The tags for each sentence token.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentence"/>
        /// or
        /// <paramref name="tags"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// null elements are not allowed in <paramref name="sentence"/> tokens!
        /// or
        /// null elements are not allowed in <paramref name="tags"/>!
        /// </exception>
        public POSSample(string[] sentence, string[] tags, string[][] additionalContext) {
            if (sentence == null) {
                throw new ArgumentNullException("sentence");
            }
            if (tags == null) {
                throw new ArgumentNullException("tags");
            }
            if (sentence.Length != tags.Length) {
                throw new ArgumentException(
                    string.Format("There must be exactly one tag for each token. sentence: {0}, tags: {1}",
                        sentence.Length, tags.Length));
            }
            if (Array.IndexOf(sentence, null) != -1) {
                throw new ArgumentException(@"null elements are not allowed in sentence tokens!", "sentence");
            }
            if (Array.IndexOf(tags, null) != -1) {
                throw new ArgumentException(@"null elements are not allowed in tags!", "sentence");
            }

            AdditionalContext = additionalContext;
            Sentence = sentence;
            Tags = tags;
        }

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current sample.
        /// </summary>
        /// <returns>
        /// A string that represents the current sample.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            for (var i = 0; i < Sentence.Length; i++) {
                sb.AppendFormat("{0}_{1} ", Sentence[i], Tags[i]);
            }
            if (sb.Length > 0) {
                // get rid of last space
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        #endregion

        #region + Properties .

        /// <summary>
        /// Gets the additional context.
        /// </summary>
        /// <value>The additional context.</value>
        public string[][] AdditionalContext { get; private set; }

        /// <summary>
        /// Gets the sentence.
        /// </summary>
        /// <value>The sentence.</value>
        public string[] Sentence { get; private set; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string[] Tags { get; private set; }

        #endregion

        #region . Parse .
        /// <summary>
        /// Parses the specified sentence string into an <see cref="POSSample"/> object. 
        /// </summary>
        /// <param name="sentenceString">The sentence string.</param>
        /// <returns>The parsed <see cref="POSSample"/> object.</returns>
        /// <exception cref="InvalidFormatException">Cannot find "_" inside token '...'!</exception>
        public static POSSample Parse(string sentenceString) {
            var tokens = WhitespaceTokenizer.Instance.Tokenize(sentenceString);
            var sentence = new string[tokens.Length];
            var tags = new string[tokens.Length];

            for (int i = 0; i < tokens.Length; i++) {
                var split = tokens[i].LastIndexOf("_", StringComparison.InvariantCulture);
                if (split == -1) {
                    throw new InvalidFormatException("Cannot find \"_\" inside token '" + tokens[i] + "'!");
                }

                sentence[i] = tokens[i].Substring(0, split);
                tags[i] = tokens[i].Substring(split + 1);
            }
            return new POSSample(sentence, tags);

        }
        #endregion

        #region + Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((POSSample)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(POSSample other) {
            return Sentence.SequenceEqual(other.Sentence) &&
                   Tags.SequenceEqual(other.Tags);
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="POSSample"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return ((Sentence != null ? Sentence.GetHashCode() : 0) * 397) ^
                        (Tags != null ? Tags.GetHashCode() : 0);
            }
        }
        #endregion

        #region . GetAdditionalContext .
        /// <summary>
        /// Gets the additional context as object array.
        /// </summary>
        internal object[] GetAdditionalContext() {
            if (AdditionalContext == null)
                return null;

            return Array.ConvertAll(AdditionalContext, input => (object) input);
        }
        #endregion

    }
}