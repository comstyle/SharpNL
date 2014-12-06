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
    /// <summary>
    /// Represents a chunker model.
    /// </summary>
    public class ChunkerModel : BaseModel {
        private const string ComponentName = "ChunkerME";
        private const string ChunkerEntry = "chunker.model";

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerModel"/> class using a <see cref="T:ISequenceClassificationModel{string}"/> as the chunker model.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="chunkerModel">The chunker model.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="factory">The chunker factory.</param>
        public ChunkerModel(string languageCode, ISequenceClassificationModel<string> chunkerModel, Dictionary<string, string> manifestInfoEntries, ChunkerFactory factory) 
            : base(ComponentName, languageCode, manifestInfoEntries, factory) {

            artifactMap.Add(ChunkerEntry, chunkerModel);

            CheckArtifactMap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerModel"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="chunkerModel">The chunker model.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="factory">The chunker factory.</param>
        public ChunkerModel(string languageCode, IMaxentModel chunkerModel, Dictionary<string, string> manifestInfoEntries, ChunkerFactory factory) 
            : this(languageCode, chunkerModel, ChunkerME.DefaultBeamSize, manifestInfoEntries, factory) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerModel"/> class with a specified <paramref name="beamSize"/> value.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="chunkerModel">The chunker model.</param>
        /// <param name="beamSize">Size of the beam.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="factory">The chunker factory.</param>
        public ChunkerModel(string languageCode, IMaxentModel chunkerModel, int beamSize, Dictionary<string, string> manifestInfoEntries, ChunkerFactory factory) 
            : base(ComponentName, languageCode, manifestInfoEntries, factory) {

            artifactMap[ChunkerEntry] = chunkerModel;

            Manifest[Parameters.BeamSize] = beamSize.ToString(CultureInfo.InvariantCulture);

            CheckArtifactMap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerModel"/> class using a input stream to deserialize the chunker model into a new instance.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
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
                return Manifest.Contains(Parameters.BeamSize)
                    ? int.Parse(Manifest[Parameters.BeamSize])
                    : NameFinderME.DefaultBeamSize;
            }
        }
        #endregion

        #region . ChunkerSequenceModel .

        /// <summary>
        /// Gets the chunker sequence model.
        /// </summary>
        /// <value>The chunker sequence model.</value>
        public ISequenceClassificationModel<string> ChunkerSequenceModel {
            get {
                var maxentModel = artifactMap[ChunkerEntry] as IMaxentModel;
                if (maxentModel != null) {
                    return new BeamSearch(BeamSize, maxentModel);
                }

                return artifactMap[ChunkerEntry] as ISequenceClassificationModel<string>;
            }
        }

        #endregion

        #region . DefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory {
            get { return typeof (ChunkerFactory); }
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

    }
}