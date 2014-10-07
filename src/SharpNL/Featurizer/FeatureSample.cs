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
using System.Text;

namespace SharpNL.Featurizer {

    /// <summary>
    /// Class for holding features for a single unit of text.
    /// </summary>
    public class FeatureSample : IEquatable<FeatureSample> {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureSample"/> class.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="lemma">The lemma.</param>
        /// <param name="tags">The POS Tags for the sentence.</param>
        /// <param name="features">The features.</param>
        public FeatureSample(string[] sentence, string[] lemma, string[] tags, string[] features) {
            ValidateArguments(sentence, lemma, tags, features);

            Sentence = sentence;
            Lemma = lemma;
            Tags = tags;
            Features = features;
        }

        #region + Properties .

        #region . Features .
        /// <summary>
        /// Gets the features.
        /// </summary>
        /// <value>The features.</value>
        public string[] Features { get; private set; }
        #endregion

        #region . Lemma .
        /// <summary>
        /// Gets the lemma.
        /// </summary>
        /// <value>The lemma.</value>
        public string[] Lemma { get; private set; }
        #endregion

        #region . Sentence .
        /// <summary>
        /// Gets the sentence.
        /// </summary>
        /// <value>The sentence.</value>
        public string[] Sentence { get; private set; }
        #endregion

        #region . Tags .
        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>The tags.</value>
        public string[] Tags { get; private set; }
        #endregion
        
        #endregion

        #region + Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(FeatureSample other) {
            return 
                Equals(Features, other.Features) && 
                Equals(Lemma, other.Lemma) && 
                Equals(Sentence, other.Sentence) && 
                Equals(Tags, other.Tags);
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FeatureSample)obj);
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
            unchecked {
                int hashCode = (Features != null ? Features.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Lemma != null ? Lemma.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Sentence != null ? Sentence.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tags != null ? Tags.GetHashCode() : 0);
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
            var sb = new StringBuilder();
            for (int i = 0; i < Sentence.Length; i++) {
                sb.AppendFormat("{0} [{1}] {2} {3}\n", Sentence[i], Lemma[i], Tags[i], Features[i]);
            }
            return sb.ToString();
        }
        #endregion

        #region . ValidateArguments .
        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="lemma">The lemma.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="features">The features.</param>
        /// <exception cref="System.ArgumentNullException">
        /// sentence
        /// or
        /// lemma
        /// or
        /// tags
        /// or
        /// features
        /// </exception>
        /// <exception cref="System.ArgumentException">All arrays must have the same length!</exception>
        private static void ValidateArguments(string[] sentence, string[] lemma, string[] tags, string[] features) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (lemma == null)
                throw new ArgumentNullException("lemma");

            if (tags == null)
                throw new ArgumentNullException("tags");

            if (features == null)
                throw new ArgumentNullException("features");

            if (!sentence.AreEqual(lemma, tags, features))
                throw new ArgumentException("All arrays must have the same length!");
        }
        #endregion
        
    }
}