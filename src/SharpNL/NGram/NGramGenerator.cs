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
using System.Collections.Generic;
                  
namespace SharpNL.NGram {
    /// <summary>
    /// Generates an nGram, with optional separator, and returns the grams as a list of strings.
    /// </summary>
    public static class NGramGenerator {

        /// <summary>
        /// Creates an ngram separated by the separator param value i.e. a,b,c,d with n = 3 and separator = "-"
        /// would return a-b-c,b-c-d.
        /// </summary>
        /// <param name="input">The input tokens the output ngrams will be derived from.</param>
        /// <param name="count">The number of tokens as the sliding window.</param>
        /// <param name="separator">
        /// each char in each gram will be separated by this value if desired. 
        /// Pass a <see cref="string.Empty"/> value if no separator is desired.
        /// </param>
        /// <returns>The nGram list.</returns>
        public static List<string> Generate(List<string> input, int count, string separator) {
            var outGrams = new List<string>(input.Count * 3);
            for (var i = 0; i < input.Count; i++) {
                var gram = new StringBuilder();
                if (i + count <= input.Count) {
                    for (var x = i; x < (count + i); x++) {
                        gram.Append(input[x]).Append(separator);
                    }
                    if (gram.Length > 0)
                        outGrams.Add(gram.ToString(0, gram.Length - 1));
                }
            }
            return outGrams;
        }

        /// <summary>
        /// Generates an nGram based on a char[] input
        /// </summary>
        /// <param name="input">The input the array of chars to convert to nGram.</param>
        /// <param name="count">The number of grams (chars) that each output gram will consist of.</param>
        /// <param name="separator">
        /// each char in each gram will be separated by this value if desired. 
        /// Pass a <see cref="string.Empty"/> value if no separator is desired.
        /// </param>
        /// <returns>The nGram list.</returns>
        public static List<string> Generate(char[] input, int count, string separator) {
            var outGrams = new List<string>();
            for (var i = 0; i < input.Length - (count - 2); i++) {
                var gram = new StringBuilder();
                if ((i + count) <= input.Length) {
                    for (var x = i; x < (count + i); x++) {
                        gram.Append(input[x]).Append(separator);
                    }
                    if (gram.Length > 0)
                        outGrams.Add(gram.ToString(0, gram.Length - 1));
                }
            }
            return outGrams;
        }

    }
}