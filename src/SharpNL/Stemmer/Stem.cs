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

namespace SharpNL.Stemmer {
    /// <summary>
    /// Represents a stem (the core meaning) and tag data associated with a given word. 
    /// This class cannot be inherited.
    /// </summary>
    public sealed class Stem : IEquatable<Stem>, ICloneable {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="Stem"/> class without a part-of-speech tag.
        /// </summary>
        /// <param name="value">The stem value.</param>
        /// <param name="word">The inflected word.</param>
        public Stem(string value, string word) : this(value, null, word) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stem"/> with the given 
        /// </summary>
        /// <param name="value">The stem value.</param>
        /// <param name="tag">The stem tag.</param>
        /// <param name="word">The inflected word.</param>
        public Stem(string value, string tag, string word) {
            Value = value;
            Tag = tag;
            Word = word;
        }
        #endregion

        #region + Properties .

        #region . Affix .
        /// <summary>
        /// Gets the affix of this stem.
        /// </summary>
        /// <value>The affix of the stem. Often with grammatical functions.</value>
        public string Affix {
            get {
                return Word.Length > Value.Length
                    ? Word.Substring(Value.Length)
                    : null;
            }
        }
        #endregion

        #region . Tag .
        /// <summary>
        /// Gets or sets the stem tag.
        /// </summary>
        /// <value>The stem tag.</value>
        public string Tag { get; private set; }
        #endregion

        #region . Value .
        /// <summary>
        /// Gets or sets the stem value.
        /// </summary>
        /// <value>The stem value.</value>
        public string Value { get; private set; }
        #endregion

        #region . Word .
        /// <summary>
        /// Gets or sets the inflected word.
        /// </summary>
        /// <value>The inflected word.</value>
        public string Word { get; private set; }
        #endregion

        #endregion

        #region . Clone .
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone() {
            return new Stem(Value, Tag, Word);
        }
        #endregion

        #region + Equals .

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Stem other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Tag, other.Tag) && string.Equals(Value, other.Value) && string.Equals(Word, other.Word);
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
            return obj is Stem && Equals((Stem) obj);
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
                var hashCode = (Tag != null ? Tag.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Word != null ? Word.GetHashCode() : 0);
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
            return string.Format("Stem: {0} - {1} ({2})", Value, Word, Tag);
        }
        #endregion

        #region . implicit (stem -> string) .
        /// <summary>
        /// Performs an implicit conversion from <see cref="Stem"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="stem">The stem.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(Stem stem) {
            return stem.Value;
        }
        #endregion

    }
}