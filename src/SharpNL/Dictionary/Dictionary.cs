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
using System.Xml;
using SharpNL.Utility;

namespace SharpNL.Dictionary {
    /// <summary>
    /// Represents an universal dictionary.
    /// </summary>
    public class Dictionary : IEnumerable<Entry> {
        
        private readonly List<Entry> list;

        public Dictionary() : this(false) {}

        public Dictionary(bool caseSensitive) {
            list = new List<Entry>();

            MinTokenCount = 0;
            MaxTokenCount = 99999;

            IsCaseSensitive = caseSensitive;
        }

        internal Dictionary(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"Stream was not readable.", "inputStream");
            }

            //list = new HashSet<Entry>();
            list = new List<Entry>();

            MinTokenCount = 0;
            MaxTokenCount = 99999;

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
                                    for (int attInd = 0; attInd < reader.AttributeCount; attInd++) {
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

        #region + Add .
        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>The new entry.</returns>
        /// <exception cref="System.ArgumentNullException">entry</exception>
        public Entry Add(Entry entry) {
            if (entry == null)
                throw new ArgumentNullException("entry");

            entry.CaseSensitive = IsCaseSensitive;

            list.Add(entry);

            MinTokenCount = Math.Min(MinTokenCount, entry.Tokens.Count);
            MaxTokenCount = Math.Max(MaxTokenCount, entry.Tokens.Count);

            return entry;
        }

        /// <summary>
        /// Adds the tokens to the dictionary as one new entry.
        /// </summary>
        /// <param name="tokens">The new entry.</param>
        /// <returns>The new entry.</returns>
        public Entry Add(StringList tokens) {
            return Add(new Entry(tokens));
        }

        /// <summary>
        /// Adds the string tokens to the dictionary as one new entry.
        /// </summary>
        /// <param name="tokens">The string tokens.</param>
        /// <returns>The new entry.</returns>
        public Entry Add(params string[] tokens) {
            return Add(new Entry(tokens));
        }

        #endregion

        #region + Properties .

        #region . Count .

        /// <summary>
        /// Gets the number of tokens in the current instance.
        /// </summary>
        /// <value>The number of tokens in the current instance.</value>
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
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less then 0.
        /// or
        /// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
        /// </exception>
        public Entry this[int index] {
            get {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index", @"index is less then 0.");
                if (index > list.Count)
                    throw new ArgumentOutOfRangeException("index", @"index is equal to or greater than Count.");

                return list[index]; 
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
            return list.Contains(new Entry(tokens));
        }

        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all entries from the <see cref="Dictionary"/>.
        /// </summary>
        public void Clear() {
            list.Clear();
        }
        #endregion

        #region . Deserialize .
        /// <summary>
        /// Deserializes the artifact using the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="ArgumentNullException">inputStream</exception>
        /// <exception cref="ArgumentException">Stream was not readable.</exception>
        internal static object Deserialize(Stream inputStream) {
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
                if (dic.list.Count != list.Count)
                    return false;

                return dic.list.SequenceEqual(list);
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
            return list.ToString().ToLower().GetHashCode();
        }

        #endregion

        #region + GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Entry> GetEnumerator() {
            return list.GetEnumerator();
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
            list.Remove(new Entry(tokens));
        }

        #endregion

        #region + Serialize .

        /// <summary>
        /// Serializes the current instance to the given stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public virtual void Serialize(Stream stream) {
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
                writer.WriteStartElement("dictionary");
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
        public virtual List<string> ToList() {
            var toList = new List<string>();
            var e = GetEnumerator();

            while (e.MoveNext()) {
                toList.Add(e.Current.Tokens[0]);
            }

            return toList;
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
            return list.ToString();
        }

        #endregion

    }
}