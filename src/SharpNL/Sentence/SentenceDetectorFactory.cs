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
using SharpNL.Java;
using SharpNL.Sentence.Language;
using SharpNL.Utility;
using SharpNL.Utility.Serialization;

namespace SharpNL.Sentence {
    using Dic = Dictionary.Dictionary;

    /// <summary>
    /// The factory that provides SentenceDetector default implementations and resources.
    /// </summary>
    [TypeClass("opennlp.tools.sentdetect.SentenceDetectorFactory")]
    public class SentenceDetectorFactory : BaseToolFactory {
        private const string ABBREVIATIONS_ENTRY_NAME = "abbreviations.dictionary";
        private const string EOS_CHARACTERS_PROPERTY = "eosCharacters";
        private const string TOKEN_END_PROPERTY = "useTokenEnd";
        private Dic abbreviationDictionary;
        private char[] eosCharacters;
        private string languageCode;
        private bool? useTokenEnd;


        public SentenceDetectorFactory() {}

        public SentenceDetectorFactory(string languageCode, bool useTokenEnd, Dic abbreviationDictionary,
            char[] eosCharacters) {
            this.languageCode = languageCode;
            this.useTokenEnd = useTokenEnd;
            this.abbreviationDictionary = abbreviationDictionary;
            this.eosCharacters = eosCharacters;
        }

        #region + Properties .

        #region . AbbreviationDictionary .

        /// <summary>
        /// Gets the abbreviation dictionary.
        /// </summary>
        /// <value>The abbreviation dictionary.</value>
        public Dic AbbreviationDictionary {
            get {
                if (abbreviationDictionary == null && ArtifactProvider != null) {
                    abbreviationDictionary = ArtifactProvider.GetArtifact<Dic>(ABBREVIATIONS_ENTRY_NAME);
                }
                return abbreviationDictionary;
            }
        }

        #endregion

        #region . EOSCharacters .

        /// <summary>
        /// Gets the EOS characters.
        /// </summary>
        /// <value>The EOS characters.</value>
        public char[] EOSCharacters {
            get {
                if (eosCharacters == null) {
                    if (ArtifactProvider != null) {
                        var prop = ArtifactProvider.Manifest[EOS_CHARACTERS_PROPERTY];
                        if (!string.IsNullOrEmpty(prop)) {
                            eosCharacters = prop.ToCharArray();
                        }
                    } else {
                        // get from language dependent factory
                        var f = new Factory();
                        eosCharacters = f.GetEOSCharacters(LanguageCode);
                    }
                }
                return eosCharacters;
            }
        }

        #endregion

        #region . LanguageCode .

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode {
            get {
                if (languageCode == null && ArtifactProvider != null) {
                    languageCode = ArtifactProvider.Manifest[ArtifactProvider.LANGUAGE_PROPERTY];
                }
                return languageCode;
            }
        }

        #endregion

        #region . UseTokenEnd .

        /// <summary>
        /// Gets a value indicating whether this instance is use token end.
        /// </summary>
        /// <value><c>true</c> if this instance is use token end; otherwise, <c>false</c>.</value>
        public bool? UseTokenEnd {
            get {
                if (useTokenEnd == null && ArtifactProvider != null) {
                    useTokenEnd = ArtifactProvider.Manifest[TOKEN_END_PROPERTY] == "true";
                }
                if (useTokenEnd.HasValue) {
                    return useTokenEnd.Value;
                }
                return null;
            }
        }

        #endregion

        #endregion

        #region . CreateArtifactMap .

        /// <summary>
        /// Creates a dictionary with pairs of keys and objects. 
        /// The models implementation should call this constructor that creates a model programmatically.
        /// </summary>
        public override Dictionary<string, object> CreateArtifactMap() {
            var map = base.CreateArtifactMap();

            // Abbreviations are optional
            if (abbreviationDictionary != null) {
                map.Add(ABBREVIATIONS_ENTRY_NAME, abbreviationDictionary);
            }

            return map;
        }

        #endregion

        #region . CreateManifestEntries .

        /// <summary>
        /// Creates the manifest entries that will be added to the model manifest
        /// </summary>
        /// <returns>The manifest entries to added to the model manifest.</returns>
        public override Dictionary<string, string> CreateManifestEntries() {
            var entries = base.CreateManifestEntries();

            entries.Add(TOKEN_END_PROPERTY, UseTokenEnd.HasValue && UseTokenEnd.Value ? "true" : "false");

            // EOS characters are optional
            if (EOSCharacters != null) {
                entries.Add(EOS_CHARACTERS_PROPERTY, new string(EOSCharacters));
            }

            return entries;
        }

        #endregion

        #region . GetEndOfSentenceScanner .

        /// <summary>
        /// Gets the end of sentence scanner.
        /// </summary>
        /// <returns>IEndOfSentenceScanner.</returns>
        public IEndOfSentenceScanner GetEndOfSentenceScanner() {
            var f = new Factory();
            if (EOSCharacters != null && EOSCharacters.Length > 0) {
                return f.CreateEndOfSentenceScanner(EOSCharacters);
            }
            return f.CreateEndOfSentenceScanner(LanguageCode);
        }

        #endregion

        #region . GetContextGenerator .

        /// <summary>
        /// Gets the context generator.
        /// </summary>
        /// <returns>ISentenceContextGenerator.</returns>
        public virtual ISentenceContextGenerator GetContextGenerator() {
            var f = new Factory();

            var abbreviations = AbbreviationDictionary != null ? AbbreviationDictionary.ToList() : new List<string>();

            if (EOSCharacters != null && EOSCharacters.Length > 0) {
                return f.CreateSentenceContextGenerator(abbreviations, EOSCharacters);
            }

            return f.CreateSentenceContextGenerator(LanguageCode, abbreviations);
        }

        #endregion

        #region . ValidateArtifactMap .

        /// <summary>
        /// Validates the parsed artifacts. 
        /// </summary>
        /// <exception cref="InvalidFormatException">Invalid artifact map.</exception>
        public override void ValidateArtifactMap() {
            if (ArtifactProvider.Manifest[TOKEN_END_PROPERTY] == null) {
                throw new InvalidFormatException(TOKEN_END_PROPERTY + " is a mandatory property!");
            }

            var abbreviationsEntry = ArtifactProvider.Manifest[ABBREVIATIONS_ENTRY_NAME];
            if (abbreviationDictionary == null || !(abbreviationsEntry is Dic)) {
                throw new InvalidFormatException(
                    "Abbreviations dictionary has wrong type, needs to be of type Dictionary!");
            }
        }

        #endregion
    }
}