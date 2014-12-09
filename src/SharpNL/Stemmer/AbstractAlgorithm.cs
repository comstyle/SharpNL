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
using System.Linq;
using System.Text;

namespace SharpNL.Stemmer {
    /// <summary>
    /// Represents a abstract stemming algorithm.
    /// </summary>
    public abstract class AbstractAlgorithm {

        /// <summary>
        /// Gets or sets the vowels.
        /// </summary>
        /// <value>The vowels.</value>
        protected HashSet<char> Vowels { get; set; }

        protected AbstractAlgorithm() {
            Vowels = new HashSet<char>(new [] { 'a', 'e', 'i', 'o', 'u' });
        }

        #region . ContainsVowel .
        /// <summary>
        /// Determines whether the specified <see cref="string" /> contains a vowel.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <returns><c>true</c> if the specified <see cref="string" /> contains a vowel; otherwise, <c>false</c>.</returns>
        protected bool ContainsVowel(string value) {
            return !string.IsNullOrEmpty(value) && value.Any(IsVowel);
        }

        #endregion

        #region . IsConsonant .
        /// <summary>
        /// Determines whether the specified character is a consonant.
        /// </summary>
        /// <param name="chr">The character.</param>
        /// <returns><c>true</c> if the specified character is a consonant; otherwise, <c>false</c>.</returns>
        protected bool IsConsonant(char chr) {
            if (!char.IsLetter(chr))
                return false;

            return !IsVowel(chr);
        }

        /// <summary>
        /// Determines whether the character at the specified <paramref name="index"/> is a consonant.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if the character at the specified <paramref name="index"/> is a consonant; otherwise, <c>false</c>.</returns>
        protected bool IsConsonant(StringBuilder sb, int index) {
            if (sb.Length < index)
                return false;

            if (!char.IsLetter(sb[index]))
                return false;

            return !IsVowel(sb, index);
        }
        #endregion

        #region . IsVowel .
        /// <summary>
        /// Determines whether the specified character is vowel.
        /// </summary>
        /// <param name="chr">The character.</param>
        /// <returns><c>true</c> if the specified character is vowel; otherwise, <c>false</c>.</returns>
        protected bool IsVowel(char chr) {
            return Vowels.Contains(chr);
        }
        /// <summary>
        /// Determines whether the character at the specified <paramref name="index"/> is vowel.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if the character is vowel; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">index</exception>
        protected bool IsVowel(StringBuilder sb, int index) {
            if (sb.Length < index)
                throw new ArgumentOutOfRangeException("index");

            return Vowels.Contains(sb[index]);
        }
        #endregion

    }
}