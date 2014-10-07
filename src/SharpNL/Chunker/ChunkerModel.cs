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
using System.Globalization;
using System.IO;
using SharpNL.ML.Model;
using SharpNL.NameFind;
using SharpNL.Utility;
using SharpNL.Utility.Model;

using BeamSearch = SharpNL.ML.BeamSearch<string>;

namespace SharpNL.Chunker {
    public class ChunkerModel : BaseModel {
        private const string ComponentName = "ChunkerME";
        private const string ChunkerEntry = "chunker.model";

        private const string BEAM_SIZE_PARAMETER = "BeamSize";

        #region + Constructors .

        public ChunkerModel(
            string languageCode,
            ISequenceClassificationModel<string> chunkerModel,
            Dictionary<string, string> manifestInfoEntries,
            ChunkerFactory factory) : base(ComponentName, languageCode, manifestInfoEntries, factory) {
            artifactMap.Add(ChunkerEntry, chunkerModel);

            CheckArtifactMap();
        }

        public ChunkerModel(
            string languageCode,
            IMaxentModel chunkerModel,
            Dictionary<string, string> manifestInfoEntries,
            ChunkerFactory factory) : this(languageCode, chunkerModel, ChunkerME.DEFAULT_BEAM_SIZE, manifestInfoEntries, factory) {
            
        }

        public ChunkerModel(string languageCode, IMaxentModel chunkerModel, int beamSize,
            Dictionary<string, string> manifestInfoEntries, ChunkerFactory factory) : base(ComponentName, languageCode, manifestInfoEntries, factory) {

            artifactMap[ChunkerEntry] = chunkerModel;

            Manifest[BEAM_SIZE_PARAMETER] = beamSize.ToString(CultureInfo.InvariantCulture);

            CheckArtifactMap();
        }

        public ChunkerModel(Stream inputStream) : base(ComponentName, inputStream) { }

        #endregion

        #region + Properties .

        #region . BeamSize .
        /// <summary>
        /// Gets the beam size.
        /// </summary>
        /// <value>The the beam size.</value>
        public int BeamSize {
            get {
                return Manifest.Contains(BEAM_SIZE_PARAMETER)
                    ? int.Parse(Manifest[BEAM_SIZE_PARAMETER])
                    : NameFinderME.DefaultBeamSize;
            }
        }
        #endregion

        #region . MaxentModel .
        /// <summary>
        /// Gets the maxent model.
        /// </summary>
        /// <value>The maxent model.</value>
        internal IMaxentModel MaxentModel {
            get {
                if (artifactMap.ContainsKey(ChunkerEntry) && artifactMap[ChunkerEntry] is IMaxentModel) {
                    return (IMaxentModel) artifactMap[ChunkerEntry];
                }
                return null;
            }
        }
        #endregion

        #endregion

        #region . Factory .
        /// <summary>
        /// Gets the chunker tool factory.
        /// </summary>
        /// <value>The chunker tool factory.</value>
        public ChunkerFactory Factory {
            get { return (ChunkerFactory)ToolFactory; }
        }
        #endregion

        #region . GetChunkerSequenceModel .

        public ISequenceClassificationModel<string> GetChunkerSequenceModel() {

            var maxentModel = artifactMap[ChunkerEntry] as IMaxentModel;
            if (maxentModel != null) {
                return new BeamSearch(BeamSize, maxentModel);
            }

            return artifactMap[ChunkerEntry] as ISequenceClassificationModel<string>;
        }

        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke base.ValidateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            if (!artifactMap.ContainsKey(ChunkerEntry) || !(artifactMap[ChunkerEntry] is AbstractModel)) {
                throw new InvalidFormatException("Chunker model is incomplete!");
            }
        }
        #endregion

        #region . GetDefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type GetDefaultFactory() {
            return typeof (ChunkerFactory);
        }
        #endregion

    }
}