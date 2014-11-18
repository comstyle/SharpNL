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
using System.Linq;

namespace SharpNL.Text {
    /// <summary>
    /// Provides extensions methods for the SharpNL.Text namespace.
    /// </summary>
    public static class Extensions {

        /// <summary>
        /// Gets the tokens from the <see cref="ISentence"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <returns>
        /// A string array containing the tokens or a <c>null</c> value if the tokens are not available.
        /// </returns>
        public static string[] GetTokens(this ISentence sentence) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            return sentence.Tokens != null
                ? sentence.Tokens.Select(token => token.Lexeme).ToArray()
                : null;
        }

        /// <summary>
        /// Gets the part-of-speech tags from the <see cref="ISentence"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <returns>A string array containing the tokens or a <c>null</c> value if the tokens are not available.</returns>
        /// <exception cref="System.ArgumentNullException">sentence</exception>
        public static string[] GetTags(this ISentence sentence) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (sentence.Tokens == null || sentence.TagProbability.Equals(0d))
                return null;

            return sentence.Tokens != null
                ? sentence.Tokens.Select(token => token.POSTag).ToArray()
                : null;

        }
    }
}