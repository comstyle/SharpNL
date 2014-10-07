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

namespace SharpNL.Utility {
    /// <summary>
    /// Represents an generic object stream.
    /// </summary>
    /// <typeparam name="T">The type of the objects in this instance.</typeparam>
    public class GenericObjectStream<T> : IObjectStream<T> {
        private IEnumerable<T> enumerable;

        private IEnumerator<T> enumerator;

        public GenericObjectStream(IEnumerable<T> enumerable) {
            this.enumerable = enumerable;
        }

        public GenericObjectStream(params T[] elements) {
            enumerable = new List<T>(elements);
        }


        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            enumerable = null;
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public T Read() {
            if (enumerable == null)
                throw new ObjectDisposedException(
                    "This instance has been disposed and can no longer be used for operations.");

            if (enumerator == null)
                enumerator = enumerable.GetEnumerator();

            if (enumerator.MoveNext())
                return enumerator.Current;

            return default(T);
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            if (enumerable == null)
                throw new ObjectDisposedException(
                    "This instance has been disposed and can no longer be used for operations.");

            enumerator = enumerable.GetEnumerator();
        }
        #endregion

    }
}