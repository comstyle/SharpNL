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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace SharpNL.Dictionary {
    /// <summary>
    /// Provides the abstract base class for a strongly typed collection of key/entry pairs.
    /// </summary>
    public abstract class DictionaryBase {
        private readonly List<Entry> list;

        /// <summary>
        /// The desired element name in the serialized xml.
        /// </summary>
        protected string ElementName = "dictionary";

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> class.
        /// </summary>
        protected DictionaryBase() : this(false) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary"/> indicating 
        /// if the dictionary is case sensitive.
        /// </summary>
        /// <param name="caseSensitive">if set to <c>true</c> if the dictionary should be case sensitive.</param>
        protected DictionaryBase(bool caseSensitive) {
            list = new List<Entry>();

            IsCaseSensitive = caseSensitive;
        }


        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of items in the current instance.
        /// </summary>
        /// <value>The number of items in the current instance.</value>
        public int Count {
            get { return list.Count; }
        }
        #endregion

        #region . IsCaseSensitive .
        /// <summary>
        /// Gets a value indicating whether this dictionary is case sensitive.
        /// </summary>
        /// <value><c>true</c> if this instance is case sensitive; otherwise, <c>false</c>.</value>
        public bool IsCaseSensitive { get; private set; }
        #endregion

        #region . this .
        /// <summary>
        /// Gets the <see cref="Entry" /> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="Entry" /> at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less then 0.
        /// or
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </exception>
        protected Entry this[int index] {
            get {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index", @"index is less then 0.");
                if (index > list.Count)
                    throw new ArgumentOutOfRangeException("index", @"index is equal to or greater than Count.");

                return list[index];
            }
            set { list[index] = value; }

        }
        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>The new entry.</returns>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected int Add(Entry entry) {
            if (entry == null)
                throw new ArgumentNullException("entry");

            entry.CaseSensitive = IsCaseSensitive;

            list.Add(entry);

            return list.Count - 1;
        }
        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all the entries from this instance.
        /// </summary>
        protected void Clear() {
            list.Clear();
        }
        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether an element is in this instance.
        /// </summary>
        /// <param name="entry">The entry to locate in this instance.</param>
        /// <returns><c>true</c> if <paramref name="entry"/> is found; otherwise, <c>false</c>.</returns>
        protected bool Contains(Entry entry) {
            return list.Contains(entry);
        }
        #endregion

        #region . Deserialize .

        /// <summary>
        /// Deserializes the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.IO.InvalidDataException">
        /// Unexpected XML dictionary format.
        /// or
        /// Unexpected XML dictionary format.
        /// </exception>
        protected void Deserialize(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            
            using (
                var reader = XmlReader.Create(inputStream, new XmlReaderSettings {
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                })) {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Element) {
                        switch (reader.Name) {
                            case "dictionary":
                                IsCaseSensitive = reader.GetAttribute("case_sensitive") == "true";
                                break;
                            case "entry":
                                /*
                                <entry tags="NN">
                                <token>jet</token>
                                </entry>
                                */

                                var tokens = new List<string>();
                                var attributes = new Attributes();
                                if (reader.HasAttributes) {
                                    for (var attInd = 0; attInd < reader.AttributeCount; attInd++) {
                                        reader.MoveToAttribute(attInd);

                                        attributes[reader.Name] = reader.Value;
                                    }
                                    reader.MoveToElement();
                                }

                                while (reader.Read() && reader.Name == "token") {
                                    if (reader.Read() && reader.NodeType == XmlNodeType.Text) {
                                        tokens.Add(reader.Value);

                                        // read </entry>
                                        if (!reader.Read() || reader.NodeType != XmlNodeType.EndElement) {
                                            throw new InvalidDataException("Unexpected XML dictionary format.");
                                        }
                                    } else {
                                        throw new InvalidDataException("Unexpected XML dictionary format.");
                                    }
                                }

                                Add(new Entry(tokens.ToArray(), attributes));
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region . GetEnumerator .
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator&lt;Entry&gt;.</returns>
        protected IEnumerator<Entry> GetEnumerator() {
            return list.GetEnumerator();
        }

        #endregion

        #region + Remove .
        /// <summary>
        /// Removes entry at the specified index.
        /// </summary>
        /// <param name="index">The entry index.</param>
        /// <returns><c>true</c> if entry was removed, <c>false</c> otherwise.</returns>
        protected bool Remove(int index) {
            if (list.Count == 0 || index > list.Count)
                return false;

            list.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the first occurrence of a specified entry.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        /// <returns><c>true</c> if the first occurrence of the entry was removed, <c>false</c> otherwise.</returns>
        protected bool Remove(Entry entry) {
            if (!list.Contains(entry)) 
                return false;

            list.Remove(entry);
            return true;
        }
        #endregion

        #region . Serialize .
        /// <summary>
        /// Serializes the current instance to the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        protected void Serialize(Stream stream) {
            if (stream == null) {
                throw new ArgumentNullException("stream");
            }
            if (!stream.CanWrite) {
                throw new ArgumentException(@"The stream is not writable.", "stream");
            }

            /*
            <entry tags="NN">
            <token>jet</token>
            </entry>
            */
            using (var writer = XmlWriter.Create(stream)) {
                writer.WriteStartDocument();
                writer.WriteStartElement(ElementName);
                writer.WriteAttributeString("case_sensitive", IsCaseSensitive ? "true" : "false");

                foreach (var item in list) {
                    writer.WriteStartElement("entry");

                    foreach (var key in item.Attributes) {
                        writer.WriteAttributeString(key, item.Attributes[key]);
                    }

                    foreach (var token in item.Tokens) {
                        writer.WriteElementString("token", token);
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        #endregion

        #region . ToArray .
        /// <summary>
        /// Copies the entries into a new array.
        /// </summary>
        /// <returns>An array containing copies of the entries of this instance.</returns>
        protected Entry[] ToArray() {
            return list.ToArray();
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            if (list.Count == 0)
                return "[]";

            var sb = new StringBuilder("[");
            foreach (var entry in list) {
                if (entry.Tokens.Count == 0)
                    continue;

                if (entry.Tokens.Count == 1) {
                    sb.Append(entry.Tokens.ToString().TrimStart('[').TrimEnd(']')).Append(",");
                    continue;
                }
                sb.Append(entry.Tokens).Append(",");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");

            return sb.ToString();
        }
        #endregion

    }
}