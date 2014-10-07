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

namespace SharpNL.POSTag {
    /// <summary>
    /// Interface that allows <see cref="ITagDictionary"/> entries to be added and removed. 
    /// This can be used to induce the dictionary from training data.
    /// </summary>
    public interface IMutableTagDictionary : ITagDictionary {

        /// <summary>
        /// Associates the specified tags with the specified word. If the dictionary
        /// previously contained keys for the word, the old tags are replaced by the
        /// specified tags.
        /// </summary>
        /// <param name="word">The word with which the specified tags is to be associated.</param>
        /// <param name="tags">The tags to be associated with the specified word.</param>
        /// <returns>The previous tags associated with the word, or null if there was no mapping for word.</returns>
        string[] Put(string word, params string[] tags);

    }
}