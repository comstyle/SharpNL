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

namespace SharpNL.Inflecter {
    /// <summary>
    /// Represents a inflector that can manipulate a string pluralization. 
    /// </summary>
    public interface IInfleter {
        /// <summary>
        /// Returns the plural of a given <paramref name="word" />.
        /// </summary>
        /// <param name="word">The word to pluralize.</param>
        /// <param name="pos">The part-of-speech tag.</param>
        /// <returns>The pluralized word.</returns>
        string Pluralize(string word, string pos);


        /// <summary>
        /// Singularizes the specified <paramref name="word"/>.
        /// </summary>
        /// <param name="word">The word to singularize.</param>
        /// <param name="pos">The part-of-speech tag.</param>
        /// <returns>The singularized world.</returns>
        string Singularize(string word, string pos);

    }
}