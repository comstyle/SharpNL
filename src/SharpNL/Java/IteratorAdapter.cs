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
using System.Collections.Generic;

namespace SharpNL.Java {
    /// <summary>
    /// Represents a <see cref="T:IIterator{T}"/> adapter from an <see cref="T:IEnumerator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    internal class IteratorAdapter<T> : IIterator<T> {

        private readonly IEnumerator<T> enumerator;
        private bool? hasNext;

        public IteratorAdapter(IEnumerable<T> enumerable) {
            enumerator = enumerable.GetEnumerator();
        }

        #region . HasNext .
        /// <summary>
        /// Determines whether the iteration has more elements.
        /// </summary>
        /// <returns><c>true</c> if the iteration has more elements.; otherwise, <c>false</c>.</returns>
        public bool HasNext() {
            if (hasNext == null) {
                hasNext = enumerator.MoveNext();
            }
            return hasNext.Value;
        }
        #endregion

        #region . Next .
        /// <summary>
        /// Gets the next element in the iteration.
        /// </summary>
        /// <returns>The next element in the iteration.</returns>
        public T Next() {
            if (!HasNext()) {
                throw new InvalidOperationException();
            }

            hasNext = null;

            return enumerator.Current;
        }
        #endregion

    }
}