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

using System.Text;

namespace SharpNL.Stemmer.Porter {
    /// <summary>
    /// Represents a Porter stemming algorithm.
    /// </summary>
    public abstract class PorterAlgorithm : AbstractAlgorithm {

        #region . Cons .
        /// <summary>
        /// cons(i) is true &lt;=&gt; b[i] is a consonant.
        /// </summary>
        protected bool Cons(StringBuilder sb, int index) {
            if (IsVowel(sb[index]))
                return false;

            if (sb[index] == 'y') {
                return index == 0 || !Cons(sb, index - 1);
            }

            return true;
        }
        #endregion

        #region . FindR .
        /// <summary>
        /// Return the standard interpretations of the string regions (R1 and R2).
        /// </summary>
        /// <param name="word">The word whose regions R1 and R2 are determined.</param>
        /// <returns>The region for the respective word.</returns>
        /// <remarks>
        /// A detailed description of how to define R1 and R2 
        /// can be found at <see cref="http://snowball.tartarus.org/texts/r1r2.html"/>
        /// </remarks>
        protected string FindR(string word) {
            if (string.IsNullOrEmpty(word) || word.Length == 1)
                return string.Empty;

            for (var i = 1; i < word.Length - 1; i++)
                if (IsConsonant(word[i]) && IsVowel(word[i - 1]))
                    return word.Substring(i + 1);

            return string.Empty;
        }
        #endregion

        #region . FindRV .
        /// <summary>
        /// Return the standard interpretation of the string region RV.
        /// </summary>
        /// <param name="word">The word whose region RV is determined.</param>
        /// <returns>The region RV for the respective word.</returns>
        protected string FindRV(string word) {
            if (word.Length <= 2)
                return string.Empty;

            if (IsConsonant(word[1])) {
                for (var i = 2; i < word.Length - 1; i++)
                    if (IsVowel(word[i]))
                        return word.Substring(i + 1);

            } else if (IsVowel(word[0]) && IsVowel(word[1])) {
                for (var i = 2; i < word.Length - 1; i++)
                    if (IsConsonant(word[i]))
                        return word.Substring(i + 1);

            } else {
                return word.Substring(3);
            }
            
            return string.Empty;
        }
        #endregion

        /// <summary>
        /// Stems the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stem.</returns>
        public abstract string Stem(string word);
    }
}