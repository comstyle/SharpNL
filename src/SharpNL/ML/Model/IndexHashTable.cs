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

namespace SharpNL.ML.Model {
    /// <summary>
    /// The <see cref="T:IndexHashTable{T}"/> is a hash table which maps entries of an array to their index in the array. All entries in the array must be unique otherwise a well-defined mapping is not possible.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// The entry objects must implement <see cref="T:IEquatable{T}"/> and <see cref="M:Object.GetHashCode"/> otherwise the behavior of this class is undefined.
    /// The implementation uses a hash table with open addressing and linear probing.
    /// The table is thread safe and can concurrently accessed by multiple threads, thread safety is achieved through immutability. Though its not strictly immutable which means, that the table must still be safely published to other threads.
    /// </remarks>
    public class IndexHashTable<T> where T : class {
        private readonly T[] keys;
        private readonly int size;
        private readonly int[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexHashTable{T}"/> class.
        /// The specified array is copied into the table and later changes to the array do not affect this table in any way.
        /// </summary>
        /// <param name="mapping">The values to be indexed, all values must be unique otherwise a well-defined mapping of an entry to an index is not possible.</param>
        /// <param name="loadFactor">The load factor, usually 0.7.</param>
        /// <exception cref="System.ArgumentException">@The load factor must be larger than 0 and equal to or smaller than 1.;loadFactor</exception>
        /// <exception cref="System.InvalidOperationException">Array must contain only unique keys!</exception>
        public IndexHashTable(T[] mapping, double loadFactor) {
            if (loadFactor <= 0 || loadFactor > 1) {
                throw new ArgumentException(@"The load factor must be larger than 0 and equal to or smaller than 1.",
                    "loadFactor");
            }

            var arraySize = (int) (mapping.Length/loadFactor) + 1;

            keys = new T[arraySize];
            values = new int[arraySize];
            
            for (var i = 0; i < mapping.Length; i++) {
                var startIndex = IndexForHash(mapping[i].GetHashCode(), keys.Length);
                //int startIndex = IndexForHash(mapping[i].HashCode2(), keys.Length);

                var index = SearchKey(startIndex, null, true);

                if (index == -1) {
                    throw new InvalidOperationException("Array must contain only unique keys!");
                }

                keys[index] = mapping[i];
                values[index] = i;
            }
            size = mapping.Length;
        }

        #region . IndexForHash .

        private static int IndexForHash(int hash, int length) {
            return (hash & 0x7fffffff)%length;
        }

        #endregion

        #region . SearchKey .

        private int SearchKey(int startIndex, T key, bool insert) {
            for (var index = startIndex;; index = (index + 1)%keys.Length) {
                // The keys array contains at least one null element, which guarantees
                // termination of the loop
                if (keys[index] == null) {
                    if (insert)
                        return index;

                    return -1;
                }

                if (keys[index].Equals(key)) {
                    if (!insert)
                        return index;

                    return -1;
                }
            }
        }

        #endregion

        #region . Size .

        public int Size {
            get { return size; }
        }

        #endregion

        #region . this .

        /// <summary>
        /// Gets the index for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The index or -1 if there is no entry to the keys.</returns>
        public int this[T key] {
            get {
                var startIndex = IndexForHash(key.GetHashCode(), keys.Length);
                //int startIndex = IndexForHash(key.HashCode2(), keys.Length);
                var index = SearchKey(startIndex, key, false);
                if (index != -1) {
                    return values[index];
                }
                return -1;
            }
        }

        #endregion

        internal T[] ToArray(T[] array) {
            for (var i = 0; i < keys.Length; i++) {
                if (keys[i] != null)
                    array[values[i]] = keys[i];
            }
            return array;
        }
    }
}