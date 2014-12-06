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
using System.Text;
using System.Text.RegularExpressions;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Class for holding names for a single unit of text.
    /// </summary>
    public class NameSample {

        /// <summary>
        /// The a default type value when there is no type in training data.
        /// </summary>
        public const string DefaultType = "default";

        private static readonly Regex StartTagRegex;

        #region + Constructors .


        /// <summary>
        /// Initializes static members of the <see cref="NameSample"/> class.
        /// </summary>
        static NameSample() {
            StartTagRegex = new Regex("<START(:([^:>\\s]*))?>(?=\\s|$)", RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSample"/> class.
        /// </summary>
        /// <param name="sentence">The sentence tokens.</param>
        /// <param name="names">The name spans.</param>
        /// <param name="clearAdaptiveData">if set to <c>true</c> adaptive data should be clear.</param>
        public NameSample(string[] sentence, Span[] names, bool clearAdaptiveData)
            : this(sentence, names, null, clearAdaptiveData) {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="NameSample"/> class.
        /// </summary>
        /// <param name="sentence">The sentence tokens.</param>
        /// <param name="names">The name spans.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="clearAdaptiveData">if set to <c>true</c> adaptive data should be clear.</param>
        public NameSample(string[] sentence, Span[] names, string[][] additionalContext, bool clearAdaptiveData)
            : this(null, sentence, names, additionalContext, clearAdaptiveData) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameSample"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sentence">The sentence tokens.</param>
        /// <param name="names">The name spans.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="clearAdaptiveData">if set to <c>true</c> adaptive data should be clear.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="sentence"/></exception>
        /// <exception cref="System.InvalidOperationException">The name spans are overlapping.</exception>
        public NameSample(string id, string[] sentence, Span[] names, string[][] additionalContext,
            bool clearAdaptiveData) {

            Id = id;

            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (names == null)
                names = new Span[0];

            if (Span.IsOverlapping(names))
                throw new InvalidOperationException("The name spans are overlapping");

            Sentence = sentence;
            Names = names;

            AdditionalContext = additionalContext;
            ClearAdaptiveData = clearAdaptiveData;

        }


        #endregion

        #region + Properties .

        #region . AdditionalContext .

        /// <summary>
        /// Gets the additional context.
        /// </summary>
        /// <value>The additional context.</value>
        public string[][] AdditionalContext { get; private set; }

        #endregion

        #region . ClearAdaptiveData .
        /// <summary>
        /// Gets a value indicating whether adaptive data should be cleared.
        /// </summary>
        /// <value><c>true</c> if adaptive data should be cleared; otherwise, <c>false</c>.</value>
        public bool ClearAdaptiveData { get; private set; }
        #endregion

        #region . Id .
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; private set; }
        #endregion

        #region . Names .
        /// <summary>
        /// Gets the name spans.
        /// </summary>
        /// <value>The name spans.</value>
        public Span[] Names { get; private set; }
        #endregion

        #region . Sentence .
        /// <summary>
        /// Gets the sentence tokens.
        /// </summary>
        /// <value>The sentence tokens.</value>
        public string[] Sentence { get; private set; }
        #endregion

        #endregion

        #region . Equals .

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

            var nameSample = obj as NameSample;
            if (nameSample != null) {
                return Equals(nameSample);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:NameSample"/> is equal to the current <see cref="T:NameSample"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="other">The other <see cref="NameSample"/> to compare with the current object. </param>
        protected bool Equals(NameSample other) {
            if (other == null)
                return false;

            return Sentence.SequenceEqual(other.Sentence) &&
                   Names.SequenceEqual(other.Names) &&
                   AdditionalContext.SequenceEqual(other.AdditionalContext) &&
                   ClearAdaptiveData == other.ClearAdaptiveData;
        }

        #endregion

        #region . ErrorTokenWithContext .
        private static String ErrorTokenWithContext(string[] sentence, int index) {

            var errorString = new StringBuilder();

            // two token before
            if (index > 1)
                errorString.Append(sentence[index - 2]).Append(" ");

            if (index > 0)
                errorString.Append(sentence[index - 1]).Append(" ");

            // token itself
            errorString.Append("###");
            errorString.Append(sentence[index]);
            errorString.Append("###").Append(" ");

            // two token after
            if (index + 1 < sentence.Length)
                errorString.Append(sentence[index + 1]).Append(" ");

            if (index + 2 < sentence.Length)
                errorString.Append(sentence[index + 2]);

            return errorString.ToString();
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:NameSample"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                int hashCode = (AdditionalContext != null ? AdditionalContext.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ClearAdaptiveData.GetHashCode();
                hashCode = (hashCode * 397) ^ (Names != null ? Names.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Sentence != null ? Sentence.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion

        #region + Parse .
        /// <summary>
        /// Parses the specified tagged tokens.
        /// </summary>
        /// <param name="taggedTokens">The tagged tokens.</param>
        /// <param name="ClearAdaptiveData">if set to <c>true</c> the clear adaptive data.</param>
        /// <returns>The parsed <see cref="NameSample"/> object.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Found unexpected annotation while handling a name sequence: [Token]
        /// or
        /// Missing a name type: [Token]
        /// or
        /// Found unexpected annotation: [Token]
        /// </exception>
        public static NameSample Parse(string taggedTokens, bool ClearAdaptiveData) {
            return Parse(taggedTokens, DefaultType, ClearAdaptiveData);
        }

        /// <summary>
        /// Parses the specified tagged tokens.
        /// </summary>
        /// <param name="taggedTokens">The tagged tokens.</param>
        /// <param name="defaultType">The default type.</param>
        /// <param name="ClearAdaptiveData">if set to <c>true</c> to clear adaptive data.</param>
        /// <returns>The parsed <see cref="NameSample"/> object.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Found unexpected annotation while handling a name sequence: [Token]
        /// or
        /// Missing a name type: [Token]
        /// or
        /// Found unexpected annotation: [Token]
        /// </exception>
        public static NameSample Parse(string taggedTokens, string defaultType, bool ClearAdaptiveData) {
            var parts = WhitespaceTokenizer.Instance.Tokenize(taggedTokens);

            var tokenList = new List<string>(parts.Length);
            var nameList = new List<Span>();

            string nameType = defaultType;
            int startIndex = -1;
            int wordIndex = 0;

            // we check if at least one name has the a type. If no one has, we will
            // leave the NameType property of NameSample null.
            bool catchingName = false;

            for (int pi = 0; pi < parts.Length; pi++) {
                var match = StartTagRegex.Match(parts[pi]);
                if (match.Success) {
                    if (catchingName) {
                        throw new InvalidOperationException(
                            "Found unexpected annotation while handling a name sequence: " +
                            ErrorTokenWithContext(parts, pi));
                    }
                    catchingName = true;
                    startIndex = wordIndex;

                    if (match.Groups[2].Success) {
                        var nameTypeFromSample = match.Groups[2].Value;
                        if (nameTypeFromSample == string.Empty) {
                            throw new InvalidOperationException("Missing a name type: " + ErrorTokenWithContext(parts, pi));
                        }
                        nameType = nameTypeFromSample;
                    }
                } else if (parts[pi] == NameSampleStream.END_TAG) {
                    if (catchingName == false) {
                        throw new InvalidOperationException("Found unexpected annotation: " +
                                                            ErrorTokenWithContext(parts, pi));
                    }
                    catchingName = false;
                    // create name
                    nameList.Add(new Span(startIndex, wordIndex, nameType));

                } else {
                    tokenList.Add(parts[pi]);
                    wordIndex++;
                }
            }
            return new NameSample(tokenList.ToArray(), nameList.ToArray(), ClearAdaptiveData);
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            var result = new StringBuilder();

            // If adaptive data must be cleared insert an empty line
            // before the sample sentence line
            if (ClearAdaptiveData)
                result.Append("\n");

            for (var tokenIndex = 0; tokenIndex < Sentence.Length; tokenIndex++) {
                // token

                foreach (var name in Names) {
                    if (name.Start == tokenIndex) {
                        // check if nameTypes is null, or if the nameType for this specific
                        // entity is empty. If it is, we leave the nameType blank.
                        if (name.Type == null) {
                            result.Append(NameSampleStream.START_TAG).Append(' ');
                        } else {
                            result.Append(NameSampleStream.START_TAG_PREFIX).Append(name.Type).Append("> ");
                        }
                    }

                    if (name.End == tokenIndex) {
                        result.Append(NameSampleStream.END_TAG).Append(' ');
                    }
                }

                result.Append(Sentence[tokenIndex]).Append(' ');
            }

            if (Sentence.Length > 1)
                result.Remove(result.Length - 1, 1);


            foreach (var name in Names) {
                if (name.End == Sentence.Length) {
                    result.Append(' ').Append(NameSampleStream.END_TAG);
                }
            }

            return result.ToString();
        }

        #endregion

    }
}