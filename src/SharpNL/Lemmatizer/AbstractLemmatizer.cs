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
using SharpNL.Utility;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a abstract lemmatizer with cache support.
    /// </summary>
    public abstract class AbstractLemmatizer : CacheBase<string[]>, ILemmatizer {

        private readonly HashSet<string> ignoreSet;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractLemmatizer"/> class.
        /// </summary>
        /// <param name="cache">if set to <c>true</c> the cache will be enabled.</param>
        protected AbstractLemmatizer(bool cache = true)
            : base(cache) {
            ignoreSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region . AddIgnore .
        /// <summary>
        /// Adds the specified words to the ignored list.
        /// </summary>
        /// <param name="words">The words to ignore.</param>
        public void AddIgnore(params string[] words) {
            foreach (var word in words)
                ignoreSet.Add(word);
        }
        #endregion

        #region . Lemmatize .
        /// <summary>
        /// Returns the lemma of the specified word without a part-of-speech tag.
        /// </summary>
        /// <param name="word">The word whose lemmas are desired.</param>
        /// <returns>The lemma of the specified word without a part-of-speech tag.</returns>
        public string[] Lemmatize(string word) {
            return Lemmatize(word, null);
        }
        /// <summary>
        /// Returns the lemma of the specified word with the specified part-of-speech.
        /// </summary>
        /// <param name="word">The word whose lemmas are desired.</param>
        /// <param name="posTag">The part-of-speech of the specified word.</param>
        /// <returns>The lemma of the specified word given the specified part-of-speech.</returns>
        public string[] Lemmatize(string word, string posTag) {
            if (string.IsNullOrEmpty(word))
                return new string[] {};

            if (ignoreSet.Contains(word))
                return new []{ word };

            if (!CacheEnabled)
                return Process(word, posTag);

            if (IsCached(word, posTag))
                return Get(word, posTag);

            var lemma = Process(word, posTag);

            Set(word, lemma, posTag);

            return lemma;
        }
        #endregion

        #region . Process .
        /// <summary>
        /// Processes the specified word into its lemmas form.
        /// A given inflected form may correspond to several lemmas (e.g. "found" -> find, found) - the correct choice depends on the context.
        /// </summary>
        /// <param name="word">The word to lemmatize.</param>
        /// <param name="posTag">The part-of-speech of the specified word.</param>
        /// <returns>The word lemmas.</returns>
        protected abstract string[] Process(string word, string posTag);
        #endregion

    }
}