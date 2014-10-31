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
using System.Text;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Represents an default end of sentence scanner.
    /// </summary>
    public class DefaultEndOfSentenceScanner : IEndOfSentenceScanner {

        private readonly char[] eosCharacters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEndOfSentenceScanner"/> with the specified eos delimiters.
        /// </summary>
        /// <param name="eosCharacters">The end of sentence characters.</param>
        public DefaultEndOfSentenceScanner(char[] eosCharacters) {

            this.eosCharacters = eosCharacters;

        }

        #region . GetEndOfSentenceCharacters .
        /// <summary>
        /// Returns an array of character which can indicate the end of a sentence.
        /// </summary>
        /// <returns>An array of character which can indicate the end of a sentence.</returns>
        public char[] GetEndOfSentenceCharacters() {
            return eosCharacters;
        }
        #endregion

        #region + GetPositions .
        /// <summary>
        /// Scans the specified string for sentence ending characters and
        /// returns their offsets.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The positions list.</returns>
        public List<int> GetPositions(string value) {
            return GetPositions(value.ToCharArray());
        }

        /// <summary>
        /// Scans the specified string for sentence ending characters and returns their offsets.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <returns>The positions list.</returns>
        public List<int> GetPositions(StringBuilder sb) {
            return GetPositions(sb.ToString().ToCharArray());
        }

        /// <summary>
        /// Scans the characters for sentence ending characters and returns their offsets.
        /// </summary>
        /// <param name="chars">The chars to scan.</param>
        /// <returns>Positions.</returns>
        public List<int> GetPositions(char[] chars) {
            var list = new List<int>();
            for (var i = 0; i < chars.Length; i++) {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var eos in eosCharacters) {
                    if (chars[i] != eos) continue;

                    list.Add(i);
                    break;
                }
            }

            return list;
        }
        #endregion

    }
}