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

using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// Represents an abstract tokenizer.
    /// </summary>
    public abstract class AbstractTokenizer : ITokenizer {

        /// <summary>
        /// Splits a string into its atomic parts.
        /// </summary>
        /// <param name="value">The string to be tokenized.</param>
        /// <returns>The String[] with the individual tokens as the array elements.</returns>
        public virtual string[] Tokenize(string value) {
            return Span.SpansToStrings(TokenizePos(value), value);
        }

        /// <summary>
        /// Finds the boundaries of atomic parts in a string.
        /// </summary>
        /// <param name="value">The string to be tokenized.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into s) for each token as the individuals array elements.</returns>
        public abstract Span[] TokenizePos(string value);
    }
}