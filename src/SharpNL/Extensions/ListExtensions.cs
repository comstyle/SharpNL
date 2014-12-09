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

namespace SharpNL.Extensions {
    /// <summary>
    /// Provide a set of methods for lists.
    /// </summary>
    public static class ListExtensions {

        #region . First .
        /// <summary>
        /// Returns the first element of a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The first element in the specified list.</returns>
        public static T First<T>(this IList<T> list) {
            if (list == null || list.Count == 0)
                return default(T);

            return list[0];
        }
        #endregion

        #region . Last .
        /// <summary>
        /// Returns the last element of a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The last element in the specified list.</returns>
        public static T Last<T>(this IList<T> list) {
            if (list == null || list.Count == 0)
                return default(T);

            return list[list.Count - 1];
        }
        #endregion

        #region . Pop .
        /// <summary>
        /// Removes the object at the top of the <see cref="IList{T}" />, and returns it.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The object removed from the top of the <see cref="IList{T}" />.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <paramref name="list"/> is empty.
        /// or
        /// The <paramref name="list"/> is read-only.
        /// </exception>
        public static T Pop<T>(this IList<T> list) {
            if (list.Count == 0)
                throw new InvalidOperationException("The list is empty.");

            if (list.IsReadOnly)
                throw new InvalidOperationException("The list is read-only.");

            T item = list[0];
            list.RemoveAt(0);
            return item;
        }
        #endregion

    }
}