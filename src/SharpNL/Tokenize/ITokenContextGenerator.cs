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

namespace SharpNL.Tokenize {
    /// <summary>
    /// Interface for <see cref="TokenizerME"/> context generators.
    /// </summary>
    public interface ITokenContextGenerator {

        /// <summary>
        /// Returns an array of features for the specified sentence string at the specified index.
        /// </summary>
        /// <param name="sentence">The string for a sentence.</param>
        /// <param name="index">The index to consider splitting as a token.</param>
        /// <returns>An array of features for the specified sentence string at the specified index.</returns>
        string[] GetContext(string sentence, int index);
    }
}