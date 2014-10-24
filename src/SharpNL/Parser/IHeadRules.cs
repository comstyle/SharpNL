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

using System.Collections.Generic;

namespace SharpNL.Parser {
    /// <summary>
    /// Interface for encoding the head rules associated with parsing.
    /// </summary>
    public interface IHeadRules {

        /// <summary>
        /// Returns the head constituent for the specified constituents of the specified type.
        /// </summary>
        /// <param name="constituents">The constituents which make up a constituent of the specified type.</param>
        /// <param name="type">The type of a constituent which is made up of the specified constituents.</param>
        /// <returns>The constituent which is the head.</returns>
        Parse GetHead(Parse[] constituents, string type);

        /// <summary>
        /// Gets a list with the punctuation tags. Attachment decisions for these tags will not be modeled.
        /// </summary>
        /// <value>The list of punctuation tags.</value>
        List<string> PunctuationTags { get; }
    }
}