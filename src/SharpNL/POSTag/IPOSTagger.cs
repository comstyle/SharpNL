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
    /// The interface for part of speech taggers.
    /// </summary>
    public interface IPOSTagger {

        /// <summary>
        /// Assigns the sentence of tokens pos tags.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be tagged.</param>
        /// <returns>an array of pos tags for each token provided in sentence.</returns>
        /// 
        string[] Tag(string[] sentence);

        /// <summary>
        /// Assigns the sentence of tokens pos tags.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be tagged.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>an array of pos tags for each token provided in sentence.</returns>
        string[] Tag(string[] sentence, object[] additionalContext);


        /// <summary>
        /// Returns the top k sequences for the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be evaluated.</param>
        /// <returns>The top k sequences for the specified sentence.</returns>
        Sequence[] TopKSequences(string[] sentence);

        /// <summary>
        /// Returns the top k sequences for the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be evaluated.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The top k sequences for the specified sentence.</returns>
        Sequence[] TopKSequences(string[] sentence, object[] additionalContext);
    }
}