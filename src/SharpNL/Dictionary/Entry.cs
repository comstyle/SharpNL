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
using SharpNL.Utility;

namespace SharpNL.Dictionary {
    /// <summary>
    /// Represents an dictionary entry.
    /// </summary>
    /// <seealso cref="Attributes"/>
    public class Entry : IEquatable<Entry>, ICloneable {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        public Entry() : this(new StringList(), new Attributes()) {}


        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> with the specified tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        public Entry(StringList tokens) : this(tokens, new Attributes()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> with the specified tokens.
        /// </summary>
        /// <param name="tokens">The string tokens.</param>
        public Entry(params string[] tokens) : this(new StringList(tokens), new Attributes()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> with the specified strnig tokens and attributes.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="attributes">The attributes.</param>
        public Entry(string[] tokens, Attributes attributes) : this(new StringList(tokens), attributes) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entry"/> class.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="attributes">The attributes.</param>
        public Entry(StringList tokens, Attributes attributes) {
            Attributes = attributes;
            Tokens = tokens;
        }



        #endregion

        #region . Attributes .
        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public Attributes Attributes { get; private set; }

        #endregion

        #region . CaseSensitive .
        /// <summary>
        /// Gets or sets a value indicating whether this entry is case sensitive.
        /// </summary>
        /// <value><c>true</c> if this entry is case sensitive; otherwise, <c>false</c>.</value>
        internal bool CaseSensitive { get; set; }
        #endregion

        #region . Clone .
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone() {
            return new Entry(Tokens, Attributes) {
                CaseSensitive = CaseSensitive
            };
        }
        #endregion

        #region . Tokens .

        /// <summary>
        /// Gets the tokens.
        /// </summary>
        /// <value>The tokens.</value>
        public StringList Tokens { get; private set; }

        #endregion

        #region . Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Entry other) {
            if (other == null)
                return false;

            if (CaseSensitive) {
                return Tokens.Equals(other.Tokens);
            }

            return Tokens.CompareToIgnoreCase(other.Tokens);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {

            if (obj == null)
                return false;

            var entry = obj as Entry;
            if (entry != null)
                return Equals(entry);

            return false;
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Returns the hash code for this entry.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:Entry"/>.
        /// </returns>
        public override int GetHashCode() {
            return Tokens.ToString().ToLowerInvariant().GetHashCode();
        }



        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current entry.
        /// </summary>
        /// <returns>
        /// A string that represents the current entry.
        /// </returns>
        public override string ToString() {
            return Tokens.ToString();
        }
        #endregion

    }
}