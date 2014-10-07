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

namespace SharpNL.Parser {
    /// <summary>
    /// Interface for full-syntactic parsers.
    /// </summary>
    public interface IParser {
        /// <summary>
        /// Returns the specified number of parses or fewer for the specified tokens.
        /// </summary>
        /// <param name="tokens">A parse containing the tokens with a single parent node.</param>
        /// <param name="numParses">The number of parses desired.</param>
        /// <returns>The specified number of parses for the specified tokens.</returns>
        /// <remarks>
        /// The nodes within the returned parses are shared with other parses and therefore their 
        /// parent node references will not be consistent with their child node reference. 
        /// <see cref="SharpNL.Parser.Parse.Parent"/> can be used to make the parents consistent with a 
        /// particular parse, but subsequent calls to this property can invalidate the results of earlier
        /// calls.
        /// </remarks>
        Parse[] Parse(Parse tokens, int numParses);

        /// <summary>
        /// Returns a parse for the specified parse of tokens.
        /// </summary>
        /// <param name="tokens">The root node of a flat parse containing only tokens.</param>
        /// <returns>A full parse of the specified tokens or the flat chunks of the tokens if a full parse could not be found.</returns>
        Parse Parse(Parse tokens);
    }
}