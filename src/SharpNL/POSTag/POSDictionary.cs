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
using SharpNL.Dictionary;


namespace SharpNL.POSTag {
    /// <summary>
    /// Provides a means of determining which tags are valid for a particular 
    /// word based on a tag dictionary read from a file.
    /// </summary>
    public class POSDictionary : Dictionary.DictionaryBase, IEnumerable<string>, IMutableTagDictionary, IEquatable<POSDictionary> {
        private const string Tags = "tags";

        private readonly Dictionary<string, Entry> items;

        /// <summary>
        /// Initializes an empty case sensitive <see cref="POSDictionary"/>.
        /// </summary>
        public POSDictionary() : this(true) {}

        /// <summary>
        /// Initializes an empty <see cref="POSDictionary"/>.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> the dictionary will be case sensitive.</param>
        public POSDictionary(bool caseSensitive) : base(caseSensitive) {
            items = new Dictionary<string, Entry>(caseSensitive 
                ? StringComparer.Ordinal 
                : StringComparer.OrdinalIgnoreCase);
        }

        public POSDictionary(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"Stream was not readable.", "inputStream");
            }

            Deserialize(inputStream);

            items = new Dictionary<string, Entry>(IsCaseSensitive
                ? StringComparer.Ordinal
                : StringComparer.OrdinalIgnoreCase);

            for (var e = base.GetEnumerator(); e.MoveNext(); ) {
                items[e.Current.Tokens[0]] = e.Current;
            }

        }


        #region + Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(POSDictionary other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(items, other.items);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((POSDictionary)obj);
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
            return (items != null ? items.GetHashCode() : 0);
        }
        #endregion

        #region . GetTags .

        /// <summary>
        /// Returns a list of valid tags for the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A list of valid tags for the specified word or null if no information is available for that word.</returns>
        public string[] GetTags(string word) {
            return !items.ContainsKey(word) ? null : items[word].Attributes["tags"].Split(' ');
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
            if (items.ContainsKey(word)) {
                var prev = items[word].Attributes[Tags].Split(' ');

                var entry = items[word];
                entry.Attributes[Tags] = string.Join(" ", tags);

                return prev;
            }

            items[word] = new Entry(word);
            items[word].Attributes[Tags] = string.Join(" ", tags);

            Add(items[word]);
            
            return null;
        }

        #endregion

        #region + GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<string> GetEnumerator() {
            return items.Keys.GetEnumerator();
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

        #region . Serialize .
        /// <summary>
        /// Serializes the current instance to the given stream.
        /// </summary>
        /// <param name="outputStream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public new void Serialize(Stream outputStream) {
            base.Serialize(outputStream);
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
            return string.Format("POSDictionary{{size={0}, caseSensitive={1}}}", Count, IsCaseSensitive);
        }

        #endregion

    }
}