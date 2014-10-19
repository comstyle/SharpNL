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
using System.Runtime.CompilerServices;

namespace SharpNL.Text {
    /// <summary>
    /// Provides a entity resolution.
    /// </summary>
    public interface IEntityResolver {
        /// <summary>
        /// Resolves an entity by the given <paramref name="sentence" /> and <paramref name="span" />.
        /// </summary>
        /// <param name="language">The language of the document.</param>
        /// <param name="sentence">The sentence.</param>
        /// <param name="span">The entity span.</param>
        /// <returns>The implemented entity. if null the analyzer will ignore this entity.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        IEntity Resolve(string language, ISentence sentence, Span span);
    }
}