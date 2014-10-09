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
using System.Collections.Generic;

using SharpNL.Tokenize;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Class which holds a classified document and its category.
    /// </summary>
    public class DocumentSample : IEquatable<DocumentSample> {
        public DocumentSample(string category, string text)
            : this(category, WhitespaceTokenizer.Instance.Tokenize(text)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSample"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="text">The text.</param>
        public DocumentSample(string category, string[] text) : this(category, text, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSample"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="text">The text.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="category"/>
        /// or
        /// <paramref name="text"/>
        /// </exception>
        public DocumentSample(string category, string[] text, Dictionary<string, Object> extraInformation) {
            if (category == null) {
                throw new ArgumentNullException("category");
            }
            if (text == null) {
                throw new ArgumentNullException("text");
            }

            Category = category;
            Text = text;
            ExtraInformation = extraInformation;
        }

        #region + Properties .

        #region . Category .
        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <value>The category.</value>
        public string Category { get; private set; }
        #endregion

        #region . ExtraInformation .
        /// <summary>
        /// Gets the extra information.
        /// </summary>
        /// <value>The extra information.</value>
        public Dictionary<string, object> ExtraInformation { get; private set; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string[] Text { get; private set; }
        #endregion

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

            sb.Append(Category).Append('\t');
            foreach (var s in Text) {
                sb.Append(s).Append(' ');
            }

            if (Text.Length > 0)
                return sb.ToString(0, sb.Length - 1);

            return sb.ToString();
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
        public bool Equals(DocumentSample other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Category == other.Category && Text.SequenceEqual(other.Text);
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
            return Equals((DocumentSample)obj);
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() {
            unchecked {
                return ((Category != null ? Category.GetHashCode() : 0) * 397) ^ (Text != null ? Text.GetHashCode() : 0);
            }
        }
        #endregion

    }
}