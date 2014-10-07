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

namespace SharpNL.POSTag {
    /// <summary>
    /// Represents a triple object with a class, lemma and features.
    /// </summary>
    public class Triple : IEquatable<Triple>, IComparable<Triple> {

        /// <summary>
        /// Initializes a new instance of the <see cref="Triple"/> class.
        /// </summary>
        /// <param name="tripleClass">The triple class.</param>
        /// <param name="tripleLemma">The triple lemma.</param>
        /// <param name="tripleFeatures">The triple features.</param>
        public Triple(string tripleClass, string tripleLemma, string tripleFeatures) {
            Class = tripleClass;
            Lemma = tripleLemma;
            Features = tripleFeatures;
        }

        #region + Properties .

        #region . Class .

        /// <summary>
        /// Gets the triple class.
        /// </summary>
        /// <value>The triple class.</value>
        public string Class { get; private set; }

        #endregion

        #region . Lemma .

        /// <summary>
        /// Gets the triple lemma.
        /// </summary>
        /// <value>The triple lemma.</value>
        public string Lemma { get; private set; }

        #endregion

        #region . Features .

        /// <summary>
        /// Gets the triple features.
        /// </summary>
        /// <value>The triple features.</value>
        public string Features { get; private set; }

        #endregion

        #endregion

        #region + Equals .

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(Triple other) {
            return string.Equals(Class, other.Class) && string.Equals(Lemma, other.Lemma) &&
                   string.Equals(Features, other.Features);
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
            return Equals((Triple) obj);
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
                var hashCode = (Class != null ? Class.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Lemma != null ? Lemma.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Features != null ? Features.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region . CompareTo .

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.
        /// Zero This object is equal to <paramref name="other"/>. 
        /// Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Triple other) {
            if (other == null)
                return 1;

            return string.Compare(Lemma, other.Lemma, StringComparison.InvariantCulture);
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
            return Lemma + ": " + Class + " " + Features;
        }

        #endregion
    }
}