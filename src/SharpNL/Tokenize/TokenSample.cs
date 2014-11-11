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
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// A <see cref="TokenSample"/> is text with token spans.
    /// </summary>
    /// <seealso cref="Span"/>
    public class TokenSample {
        /// <summary>
        /// The default separator
        /// </summary>
        public const string DefaultSeparator = "<SPLIT>";

        public TokenSample(string text, Span[] tokenSpans) {
            if (text == null)
                throw new ArgumentNullException("text");

            if (tokenSpans == null)
                throw new ArgumentNullException("tokenSpans");

            foreach (var span in tokenSpans) {
                if (span.Start < 0 ||
                    span.Start > text.Length ||
                    span.End > text.Length ||
                    span.End < 0)
                    throw new ArgumentOutOfRangeException("tokenSpans",
                        string.Format("Span {0} is out of bounds, text length: {1}", span, text.Length));
            }

            Text = text;
            TokenSpans = tokenSpans;
            Separator = DefaultSeparator;
        }

        public TokenSample(IDetokenizer detokenizer, string[] tokens) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            var sb = new StringBuilder();

            var operations = detokenizer.Detokenize(tokens);

            var tokenSpans = new List<Span>();

            for (var i = 0; i < operations.Length; i++) {
                if (i > 0 && !IsMergeToLeft(operations[i]) && !IsMergeToRight(operations[i - 1])) {
                    sb.Append(' ');
                }
                var beginIndex = sb.Length;

                sb.Append(tokens[i]);
                tokenSpans.Add(new Span(beginIndex, sb.Length));
            }

            Text = sb.ToString();
            TokenSpans = tokenSpans.ToArray();
            Separator = DefaultSeparator;
        }

        #region + Properties .

        #region . Separator .

        /// <summary>
        /// Gets or sets the separator chars.
        /// </summary>
        /// <value>The separator chars.</value>
        public string Separator { get; protected set; }

        #endregion

        #region . Text .

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        #endregion

        #region . TokenSpans .

        /// <summary>
        /// Gets the token spans.
        /// </summary>
        /// <value>The token spans.</value>
        public Span[] TokenSpans { get; private set; }

        #endregion

        #endregion

        #region . AddToken .

        private static void AddToken(StringBuilder sample, List<Span> tokenSpans, string token, bool isNextMerged) {
            var start = sample.Length;
            sample.Append(token);

            tokenSpans.Add(new Span(start, sample.Length));

            if (!isNextMerged)
                sample.Append(" ");
        }

        #endregion

        #region . Equals .

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if if the specified object is equal to the current object<c>false</c> otherwise.</returns>
        protected bool Equals(TokenSample other) {
            return Text == other.Text &&
                   TokenSpans.SequenceEqual(other.TokenSpans);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TokenSample) obj);
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
                return ((Text != null ? Text.GetHashCode() : 0)*397) ^
                       (TokenSpans != null ? TokenSpans.GetHashCode() : 0);
            }
        }

        #endregion

        #region . IsMergeToRight .

        internal static bool IsMergeToRight(DetokenizationOperation operation) {
            return operation == DetokenizationOperation.MergeToRight ||
                   operation == DetokenizationOperation.MergeBoth;
        }

        #endregion

        #region . IsMergeToLeft .

        internal static bool IsMergeToLeft(DetokenizationOperation operation) {
            return operation == DetokenizationOperation.MergeToLeft ||
                   operation == DetokenizationOperation.MergeBoth;
        }

        #endregion

        #region . Parse .

        public static TokenSample Parse(string sampleString, string separator) {
            if (sampleString == null)
                throw new ArgumentNullException("sampleString");

            if (separator == null)
                throw new ArgumentNullException("separator");

            var whitespaceTokenSpans = WhitespaceTokenizer.Instance.TokenizePos(sampleString);

            // Pre-allocate 20% for newly created tokens
            var realTokenSpans = new List<Span>((int) (whitespaceTokenSpans.Length*1.2d));

            var sb = new StringBuilder();
            foreach (var span in whitespaceTokenSpans) {
                var whitespaceToken = span.GetCoveredText(sampleString);
                var wasTokenReplaced = false;

                var tokStart = 0;
                int tokEnd;
                while ((tokEnd = whitespaceToken.IndexOf(separator, tokStart, StringComparison.Ordinal)) > -1) {
                    var token = whitespaceToken.Substring(tokStart, tokEnd - tokStart);

                    AddToken(sb, realTokenSpans, token, true);

                    tokStart = tokEnd + separator.Length;
                    wasTokenReplaced = true;
                }

                if (wasTokenReplaced) {
                    // If the token contains the split chars at least once
                    // a span for the last token must still be added
                    var token = whitespaceToken.Substring(tokStart);

                    AddToken(sb, realTokenSpans, token, false);
                } else {
                    // If it does not contain the split chars at lest once
                    // just copy the original token span

                    AddToken(sb, realTokenSpans, whitespaceToken, false);
                }
            }

            return new TokenSample(sb.ToString(), realTokenSpans.ToArray());
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current token sample.
        /// </summary>
        /// <returns>
        /// A string that represents the current token sample.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            var lastEndIndex = -1;
            foreach (var token in TokenSpans) {
                if (lastEndIndex != -1) {
                    // If there are no chars between last token
                    // and this token insert the separator chars
                    // otherwise insert a space

                    var separator = lastEndIndex == token.Start ? Separator : " ";
                    sb.Append(separator);
                }

                sb.Append(token.GetCoveredText(Text));

                lastEndIndex = token.End;
            }

            return sb.ToString();
        }

        #endregion
    }
}