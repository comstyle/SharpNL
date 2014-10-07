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
using SharpNL.Featurizer;

namespace SharpNL.POSTag {
    /// <summary>
    /// Provides a means of determining which tags are valid for a particular word 
    /// based on a tag dictionary read from a file.
    /// </summary>
    public class ExtendedPOSDictionary : IExtendedTagDictionary, IEnumerable<WordTag> {
        private const string ATTR_TAGS = "tags";
        private const string ATTR_LEMMAS = "lemmas";
        private const string ATTR_FEATS = "feats";

        private readonly Dictionary<string, List<Triple>> dictionary;

        #region + Properties .
        /// <summary>
        /// Gets the <see cref="List{Triple}"/> with the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
       public List<Triple> this[string word] {
           get {
               if (!dictionary.ContainsKey(word))
                   return null;

               return dictionary[word];
           }
        }

        #endregion

        #region + Constructors .

        public ExtendedPOSDictionary() : this(true) {}

        public ExtendedPOSDictionary(bool caseSensitive) {
            dictionary = caseSensitive ? 
                new Dictionary<string, List<Triple>>() : 
                new Dictionary<string, List<Triple>>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region . AddTriple .
        /// <summary>
        /// Adds the triple to this dictionary.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="triple">The triple.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="word"/>
        /// or
        /// <paramref name="triple"/>
        /// </exception>
        protected void AddTriple(string word, Triple triple) {
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");

            if (triple == null)
                throw new ArgumentNullException("triple");
            
            if (!dictionary.ContainsKey(word)) {
                dictionary.Add(word, new List<Triple>());
            }
            dictionary[word].Add(triple);
        }
        #endregion

        #region . CreateTriple .

        /// <summary>
        /// Creates the triples with its respective tag, lemma and features.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <param name="lemmas">The lemmas.</param>
        /// <param name="features">The features.</param>
        /// <returns>Triple[].</returns>
        protected static Triple[] CreateTriples(string[] tags, string[] lemmas, string[] features) {
            var triples = new Triple[tags.Length];
            for (int i = 0; i < tags.Length; i++) {
                triples[i] = new Triple(tags[i], lemmas[i], features[i]);
            }
            return triples;
        }

        #endregion

        #region + GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<WordTag> GetEnumerator() {
            var list = new List<WordTag>();
            // ReSharper disable once LoopCanBeConvertedToQuery 
            // Yield is much faster then your bullshit ReSharper :P
            foreach (var pair in dictionary) {
                foreach (var triple in pair.Value) {
                    yield return new WordTag(pair.Key, triple.Class);
                }
            }
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

        #region + GetTags .
        /// <summary>
        /// Returns a list of valid tags for the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>A list of valid tags for the specified word or null if no information is available for that word.</returns>
        public string[] GetTags(string word) {
            return GetTags(this[word]);
        }

        private static string[] GetTags(List<Triple> triples) {
            if (triples == null)
                return null;

            var tags = new string[triples.Count];
            for (var i = 0; i < tags.Length; i++) {
                tags[i] = triples[i].Class;
            }
            return tags;
        }

        #endregion

        #region + GetCompleteTag .
        /// <summary>
        /// Gets the complete tag.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>System.String[].</returns>
        public string[] GetCompleteTag(string word) {
            return GetCompleteTag(this[word]);
        }
        private static string[] GetCompleteTag(List<Triple> triples) {
            if (triples == null)
                return null;

            var feats = new string[triples.Count];
            for (int i = 0; i < triples.Count; i++) {
                feats[i] = triples[i].Class + "_" + triples[i].Features;
            }
            return feats;
        }
        #endregion

        #region . GetFeatureTag .
        /// <summary>
        /// Gets the feature tag.
        /// </summary>
        /// <param name="word">The word.</param>
        public string[] GetFeatureTag(string word) {
            return GetFeatureTag(this[word]);
        }
        private static string[] GetFeatureTag(List<Triple> triples) {
            if (triples == null)
                return null;

            var features = new string[triples.Count];
            for (int i = 0; i < triples.Count; i++) {
                features[i] = triples[i].Features;
            }
            return features;
        }
        #endregion

        #region . GetLemma .
        /// <summary>
        /// Gets the lemma.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>System.String.</returns>
        public string GetLemma(string word, string tag) {
            var triples = this[word];
            if (triples == null)
                return null;

            foreach (var t in triples) {
                if (tag.Equals(t.Class)) {
                    return t.Lemma;
                }
            }

            return null;
        }
        #endregion

        #region . GetFeatures .

        /// <summary>
        /// Gets the features to the specified word and tag.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The features</returns>
        public string[] GetFeatures(string word, string tag) {
            var feats = new List<string>();

            var triples = dictionary[word];
            if (triples == null)
                return null;

            foreach (var t in triples) {
                if (tag.Equals(t.Class)) {
                    feats.Add(t.Features);
                }
            }
            return feats.Count > 0 ? feats.ToArray() : null;
        }

        #endregion

    }
}