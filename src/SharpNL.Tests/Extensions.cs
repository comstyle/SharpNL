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

namespace SharpNL.Tests {
    internal static class Extensions {

        #region . SequenceEqual .

        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first enumerable.</param>
        /// <param name="second">The second enumerable.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns><c>true</c> if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">first or second.</exception>
        internal static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second,
            IEqualityComparer<T> comparer = null) {
            if (first == null) {
                throw new ArgumentNullException("first");
            }
            if (second == null) {
                throw new ArgumentNullException("second");
            }
            if (comparer == null) {
                comparer = EqualityComparer<T>.Default;
            }

            using (var enumerator = first.GetEnumerator())
            using (var enumerator2 = second.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    if (!enumerator2.MoveNext() || !comparer.Equals(enumerator.Current, enumerator2.Current)) {
                        return false;
                    }
                }
                if (enumerator2.MoveNext()) {
                    return false;
                }
            }
            return true;
        }

        #endregion

    }
}