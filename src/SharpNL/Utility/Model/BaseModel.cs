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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using SharpNL.Utility.Serialization;

namespace SharpNL.Utility.Model {
    /// <summary>
    /// Abstract class used by the models.
    /// </summary>
    public abstract class BaseModel : ArtifactProvider {
        private readonly string componentName;
        private readonly bool isLoadedFromSerialized;

        protected BaseToolFactory ToolFactory;

        #region + Constructors .

        #region . BaseModel(string, bool) .

        private BaseModel(string componentName, bool isLoadedFromSerialized) {
            if (string.IsNullOrEmpty(componentName)) {
                throw new ArgumentNullException("componentName", @"The componentName cannot be null.");
            }

            this.isLoadedFromSerialized = isLoadedFromSerialized;
            this.componentName = componentName;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="toolFactory">The tool factory.</param>
        /// <exception cref="System.ArgumentNullException">The language cannot be null.</exception>
        /// <exception cref="System.InvalidOperationException">Unable to initialize the factory.</exception>
        protected BaseModel(
            string componentName,
            string languageCode,
            Dictionary<string, string> manifestInfoEntries,
            BaseToolFactory toolFactory) : this(componentName, false) {

            if (string.IsNullOrEmpty(languageCode)) {
                throw new ArgumentNullException("languageCode", @"The language cannot be null.");
            }

            Manifest[MANIFEST_VERSION_PROPERTY] = "1.0";
            Manifest[LanguageEntry] = languageCode;
            Manifest[VERSION_PROPERTY] = "1.5.3"; // TODO: Implement a better version system for OpenNLP
            Manifest[TimestampEntry] = Library.CurrentTimeMillis().ToString(CultureInfo.InvariantCulture);
            Manifest[ComponentNameEntry] = componentName;

            if (manifestInfoEntries != null) {
                foreach (var entry in manifestInfoEntries) {
                    Manifest[entry.Key] = entry.Value;
                }
            }

            if (toolFactory != null) {
                ToolFactory = toolFactory;

                if (!Library.TypeResolver.IsRegistered(toolFactory.GetType()))
                    throw new InvalidOperationException("The tool factory type " + toolFactory.GetType().Name + " is not registered in the type resolver.");

                Manifest[FACTORY_NAME] = Library.TypeResolver.ResolveName(toolFactory.GetType());

                var map = toolFactory.CreateArtifactMap();
                foreach (var item in map) {
                    artifactMap.Add(item.Key, item.Value);
                }

                var entries = toolFactory.CreateManifestEntries();
                foreach (var item in entries) {
                    Manifest[item.Key] = item.Value;
                }
            }

            if (ToolFactory == null) {
                try {
                    InitializeFactory();
                } catch (Exception ex) {
                    throw new InvalidOperationException("Unable to initialize the factory.", ex);
                }
            }

            CreateBaseArtifactSerializers();

        }

        /// <summary>
        /// Initializes the current instance. The sub-class constructor should call the method <see cref="BaseModel.CheckArtifactMap"/> to check the artifact map is OK.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="manifestInfoEntries">Additional information in the manifest.</param>
        protected BaseModel(string componentName, string languageCode, Dictionary<string, string> manifestInfoEntries)
            : this(componentName, languageCode, manifestInfoEntries, null) {
            
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="stream">The input stream containing the model.</param>
        protected BaseModel(string componentName, Stream stream) : this(componentName, true) {
            Deserialize(stream);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <param name="fileName">The model filename.</param>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        protected BaseModel(string componentName, string fileName) : this(componentName, true) {
            if (!File.Exists(fileName)) {
                throw new FileNotFoundException("The model file does not exist.", fileName);
            }

            var file = new FileStream(fileName, FileMode.Open);
            try {
                Deserialize(file);
            } finally {
                file.Close();
            }
        }

        #endregion

        #region + Properties .

        #region . DefaultFactory .

        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected abstract Type DefaultFactory { get; }

        #endregion

        #region . Language .
        /// <summary>
        /// Gets the language code of the material which was used to train the model or x-unspecified if non was set.
        /// </summary>
        /// <value>The language code of this model.</value>
        public string Language {
            get { return Manifest[LanguageEntry]; }
            protected set { Manifest[LanguageEntry] = value; }
        }
        #endregion

        #endregion

        #region . CreateArtifactSerializers .
        /// <summary>
        /// Registers all serializers for their artifact file name extensions.
        /// Override this method to register custom file extensions.
        /// </summary>
        /// <remarks>
        /// The subclasses should invoke the <see cref="ArtifactProvider.RegisterArtifactType"/> to register 
        /// the proper serialization/deserialization methods for an new extension.
        /// </remarks>
        protected override void CreateArtifactSerializers() {
            base.CreateArtifactSerializers();

            // generic model 
            RegisterArtifactType(".model", GenericModelSerializer.SerializeModel, GenericModelSerializer.DeserializeModel);

            if (ToolFactory != null)
                ToolFactory.CreateArtifactSerializers(this);

        }
        #endregion

        #region . CheckArtifactMap .

        /// <summary>
        /// Checks the artifact map.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The method BaseModel.finishLoadingArtifacts(..) was not called by BaseModel sub-class.
        /// or
        /// Invalid artifact map.
        /// </exception>
        /// <remarks>A subclass should call this method from a constructor which accepts the individual artifact map items, to validate that these items form a valid model.</remarks>
        protected void CheckArtifactMap() {
            try {
                ValidateArtifactMap();
            } catch (Exception ex) {
                throw new InvalidOperationException("Invalid artifact map.", ex);
            }
        }

        #endregion

        #region . InitializeFactory .
        /// <summary>
        /// Initializes the tool factory.
        /// </summary>
        /// <exception cref="InvalidFormatException">The specified factory is invalid or not supported.</exception>
        /// <exception cref="System.InvalidOperationException">Unable to initialize the tool factory.</exception>
        private void InitializeFactory() {
            var factoryName = Manifest[FACTORY_NAME];

            if (string.IsNullOrEmpty(factoryName)) {
                if (DefaultFactory != null) {
                    ToolFactory = (BaseToolFactory)Activator.CreateInstance(DefaultFactory);
                    ToolFactory.Initialize(this);
                }
            } else {
                if (Library.TypeResolver.IsRegistered(factoryName)) {
                    try {
                        var type = Library.TypeResolver.ResolveType(factoryName);

                        ToolFactory = Library.CreateInstance<BaseToolFactory>(type);
                        ToolFactory.Initialize(this);
                    } catch (Exception ex) {
                        throw new InvalidOperationException("Unable to initialize the tool factory.", ex);
                    }
                } else {
                    throw new InvalidOperationException(string.Format("The tool factory '{0}' is not registered in the type resolver.", factoryName));
                }
            }
        }

        #endregion

        #region . ManifestLoaded .
        /// <summary>
        /// Called when the manifest is deserialized from a stream. This method initialize the tool factory of the model.
        /// </summary>
        /// <remarks>Subclasses MUST invoke base.ManifestDeserialized at the beginning of this method, otherwise the factory will be not initialized properly.</remarks>
        protected override void ManifestDeserialized() {
            InitializeFactory();
        }
        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke base.ValidateArtifactMap at the beginning of this method.</remarks>
        /// <exception cref="InvalidFormatException">The artifact map is invalid.</exception>
        /// <remarks>
        /// The exception <see cref="InvalidFormatException"/> should be thrown if a problem is found during the validation.
        /// </remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            var version = Version.Parse(Manifest[VERSION_PROPERTY]);

            if (version == null) {
                throw new InvalidFormatException("Unable to parse the model version.");
            }
            if (version.Major != 1 || version.Minor != 5) {
                throw new InvalidFormatException(string.Format(
                    "The model version {0} is not supported by this version.", version));
            }
            if (version.Snapshot) {
                throw new InvalidFormatException("Snapshot models are not supported!");
            }
        }

        #endregion
    }
}