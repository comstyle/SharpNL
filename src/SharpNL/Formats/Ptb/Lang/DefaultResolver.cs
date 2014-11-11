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

using System.Text.RegularExpressions;

namespace SharpNL.Formats.Ptb.Lang {
    public class DefaultResolver : IResolver {
        private static DefaultResolver instance;

        private const string BracketLRB = "(";
        private const string BracketRRB = ")";
        private const string BracketLCB = "{";
        private const string BracketRCB = "}";
        private const string BracketLSB = "[";
        private const string BracketRSB = "]";

        /// <summary>
        /// Gets the instance of the default resolver.
        /// </summary>
        /// <value>The instance of the default resolver.</value>
        public static DefaultResolver Instance {
            get { return instance ?? (instance = new DefaultResolver()); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResolver"/> class.
        /// </summary>
        protected DefaultResolver() { }

        /// <summary>
        /// The pattern used to find the base constituent label of a Penn Treebank labeled constituent.
        /// </summary>
        private static readonly Regex typePattern;

        /// <summary>
        /// The pattern used to find the function tags.
        /// </summary>
        private static readonly Regex funTypePattern;

        /// <summary>
        /// The patter used to identify tokens in Penn Treebank labeled constituents.
        /// </summary>
        private static readonly Regex tokenPattern;

        static DefaultResolver() {
            typePattern = new Regex("^([-]*[^ )=-]+)", RegexOptions.Compiled);

            funTypePattern = new Regex("^[-]*[^ )=-]+-([^ )=-]+)", RegexOptions.Compiled);

            tokenPattern = new Regex("^[^ ()]+ ([^ ()]+)\\s*\\)", RegexOptions.Compiled);
        }

        /// <summary>
        /// Encodes the escapes from token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The encoded string.</returns>
        protected internal static string EncodeToken(string token) {
            switch (token) {
                case BracketLRB:
                    return "-LRB-";
                case BracketRRB:
                    return "-RRB-";
                case BracketLCB:
                    return "-LCB-";
                case BracketRCB:
                    return "-RCB-";
                case BracketLSB:
                    return "-LSB-";
                case BracketRSB:
                    return "-RSB-";
                default:
                    return token;
            }
        }

        /// <summary>
        /// Decodes the string escapes from the token <c>(-LRB-; -RRB-; -LCB-; -RCB-; -LSB-; -RSB-)</c>.
        /// </summary>
        /// <param name="token">The token string.</param>
        /// <returns>The decoded string.</returns>
        protected internal static string DecodeToken(string token) {
            switch (token) {
                case "-LRB-":
                    return BracketLRB;
                case "-RRB-":
                    return BracketRRB;
                case "-LCB-":
                    return BracketLCB;
                case "-RCB-":
                    return BracketRCB;
                case "-LSB-":
                    return BracketLSB;
                case "-RSB-":
                    return BracketRSB;
                default:
                    return token;
            }
        }

        /// <summary>
        /// Returns the node type for the specified portion of the parse or a <c>null</c> if
        /// the portion of the parse string does not represent a type.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <param name="useFunctionTags">if set to <c>true</c> the functions tags will be included.</param>
        /// <returns>The node type.</returns>
        public virtual string GetType(string rest, bool useFunctionTags) {
            if (string.IsNullOrEmpty(rest))
                return null;

            if (rest.StartsWith("-LCB-"))
                return "-LCB-";

            if (rest.StartsWith("-RCB-"))
                return "-RCB-";

            if (rest.StartsWith("-LRB-"))
                return "-LRB-";

            if (rest.StartsWith("-RRB-"))
                return "-RRB-";

            if (rest.StartsWith("-RSB-"))
                return "-RSB-";

            if (rest.StartsWith("-LSB-"))
                return "-LSB-";

            if (rest.StartsWith("-NONE-"))
                return "-NONE-";

            var match = typePattern.Match(rest);
            if (!match.Success)
                return null;

            var type = match.Groups[1].Value;

            if (!useFunctionTags)
                return type;

            match = funTypePattern.Match(rest);
            return match.Success
                ? type + "-" + match.Groups[1].Value
                : type;
        }

        /// <summary>
        /// Returns the string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <returns>
        /// The string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </returns>
        public virtual string GetToken(string rest) {
            var match = tokenPattern.Match(rest);
            return match.Success
                ? DecodeToken(match.Groups[1].Value)
                : null;
        }

    }
}