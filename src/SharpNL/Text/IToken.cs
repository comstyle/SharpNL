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

using System;

namespace SharpNL.Text {
    /// <summary>
    /// Represents a token.
    /// </summary>
    public interface IToken {

        #region + Properties .

        #region . ChunkTag .

        /// <summary>
        /// Gets or sets the chunk tag.
        /// </summary>
        /// <value>The chunk tag.</value>
        string ChunkTag { get; set; }

        #endregion

        #region . End .

        /// <summary>
        /// Gets the token end position.
        /// </summary>
        /// <value>The token end position.</value>
        int End { get; }

        #endregion

        #region . Features .
        /// <summary>
        /// Gets the token features.
        /// </summary>
        /// <value>The token features.</value>
        string Features { get; }
        #endregion

        #region . IsChunkHead .
        /// <summary>
        /// Gets a value indicating whether this token is a chunk head.
        /// </summary>
        /// <value><c>true</c> if this token is a chunk head; otherwise, <c>false</c>.</value>
        bool IsChunkHead { get; }
        #endregion

        #region . Lemmas .
        /// <summary>
        /// Gets the token lemmas.
        /// </summary>
        /// <value>The token lemmas.</value>
        string[] Lemmas { get; }

        #endregion

        #region . Length .

        /// <summary>
        /// Gets the token length.
        /// </summary>
        /// <value>The token length.</value>
        int Length { get; }

        #endregion

        #region . Lexeme .

        /// <summary>
        /// Gets the lexeme.
        /// </summary>
        /// <value>The lexeme.</value>
        string Lexeme { get; }

        #endregion

        #region . POSTag .

        /// <summary>
        /// Gets the POSTag.
        /// </summary>
        /// <value>The POSTag.</value>
        string POSTag { get; set; }

        #endregion

        #region . POSTagProb .

        /// <summary>
        /// Gets the POSTag probability.
        /// </summary>
        /// <value>The POSTag probability.</value>
        double POSTagProbability { get; set; }

        #endregion

        #region . Probability .
        /// <summary>
        /// Gets or sets the token probability.
        /// </summary>
        /// <value>The token probability.</value>
        double Probability { get; set; }
        #endregion

        #region . Start .

        /// <summary>
        /// Gets the token start position.
        /// </summary>
        /// <value>The token start position.</value>
        int Start { get; }

        #endregion

        #region . Sentence .
        /// <summary>
        /// Gets the sentence.
        /// </summary>
        /// <value>The sentence.</value>
        ISentence Sentence { get; }
        #endregion

        #region . SyntacticTag .

        /// <summary>
        /// Gets the syntactic tag.
        /// </summary>
        /// <value>The syntactic tag.</value>
        string SyntacticTag { get; }

        #endregion

        #endregion

    }
}