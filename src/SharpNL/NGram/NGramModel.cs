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
using System.Globalization;
using System.IO;
using System.Linq;
using SharpNL.Utility;

using  Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.NGram {
    /// <summary>
    /// The <seealso cref="NGramModel"/> can be used to crate ngrams and character ngrams.
    /// </summary>
    /// <seealso cref="StringList"/>
    public class NGramModel : IEnumerable<StringList> {
        private readonly Dictionary<StringList, int> mNGrams;

        #region + Constructors .

        /// <summary>
        /// Initializes a new empty instance of the <see cref="NGramModel"/>.
        /// </summary>
        public NGramModel() {
            mNGrams = new Dictionary<StringList, int>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NGramModel"/>.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="inputStream"/> was not readable.
        /// </exception>
        /// <exception cref="InvalidFormatException">
        /// Unable to deserialize the dictionary.
        /// or
        /// The count attribute must be set!
        /// or
        /// The count attribute '...' must be a number!
        /// </exception>
        public NGramModel(Stream inputStream) {
            
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (inputStream.CanRead)
                throw new ArgumentException(@"Stream was not readable.", "inputStream");

            var dic = Dic.Deserialize(inputStream) as Dic;

            if (dic == null) {
                throw new InvalidFormatException("Unable to deserialize the dictionary.");
            }

            foreach (var entry in dic) {
                int count;
                if (!entry.Attributes.Contains("count"))
                    throw new InvalidFormatException("The count attribute must be set!");

                if (!int.TryParse(entry.Attributes["count"], out count))
                    throw new InvalidFormatException("The count attribute '" + entry.Attributes["count"] + 
                                                     "' must be a number!");

                
                Add(entry.Tokens);
                SetCount(entry.Tokens, count);
            }

        }

        #endregion

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of entries in the current instance.
        /// </summary>
        /// <value>The number of different grams.</value>
        public int Count {
            get { return mNGrams.Count; }
        }
        #endregion

        #region . NumberOfGrams .

        /// <summary>
        /// Gets the total count of all NGrams.
        /// </summary>
        /// <value>The total count of all NGrams.</value>
        public int NumberOfGrams {
            get {
                return mNGrams.Sum(ngram => ngram.Value);
            }
        }

        #endregion

        #endregion

        #region + Add .
        /// <summary>
        /// Adds one NGram, if it already exists the count increase by one.
        /// </summary>
        /// <param name="ngram">The ngram.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="ngram"/></exception>
        public void Add(StringList ngram) {
            if (ngram == null)
                throw new ArgumentNullException("ngram");

            if (Contains(ngram)) {
                SetCount(ngram, GetCount(ngram) + 1);
            } else {
                mNGrams.Add(ngram, 1);
            }
        }

        /// <summary>
        /// Adds NGrams up to the specified length to the current instance.
        /// </summary>
        /// <param name="ngram">The tokens to build the uni-grams, bi-grams, tri-grams, ... from.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public void Add(StringList ngram, int minLength, int maxLength) {
            if (ngram == null)
                throw new ArgumentNullException("ngram");

            if (minLength < 1)
                throw new ArgumentOutOfRangeException("minLength", minLength, @"minLength param must be at least 1.");

            if (maxLength < 1)
                throw new ArgumentOutOfRangeException("maxLength", maxLength, @"maxLength param must be at least 1.");

            if (minLength > maxLength)
                throw new ArgumentOutOfRangeException("minLength", minLength, @"minLength param must not be larger than maxLength param.");

            for (int lengthIndex = minLength; lengthIndex < maxLength + 1; lengthIndex++) {
                for (int textIndex = 0;

                    textIndex + lengthIndex - 1 < ngram.Count; textIndex++) {

                    var grams = new String[lengthIndex];

                    for (int i = textIndex; i < textIndex + lengthIndex; i++) {
                        grams[i - textIndex] = ngram[i];
                    }

                    Add(new StringList(grams));
                }
            }

        }

        /// <summary>
        /// Adds character NGrams to the current instance.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public void Add(string chars, int minLength, int maxLength) {

            for (int lengthIndex = minLength; lengthIndex < maxLength + 1;
            lengthIndex++) {
                for (int textIndex = 0;
                    textIndex + lengthIndex - 1 < chars.Length; textIndex++) {

                    var gram = chars.Substring(textIndex, textIndex + lengthIndex).ToLowerInvariant();

                    Add(new StringList(gram));

                }
            }
        }

        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether the ngrams are contained bu the current instance.
        /// </summary>
        /// <param name="ngram">The ngram.</param>
        /// <returns><c>true</c> if the ngram is contained; otherwise, <c>false</c>.</returns>
        public bool Contains(StringList ngram) {
            return mNGrams.ContainsKey(ngram);
        }
        #endregion

        #region . CutOff .
        /// <summary>
        /// Deletes all ngram which do appear less than the <paramref name="cutoffUnder"/> value 
        /// and more often than the <paramref name="cutoffOver"/> value.
        /// </summary>
        /// <param name="cutoffUnder">The cutoff under.</param>
        /// <param name="cutoffOver">The cutoff over.</param>
        public void CutOff(int cutoffUnder, int cutoffOver) {
            if (cutoffUnder > 0 || cutoffOver < int.MaxValue) {
                var trash = mNGrams.Where(k => k.Value < cutoffUnder || 
                                               k.Value > cutoffOver).ToArray();

                foreach (var pair in trash) {
                    mNGrams.Remove(pair.Key);
                }
            }
        }
        #endregion

        #region . Equals .
        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="NGramModel"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (obj == this)
                return true;

            var model = obj as NGramModel;
            if (model != null)
                return mNGrams.Equals(model.mNGrams);

            return false;
        }
        #endregion

        #region . GetCount .
        /// <summary>
        /// Gets the count of the given ngram.
        /// </summary>
        /// <param name="ngram">The ngram.</param>
        /// <returns>count of the ngram or 0 if it is not contained.</returns>
        public int GetCount(StringList ngram) {
            if (mNGrams.ContainsKey(ngram)) 
                return mNGrams[ngram];

            return 0;
        }
        #endregion

        #region . GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<StringList> GetEnumerator() {
            return mNGrams.Keys.GetEnumerator();
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

        #region . GetHashCode .
        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="NGramModel"/>.
        /// </returns>
        public override int GetHashCode() {
            return mNGrams.GetHashCode();
        }
        #endregion

        #region . Remove .
        /// <summary>
        /// Removes the specified ngram.
        /// </summary>
        /// <param name="ngram">The ngram to remove from the instance.</param>
        /// <returns><c>true</c> if ngram was successfully removed from the instance, <c>false</c> otherwise.</returns>
        public bool Remove(StringList ngram) {
            return mNGrams.Remove(ngram);
        }
        #endregion

        #region . Serialize .

        /// <summary>
        /// Serializes this instance to the given output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">outputStream</exception>
        /// <exception cref="System.ArgumentException">Stream was not writable.</exception>
        public void Serialize(Stream outputStream) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");

            if (!outputStream.CanWrite)
                throw new ArgumentException(@"Stream was not writable.", "outputStream");

            var dic = new Dic();
            foreach (var item in mNGrams) {
                var entry = dic.Add(item.Key);
                entry.Attributes["count"] = item.Value.ToString(CultureInfo.InvariantCulture);
            }
            dic.Serialize(outputStream);
        }
        #endregion

        #region . Size .
