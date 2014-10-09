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

namespace SharpNL.Doccat {
    /// <summary>
    /// Represents a Doccat model.
    /// </summary>
    public class DoccatModel : BaseModel {

        private const string ComponentName = "DocumentCategorizerME";
        private const string DoccatEntry = "doccat.model";

        #region + Language .
        /// <summary>
        /// Initializes a new instance of the <see cref="DoccatModel"/> with a input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public DoccatModel(Stream inputStream) : base(ComponentName, inputStream) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoccatModel"/> with the default parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="doccatModel">The doccat model.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="factory">The factory.</param>
        public DoccatModel(string languageCode, IMaxentModel doccatModel, Dictionary<string, string> manifestInfoEntries, DoccatFactory factory)
            : base(ComponentName, languageCode, manifestInfoEntries, factory) {

            artifactMap.Add(DoccatEntry, doccatModel);
            CheckArtifactMap();
        }
        #endregion

        #region . DefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory {
            get { return typeof(DoccatFactory); }
        }
        #endregion

        #region . MaxentModel .
        /// <summary>
        /// Gets the maxent model.
        /// </summary>
        /// <value>The maxent model.</value>
        public IMaxentModel MaxentModel {
            get {
                if (artifactMap != null && artifactMap.ContainsKey(DoccatEntry))
                    return (IMaxentModel)artifactMap[DoccatEntry];

                return null;
            }
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

            if (!artifactMap.ContainsKey(DoccatEntry))
                throw new InvalidFormatException("The artifact does not contain the doccat entry.");

            if (!(artifactMap[DoccatEntry] is IMaxentModel))
                throw new InvalidFormatException("The doccat entry is not supported!");
        }
        #endregion

    }
}