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
using System.Runtime.Caching;
using SharpNL.Utility;

namespace SharpNL.Stemmer {
    /// <summary>
    /// Represents a abstract stemmer.
    /// </summary>
    public abstract class AbstractStemmer : CacheBase<string>, IStemmer {

        private readonly HashSet<string> ignoreSet;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractStemmer"/> class.
        /// </summary>
        protected AbstractStemmer(bool cache = true) : base(cache) {
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

        #region + Stem .
        /// <summary>
        /// Reduces the given word into its stem.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stemmed form.</returns>
        public string Stem(string word) {
            return Stem(word, null);
        }

        /// <summary>
        /// Reduces the given word with a specific part-of-speech tag into its stem.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <param name="posTag">The part-of-speech tag.</param>
        /// <returns>The stemmed form.</returns>
        public string Stem(string word, string posTag) {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            if (ignoreSet.Contains(word))
                return word;

            word = word.ToLowerInvariant();

            if (!CacheEnabled) 
                return Stemming(word, posTag);

            if (IsCached(word, posTag))
                return Get(word, posTag);

            var stem = Stemming(word, posTag);

            Set(word, stem, posTag);

            return stem;
        }
        #endregion

        #region . GetStem .
        /// <summary>
        /// Gets a <see cref="Stemmer.Stem"/> representation from the given <paramref name="word"/>.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <returns>A <see cref="Stemmer.Stem"/> object.</returns>
        public Stem GetStem(string word) {
            return GetStem(word, null);
        }
        /// <summary>
        /// Gets a <see cref="Stemmer.Stem"/> representation from the given <paramref name="word"/>
        /// with a specific part-of-speech.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <param name="posTag">The part-of-speech tag.</param>
        /// <returns>A <see cref="Stemmer.Stem"/> object.</returns>
        public Stem GetStem(string word, string posTag) {
            return new Stem(Stem(word, posTag), posTag, word);
        }
        #endregion

        #region . Stemming .
        /// <summary>
        /// Performs stemming on the specified word.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <param name="posTag">The part-of-speech tag or a <c>null</c> value if none.</param>
        /// <returns>The stemmed word.</returns>
        protected abstract string Stemming(string word, string posTag);
        #endregion

    }
}