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
using System.Collections;
using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a immutable list of strings.
    /// </summary>
    public class StringList : IEnumerable<string> {
        private readonly List<string> values;

        #region + Constructors .

        /// <summary>
        /// Initializes the current instance..
        /// </summary>
        /// <param name="value">The value.</param>
        /// <remarks>Token String will be replaced by identical internal String object.</remarks>
        public StringList(string value) {
            values = new List<string>(new[] {string.Intern(value)});
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringList"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <exception cref="System.ArgumentNullException">values</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">values;@The values must not be empty.</exception>
        public StringList(params string[] values) {
            if (values == null) {
                throw new ArgumentNullException("values");
            }

            if (values.Length == 0) {
                throw new ArgumentOutOfRangeException("values", @"The values must not be empty.");
            }

            this.values = new List<string>();
            foreach (var value in values) {
                this.values.Add(string.Intern(value));
            }
        }

        #endregion

        #region + Properties .

        #region . this .

        /// <summary>
        /// Gets the <see cref="System.String"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The string at given index.</returns>
        public string this[int index] {
            get { return values[index]; }
        }

        #endregion

        #region . Count .

        /// <summary>
        /// Retrieves the number of values inside this list.
        /// </summary>
        /// <value>The number of values.</value>
        public int Count {
            get { return values.Count; }
        }

        #endregion

        #region . Size .

#if DEBUG
        [Obsolete("Use the Count property.")]
        public int Size {
            get { return Count; }
        }
#endif

        #endregion

        #endregion

        #region . CompareToIgnoreCase .

        /// <summary>
        /// Compares to another <see cref="T:StringList"/> and ignores the case of the values.
        /// </summary>
        /// <param name="list">The list to compare.</param>
        /// <returns><c>true</c> if if identically with ignore the case, <c>false</c> otherwise.</returns>
        public bool CompareToIgnoreCase(StringList list) {
            if (Count == list.Count) {
                for (var i = 0; i < Count; i++) {
                    if (string.Compare(this[i], list[i], StringComparison.OrdinalIgnoreCase) != 0) {
                        return false;
                    }
                }
            } else {
                return false;
            }
            return true;
        }

        #endregion

        #region . Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (this == obj) {
                return true;
            }
            if (obj.GetType() == typeof (StringList)) {
                var list = (StringList) obj;
                return values.SequenceEqual(list.values);
            }
            return false;
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            var numBitsRegular = 32/Count;
            var numExtra = 32%Count;
            var maskExtra = 0xFFFFFFFF >> (32 - numBitsRegular + 1);
            var maskRegular = 0xFFFFFFFF >> 32 - numBitsRegular;
            uint code = 0x000000000;
            var leftMostBit = 0;

            for (var wi = 0; wi < Count; wi++) {
                uint mask;
                int numBits;
                if (wi < numExtra) {
                    mask = maskExtra;
                    numBits = numBitsRegular + 1;
                } else {
                    mask = maskRegular;
                    numBits = numBitsRegular;
                }
                var word = (uint) this[wi].GetHashCode() & mask;
                word <<= 32 - leftMostBit - numBits; // move to correct position
                leftMostBit += numBits; // set for next iteration
                code |= word;
            }

            return (int) code;
        }

        #endregion

        #region + GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            return values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region . GetToken .

#if DEBUG
        [Obsolete("Use the indexed property.")]
        public string GetToken(int index) {
            return this[index];
        }
#endif

        #endregion

        #region . ToArray .
        internal string[] ToArray() {
            return values.ToArray();
        }
        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current <see cref="T:StringList"/>.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("[{0}]", string.Join(",", values));
        }

        #endregion
    }
}