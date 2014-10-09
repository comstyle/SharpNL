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
//  Note: 
//   This class is based/inspired on code extracted from the CoGrOO (http://cogroo.sourceforge.net/)
//   under Apache V2 license.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SharpNL.Sentence.Language.pt {
    /// <summary>
    /// Generate event contexts for maxent decisions for sentence detection.
    /// </summary>
    public class PtSentenceContextGenerator : ISentenceContextGenerator {
        protected readonly List<string> collectFeats;

        private readonly char[] eosCharacters;
        private readonly List<string> inducedAbbreviations;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtSentenceContextGenerator"/> with no induced abbreviations.
        /// </summary>
        /// <param name="eosCharacters">The eos characters.</param>
        public PtSentenceContextGenerator(char[] eosCharacters) : this(new List<string>(), eosCharacters) {}

        /// <summary>
        /// Creates a new <see cref="T:SentenceContextGenerator"/> instance which uses the set of induced abbreviations.
        /// </summary>
        /// <param name="inducedAbbreviations">The induced abbreviations. Example: &quot;Mr.&quot;</param>
        /// <param name="eosCharacters">The eos characters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inducedAbbreviations"/>
        /// or
        /// <paramref name="eosCharacters"/>
        /// </exception>
        public PtSentenceContextGenerator(List<string> inducedAbbreviations, char[] eosCharacters) {
            if (inducedAbbreviations == null)
                throw new ArgumentNullException("inducedAbbreviations");

            if (eosCharacters == null)
                throw new ArgumentNullException("eosCharacters");

            this.inducedAbbreviations = inducedAbbreviations;
            this.eosCharacters = eosCharacters;

            collectFeats = new List<string>();
        }

        /// <summary>
        /// Returns an array of contextual features for the potential sentence boundary at the
        /// specified position within the specified string buffer.
        /// </summary>
        /// <param name="value">The value for which sentences are being determined.</param>
        /// <param name="position">An index into the specified string buffer when a sentence boundary may occur.</param>
        /// <returns>An array of contextual features for the potential sentence boundary at the specified position within the specified string buffer.</returns>
        public string[] GetContext(string value, int position) {
            // string preceding the eos character in the eos token.
            string prefix;

            // string following the eos character in the eos token.
            string suffix;

            // Space delimited token following token containing eos character.
            string next;

            var lastIndex = value.Length - 1;
            {
                // compute space previous and space next features.
                if (position > 0 && char.IsWhiteSpace(value[position - 1]))
                    collectFeats.Add("sp");
                if (position < lastIndex && char.IsWhiteSpace(value[position + 1]))
                    collectFeats.Add("sn");
                collectFeats.Add("eos=" + value[position]);
            }
            var prefixStart = PreviousSpaceIndex(value, position);

            var c = position;
            {
                // /assign prefix, stop if you run into a period though otherwise stop at
                // space
                while (--c > prefixStart) {
                    for (int eci = 0, ecl = eosCharacters.Length; eci < ecl; eci++) {
                        if (value[c] == eosCharacters[eci]) {
                            prefixStart = c;
                            c++; // this gets us out of while loop.
                            break;
                        }
                    }
                }
                prefix = value.Substring(prefixStart, position - prefixStart).Trim();
            }
            var prevStart = PreviousSpaceIndex(value, prefixStart);

            //Space delimited token preceding token containing eos character.
            var previous = value.Substring(prevStart, prefixStart - prevStart).Trim();

            var suffixEnd = NextSpaceIndex(value, position, lastIndex);
            {
                c = position;
                while (++c < suffixEnd) {
                    for (int eci = 0, ecl = eosCharacters.Length; eci < ecl; eci++) {
                        if (value[c] == eosCharacters[eci]) {
                            suffixEnd = c;
                            c--; // this gets us out of while loop.
                            break;
                        }
                    }
                }
            }
            var nextEnd = NextSpaceIndex(value, suffixEnd + 1, lastIndex + 1);
            if (position == lastIndex) {
                suffix = "";
                next = "";
            } else {
                suffix = value.Substring(position + 1, suffixEnd - (position + 1)).Trim();
                next = value.Substring(suffixEnd + 1, nextEnd - (suffixEnd + 1)).Trim();
            }

            CollectFeatures(prefix, suffix, previous, next, value[position]);
            var sentEnd = Math.Max(position + 1, suffixEnd);
            collectFeats.AddRange(
                GetSentenceContext(value.Substring(prefixStart, sentEnd - prefixStart), position - prefixStart));

            var context = collectFeats.ToArray();
            collectFeats.Clear();
            return context;
        }

        #region . AddCharPreds .

        private static void AddCharPreds(string key, char c, List<string> preds) {
            preds.Add(key + "=" + c);
            if (Char.IsLetter(c)) {
                preds.Add(key + "_alpha");
                if (char.IsUpper(c)) {
                    preds.Add(key + "_caps");
                }
            } else if (Char.IsDigit(c)) {
                preds.Add(key + "_num");
            } else if (char.IsWhiteSpace(c)) {
                preds.Add(key + "_ws");
            } else {
                if (c == '.' || c == '?' || c == '!') {
                    preds.Add(key + "_eos");
                } else if (c == ',' || c == ';' || c == ':') {
                    preds.Add(key + "_reos");
                } else if (c == '`' || c == '"' || c == '\'') {
                    preds.Add(key + "_quote");
                } else if (c == '[' || c == '{' || c == '(') {
                    preds.Add(key + "_lp");
                } else if (c == ']' || c == '}' || c == ')') {
                    preds.Add(key + "_rp");
                }
            }
        }

        #endregion

        #region . CollectFeatures .

        /// <summary>
        /// Determines some of the features for the sentence detector and adds them to list features.
        /// </summary>
        /// <param name="prefix">String preceding the eos character in the eos token.</param>
        /// <param name="suffix">String following the eos character in the eos token.</param>
        /// <param name="previous">Space delimited token preceding token containing eos character.</param>
        /// <param name="next">Space delimited token following token containing eos character.</param>
        /// <param name="eosChar">The eos character.</param>
        protected void CollectFeatures(string prefix, string suffix, string previous, string next, char eosChar) {
            var buf = new StringBuilder(25);

            buf.Append("x=");
            buf.Append(prefix);
            collectFeats.Add(buf.ToString());
            buf.Clear();
            if (!string.IsNullOrEmpty(prefix)) {
                collectFeats.Add(prefix.Length.ToString(CultureInfo.InvariantCulture));
                if (IsFirstUpper(prefix)) {
                    collectFeats.Add("xcap");
                }
                if (inducedAbbreviations.Contains(prefix + eosChar)) {
                    collectFeats.Add("xabbrev");
                }
                var c = prefix[0];
                if (prefix.Length == 1 && char.IsLetter(c) && Char.IsUpper(c) && eosChar == '.') {
                    // looks like name abb
                    collectFeats.Add("xnabb");
                }
            }

            buf.Append("v=");
            buf.Append(previous);
            collectFeats.Add(buf.ToString());
            buf.Clear();

            if (!string.IsNullOrEmpty(previous)) {
                if (IsFirstUpper(previous)) {
                    collectFeats.Add("vcap");
                }
                if (inducedAbbreviations.Contains(previous)) {
                    collectFeats.Add("vabbrev");
                }
            }

            buf.Append("s=");
            buf.Append(suffix);
            collectFeats.Add(buf.ToString());
            buf.Clear();
            if (!string.IsNullOrEmpty(suffix)) {
                if (IsFirstUpper(suffix)) {
                    collectFeats.Add("scap");
                }
                if (inducedAbbreviations.Contains(suffix)) {
                    collectFeats.Add("sabbrev");
                }
            }

            buf.Append("n=");
            buf.Append(next);
            collectFeats.Add(buf.ToString());
            buf.Clear();

            if (!string.IsNullOrEmpty(next)) {
                if (IsFirstUpper(next)) {
                    collectFeats.Add("ncap");
                }
                if (inducedAbbreviations.Contains(next)) {
                    collectFeats.Add("nabbrev");
                }
            }
        }

        #endregion

        #region . GetSentenceContext .

        private static IEnumerable<string> GetSentenceContext(string sentence, int index) {
            var preds = new List<string>();

            if (index > 0) {
                AddCharPreds("p1", sentence[index - 1], preds);
                if (index > 1) {
                    AddCharPreds("p2", sentence[index - 2], preds);
                    preds.Add("p21=" + sentence[index - 2] + sentence[index - 1]);
                } else {
                    preds.Add("p2=bok");
                }
                preds.Add("p1f1=" + sentence[index - 1] + sentence[index]);
            } else {
                preds.Add("p1=bok");
            }

            // addCharPreds("f1", sentence[index), preds);
            if (index + 1 < sentence.Length) {
                AddCharPreds("f2", sentence[index + 1], preds);
                preds.Add("f12=" + sentence[index] + sentence[index + 1]);
            } else {
                preds.Add("f2=bok");
            }
            if (sentence[0] == '&'
                && sentence[sentence.Length - 1] == ';') {
                preds.Add("cc"); // character code
            }

            return preds;
        }

        #endregion

        #region . IsFirstUpper .

        /// <summary>
        /// Determines whether the first character of specified string is categorized as an uppercase letter.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the first character of specified string is categorized as an uppercase letter; otherwise, <c>false</c>.</returns>
        private static bool IsFirstUpper(string value) {
            if (string.IsNullOrEmpty(value))
                return false;

            return char.IsUpper(value[0]);
        }

        #endregion

        #region . NextSpaceIndex .

        /// <summary>
        /// Finds the index of the nearest space after a specified index.
        /// </summary>
        /// <param name="value">The string buffer which contains the text being examined.</param>
        /// <param name="seek">The index to begin searching from.</param>
        /// <param name="lastIndex">The highest index of the <paramref name="value"/>.</param>
        /// <returns>The index which contains the nearest space.</returns>
        private static int NextSpaceIndex(string value, int seek, int lastIndex) {
            seek++;
            while (seek < lastIndex) {
                if (char.IsWhiteSpace(value[seek])) {
                    while (value.Length > seek + 1 && char.IsWhiteSpace(value[seek + 1]))
                        seek++;
                    return seek;
                }
                seek++;
            }
            return lastIndex;
        }

        #endregion

        #region . PreviousSpaceIndex .

        /// <summary>
        /// Finds the index of the nearest space before a specified index which is not
        /// itself preceded by a space.
        /// </summary>
        /// <param name="value">The string buffer which contains the text being examined.</param>
        /// <param name="seek">The index to begin searching from.</param>
        /// <returns>The index which contains the nearest space.</returns>
        private static int PreviousSpaceIndex(string value, int seek) {
            seek--;
            while (seek > 0 && !char.IsWhiteSpace(value[seek])) {
                seek--;
            }
            if (seek > 0 && char.IsWhiteSpace(value[seek])) {
                while (seek > 0 && char.IsWhiteSpace(value[seek - 1]))
                    seek--;
                return seek;
            }
            return 0;
        }

        #endregion

    }
}