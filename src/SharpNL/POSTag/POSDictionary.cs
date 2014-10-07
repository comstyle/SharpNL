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
using System.IO;

namespace SharpNL.POSTag {
    /// <summary>
    /// Provides a means of determining which tags are valid for a particular 
    /// word based on a tag dictionary read from a file.
    /// </summary>
    public class POSDictionary : IEnumerable<string>, IMutableTagDictionary {
        private readonly bool caseSensitive;
        private readonly Dictionary<string, string[]> dictionary;

        /// <summary>
        /// Initializes an empty case sensitive <see cref="POSDictionary"/>.
        /// </summary>
        public POSDictionary() : this(true) {}

        /// <summary>
        /// Initializes an empty <see cref="POSDictionary"/>.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> the dictionary will be case sensitive.</param>
        public POSDictionary(bool caseSensitive) {
            this.caseSensitive = caseSensitive;

            dictionary = caseSensitive
                ? new Dictionary<string, string[]>()
                : new Dictionary<string, string[]>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region . Create .
        /*
        public static POSDictionary Create(Stream inputStream) {
            var dictionary = new POSDictionary();


            return dictionary;
        }*/
        #endregion

        #region . IsCaseSensitive .

        /// <summary>
        /// Gets a value indicating whether this instance is case sensitive.
        /// </summary>
        /// <value><c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.</value>
        public bool IsCaseSensitive {
            get { return caseSensitive; }
        }

        #endregion

        #region . GetTags .

        /// <summary>
        /// Returns a list of valid tags for the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A list of valid tags for the specified word or null if no information is available for that word.</returns>
        public string[] GetTags(string word) {
            return dictionary[word];
        }

        #endregion

        #region . Put .

        /// <summary>
        /// Associates the specified tags with the specified word. If the dictionary
        /// previously contained keys for the word, the old tags are replaced by the
        /// specified tags.
        /// </summary>
        /// <param name="word">The word with which the specified tags is to be associated.</param>
        /// <param name="tags">The tags to be associated with the specified word.</param>
        /// <returns>The previous tags associated with the word, or null if there was no mapping for word.</returns>
        public string[] Put(string word, params string[] tags) {
            if (dictionary.ContainsKey(word)) {
                if (tags == null)
                    dictionary.Remove(word);
                else
                    dictionary[word] = tags;

                return null;
            }
            var prev = dictionary[word];

            if (tags != null)
                dictionary[word] = tags;

            return prev;
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
            return dictionary.Keys.GetEnumerator();
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

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current dictionary.
        /// </summary>
        /// <returns>
        /// A string that represents the current dictionary.
        /// </returns>
        public override string ToString() {
            return string.Format("POSDictionary{{size={0}, caseSensitive={1}}}", dictionary.Count, caseSensitive);
        }

        #endregion
    }
}