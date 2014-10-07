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
using SharpNL.Chunker;

namespace SharpNL.Featurizer {
    public class WordTag {
        public WordTag(string word, string posTag) : this(word, posTag, null) {}

        public WordTag(string word, string posTag, string chunkTag) {
            Word = word;
            POSTag = posTag;
            ChunkTag = chunkTag;
        }

        #region + Properties .

        #region . ChunkTag .

        /// <summary>
        /// Gets the chunk tag.
        /// </summary>
        /// <value>The chunk tag.</value>
        public string ChunkTag { get; private set; }

        #endregion

        #region . POSTag .

        /// <summary>
        /// Gets the POS tag.
        /// </summary>
        /// <value>The POS tag.</value>
        public string POSTag { get; private set; }

        #endregion

        #region . Word .

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <value>The word.</value>
        public string Word { get; private set; }

        #endregion

        #endregion

        #region + Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:WordTag"/> is equal to the current <see cref="T:WordTag"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="other">The object to compare with the current object. </param>
        public bool Equals(WordTag other) {
            return
                string.Equals(ChunkTag, other.ChunkTag) &&
                string.Equals(POSTag, other.POSTag) &&
                string.Equals(Word, other.Word);
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
            return Equals((WordTag) obj);
        }

        #endregion

        #region + Create .

        public static WordTag[] Create(ChunkSample sample) {
            var wt = new WordTag[sample.Sentence.Count];

            var sentence = sample.Sentence;
            var pos = sample.Tags;
            var chunks = sample.Preds;

            for (var i = 0; i < wt.Length; i++) {
                wt[i] = new WordTag(sentence[i], pos[i], chunks[i]);
            }

            return wt;
        }

        public static WordTag[] Create(string[] word, string[] posTag) {
            if (word == null)
                throw new ArgumentNullException("word");
            if (posTag == null)
                throw new ArgumentNullException("posTag");
            if (word.Length != posTag.Length)
                throw new ArgumentException("The arrays does not have the same length.");

            var list = new WordTag[word.Length];
            for (var i = 0; i < word.Length; i++) {
                list[i] = new WordTag(word[i], posTag[i]);
            }

            return list;
        }

        public static WordTag[] Create(string[] word, string[] posTag, string[] chunkTag) {
            if (word == null)
                throw new ArgumentNullException("word");
            if (posTag == null)
                throw new ArgumentNullException("posTag");
            if (chunkTag == null)
                throw new ArgumentNullException("chunkTag");

            if (word.Length != posTag.Length || posTag.Length != chunkTag.Length)
                throw new ArgumentException("The arrays does not have the same length.");

            var list = new WordTag[word.Length];
            for (var i = 0; i < word.Length; i++) {
                list[i] = new WordTag(word[i], posTag[i], chunkTag[i]);
            }

            return list;
        }

        #endregion

        #region . Extract .

        public static void Extract(WordTag[] wordTags, string[] words, string[] tags) {
            words = new string[wordTags.Length];
            tags = new string[wordTags.Length];

            for (var i = 0; i < wordTags.Length; i++) {
                words[i] = wordTags[i].Word;
                tags[i] = wordTags[i].POSTag;
            }
        }

        public static void Extract(WordTag[] wordTags, string[] words, string[] tags, string[] chunks) {
            for (var i = 0; i < wordTags.Length; i++) {
                words[i] = wordTags[i].Word;
                if (wordTags[i].ChunkTag == null) {
                    var t = wordTags[i].POSTag;
                    var bar = t.IndexOf("|", StringComparison.InvariantCulture);

                    tags[i] = t.Substring(0, bar);
                    chunks[i] = t.Substring(bar + 1);
                } else {
                    tags[i] = wordTags[i].POSTag;
                    chunks[i] = wordTags[i].ChunkTag;
                }
            }
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:WordTag"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                var hashCode = (ChunkTag != null ? ChunkTag.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (POSTag != null ? POSTag.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Word != null ? Word.GetHashCode() : 0);
                return hashCode;
            }
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
            if (ChunkTag == null)
                return Word + "_" + POSTag;

            return Word + "_" + POSTag + "_" + ChunkTag;
        }

        #endregion
    }
}