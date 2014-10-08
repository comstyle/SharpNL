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
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Name finder based on a series of regular expressions.
    /// </summary>
    public class RegexNameFinder : ITokenNameFinder {
        private readonly string type;
        private readonly Regex[] regexList;
        private readonly Dictionary<string, Regex[]> mapping;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="RegexNameFinder"/> with the given regex mapping dictionary.
        /// </summary>
        /// <param name="mapping">The regex mapping dictionary.</param>
        /// <exception cref="System.ArgumentNullException">mapping</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The regex mapping is empty.</exception>
        public RegexNameFinder(Dictionary<string, Regex[]> mapping) {
            if (mapping == null)
                throw new ArgumentNullException("mapping");

            if (mapping.Count == 0)
                throw new ArgumentOutOfRangeException("mapping", @"The regex mapping is empty.");

            this.mapping = mapping;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexNameFinder"/> with the given expressions and annotation type.
        /// </summary>
        /// <param name="regexList">The regex list.</param>
        /// <param name="type">The annotation type.</param>
        /// <exception cref="System.ArgumentNullException">regexList</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The regex list is empty.</exception>
        public RegexNameFinder(Regex[] regexList, string type) {
            if (regexList == null)
                throw new ArgumentNullException("regexList");

            if (regexList.Length == 0)
                throw new ArgumentOutOfRangeException("regexList", @"The regex list is empty.");

            this.regexList = regexList;
            this.type = type;
        }
        #endregion

        #region + Find .

        /// <summary>
        /// Generates name tags for the given sequence, typically a sentence, returning token spans for any identified names.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence, typically a sentence.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string[] tokens) {
            var map = new Dictionary<int, int>();
            var sb = new StringBuilder(tokens.Length*10);

            for (var i = 0; i < tokens.Length; i++) {

                map[sb.Length] = i;

                sb.Append(tokens[i]);

                map[sb.Length] = i + 1;

                if (i < tokens.Length - 1)
                    sb.Append(' ');

            }

            var sentence = sb.ToString();
            var spans = new List<Span>();

            if (regexList == null && mapping != null) {
                foreach (var pair in mapping) {
                    foreach (var regex in pair.Value) {
                        var match = regex.Match(sentence);
                        while (match.Success) {
                            var end = match.Index + match.Length;
                            var tokenStart = map.ContainsKey(match.Index) ? map[match.Index] : -1;
                            var tokenEnd = map.ContainsKey(end) ? map[end] : -1;

                            if (tokenStart != -1 && tokenEnd != -1) {
                                spans.Add(new Span(tokenStart, tokenEnd, pair.Key));
                            }
                            match = match.NextMatch();
                        }
                    }
                }
                return spans.ToArray();
            }

            if (regexList == null)
                throw new InvalidOperationException(); // make the compiler happy... its impossible to be null here

            foreach (var regex in regexList) {
                var match = regex.Match(sentence);
                while (match.Success) {
                    var end = match.Index + match.Length;
                    var tokenStart = map.ContainsKey(match.Index) ? map[match.Index] : -1;
                    var tokenEnd = map.ContainsKey(end) ? map[end] : -1;

                    if (tokenStart != -1 && tokenEnd != -1) {
                        spans.Add(new Span(tokenStart, tokenEnd, type));
                    }
                    match = match.NextMatch();
                }

            }
            return spans.ToArray();
        }

        /// <summary>
        /// Generates name tags for the given sequence, returning token spans for any identified names.
        /// </summary>
        /// <param name="text">The text to be analized, typically a sentence.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string text) {
            var spans = new List<Span>();

            if (regexList == null && mapping != null) {
                foreach (var pair in mapping) {
                    foreach (var regex in pair.Value) {
                        var match = regex.Match(text);
                        while (match.Success) {
                            var end = match.Index + match.Length;

                            spans.Add(new Span(match.Index, end, pair.Key));

                            match = match.NextMatch();
                        }
                    }
                }
                return spans.ToArray();
            }

            if (regexList == null)
                throw new InvalidOperationException(); // make the compiler happy... its impossible to be null here

            foreach (var regex in regexList) {
                var match = regex.Match(text);
                while (match.Success) {
                    var end = match.Index + match.Length;

                    spans.Add(new Span(match.Index, end, type));

                    match = match.NextMatch();
                }

            }


            return spans.ToArray();
        }

        #endregion

        #region . ClearAdaptiveData .
        /// <summary>
        /// Forgets all adaptive data which was collected during previous calls to one of the find methods.
        /// </summary>
        /// <remarks>This method is typical called at the end of a document.</remarks>
        public void ClearAdaptiveData() {
            // nothing to clear.
        }
        #endregion

    }
}