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
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a property file.
    /// </summary>
    public class Properties : IEnumerable<string> {

        private static readonly Encoding encoding;

        static Properties() {
            encoding = Encoding.GetEncoding("ISO-8859-1"); // latin1
        }

        internal readonly Dictionary<string, string> list;

        public Properties() {
            list = new Dictionary<string, string>();
        }

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of properties in this instance.
        /// </summary>
        /// <value>The number of properties in this instance.</value>
        public int Count {
            get { return list.Count; }
        }
        #endregion

        #region . this .
        /// <summary>
        /// Gets or sets the property with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The property value.</returns>
        public string this[string key] {
            get {
                if (list.ContainsKey(key))
                    return list[key];

                return null;
            }
            set { list[key] = value; }
        }
        #endregion

        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether the <see cref="Properties" /> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns><c>true</c> if this instance contains the specified key; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Contains(string key) {
            if (key == null)
                throw new ArgumentNullException("key", @"key is null.");
            return list.ContainsKey(key);
        }
        #endregion

        #region . ContainsAnyValue .

        /// <summary>
        /// Determines whether the key contains any specified value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The values to check.</param>
        /// <returns><c>true</c> if the key contains ANY specified value; otherwise, <c>false</c>.</returns>
        public bool ContainsAnyValue(string key, params string[] values) {
            if (list.ContainsKey(key))
                return false;

            return values.Any(value => value == list[key]);
        }
        #endregion

        #region . Clear .
        /// <summary>
        /// Remove all properties in this instance.
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

            var list = new Properties();
            list.Load(inputStream);
            return list;
        }
        #endregion

        #region . Get .
        /// <summary>
        /// Gets the value of the specified key as a type. 
        /// If the key does not exist or the value can't be converted, the <paramref name="defaultValue"/> 
        /// will be returned as result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The <paramref name="defaultValue"/> if value does not exist or if the value can't be
        /// converted; otherwise, the value associated with the specified <paramref name="key"/>.
        /// </returns>
        public T Get<T>(string key, T defaultValue) {

            if (!list.ContainsKey(key))
                return defaultValue;

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter.CanConvertFrom(typeof(string))) {
                return (T)typeConverter.ConvertFrom(list[key]);
            }

            // not convertible :(
            return defaultValue;           
        }
        #endregion

        #region . Load .

        public void Load(Stream inputStream) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"Stream was not readable.", "inputStream");
            }

            using (var reader = new StreamReader(inputStream, encoding)) {
                string line;
                while ((line = reader.ReadLine()) != null) {

                    if (!string.IsNullOrWhiteSpace(line) &&
                        !line.StartsWith("#") &&
                        !line.StartsWith(";") &&
                        !line.StartsWith("'") &&
                        line.Contains("=")) {
                        var token = line.Split(new[] { '=' }, 2);

                        if (token[1].StartsWith("\"") && token[1].EndsWith("\"") ||
                            token[1].StartsWith("'") && token[1].EndsWith("'")) {
                            token[1] = token[1].Substring(1, token[0].Length - 2);
                        }

                        list[token[0]] = token[1];
                    }
                }
            }
        }
        #endregion

        #region . Remove .
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the element is successfully found and removed, <c>false</c> otherwise.</returns>
        public bool Remove(string key) {
            return list.Remove(key);
        }
        #endregion

        #region . Save .

        public void Save(Stream outputStream) {
            if (outputStream == null) {
                throw new ArgumentNullException("outputStream");
            }
            if (!outputStream.CanWrite) {
                throw new ArgumentException("Stream was not writable.");
            }

            using (var writer = new StreamWriter(outputStream, encoding)) {
                foreach (var entry in list) {
                    writer.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
            }
        }
        #endregion

        #region . Serialize .
        /// <summary>
        /// Serializes the the artifact into the specified stream.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">outputStream</exception>
        /// <exception cref="ArgumentException">Stream was not writable.</exception>
        internal static void Serialize(object artifact, Stream outputStream) {
            var prop = artifact as Properties;
            if (prop == null) {
                throw new InvalidOperationException("Invalid object type.");
            }
            if (outputStream == null) {
                throw new ArgumentNullException("outputStream");
            }
            if (!outputStream.CanWrite) {
                throw new ArgumentException("Stream was not writable.");
            }
            using (var writer = new StreamWriter(outputStream, encoding, 1024, true)) {
                foreach (var entry in prop.list) {
                    writer.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
                writer.Flush();
            }
        }
        #endregion

        #region . GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            return list.Keys.GetEnumerator();
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

    }
}