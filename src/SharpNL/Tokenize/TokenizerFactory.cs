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
using SharpNL.Tokenize.Language;
using SharpNL.Utility;
using SharpNL.Utility.Serialization;

namespace SharpNL.Tokenize {
    /// <summary>
    /// The factory that provides <see cref="ITokenizer"/> default implementations and
    /// resources. Users can extend this class if their application requires
    /// overriding the <see cref="ITokenContextGenerator"/>, <see cref="Dictionary.Dictionary"/> etc.
    /// </summary>
    [TypeClass("opennlp.tools.tokenize.TokenizerFactory")]
    public class TokenizerFactory : BaseToolFactory {

        private string languageCode;
        private Dictionary.Dictionary abbreviationDictionary;
        private bool? useAlphaNumericOptimization;
        private string alphaNumericPattern;
        private ITokenContextGenerator contextGenerator;

        private const string ABBREVIATIONS_ENTRY_NAME = "abbreviations.dictionary";
        private const string USE_ALPHA_NUMERIC_OPTIMIZATION = "useAlphaNumericOptimization";
        private const string ALPHA_NUMERIC_PATTERN = "alphaNumericPattern";


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerFactory"/> class.
        /// This constructor should be used only burring de deserialization process. Use other constructors to create this tool factory.
        /// </summary>
        public TokenizerFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerFactory"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="abbreviationDictionary">The abbreviation dictionary.</param>
        /// <param name="useAlphaNumericOptimization">if true alpha numerics are skipped.</param>
        public TokenizerFactory(string languageCode, Dictionary.Dictionary abbreviationDictionary,
            bool useAlphaNumericOptimization) {
            this.languageCode = languageCode;
            this.useAlphaNumericOptimization = useAlphaNumericOptimization;
            this.abbreviationDictionary = abbreviationDictionary;

            alphaNumericPattern = Factory.GetAlphanumeric(languageCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerFactory"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="abbreviationDictionary">The abbreviation dictionary.</param>
        /// <param name="useAlphaNumericOptimization">if true alpha numerics are skipped.</param>
        /// <param name="alphaNumericPattern">The alpha numeric pattern.</param>
        public TokenizerFactory(string languageCode, Dictionary.Dictionary abbreviationDictionary, bool useAlphaNumericOptimization, string alphaNumericPattern) {
            this.languageCode = languageCode;
            this.useAlphaNumericOptimization = useAlphaNumericOptimization;
            this.alphaNumericPattern = alphaNumericPattern;
            this.abbreviationDictionary = abbreviationDictionary;
        }

        #region + Properties .

        #region . AbbreviationDictionary .

        /// <summary>
        /// Gets the abbreviation dictionary.
        /// </summary>
        /// <value>The abbreviation dictionary.</value>
        public virtual Dictionary.Dictionary AbbreviationDictionary {
            get {
                if (abbreviationDictionary == null && ArtifactProvider != null)
                    abbreviationDictionary =
                        ArtifactProvider.GetArtifact<Dictionary.Dictionary>(ABBREVIATIONS_ENTRY_NAME);

                return abbreviationDictionary;
            }
        }
        #endregion

        #region . AlphaNumericPattern .
        /// <summary>
        /// Gets the alpha numeric pattern.
        /// </summary>
        /// <value>The user specified alpha numeric pattern or a default.</value>
        public virtual string AlphaNumericPattern {
            get {
                if (alphaNumericPattern == null && ArtifactProvider != null) {
                    if (ArtifactProvider.Manifest.Contains(ALPHA_NUMERIC_PATTERN)) {
                        alphaNumericPattern = ArtifactProvider.Manifest[ALPHA_NUMERIC_PATTERN];
                    }
                }

                return alphaNumericPattern ?? (alphaNumericPattern = Factory.GetAlphanumeric(languageCode));
            }
        }
        #endregion

        #region . ContextGenerator .

        /// <summary>
        /// Gets the context generator.
        /// </summary>
        /// <value>The context generator.</value>
        public virtual ITokenContextGenerator ContextGenerator {
            get {
                return contextGenerator ?? (contextGenerator = Factory.CreateTokenContextGenerator(LanguageCode, AbbreviationDictionary != null ? AbbreviationDictionary.ToList() : new List<string>()));
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
                if (languageCode == null && ArtifactProvider != null)
                    languageCode = ArtifactProvider.Manifest[ArtifactProvider.LanguageEntry];

                return languageCode;
            }
        }

        #endregion

        #region . UseAlphaNumericOptimization .
        /// <summary>
        /// Gets a value indicating whether to use alpha numeric optimization.
        /// </summary>
        /// <value><c>true</c> if alpha numeric optimization will be used; otherwise, <c>false</c>.</value>
        public bool UseAlphaNumericOptimization {
            get {
                if (useAlphaNumericOptimization == null && ArtifactProvider != null)
                    useAlphaNumericOptimization = ArtifactProvider.Manifest[USE_ALPHA_NUMERIC_OPTIMIZATION] == "true";

                if (useAlphaNumericOptimization != null)
                    return useAlphaNumericOptimization.Value;

                return false;
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
            var artifactMap =  base.CreateArtifactMap();

            if (abbreviationDictionary != null)
                artifactMap.Add(ABBREVIATIONS_ENTRY_NAME, abbreviationDictionary);

            return artifactMap;
        }

        #endregion

        #region . CreateManifestEntries .
        /// <summary>
        /// Creates the manifest entries that will be added to the model manifest
        /// </summary>
        /// <returns>The manifest entries to added to the model manifest.</returns>
        public override Dictionary<string, string> CreateManifestEntries() {
            var manifestEntries = base.CreateManifestEntries();

            manifestEntries[USE_ALPHA_NUMERIC_OPTIMIZATION] = UseAlphaNumericOptimization ? "true" : "false";

            if (AlphaNumericPattern != null)
                manifestEntries[ALPHA_NUMERIC_PATTERN] = AlphaNumericPattern;


            return manifestEntries;
        }
        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts. 
        /// </summary>
        /// <exception cref="InvalidFormatException">Invalid artifact map.</exception>
        public override void ValidateArtifactMap() {
            if (ArtifactProvider.Manifest.Contains(USE_ALPHA_NUMERIC_OPTIMIZATION)) {
                throw new InvalidFormatException(USE_ALPHA_NUMERIC_OPTIMIZATION + " is a mandatory property!");
            }

            var abbreviationsEntry = ArtifactProvider.GetArtifact<object>(ABBREVIATIONS_ENTRY_NAME);
            if (abbreviationsEntry != null && !(abbreviationsEntry is Dictionary.Dictionary)) {
                throw new InvalidFormatException("Abbreviations dictionary is not an valid dictionary.");
            }
        }
        #endregion



    }
}