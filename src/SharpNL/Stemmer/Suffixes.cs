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
using System.Linq;
using System.Text;

namespace SharpNL.Stemmer {
    /// <summary>
    /// Represents a group of suffixes. This class cannot be inherited.
    /// </summary>
    public sealed class Suffixes {

        private readonly HashSet<string> suffixes;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="Suffixes"/> class with the specified suffixes.
        /// </summary>
        /// <param name="values">The suffixes of this instance.</param>
        public Suffixes(params string[] values) {
            suffixes = new HashSet<string>(values);
        }
        #endregion

        #region . IsMatch .
        /// <summary>
        /// Indicates whether the <paramref name="input"/> string finds a suffix in this instance.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <returns><c>true</c> if the specified input matches with a suffix in this instance; otherwise, <c>false</c>.</returns>
        public bool IsMatch(string input) {
            return suffixes.Any(cur => input.EndsWith(cur, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region . TryMatch .
        /// <summary>
        /// Searches the specified <paramref name="input"/> string for the first occurrence of 
        /// the suffixes specified in the <see cref="Suffixes"/> constructor.
        /// </summary>
        /// <param name="input">The string to search for a match.</param>
        /// <param name="suffix">When this method returns, contains the matched suffix, if the conversion succeeded or a <c>null</c> value if fails.</param>
        /// <returns><c>true</c> if the search finds a match, <c>false</c> otherwise.</returns>
        public bool TryMatch(string input, out string suffix) {
            if (string.IsNullOrEmpty(input))
                goto nop;
            
            foreach (var cur in suffixes) {
                if (input.EndsWith(cur, StringComparison.OrdinalIgnoreCase)) {
                    suffix = cur;
                    return true;
                }
            }
        nop:
            suffix = null;
            return false;
        }
        #endregion

        #region . GetMatches .
        /// <summary>
        /// Gets all the matched suffixes from the <paramref name="input"/> string.
        /// </summary>
        /// <param name="input">The string to search for the matches.</param>
        /// <returns>A array of matching suffixes.</returns>
        public string[] GetMatches(string input) {
            return Enumerable.ToArray(
                suffixes.Where(cur => input.EndsWith(cur, StringComparison.OrdinalIgnoreCase)));
        }
        #endregion

    }
}