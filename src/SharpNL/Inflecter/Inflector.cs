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

namespace SharpNL.Inflecter {
    /// <summary>
    /// Represents the inflector manager. This class cannot be inherited.
    /// </summary>
    public sealed class Inflector {

        private static readonly Dictionary<string, IInfleter> infletors;

        /// <summary>
        /// Initializes static members of the <see cref="Inflector"/> class.
        /// </summary>
        static Inflector() {
            infletors = new Dictionary<string, IInfleter>();
        }

        private static bool LoadInternal(string language) {
            switch (language) {
                case "en":
                    infletors.Add(language, new Lang.en.Inflecter());
                    return true;
            }
            return false;
        }

        #region . GetInfleter .
        /// <summary>
        /// Gets the infleter associated to the given language code.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <returns>The associated language code or a <c>null</c> value if none.</returns>
        public static IInfleter GetInfleter(string language) {
            if (infletors.ContainsKey(language) || LoadInternal(language))
                return infletors[language];

            return null;
        }
        #endregion

        #region . SetInflecter .
        /// <summary>
        /// Sets a inflecter to the specified language code.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <param name="infleter">The infletor.</param>
        /// <exception cref="System.ArgumentNullException">
        /// language
        /// or
        /// infletor
        /// </exception>
        public static void SetInflecter(string language, IInfleter infleter) {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");

            if (infleter == null)
                throw new ArgumentNullException("infleter");

            infletors[language] = infleter;
        }
        #endregion


        /// <summary>
        /// Returns the plural of a given <paramref name="word" />.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <param name="word">The word to pluralize.</param>
        /// <returns>The pluralized word.</returns>
        public static string Pluralize(string language, string word) {
            return Pluralize(language, word, null, null);
        }

        /// <summary>
        /// Returns the plural form of the given <paramref name="word" />.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <param name="word">The word to pluralize.</param>
        /// <param name="pos">The part-of-speech tag.</param>
        /// <param name="custom">The custom dictionary is for user-defined replacements. This value can be a <c>null</c> value.</param>
        /// <returns>The pluralized word.</returns>
        public static string Pluralize(string language, string word, string pos, IDictionary<string, string> custom) {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");

            var infleter = GetInfleter(language);
            if (infleter == null)
                throw new NotSupportedException("There is no infleter associated to the given language.");

            return infleter.Pluralize(word, pos);
        }


        /// <summary>
        /// Returns the singular form of the given <paramref name="word" />.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <param name="word">The word to singularize.</param>
        /// <returns>The singularized word.</returns>
        public static string Singularize(string language, string word) {
            return Singularize(language, word, null, null);
        }

        /// <summary>
        /// Returns the singular form of the given <paramref name="word" />.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <param name="word">The word to singularize.</param>
        /// <param name="pos">The part-of-speech tag.</param>
        /// <param name="custom">The custom dictionary is for user-defined replacements. This value can be a <c>null</c> value.</param>
        /// <returns>The singularized word.</returns>
        public static string Singularize(string language, string word, string pos, IDictionary<string, string> custom) {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentNullException("language");

            var infleter = GetInfleter(language);
            if (infleter == null)
                throw new NotSupportedException("There is no infleter associated to the given language.");

            return infleter.Singularize(word, pos);
        }

    }
}