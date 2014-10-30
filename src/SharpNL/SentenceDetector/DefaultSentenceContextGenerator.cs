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

using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Generate event contexts for maxent decisions for sentence detection.
    /// </summary>
    public class DefaultSentenceContextGenerator : ISentenceContextGenerator {

        protected StringBuilder buf;
        protected List<string> collectFeats;
        private readonly char[] eosCharacters;
        private readonly List<string> inducedAbbreviations;

        public DefaultSentenceContextGenerator(char[] eosCharacters) : this(new List<string>(), eosCharacters) { }

        public DefaultSentenceContextGenerator(List<string> inducedAbbreviations, char[] eosCharacters) {
            this.inducedAbbreviations = inducedAbbreviations;
            this.eosCharacters = eosCharacters;

            buf = new StringBuilder();
            collectFeats = new List<string>();

        }

        #region . EscapeChar .
        private static string EscapeChar(char value) {
            if (value == '\n') {
                return "<LF>";
            }
            if (value == '\r') {
                return "<CR>";
            }
            return char.ToString(value);
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
        /// <param name="eosChar">The EOS character been analyzed.</param>
        protected virtual void CollectFeatures(string prefix, string suffix, string previous, string next, char? eosChar) {
            buf.Append("x=");
            buf.Append(prefix);
            collectFeats.Add(buf.ToString());
            buf.Clear();

            if (!string.IsNullOrEmpty(prefix)) {
                collectFeats.Add(prefix.Length.ToString(CultureInfo.InvariantCulture));
                if (IsFirstUpper(prefix)) {
                    collectFeats.Add("xcap");
                }
                if (eosChar.HasValue && inducedAbbreviations.Contains(prefix + eosChar.Value)) {
                    collectFeats.Add("xabbrev");
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

        #region . GetContext .
        /// <summary>
        /// Returns an array of contextual features for the potential sentence boundary at the
        /// specified position within the specified string buffer.
        /// </summary>
        /// <param name="value">The value for which sentences are being determined.</param>
        /// <param name="position">An index into the specified string buffer when a sentence boundary may occur.</param>
        /// <returns>An array of contextual features for the potential sentence boundary at the specified position within the specified string buffer.</returns>
        public string[] GetContext(string value, int position) {
            string prefix;   // String preceding the eos character in the eos token.
            string suffix;   // String following the eos character in the eos token.
            string next;     // Space delimited token following token containing eos character.

            int lastIndex = value.Length - 1;

            { // compute space previous and space next features.
                if (position > 0 && char.IsWhiteSpace(value[position - 1])) {
                    collectFeats.Add("sp");
                }
                if (position < lastIndex && char.IsWhiteSpace(value[position + 1])) {
                    collectFeats.Add("sn");
                }
                collectFeats.Add("eos=" + EscapeChar(value[position]));
            }

            int prefixStart = PreviousSpaceIndex(value, position);

            int c = position;
            { // assign prefix, stop if you run into a period though otherwise stop at space
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
            int prevStart = PreviousSpaceIndex(value, prefixStart);

            // Space delimited token preceding token containing eos character.
            string previous = value.Substring(prevStart, prefixStart - prevStart).Trim();

            int suffixEnd = NextSpaceIndex(value, position, lastIndex);
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
            int nextEnd = NextSpaceIndex(value, suffixEnd + 1, lastIndex + 1);
            if (position == lastIndex) {
                suffix = string.Empty;
                next = string.Empty;
            } else {
                suffix = value.Substring(position + 1, suffixEnd - (position + 1)).Trim();
                next = value.Substring(suffixEnd + 1, nextEnd - (suffixEnd + 1)).Trim();

            }

            CollectFeatures(prefix, suffix, previous, next, value[position]);

            var context = collectFeats.ToArray();
            collectFeats.Clear();
            return context;
        }

        #endregion

        #region . IsFirstUpper .
        private static bool IsFirstUpper(string s) {
            return char.IsUpper(s[0]);
        }
        #endregion

        #region . NextSpaceIndex .
        /// <summary>
        /// Finds the index of the nearest space after a specified index.
        /// </summary>
        /// <param name="sb">The string buffer which contains the text being examined.</param>
        /// <param name="seek">The index to begin searching from.</param>
        /// <param name="lastIndex">The last index of the <paramref name="sb"/>.</param>
        /// <returns>The index which contains the nearest space.</returns>
        private static int NextSpaceIndex(string sb, int seek, int lastIndex) {
            seek++;
            while (seek < lastIndex) {
                if (char.IsWhiteSpace(sb[seek])) {
                    while (sb.Length > seek + 1 && char.IsWhiteSpace(sb[seek + 1])) {
                        seek++;
                    }

                    return seek;
                }
                seek++;
            }
            return lastIndex;
        }
        #endregion

        #region . PreviousSpaceIndex .
        /// <summary>
        /// Finds the index of the nearest space before a specified index which is not itself preceded by a space.
        /// </summary>
        /// <param name="sb">The string buffer which contains the text being examined.</param>
        /// <param name="seek">The index to begin searching from.</param>
        /// <returns>The index which contains the nearest space.</returns>
        private static int PreviousSpaceIndex(string sb, int seek) {
            seek--;
            while (seek > 0 && !char.IsWhiteSpace(sb[seek])) {
                seek--;
            }
            if (seek > 0 && char.IsWhiteSpace(sb[seek])) {
                while (seek > 0 && char.IsWhiteSpace(sb[seek - 1])) {
                    seek--;
                }
                return seek;
            }
            return 0;
        }
        #endregion

    }
}