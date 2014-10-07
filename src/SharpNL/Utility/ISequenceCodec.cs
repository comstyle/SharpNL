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

namespace SharpNL.Utility {
    /// <summary>
    /// Interface ISequenceCodec
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequenceCodec<T> {
        /// <summary>
        /// Decodes a sequence T objects into <see cref="Span"/> objects.
        /// </summary>
        /// <param name="objectList">The object list.</param>
        /// <returns>A array with the decoded objects.</returns>
        Span[] Decode(T[] objectList);

        /// <summary>
        /// Encodes the specified names.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="length">The length.</param>
        /// <returns>T[].</returns>
        T[] Encode(Span[] names, int length);


        /// <summary>
        /// Creates a sequence validator which can validate a sequence of outcomes.
        /// </summary>
        /// <returns>A sequence validator which can validate a sequence of outcomes.</returns>
        ISequenceValidator<T> CreateSequenceValidator();

        /// <summary>
        /// Checks if the outcomes of the model are compatible with the codec.
        /// </summary>
        /// <param name="outcomes">All possible model outcomes</param>
        /// <returns><c>true</c> if the outcomes of the model are compatible with the codec, <c>false</c> otherwise.</returns>
        bool AreOutcomesCompatible(string[] outcomes);
    }
}