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
using System.Linq;

namespace SharpNL.Extensions {

    /// <summary>
    /// Provide a set of methods for generic data types.
    /// </summary>
    public static class GenericExtensions {

        #region . ForEach .
        /// <summary>
        /// Performs the specified action on each element of the array.
        /// </summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="action">The action to perform.</param>
        public static void ForEach<T>(this T[] array, Action<T> action) {
            Array.ForEach(array, action);
        }
        #endregion

        #region . In .
        /// <summary>
        /// Determines if the instance is equal to any of the specified values
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="values">The values to be compared.</param>
        /// <returns><c>true</c> if the instance is equal to any of the specified values, <c>false</c> otherwise.</returns>
        public static bool In<T>(this T input, params T[] values) {
            foreach (var value in values) {
                if (ReferenceEquals(value, input))
                    return true;

                if (value.Equals(input))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the instance is equal to any of the specified values
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="values">The values to be compared.</param>
        /// <returns><c>true</c> if the instance is equal to any of the specified values, <c>false</c> otherwise.</returns>
        public static bool In<T>(this T input, IEnumerable<T> values) {
            return values.Any(arg => input.Equals(arg));
        }
        #endregion

        #region + SubArray .

        /// <summary>
        /// Gets sub array of an existing array.
        /// </summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="index">The starting index of the sub array.</param>
        /// <returns>The sub array.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/>
        /// </exception>
        public static T[] SubArray<T>(this T[] data, int index) {
            return SubArray(data, index, data.Length - index);
        }

        /// <summary>
        /// Gets sub array of an existing array.
        /// </summary>
        /// <typeparam name="T">Type of array elements.</typeparam>
        /// <param name="data">The data.</param>
        /// <param name="index">The starting index of the sub array.</param>
        /// <param name="length">The length of the sub array.</param>
        /// <returns>The sub array.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/>
        /// or
        /// <paramref name="length"/>
        /// </exception>
        public static T[] SubArray<T>(this T[] data, int index, int length) {
            if (index > data.Length)
                throw new ArgumentOutOfRangeException("index");

            if (length > data.Length - index)
                throw new ArgumentOutOfRangeException("length");

            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        #endregion

    }
}