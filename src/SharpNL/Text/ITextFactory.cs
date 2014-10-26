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

namespace SharpNL.Text {

    /// <summary>
    /// A factory that provides text resources from any implementation.
    /// </summary>
    public interface ITextFactory {

        #region . CreateChunk .
        /// <summary>
        /// Creates the <see cref="IChunk"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The chunk span.</param>
        /// <returns>The <see cref="IChunk"/> representation.</returns>
        IChunk CreateChunk(ISentence sentence, Span span);
        #endregion

        #region . CreateDocument .
        /// <summary>
        /// Creates the document object.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="text">The text.</param>
        /// <returns>The created <see cref="IDocument"/> object.</returns>
        IDocument CreateDocument(string language, string text);
        #endregion

        #region . CreateEntity .
        /// <summary>
        /// Creates an entity object.
        /// </summary>
        /// <param name="sentence">The sentence where the entity is present.</param>
        /// <param name="span">The entity span.</param>
        /// <returns>The new <see cref="IEntity"/> object.</returns>
        IEntity CreateEntity(ISentence sentence, Span span);
        #endregion

        #region . CreateSentence .
        /// <summary>
        /// Creates a sentence object.
        /// </summary>
        /// <param name="span">The sentence span.</param>
        /// <param name="document">The document.</param>
        /// <returns>The created <see cref="ISentence" /> object.</returns>
        ISentence CreateSentence(Span span, IDocument document);
        #endregion

        #region . CreateToken .

        /// <summary>
        /// Creates an token object.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        /// <param name="lexeme">The lexeme.</param>
        /// <param name="probability">The token probability</param>
        /// <returns>The created <see cref="IToken"/> object.</returns>
        IToken CreateToken(int start, int end, string lexeme, double probability);
        #endregion

    }
}