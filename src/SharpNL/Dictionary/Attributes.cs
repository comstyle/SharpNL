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

namespace SharpNL.Dictionary {
    /// <summary>
    /// The <see cref="Attributes"/> class stores name value pairs.
    /// </summary>
    public class Attributes : IEnumerable<string> {
        private readonly Dictionary<string, string> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="Attributes"/> class.
        /// </summary>
        public Attributes() {
            items = new Dictionary<string, string>();
        }

        #region . Count .

        /// <summary>
        /// Gets the number of attributes in this collection.
        /// </summary>
        /// <value>The number of attributes in this collection.</value>
        public int Count {
            get { return items.Count; }
        }
        #endregion

        #region . this .

        /// <summary>
        /// Gets or sets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// key must not be null.
        /// or
        /// value must not be null.
        /// </exception>
        public string this[string key] {
            get {
                if (items.ContainsKey(key))
                    return items[key];

                return null;
            }
            set {
                if (key == null)
                    throw new InvalidOperationException("key must not be null.");
                if (value == null)
                    throw new InvalidOperationException("value must not be null.");

                items[key] = value;
            }
        }

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified key and value to the attributes.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value.</param>
        public void Add(string key, string value) {
            items.Add(key, value);
        }
        #endregion

        #region . Contains .

        /// <summary>
        /// Determines whether this attribute collection contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this attribute collection contains the specified key; otherwise, <c>false</c>.</returns>
        public bool Contains(string key) {
            return items.ContainsKey(key);
        }

        #endregion

        #region . Clear .

        /// <summary>
        /// Removes all attributes from this instance.
        /// </summary>
        public void Clear() {
            items.Clear();
        }

        #endregion

        #region + GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through keys in the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            return items.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the keys in the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}