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

namespace SharpNL.POSTag {
    /// <summary>
    /// The interface for a context generator for the POS Tagger.
    /// </summary>
    public interface IPOSContextGenerator : IBeamSearchContextGenerator<string> {

        /// <summary>
        /// Returns the context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
        /// </summary>
        /// <param name="index">The index of the token for which the context is provided.</param>
        /// <param name="tokens">The tokens in the sentence.</param>
        /// <param name="prevTags">The tags assigned to the previous words in the sentence.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for making a pos tag decision at the specified token index given the specified tokens and previous tags.</returns>
        new string[] GetContext(int index, string[] tokens, string[] prevTags, object[] additionalContext);

    }
}