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
using SharpNL.NameFind;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.POSTag {
    /// <summary>
    /// The <see cref="POSModel"/> is the model used by a learnable <see cref="IPOSTagger"/>.
    /// </summary>
    /// <seealso cref="POSTaggerME"/>
    public class POSModel : BaseModel {

        internal const string ComponentName = "POSTaggerME";
        internal const string EntryName = "pos.model";

        #region + Properties .

        #region . Factory .
        /// <summary>
        /// Gets the <see cref="POSTaggerFactory"/> factory.
        /// </summary>
        /// <value>The tool factory.</value>
        public POSTaggerFactory Factory {
            get { return (POSTaggerFactory) ToolFactory; }
        }
        #endregion

        #region . MaxentModel .
        /// <summary>
        /// Gets the maxent model.
        /// </summary>
        /// <value>The maxent model.</value>
        internal IMaxentModel MaxentModel {
            get {
                return artifactMap[EntryName] as IMaxentModel;
            }
        }
        #endregion

        #endregion


        public POSModel(
            string languageCode, 
            ISequenceClassificationModel<string> posModel,
            Dictionary<string, string> manifestInfoEntries,
            POSTaggerFactory posFactory) : base(ComponentName, languageCode, manifestInfoEntries, posFactory) {
            
            if (posModel == null)
                throw new ArgumentNullException("posModel");

            // TODO: This fails probably for the sequence model ... ?!

            artifactMap[EntryName] = posModel;

        }

        public POSModel(string languageCode, Dictionary<string, string> manifestInfoEntries,
            BaseToolFactory toolFactory) : base(ComponentName, languageCode, manifestInfoEntries, toolFactory) {}

        public POSModel(string languageCode, IMaxentModel posModel,
            Dictionary<string, string> manifestInfoEntries, POSTaggerFactory posFactory)
            : this(languageCode, posModel, POSTaggerME.DefaultBeamSize, manifestInfoEntries, posFactory) {
            
        }


        public POSModel(string languageCode, IMaxentModel posModel, int beamSize, Dictionary<string, string> manifestInfoEntries, POSTaggerFactory posFactory)
            : base(ComponentName, languageCode, manifestInfoEntries, posFactory) {

            // TODO: fix the beamSize parameter or use it !

            if (posModel == null)
                throw new InvalidOperationException("The maxentPosModel param must not be null!");

            artifactMap[EntryName] = posModel;
            
            CheckArtifactMap();
        }

        /// <summary>
        /// Initializes the current instance. The sub-class constructor should call the method <see cref="BaseModel.CheckArtifactMap"/> to check the artifact map is OK.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="manifestInfoEntries">Additional information in the manifest.</param>
        public POSModel(string languageCode, Dictionary<string, string> manifestInfoEntries)
            : base(ComponentName, languageCode, manifestInfoEntries) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="stream">The input stream containing the model.</param>
        public POSModel(Stream stream) : base(ComponentName, stream) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="fileName">The model filename.</param>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        public POSModel(string fileName) : base(ComponentName, fileName) {}

        #region . GetDefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type GetDefaultFactory() {
            return typeof (POSTaggerFactory);
        }
        #endregion

        #region . GetPosSequenceModel .
        /// <summary>
        /// Gets the position sequence model.
        /// </summary>
        /// <returns>The position sequence model.</returns>
        public ISequenceClassificationModel<string> GetPosSequenceModel() {

            var posModel = artifactMap[EntryName];
            var meModel = posModel as IMaxentModel;
            if (meModel != null) {
                var beamSize = Manifest.Get(Parameters.BeamSize, NameFinderME.DefaultBeamSize);
                return new ML.BeamSearch<string>(beamSize, meModel);
            }

            return posModel as ISequenceClassificationModel<string>;
            
        }
        #endregion


    }
}