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
using System.IO;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.Sentence {
    /// <summary>
    /// The <see cref="SentenceModel"/> is the model used by a learnable <see cref="ISentenceDetector"/>.
    /// </summary>
    public class SentenceModel : BaseModel {
        private const string ComponentName = "SentenceDetectorME";
        private const string EntryName = "sent.model";

        public SentenceModel(string languageCode, IMaxentModel sentModel, Dictionary<string, string> manifestInfoEntries,
            SentenceDetectorFactory sdFactory) : base(ComponentName, languageCode, manifestInfoEntries, sdFactory) {
            artifactMap.Add(EntryName, sentModel);
            CheckArtifactMap();
        }

        public SentenceModel(string languageCode, Dictionary<string, string> manifestInfoEntries,
            BaseToolFactory toolFactory)
            : base(ComponentName, languageCode, manifestInfoEntries, toolFactory) {}

        /// <summary>
        /// Initializes the current instance. The sub-class constructor should call the method <see cref="BaseModel.CheckArtifactMap"/> to check the artifact map is OK.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="manifestInfoEntries">Additional information in the manifest.</param>
        public SentenceModel(string languageCode, Dictionary<string, string> manifestInfoEntries)
            : base(ComponentName, languageCode, manifestInfoEntries) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="stream">The input stream containing the model.</param>
        public SentenceModel(Stream stream) : base(ComponentName, stream) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="fileName">The model filename.</param>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        public SentenceModel(string fileName) : base(ComponentName, fileName) {}

        #region + Properties .

        #region . Abbreviations .

        /// <summary>
        /// Gets the abbreviation dictionary.
        /// </summary>
        /// <value>The abbreviation dictionary.</value>
        /// <remarks>Default value is null.</remarks>
        public Dictionary.Dictionary Abbreviations {
            get {
                if (Factory != null) {
                    return Factory.AbbreviationDictionary;
                }
                return null;
            }
        }

        #endregion

        #region . DefaultFactory .

        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory {
            get { return typeof(SentenceDetectorFactory); }
        }

        #endregion

        #region . EosCharacters .

        /// <summary>
        /// Gets the EOS characters.
        /// </summary>
        /// <value>The EOS characters.</value>
        public char[] EosCharacters {
            get {
                if (Factory != null) {
                    return Factory.EOSCharacters;
                }
                return null;
            }
        }

        #endregion

        #region . MaxentModel .

        /// <summary>
        /// Gets the maximum entropy model.
        /// </summary>
        /// <value>The maximum entropy model.</value>
        public IMaxentModel MaxentModel {
            get { return (IMaxentModel) artifactMap[EntryName]; }
        }

        #endregion

        #region . Factory .

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public SentenceDetectorFactory Factory {
            get { return (SentenceDetectorFactory) ToolFactory; }
        }

        #endregion

        #region . UseTokenEnd .

        /// <summary>
        /// Gets a value indicating whether end token is used.
        /// </summary>
        /// <value><c>true</c> if the end token is used.; otherwise, <c>false</c>.</value>
        /// <remarks>Default value is <c>true</c>.</remarks>
        public bool UseTokenEnd {
            get {
                if (Factory != null && Factory.IsUseTokenEnd.HasValue) {
                    return Factory.IsUseTokenEnd.Value;
                }
                return true;
            }
        }

        #endregion

        #endregion

        #region . ValidateArtifactMap .

        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke super.validateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            var model = artifactMap[EntryName];

            if (model == null) {
                //GISModel
                throw new InvalidFormatException("Unable to find the model entry in the manifest.");
            }

            if (!(model is IMaxentModel)) {
                throw new InvalidFormatException("Invalid model type.");
            }

            if (!ModelUtility.ValidateOutcomes(MaxentModel, SentenceDetectorME.SPLIT, SentenceDetectorME.NO_SPLIT)) {
                throw new InvalidFormatException("The maxent model is not compatible with the sentence detector!");
            }
        }

        #endregion
    }
}