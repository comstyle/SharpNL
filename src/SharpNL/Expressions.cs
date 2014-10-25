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
    /// Helper class for regular expressions.
    /// </summary>
    internal static class Expressions {
        public enum Expression {
            Space,
            Underline
        }

        private static readonly Dictionary<Expression, Regex> lib;

        static Expressions() {
            lib = new Dictionary<Expression, Regex> {
                {Expression.Space, new Regex("\\s+", RegexOptions.Compiled)},
                {Expression.Underline, new Regex("[_]+", RegexOptions.Compiled)}
            };
        }

        public static string[] RegExSplit(this string value, Expression expression, bool removeEmpty = true) {
            var tokens = lib[expression].Split(value);

            if (removeEmpty)
                return tokens.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return tokens;
        }

        public static string RegExReplace(this string value, Expression expression, string replacement) {
            return lib[expression].Replace(value, replacement);
        }

        public static string RegExMatch(this string value, string pattern, int group = 1, RegexOptions options = RegexOptions.IgnoreCase) {
            var regex = new Regex(pattern, options);
            var match = regex.Match(value);
            if (match.Success && match.Groups[group].Success) {
                return match.Groups[group].Value;
            }
            return null;
        }

        
    }
}