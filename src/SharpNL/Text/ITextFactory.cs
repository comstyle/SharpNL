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

using System.Collections.Generic;

namespace SharpNL.Text {
    /// <summary>
    /// A factory that provides text resources from any implementation.
    /// </summary>
    public interface ITextFactory {

        #region . CreateCategory .
        /// <summary>
        /// Creates the <see cref="ICategory"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="dict">The scored dictionary of categories.</param>
        /// <returns>The new <see cref="ICategory"/> object or a <c>null</c> value if the category is invalid or should be ignored.</returns>
        ICategory CreateCategory(ISentence sentence, Dictionary<string, double> dict);
        #endregion

        #region . CreateChunk .
        /// <summary>
        /// Creates the <see cref="IChunk"/> object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The chunk span.</param>
        /// <returns>The created <see cref="IChunk"/> object or a <c>null</c> value if the chunk is invalid or not needed.</returns>
        IChunk CreateChunk(ISentence sentence, Span span);
        #endregion

        #region . CreateDocument .
        /// <summary>
        /// Creates the document object.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="text">The text.</param>
        /// <returns>The created <see cref="IDocument"/> object or a <c>null</c> value if the document is invalid or not needed.</returns>
        IDocument CreateDocument(string language, string text);
        #endregion

        #region . CreateEntity .
        /// <summary>
        /// Creates an entity object.
        /// </summary>
        /// <param name="sentence">The sentence where the entity is present.</param>
        /// <param name="span">The entity span.</param>
        /// <returns>The new <see cref="IEntity"/> object or a <c>null</c> value if the entity should be ignored.</returns>
        IEntity CreateEntity(ISentence sentence, Span span);
        #endregion

        #region . CreateSentence .
        /// <summary>
        /// Creates a sentence object.
        /// </summary>
        /// <param name="span">The sentence span.</param>
        /// <param name="document">The document.</param>
        /// <returns>The created <see cref="ISentence" /> object or a <c>null</c> value if the sentence should be skipped.</returns>
        ISentence CreateSentence(Span span, IDocument document);
        #endregion

        #region . CreateToken .
        /// <summary>
        /// Creates an token object.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The span.</param>
        /// <param name="lexeme">The lexeme.</param>
        /// <returns>The created <see cref="IToken" /> object or a <c>null</c> value if the token is invalid or should be ignored.</returns>
        IToken CreateToken(ISentence sentence, Span span, string lexeme);
        #endregion

        #region . WordNet .

        /// <summary>
        /// Gets the WordNet instance.
        /// </summary>
        /// <returns>A WordNet instance or a <c>null</c> value if the WordNet is not necessary.</returns>
        WordNet.WordNet WordNet { get; }

        #endregion

    }
}