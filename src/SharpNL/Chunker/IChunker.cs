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

namespace SharpNL.Chunker {
    /// <summary>
    /// The interface for chunkers which provide chunk tags for a sequence of tokens.
    /// </summary>
    public interface IChunker {
        /// <summary>
        /// Generates chunk tags for the given sequence returning the result in an array.
        /// </summary>
        /// <param name="tokens">an array of the tokens or words of the sequence.</param>
        /// <param name="tags">an array of the pos tags of the sequence.</param>
        /// <returns>an array of chunk tags for each token in the sequence.</returns>
        string[] Chunk(string[] tokens, string[] tags);


        /// <summary>
        /// Generates tagged chunk spans for the given sequence returning the result in a span array.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence.</param>
        /// <param name="tags">An array of the pos tags of the sequence.</param>
        /// <returns>An array of spans with chunk tags for each chunk in the sequence.</returns>
        Span[] ChunkAsSpans(string[] tokens, string[] tags);

        /// <summary>
        /// Returns the top k chunk sequences for the specified sentence with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence.</param>
        /// <param name="tags">The pos-tags for the specified sentence.</param>
        /// <returns>The top k chunk sequences for the specified sentence.</returns>
        Sequence[] TopKSequences(string[] tokens, string[] tags);


        /// <summary>
        /// Returns the top k chunk sequences for the specified sentence with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence.</param>
        /// <param name="tags">The pos-tags for the specified sentence.</param>
        /// <param name="minScore">A lower bound on the score of a returned sequence.</param>
        /// <returns>The top k chunk sequences for the specified sentence.</returns>
        Sequence[] TopKSequences(string[] tokens, string[] tags, double minScore);
    }
}