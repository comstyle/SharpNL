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
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// This is a dictionary based name finder, it scans text for names inside a dictionary.
    /// </summary>
    public class DictionaryNameFinder : ITokenNameFinder {
        private const string DEFAULT_TYPE = "default";

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryNameFinder"/> with the provided dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DictionaryNameFinder(Dictionary.Dictionary dictionary) : this(dictionary, DEFAULT_TYPE) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryNameFinder"/> with the provided dictionary and a type.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="type">The type of the result spans.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public DictionaryNameFinder(Dictionary.Dictionary dictionary, string type) {
            if (type == null)
                throw new ArgumentNullException("type");

            Dictionary = dictionary;
            Type = type;
        }

        #endregion

        #region + Properties .

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        public Dictionary.Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the result span type.
        /// </summary>
        /// <value>The result span type string.</value>
        public string Type { get; private set; }

        #endregion

        #region . Find .

        /// <summary>
        /// Generates name tags for the given sequence, typically a sentence, returning token spans for any identified names.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence, typically a sentence.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string[] tokens) {
            var list = new List<Span>();

            for (var offsetFrom = 0; offsetFrom < tokens.Length; offsetFrom++) {
                Span nameFound = null;
                for (var offsetTo = offsetFrom; offsetTo < tokens.Length; offsetTo++) {
                    var lengthSearching = offsetTo - offsetFrom + 1;

                    if (lengthSearching > Dictionary.MaxTokenCount) {
                        break;
                    }
                    var entryForSearch = new StringList(tokens.SubArray(offsetFrom, lengthSearching));

                    if (Dictionary.Contains(entryForSearch)) {
                        nameFound = new Span(offsetFrom, offsetTo + 1, Type);
                    }
                }

                if (nameFound != null) {
                    list.Add(nameFound);
                    // skip over the found tokens for the next search
                    offsetFrom += (nameFound.Length - 1);
                }
            }
            return list.ToArray();
        }

        #endregion

        #region . ClearAdaptiveData .

        /// <summary>
        /// Forgets all adaptive data which was collected during previous calls to one of the find methods.
        /// </summary>
        /// <remarks>This method is typical called at the end of a document.</remarks>
        public void ClearAdaptiveData() {
            // nothing to clear
        }

        #endregion

    }
}