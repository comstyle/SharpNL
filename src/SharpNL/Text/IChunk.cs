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

namespace SharpNL.Text {
    /// <summary>
    /// Represents a chunk.
    /// </summary>
    public interface IChunk {

        #region + Properties .

        #region . End .
        /// <summary>
        /// Gets the chunk end position.
        /// </summary>
        /// <value>The chunk end position.</value>
        int End { get; }
        #endregion

        #region . HeadIndex .
        /// <summary>
        /// Gets or sets the index of the head. Default value is -1.
        /// </summary>
        /// <value>The index of the head.</value>
        int HeadIndex { get; set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the chunk length.
        /// </summary>
        /// <value>The chunk length.</value>
        int Length { get; }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets the chunk start position.
        /// </summary>
        /// <value>The chunk start position.</value>
        int Start { get; }
        #endregion

        #region . Tag .
        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <value>The chunk tag.</value>
        string Tag { get; }
        #endregion

        #region . Tokens .

        /// <summary>
        /// Gets the chunk tokens.
        /// </summary>
        /// <value>The chunk tokens.</value>
        IReadOnlyList<IToken> Tokens { get; }

        #endregion

        #endregion

    }
}