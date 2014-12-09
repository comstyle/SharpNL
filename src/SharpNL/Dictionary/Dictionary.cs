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
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.Dictionary {
    /// <summary>
    /// Represents an universal dictionary.
    /// </summary>
    public class Dictionary : DictionaryBase, IEnumerable<Entry> {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        public Dictionary() : this(false) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> indicating 
        /// if the dictionary is case sensitive.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> if the dictionary should be case sensitive.</param>
        public Dictionary(bool caseSensitive) : base(caseSensitive) {
            MinTokenCount = 0;
            MaxTokenCount = 99999;
        }

        internal Dictionary(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"Stream was not readable.", "inputStream");
            }

            MinTokenCount = 0;
            MaxTokenCount = 99999;

            base.Deserialize(inputStream);
        }

        #endregion

        #region + Add .
        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>The new entry.</returns>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        public new Entry Add(Entry entry) {

            base.Add(entry);

            MinTokenCount = Math.Min(MinTokenCount, entry.Tokens.Count);
            MaxTokenCount = Math.Max(MaxTokenCount, entry.Tokens.Count);

            return entry;
        }

        /// <summary>
        /// Adds the tokens to the dictionary as one new entry.
        /// </summary>
        /// <param name="tokens">The new entry.</param>
        /// <returns>The new entry.</returns>
        public virtual Entry Add(StringList tokens) {
            return Add(new Entry(tokens));
        }

        /// <summary>
        /// Adds the string tokens to the dictionary as one new entry.
        /// </summary>
        /// <param name="tokens">The string tokens.</param>
        /// <returns>The new entry.</returns>
        /// <remarks>
        /// Be careful to use this method as a object initializer, it may create an unexpected result!
        /// <para>
        ///  This method is the same as:
        ///  <code>
        ///  Add(new Entry(new string[] { ... }));
        ///  </code>
        /// </para>
        /// </remarks>
        public virtual Entry Add(params string[] tokens) {
            return Add(new Entry(tokens));
        }

        #endregion

        #region + Properties .

        #region . MinTokenCount .

        /// <summary>
        /// Gets the minimum token count in the dictionary.
        /// </summary>
        /// <value>The minimum token count in the dictionary.</value>
        public int MinTokenCount { get; private set; }

        #endregion

        #region . MaxTokenCount .

        /// <summary>
        /// Gets the maximum token count in the dictionary.
        /// </summary>
        /// <value>The maximum token count in the dictionary.</value>
        public int MaxTokenCount { get; private set; }

        #endregion

        #region . this .
        /// <summary>
        /// Gets the <see cref="Entry" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="Entry" /> at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public new Entry this[int index] {
            get {
                if (index < 0 || index > Count)
                    throw new ArgumentOutOfRangeException("index");

                return base[index]; 
            }
        }
        #endregion

        #endregion

        #region . Contains .

        /// <summary>
        /// Determines whether this dictionary has the given entry.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns><c>true</c> if it contains the entry; otherwise, <c>false</c>.</returns>
        public bool Contains(StringList tokens) {
            return Contains(new Entry(tokens));
        }

        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all entries from the <see cref="Dictionary"/>.
        /// </summary>
        public new void Clear() {
            base.Clear();
        }
        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the artifact using the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="ArgumentNullException">inputStream</exception>
        /// <exception cref="ArgumentException">Stream was not readable.</exception>
        internal static new object Deserialize(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"Stream was not readable.", "inputStream");
            }
            return new Dictionary(inputStream);
        }
        #endregion

        #region . Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:Dictionary"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            var dic = obj as Dictionary;
            if (dic != null) {
                if (dic.Count != Count)
                    return false;

                return dic.SequenceEqual(this);
            }
            return false;
        }

        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() {
            return ToString().ToLower().GetHashCode();
        }
        #endregion

        #region + GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Entry> GetEnumerator() {
            return base.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return base.GetEnumerator();
        }

        #endregion

        #region . ParseOneEntryPerLine .

        /// <summary>
        /// Reads a dictionary which has one entry per line. The tokens inside an entry are whitespace delimited.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The parsed dictionary.</returns>
        public static Dictionary ParseOneEntryPerLine(StringReader reader) {
            var dic = new Dictionary();
            string line;
            while ((line = reader.ReadLine()) != null) {
                dic.Add(new StringList(line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)));
            }
            return dic;
        }

        #endregion

        #region . Remove .

        /// <summary>
        /// Removes the given tokens form the current instance.
        /// </summary>
        /// <param name="tokens">The tokens to remove.</param>
        public virtual void Remove(StringList tokens) {
            Remove(new Entry(tokens));
        }

        #endregion

        #region + Serialize .

        /// <summary>
        /// Serializes the current instance to the given stream.
        /// </summary>
        /// <param name="outputStream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public new void Serialize(Stream outputStream) {
            base.Serialize(outputStream);
        }

        /// <summary>
        /// Serializes the the artifact into the specified stream.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">outputStream</exception>
        /// <exception cref="ArgumentException">Stream was not writable.</exception>
        internal static void Serialize(object artifact, Stream outputStream) {
            var dic = artifact as Dictionary;
            if (dic == null) {
                throw new InvalidOperationException("Invalid artifact type.");
            }
            if (outputStream == null) {
                throw new ArgumentNullException("outputStream");
            }
            if (!outputStream.CanWrite) {
                throw new ArgumentException(@"Stream was not writable.", "outputStream");
            }

            dic.Serialize(outputStream);
        }

        #endregion

        #region . ToHashSet .
        /// <summary>
        /// Converts the dictionary to a <see cref="HashSet{T}"/> object.
        /// </summary>
        /// <returns>The HashSet&lt;string&gt; object.</returns>
        public virtual HashSet<string> ToHashSet() {
            var set = new HashSet<string>();
            var e = GetEnumerator();

            while (e.MoveNext()) {
                set.Add(e.Current.Tokens[0]);
            }

            return set;
        }

        #endregion

        #region . ToList .
        /// <summary>
        /// Converts the dictionary to a <see cref="List{T}"/>.
        /// </summary>
        /// <returns>A string list object.</returns>
        public virtual List<string> ToList() {
            var toList = new List<string>();
            var e = GetEnumerator();

            while (e.MoveNext()) {
                toList.Add(e.Current.Tokens[0]);
            }

            return toList;
        }
        #endregion

    }
}