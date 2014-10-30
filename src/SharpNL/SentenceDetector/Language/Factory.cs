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

using System.Collections.Generic;

namespace SharpNL.SentenceDetector.Language {
    /// <summary>
    /// Represents the sentence detection factory.
    /// </summary>
    internal class Factory {

        internal static readonly char[] defaultEosCharacters = { '.', '!', '?' };

        internal static readonly char[] ptEosCharacters = { '.', '?', '!', ';', ':', '(', ')', '«', '»', '\'', '"' };

        internal static readonly char[] thEosCharacters = { ' ', '\n' };

        #region + CreateEndOfSentenceScanner .

        /// <summary>
        /// Creates the end of sentence scanner for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>IEndOfSentenceScanner.</returns>
        public IEndOfSentenceScanner CreateEndOfSentenceScanner(string languageCode) {
            switch (languageCode) {
                case "pt":
                    return new DefaultEndOfSentenceScanner(ptEosCharacters);
                case "th":
                    return new DefaultEndOfSentenceScanner(thEosCharacters);
                default:
                    return new DefaultEndOfSentenceScanner(defaultEosCharacters);
            }
        }

        /// <summary>
        /// Creates the end of sentence scanner with the specified end of sentence characters.
        /// </summary>
        /// <param name="customEOSCharacters">The custom eos characters.</param>
        /// <returns>IEndOfSentenceScanner.</returns>
        public IEndOfSentenceScanner CreateEndOfSentenceScanner(char[] customEOSCharacters) {
            return new DefaultEndOfSentenceScanner(customEOSCharacters);
        }

        #endregion

        #region . CreateSentenceContextGenerator .

        /// <summary>
        /// Creates the sentence context generator.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>ISentenceContextGenerator.</returns>
        public ISentenceContextGenerator CreateSentenceContextGenerator(string languageCode) {
            return CreateSentenceContextGenerator(languageCode, new List<string>());
        }

        /// <summary>
        /// Creates the sentence context generator with the specified abbreviation set and the custom EOS characters.
        /// </summary>
        /// <param name="abbreviations">The abbreviations.</param>
        /// <param name="customEOSCharacters">The custom eos characters.</param>
        /// <returns>The context generator.</returns>
        public ISentenceContextGenerator CreateSentenceContextGenerator(List<string> abbreviations,
            char[] customEOSCharacters) {
            return new DefaultSentenceContextGenerator(abbreviations, customEOSCharacters);
        }

        /// <summary>
        /// Creates the sentence context generator with the specified abbreviation set.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="abbreviations">The abbreviations.</param>
        /// <returns>The context generator.</returns>
        public ISentenceContextGenerator CreateSentenceContextGenerator(string languageCode, List<string> abbreviations) {
            switch (languageCode) {
                case "pt":
                    //return new DefaultSentenceContextGenerator(abbreviations, ptEosCharacters);
                    return new pt.PtSentenceContextGenerator(abbreviations, ptEosCharacters);
                case "th":
                    return new th.ThSentenceContextGenerator();
                default:
                    return new DefaultSentenceContextGenerator(abbreviations, defaultEosCharacters);
            }
        }




        #endregion

        #region . GetEOSCharacters .
        /// <summary>
        /// Gets the end of sentence characters for the specified language.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <returns>System.Char[].</returns>
        public char[] GetEOSCharacters(string languageCode) {
            switch (languageCode) {
                case "pt":
                    return ptEosCharacters;
                case "th":
                    return thEosCharacters;
                default:
                    return defaultEosCharacters;
            }
        }
        #endregion


    }
}