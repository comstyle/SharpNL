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
using System.Collections.ObjectModel;
using System.IO;
using SharpNL.Extensions;

namespace SharpNL.Lemmatizer {

    /// <summary>
    /// Represents a dictionary based lemmatizer. This class cannot be inherited.
    /// </summary>
    public sealed class DictionaryLemmatizer : AbstractLemmatizer, IEnumerable<string> {

        // word -> [tag] -> lemmas
        private readonly Dictionary<string, Dictionary<string, string[]>> dictionary;

        #region + Constructors .

        /// <summary>
        /// Initializes a empty instance of the <see cref="DictionaryLemmatizer" />.
        /// </summary>
        /// <param name="cache">if set to <c>true</c> a cache will be enabled on this lemmatizer.</param>
        public DictionaryLemmatizer(bool cache = true) : base(cache) {
            dictionary = new Dictionary<string, Dictionary<string, string[]>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryLemmatizer"/> using
        /// a <see cref="StreamReader"/> to read the dictionary.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/>
        /// </exception>
        /// <remarks>
        /// Expected format per line (without spaces):
        /// <para>word [tab] tag [tab] lemma</para>
        /// or
        /// <para>word [tab] lemma</para>
        /// </remarks>
        public DictionaryLemmatizer(StreamReader reader) {
            if (reader == null)
                throw new ArgumentNullException("reader");

            dictionary = new Dictionary<string, Dictionary<string, string[]>>();

            string line;
            while ((line = reader.ReadLine()) != null) {

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var tokens = line.Split('\t');
                switch (tokens.Length) {
                    case 2:
                        if (string.IsNullOrEmpty(tokens[0]) ||
                            string.IsNullOrEmpty(tokens[1]))
                            continue;

                        if (!dictionary.ContainsKey(tokens[0]))
                            dictionary[tokens[0]] = new Dictionary<string, string[]>(1);

                        dictionary[tokens[0]][string.Empty] = new []{ tokens[1] };
                        break;
                    case 3:
                        if (string.IsNullOrEmpty(tokens[0]) ||
                            string.IsNullOrEmpty(tokens[2]))
                            continue;

                        if (string.IsNullOrWhiteSpace(tokens[1]))
                            tokens[1] = string.Empty;

                        if (!dictionary.ContainsKey(tokens[0]))
                            dictionary[tokens[0]] = new Dictionary<string, string[]>(1);

                        dictionary[tokens[0]][tokens[1]] = tokens.SubArray(2);
                        break;
                    default:
                        continue;
                }
            }
        }
        
        #endregion

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of entries contained in the <see cref="DictionaryLemmatizer"/>.
        /// </summary>
        /// <value>The number of entries contained in the <see cref="DictionaryLemmatizer"/>.</value>
        public int Count {
            get { return dictionary.Count; }
        }
        #endregion

        #region . this .
        /// <summary>
        /// Gets the <see cref="T:ReadOnlyDictionary{string, string}"/> with the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The <see cref="T:ReadOnlyDictionary{string, string}"/> with the specified word or a <c>null</c> value.</returns>
        public ReadOnlyDictionary<string, string[]> this[string word] {
            get {
                return dictionary.ContainsKey(word)
                    ? new ReadOnlyDictionary<string, string[]>(dictionary[word])
                    : null;
            }
        }
        #endregion
        
        #endregion

        #region + Add .
        /// <summary>
        /// Adds the specified lemma without a posTag.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="lemma">The lemma.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="word"/>
        /// or
        /// <paramref name="lemma"/>
        /// </exception>
        public void Add(string word, string lemma) {
            Add(word, null, lemma);
        }

        /// <summary>
        /// Adds the specified lemmas with a <paramref name="posTag"/>.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="posTag">The position tag.</param>
        /// <param name="lemmas">The lemmas.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="word"/>
        /// or
        /// <paramref name="lemmas"/>
        /// </exception>
        public void Add(string word, string posTag, params string[] lemmas) {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");

            if (lemmas.Length == 0)
                throw new ArgumentNullException("lemmas");

            if (string.IsNullOrWhiteSpace(posTag))
                posTag = string.Empty;

            if (!dictionary.ContainsKey(word))
                dictionary.Add(word, new Dictionary<string, string[]>(1));

            dictionary[word][posTag] = lemmas;
        }
        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all the entries from this dictionary.
        /// </summary>
        public void Clear() {
            dictionary.Clear();
        }
        #endregion

        #region + Process .
        /// <summary>
        /// Processes the specified word into its lemma form.
        /// </summary>
        /// <param name="word">The word to lemmatize.</param>
        /// <param name="posTag">The part-of-speech of the specified word.</param>
        /// <returns>The word lemmas.</returns>
        protected override string[] Process(string word, string posTag) {
            if (string.IsNullOrEmpty(word))
                return new[] {word};

            if (string.IsNullOrWhiteSpace(posTag))
                posTag = string.Empty;

            if (dictionary.ContainsKey(word) && dictionary[word].ContainsKey(posTag))
                return dictionary[word][posTag];

            return new [] { word };
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

    }
}