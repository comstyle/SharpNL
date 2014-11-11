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

namespace SharpNL.Formats.Ptb {
    /// <summary>
    /// Interface that is used to resolve information of a Penn Treebank node.
    /// </summary>
    internal interface IResolver {
        /// <summary>
        /// Returns the node type for the specified portion of the parse or a <c>null</c> if
        /// the portion of the parse string does not represent a type.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <param name="useFunctionTags">if set to <c>true</c> the function tags will be included.</param>
        /// <returns>The node type.</returns>
        string GetType(string rest, bool useFunctionTags);

        /// <summary>
        /// Returns the string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <returns>
        /// The string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </returns>
        string GetToken(string rest);
    }
}