#if DEBUG
        [Obsolete("Use the standard count object.", true)]
        internal int Size() {
            return Count;
        }
#endif
        #endregion

        #region . SetCount .
        /// <summary>
        /// Sets the count of an existing ngram.
        /// </summary>
        /// <param name="ngram">The ngram.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The specified ngram does not exist in this instance.</exception>
        public void SetCount(StringList ngram, int count) {
            if (mNGrams.ContainsKey(ngram)) {
                mNGrams[ngram] = count;
                return;
            }
            throw new KeyNotFoundException("The specified ngram does not exist in this instance.");
        }
        #endregion

        #region + ToDictionary .

        /// <summary>
        /// Creates a dictionary which contain all <see cref="StringList"/> which 
        /// are in the current <see cref="NGramModel"/>.
        /// Entries which are only different in the case are merged into one.
        /// </summary>
        /// <returns>A dictionary of the NGrams.</returns>
        /// <remarks>Calling this method is the same as calling <see cref="ToDictionary(bool)"/> with false.</remarks>
        public Dic ToDictionary() {
            return ToDictionary(false);
        }
        /// <summary>
        /// Creates a dictionary which contains all <see cref="StringList"/>s which
        /// are in the current <see cref="NGramModel"/>.
        /// </summary>
        /// <param name="caseSensitive">Specifies whether case distinctions should be kept in the creation of the dictionary.</param>
        /// <returns>A dictionary of the NGrams.</returns>
        public Dic ToDictionary(bool caseSensitive) {
            var dic = new Dic(caseSensitive);
            foreach (var value in mNGrams.Keys) {
                dic.Add(value);
            }
            return dic;
        }
        
        #endregion

    }
}