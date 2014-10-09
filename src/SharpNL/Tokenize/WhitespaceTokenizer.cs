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
using SharpNL.Java;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// This tokenizer uses white spaces to tokenize the input text.
    /// </summary>
    [JavaClass("opennlp.tools.tokenize.WhitespaceTokenizer")]
    public class WhitespaceTokenizer : AbstractTokenizer {
        static WhitespaceTokenizer() {
            Instance = new WhitespaceTokenizer();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="WhitespaceTokenizer"/> class from being created.
        /// </summary>
        /// <remarks>The the <see cref="Instance"/> to retrieve the instance.</remarks>
        private WhitespaceTokenizer() {}

        /// <summary>
        /// Gets the instance of the whitespace tokenizer.
        /// </summary>
        /// <value>The instance of the whitespace tokenizer.</value>
        /// <remarks>Use this static reference to retrieve an instance of the <see cref="WhitespaceTokenizer"/>.</remarks>
        public static WhitespaceTokenizer Instance { get; private set; }

        /// <summary>
        /// Finds the boundaries of atomic parts in a string.
        /// </summary>
        /// <param name="value">The string to be tokenized.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into s) for each token as the individuals array elements.</returns>
        public override Span[] TokenizePos(string value) {
            var tokStart = -1;
            var tokens = new List<Span>();
            var inTok = false;

            //gather up potential tokens
            var end = value.Length;
            for (var i = 0; i < end; i++) {
                if (char.IsWhiteSpace(value[i])) {
                    if (inTok) {
                        tokens.Add(new Span(tokStart, i));
                        inTok = false;
                        tokStart = -1;
                    }
                } else {
                    if (!inTok) {
                        tokStart = i;
                        inTok = true;
                    }
                }
            }

            if (inTok) {
                tokens.Add(new Span(tokStart, end));
            }

            return tokens.ToArray();
        }
    }
}