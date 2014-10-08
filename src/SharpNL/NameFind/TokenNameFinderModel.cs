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
using SharpNL.Utility;
using SharpNL.Utility.Model;
using SharpNL.Utility.Serialization;

namespace SharpNL.NameFind {
    // TODO: Fix the model validation, on loading via constructors and input streams

    /// <summary>
    /// The <see cref="TokenNameFinderModel"/> is the model used by a learnable <see cref="ITokenNameFinder"/>.
    /// </summary>
    /// <seealso cref="ITokenNameFinder"/>
    public class TokenNameFinderModel : BaseModel {
        internal const string ComponentName = "NameFinderME";

        internal const string MaxentModelEntry = "nameFinder.model";
        internal const string GeneratorDescriptorEntry = "generator.featuregen";
        internal const string SequenceCodecNameParam = "sequenceCodecImplName";

        #region + Constructors .


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderModel"/> with the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public TokenNameFinderModel(Stream inputStream) : base(ComponentName, inputStream) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderModel"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="nameFinderModel">The name finder model.</param>
        /// <param name="generatorDescriptor">The generator descriptor.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="sequenceCodec">The sequence codec.</param>
        /// <exception cref="System.InvalidOperationException">Model not compatible with name finder!</exception>
        public TokenNameFinderModel(
            string languageCode,
            ISequenceClassificationModel<string> nameFinderModel,
            byte[] generatorDescriptor,
            Dictionary<string, object> resources,
            Dictionary<string, string> manifestInfoEntries,
            ISequenceCodec<string> sequenceCodec)
            : base(ComponentName, languageCode, manifestInfoEntries) {
            Init(nameFinderModel, generatorDescriptor, resources, sequenceCodec);

            if (!sequenceCodec.AreOutcomesCompatible(nameFinderModel.GetOutcomes())) {
                throw new InvalidOperationException("Model not compatible with name finder!");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderModel" /> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="nameFinderModel">The name finder model.</param>
        /// <param name="beamSize">Size of the beam.</param>
        /// <param name="generatorDescriptor">The generator descriptor.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="sequenceCodec">The sequence codec.</param>
        /// <exception cref="System.InvalidOperationException">Model not compatible with name finder!</exception>
        public TokenNameFinderModel(
            string languageCode,
            IMaxentModel nameFinderModel,
            int beamSize,
            byte[] generatorDescriptor,
            Dictionary<string, object> resources,
            Dictionary<string, string> manifestInfoEntries,
            ISequenceCodec<string> sequenceCodec)
            : base(ComponentName, languageCode, manifestInfoEntries) {
            Manifest[Parameters.BeamSize] = beamSize.ToString(CultureInfo.InvariantCulture);

            Init(nameFinderModel, generatorDescriptor, resources, sequenceCodec);

            if (!IsModelValid(nameFinderModel))
                throw new InvalidOperationException("Model not compatible with name finder!");

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderModel"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="nameFinderModel">The name finder model.</param>
        /// <param name="generatorDescriptor">The generator descriptor.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        public TokenNameFinderModel(
            string languageCode, 
            IMaxentModel nameFinderModel,
            byte[] generatorDescriptor, 
            Dictionary<string, object> resources,
            Dictionary<string, string> manifestInfoEntries)
            : this(languageCode, nameFinderModel, NameFinderME.DefaultBeamSize, generatorDescriptor, resources, manifestInfoEntries, new BioCodec()) {
            
        }

        #endregion

        #region + Properties .

        #region . DefaultFactory .
        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory {
            get { return typeof (TokenNameFinderFactory); }
        }
        #endregion

        #region . Factory .

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public TokenNameFinderFactory Factory {
            get { return (TokenNameFinderFactory) ToolFactory; }
        }

        #endregion

        #region . NameFinderSequenceModel .
        /// <summary>
        /// Gets the name finder sequence model.
        /// </summary>
        /// <value>The name finder sequence model.</value>
        public ISequenceClassificationModel<string> NameFinderSequenceModel {
            get {
                if (artifactMap == null || !artifactMap.ContainsKey(MaxentModelEntry))
                    return null;

                var meModel = artifactMap[MaxentModelEntry] as IMaxentModel;
                if (meModel != null) {
                    return new ML.BeamSearch<string>(Manifest.Get(Parameters.BeamSize, NameFinderME.DefaultBeamSize), meModel);
                }

                return artifactMap[MaxentModelEntry] as ISequenceClassificationModel<string>;
            }
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

            RegisterArtifactType("featuregen", SerializeFeatureGen, DeserializeFeatureGen);
        }

        #endregion

        #region . DeserializeFeatureGen .
        private static object DeserializeFeatureGen(Stream inputStream) {
            return inputStream.ReadAllBytes();
        }
        #endregion

        #region . SerializeFeatureGen .
        private static void SerializeFeatureGen(object artifact, Stream outputStream) {
            var bytes = artifact as byte[];
            if (bytes == null)
                throw new InvalidOperationException();

            outputStream.Write(bytes, 0, bytes.Length);
        }
        #endregion

        #region . Init .

        /// <summary>
        /// Initializes the specified name finder model.
        /// </summary>
        /// <param name="nameFinderModel">The name finder model.</param>
        /// <param name="generatorDescriptor">The generator descriptor.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="seqCodec">The seq codec.</param>
        /// <exception cref="System.ArgumentException"></exception>
        private void Init(object nameFinderModel, byte[] generatorDescriptor, Dictionary<string, object> resources, ISequenceCodec<string> seqCodec) {
            
            if (!Library.TypeResolver.IsRegistered(seqCodec.GetType()))
                throw new NotSupportedException("The sequence codec " + seqCodec.GetType().Name + " is not registered on the TypeResolver.");

            Manifest[SequenceCodecNameParam] = Library.TypeResolver.ResolveName(seqCodec.GetType());

            artifactMap[MaxentModelEntry] = nameFinderModel;

            if (generatorDescriptor != null && generatorDescriptor.Length > 0)
                artifactMap[GeneratorDescriptorEntry] = generatorDescriptor;

            if (resources != null) {
                // The resource map must not contain key which are already taken
                // like the name finder maxent model name

                if (resources.ContainsKey(MaxentModelEntry) ||
                    resources.ContainsKey(GeneratorDescriptorEntry)) {
                    throw new ArgumentException();
                }

                // TODO: Add checks to not put resources where no serializer exists,
                //       make that case fail here, should be done in the BaseModel
                artifactMap.AddRange(resources);
            }

            CheckArtifactMap();
        }
        
        #endregion

        #region . IsModelValid .
        private bool IsModelValid(IMaxentModel model) {
            var outcomes = new string[model.GetNumOutcomes()];

            for (var i = 0; i < model.GetNumOutcomes(); i++) {
                outcomes[i] = model.GetOutcome(i);
            }

            return Factory.CreateSequenceCodec().AreOutcomesCompatible(outcomes);
        }
        #endregion

        #region . UpdateFeatureGenerator .
        /// <summary>
        /// Updates the feature generator with the given descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>A updated <see cref="ITokenNameFinder"/> model.</returns>
        public TokenNameFinderModel UpdateFeatureGenerator(byte[] descriptor) {
            if (artifactMap == null || !artifactMap.ContainsKey(MaxentModelEntry))
                return null;

            TokenNameFinderModel model;
            var meModel = artifactMap[MaxentModelEntry] as IMaxentModel;
            if (meModel != null) {
                model = new TokenNameFinderModel(
                    Language, 
                    meModel, 
                    1, 
                    descriptor, 
                    new Dictionary<string, object>(), 
                    new Dictionary<string, string>(), 
                    Factory.CreateSequenceCodec());

                goto found;
            } 

            var scModel = artifactMap[MaxentModelEntry] as ISequenceClassificationModel<string>;
            if (scModel != null) {
                model = new TokenNameFinderModel(
                    Language, 
                    NameFinderSequenceModel, 
                    descriptor, 
                    new Dictionary<string, object>(), 
                    new Dictionary<string, string>(), 
                    Factory.CreateSequenceCodec());
                goto found;
            }

            throw new NotSupportedException("Unexpected model type in the artifact map.");

            found:

            model.artifactMap.Clear();
            model.artifactMap.AddRange(artifactMap);
            model.artifactMap[GeneratorDescriptorEntry] = descriptor;

            return model;
        }

        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke base.validateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            if (NameFinderSequenceModel == null)
                throw new InvalidFormatException("Token Name Finder model is incomplete!");

        }
        #endregion


    }
}