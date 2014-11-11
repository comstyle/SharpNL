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

namespace SharpNL.Formats.Ptb.Lang.pt {
    /// <summary>
    /// Represents a portuguese Penn Treebank resolver.
    /// </summary>
    public class Resolver : DefaultResolver {

        private static Resolver instance;

        private static readonly Regex punctRegex;

        static Resolver() {
            punctRegex = new Regex("^([-!$%^&*()_+|~=`{}\\[\\]:\";'<>?,.\\/]+)\\)", RegexOptions.Compiled);
        }

        /// <summary>
        /// Gets the resolver instance.
        /// </summary>
        /// <value>The resolver instance.</value>
        public new static Resolver Instance {
            get { return instance ?? (instance = new Resolver()); }
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Resolver"/> class from being created.
        /// </summary>
        private Resolver() { }

        /// <summary>
        /// Returns the node type for the specified portion of the parse or a <c>null</c> if
        /// the portion of the parse string does not represent a type.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <param name="useFunctionTags">if set to <c>true</c> the functions tags will be included.</param>
        /// <returns>The node type.</returns>
        /// 
        public override string GetType(string rest, bool useFunctionTags) {

            if (rest.StartsWith("\u00AB")) // « (left-pointing double angle quotation mark)
                return "\u00AB";

            if (rest.StartsWith("\u00BB")) // »	(right-pointing double angle quotation mark)
                return "\u00BB";

            /*
             * From http://www.linguateca.pt/floresta/doc/MainDocumentationFlorestaSintactica.html (C. Punctuation)
             * 
             * There is not a standard, robust set of rules for punctuation yet, and there isn't so far a thorough 
             * discussion on the topic. However, some regularities can already be documented. 
             * 
             * Final sentence punctuation (full stop, exclamation/interrogation mark, colon, semi-colon): top level 
             * 
             * Inner sentence punctuation: 
             *   punctuation chunking constituents (double commas, parentheses, quotes, hyphens) should be placed at
             *   the same level (i.e. with the same indentation) as the "chunked" constituent or word. 
             *   The opening punctuation of a chunk is placed before (i.e. outside) the highest node in the chunk. 
             *   The same holds for separators (hyphen, colon, comma), which are placed at the same level 
             *   (i.e. with the same indentation) as the units they separate. Here, too, node lines go with what's 
             *   inside the node, separators are kept outside nodes. 
            */

            var match = punctRegex.Match(rest);
            if (match.Success) {
                switch (match.Groups[1].Value) {
                    case "-":
                        return "-";
                    case "--":
                        return "--";
                    case ":":
                        return ":";
                    case ",":
                        return ",";

                    case ";":
                    case ".":
                    case "..":
                    case "...":
                        return ".";
                    case "!":
                    case "!!":
                    case "!!!":
                        return "!";
                    case "?":
                    case "??":
                    case "???":
                        return "?";
                    default:
                        return "SYM";
                }
            }

            if (rest.StartsWith("-- "))
                return "--";

            return base.GetType(rest, useFunctionTags);
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
        public override string GetToken(string rest) {
            if (rest.StartsWith("\u00AB")) // « (left-pointing double angle quotation mark)
                return "\u00AB";

            if (rest.StartsWith("\u00BB")) // »	(right-pointing double angle quotation mark)
                return "\u00BB";

            var match = punctRegex.Match(rest);

            return match.Success 
                ? match.Groups[1].Value
                : base.GetToken(rest);
           
        }
    }
}