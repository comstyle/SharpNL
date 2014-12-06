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

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpNL {
    /// <summary>
    /// Helper class for common regular expressions.
    /// </summary>
    internal static class Expressions {
        public enum Expression {
            Space,
            Underline
        }

        /// <summary>
        /// The regex library
        /// </summary>
        private static readonly Dictionary<Expression, Regex> lib;

        /// <summary>
        /// Initializes static members of the <see cref="Expressions"/> class.
        /// </summary>
        static Expressions() {
            lib = new Dictionary<Expression, Regex> {
                {Expression.Space, new Regex("\\s+", RegexOptions.Compiled)},
                {Expression.Underline, new Regex("[_]+", RegexOptions.Compiled)}
            };
        }

        #region . RegExSplit .
        /// <summary>
        /// Splits the input string at the positions defined by a specified regular expression and with the specified options.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="removeEmpty">if set to <c>true</c> the empty elements will be ignored.</param>
        /// <returns>An array of strings.</returns>
        public static string[] RegExSplit(this string value, Expression expression, bool removeEmpty = true) {
            var tokens = lib[expression].Split(value);

            return removeEmpty
                ? tokens.Where(x => !string.IsNullOrEmpty(x)).ToArray()
                : tokens;
        }
        #endregion

        #region . RegExReplace .
        /// <summary>
        /// Within a specified input string, replaces all strings that match a regular expression 
        /// with a specified replacement string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>
        /// A new string that is identical to the input string, except
        /// that a replacement string takes the place of each matched string.</returns>
        public static string RegExReplace(this string value, Expression expression, string replacement) {
            return lib[expression].Replace(value, replacement);
        }
        #endregion

        #region . RegExMatch .
        /// <summary>
        /// Searches the specified input string for the first occurrence of the 
        /// regular expression <paramref name="pattern"/>.
        /// </summary>
        /// <param name="value">The string to search for a match.</param>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="group">The result group name.</param>
        /// <param name="options">The regex options.</param>
        /// <returns>The result group value or a <c>null</c> value if no match.</returns>
        public static string RegExMatch(this string value, string pattern, int group = 1, RegexOptions options = RegexOptions.IgnoreCase) {
            var regex = new Regex(pattern, options);
            var match = regex.Match(value);
            if (match.Success && match.Groups[group].Success) {
                return match.Groups[group].Value;
            }
            return null;
        }
        #endregion
        
    }
}