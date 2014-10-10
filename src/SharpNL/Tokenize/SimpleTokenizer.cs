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
    [TypeClass("opennlp.tools.tokenize.SimpleTokenizer")]
    public class SimpleTokenizer : AbstractTokenizer {

        /// <summary>
        /// Gets the instance of the simple tokenizer.
        /// </summary>
        /// <value>The instance of the simple tokenizer.</value>
        /// <remarks>Use this static reference to retrieve an instance of the <see cref="SimpleTokenizer"/>.</remarks>
        public static SimpleTokenizer Instance { get; private set; }

        static SimpleTokenizer() {
            Instance = new SimpleTokenizer();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="SimpleTokenizer"/> class from being created.
        /// </summary>
        /// <remarks>Use the <see cref="Instance"/> to retrieve the <see cref="SimpleTokenizer"/> </remarks>
        private SimpleTokenizer() { }

        /// <summary>
        /// Finds the boundaries of atomic parts in a string.
        /// </summary>
        /// <param name="value">The string to be tokenized.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into s) for each token as the individuals array elements.</returns>
        public override Span[] TokenizePos(string value) {
            
            var charType = CharacterEnum.Whitespace;
            var state = CharacterEnum.Whitespace;

            var tokens = new List<Span>();

            var sl = value.Length;
            int start = -1;
            char? pc = null;

            for (int ci = 0; ci < sl; ci++) {
                var c = value[ci];
                if (char.IsWhiteSpace(c)) 
                    charType = CharacterEnum.Whitespace;
                else if (char.IsLetter(c))
                    charType = CharacterEnum.Alphabetic;
                else if (char.IsDigit(c))
                    charType = CharacterEnum.Numeric;
                else
                    charType = CharacterEnum.Other;

                if (state == CharacterEnum.Whitespace) {
                    if (charType != CharacterEnum.Whitespace)
                        start = ci;
                } else {
                    if (charType != state || charType == CharacterEnum.Other && (!pc.HasValue || c != pc.Value)) {
                        tokens.Add(new Span(start, ci));
                        start = ci;
                    }
                }
                state = charType;
                pc = c;
            }
            if (charType != CharacterEnum.Whitespace)
                tokens.Add(new Span(start, sl));

            return tokens.ToArray();
        }
    }
}