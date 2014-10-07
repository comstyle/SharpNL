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
using SharpNL.Chunker;
using SharpNL.ML.Model;
using SharpNL.Parser.Lang;
using SharpNL.POSTag;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.Parser {
    /// <summary>
    /// This is an abstract base class for <see cref="ParserModel"/> implementations.
    /// </summary>
    public class ParserModel : BaseModel {
        private const string ComponentName = "Parser";

        private const string BUILD_MODEL_ENTRY_NAME = "build.model";

        private const string CHECK_MODEL_ENTRY_NAME = "check.model";

        private const string ATTACH_MODEL_ENTRY_NAME = "attach.model";

        private const string PARSER_TAGGER_MODEL_ENTRY_NAME = "parsertager.postagger";

        private const string CHUNKER_TAGGER_MODEL_ENTRY_NAME = "parserchunker.chunker";

        private const string HEAD_RULES_MODEL_ENTRY_NAME = "head-rules.headrules";

        private const string PARSER_TYPE = "parser-type";

        #region + Constructors .

        public ParserModel(
            string languageCode,
            IMaxentModel buildModel,
            IMaxentModel checkModel,
            IMaxentModel attachModel,
            POSModel parserTagger,
            ChunkerModel chunkerTagger,
            AbstractHeadRules headRules,
            ParserType modelType,
            Dictionary<string, string> manifestInfoEntries) : base(ComponentName, languageCode, manifestInfoEntries) {
            switch (modelType) {
                case ParserType.Chunking:
                    if (attachModel != null)
                        throw new ArgumentException(@"attachModel must be null for chunking parser!", "attachModel");

                    Manifest[PARSER_TYPE] = "CHUNKING";
                    break;
                case ParserType.TreeInsert:
                    if (attachModel == null)
                        throw new ArgumentException(@"attachModel must not be null for treeinsert parser!",
                            "attachModel");

                    Manifest[PARSER_TYPE] = "TREEINSERT";

                    artifactMap[ATTACH_MODEL_ENTRY_NAME] = attachModel;

                    break;
                default:
                    throw new ArgumentException(@"Unknown mode type.", "modelType");
            }

            artifactMap[BUILD_MODEL_ENTRY_NAME] = buildModel;
            artifactMap[CHECK_MODEL_ENTRY_NAME] = checkModel;
            artifactMap[PARSER_TAGGER_MODEL_ENTRY_NAME] = parserTagger;
            artifactMap[CHUNKER_TAGGER_MODEL_ENTRY_NAME] = chunkerTagger;
            artifactMap[HEAD_RULES_MODEL_ENTRY_NAME] = headRules;

            CheckArtifactMap();
        }

        public ParserModel(string languageCode, IMaxentModel buildModel, IMaxentModel checkModel,
            IMaxentModel attachModel, POSModel parserTagger, ChunkerModel chunkerTagger, AbstractHeadRules headRules,
            ParserType modelType)
            : this(
                languageCode, buildModel, checkModel, attachModel, parserTagger, chunkerTagger, headRules, modelType,
                null) {}


        public ParserModel(string languageCode, IMaxentModel buildModel, IMaxentModel checkModel, POSModel parserTagger,
            ChunkerModel chunkerTagger, AbstractHeadRules headRules, ParserType type,
            Dictionary<string, string> manifestInfoEntries)
            : this(
                languageCode, buildModel, checkModel, null, parserTagger, chunkerTagger, headRules, type,
                manifestInfoEntries) {}

        public ParserModel(Stream inputStream) : base(ComponentName, inputStream) {}

        #endregion

        #region + Properties .

        #region . AttachModel .

        public IMaxentModel AttachModel {
            get {
                if (artifactMap.ContainsKey(ATTACH_MODEL_ENTRY_NAME))
                    return artifactMap[ATTACH_MODEL_ENTRY_NAME] as IMaxentModel;

                return null;
            }
        }
        #endregion

        #region . BuildModel .

        public IMaxentModel BuildModel {
            get {
                if (artifactMap.ContainsKey(BUILD_MODEL_ENTRY_NAME))
                    return artifactMap[BUILD_MODEL_ENTRY_NAME] as IMaxentModel ;

                return null;
            }
        }
        #endregion

        #region . CheckModel .

        public IMaxentModel CheckModel {
            get {
                if (artifactMap.ContainsKey(CHECK_MODEL_ENTRY_NAME))
                    return artifactMap[CHECK_MODEL_ENTRY_NAME] as IMaxentModel;

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
            get { return null; }
        }
        #endregion

        #region . HeadRules .

        public AbstractHeadRules HeadRules {
            get {
                if (artifactMap.ContainsKey(HEAD_RULES_MODEL_ENTRY_NAME))
                    return artifactMap[HEAD_RULES_MODEL_ENTRY_NAME] as AbstractHeadRules;

                return null;
            }
        }
        #endregion

        #region . ParserChunkerModel .

        public ChunkerModel ParserChunkerModel {
            get {
                if (artifactMap.ContainsKey(CHUNKER_TAGGER_MODEL_ENTRY_NAME))
                    return artifactMap[CHUNKER_TAGGER_MODEL_ENTRY_NAME] as ChunkerModel;

                return null;
            }
        }
        #endregion

        #region . ParserTaggerModel .

        public POSModel ParserTaggerModel {
            get {
                if (artifactMap.ContainsKey(PARSER_TAGGER_MODEL_ENTRY_NAME))
                    return artifactMap[PARSER_TAGGER_MODEL_ENTRY_NAME] as POSModel;

                return null;
            }
        }
        #endregion

        #region . ParserType .
        public ParserType ParserType {
            get {
                switch (Manifest[PARSER_TYPE]) {
                    case "CHUNKING":
                        return ParserType.Chunking;
                    case "TREEINSERT":
                        return ParserType.TreeInsert;
                }
                return default(ParserType);
            }
        }
        #endregion

        #endregion

        #region . CreateArtifactSerializers .

        protected override void CreateArtifactSerializers() {
            base.CreateArtifactSerializers();
            // note from OpenNLP (for future adaptations)

            // In 1.6.x the headrules artifact is serialized with the new API
            // which uses the Serializable interface
            // This change is not backward compatible with the 1.5.x models.
            // In order to load 1.5.x model the English headrules serializer must be
            // put on the serializer map.

            RegisterArtifactType(".headrules",
                (artifact, stream) => HeadRulesManager.Serialize(artifact as AbstractHeadRules, stream),
                stream => HeadRulesManager.Deserialize(Language, stream));

            RegisterArtifactType(".postagger", (artifact, stream) => {
                var model = artifact as POSModel;
                if (model == null)
                    throw new InvalidOperationException();

                model.Serialize(stream);
            }, stream => new POSModel(stream));

            RegisterArtifactType(".chunker", (artifact, stream) => {
                var model = artifact as ChunkerModel;
                if (model == null)
                    throw new InvalidOperationException();

                model.Serialize(stream);
            }, stream => new ChunkerModel(stream));
        }

        #endregion

        #region . UpdateBuildModel .
        public ParserModel UpdateBuildModel(IMaxentModel buildModel) {
            return new ParserModel(Language, buildModel, CheckModel, AttachModel, ParserTaggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateCheckModel .
        public ParserModel UpdateCheckModel(IMaxentModel checkModel) {
            return new ParserModel(Language, BuildModel, checkModel, AttachModel, ParserTaggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateTaggerModel .
        public ParserModel UpdateTaggerModel(POSModel taggerModel) {
            return new ParserModel(Language, BuildModel, CheckModel, AttachModel, taggerModel, ParserChunkerModel, HeadRules, ParserType);
        }
        #endregion

        #region . UpdateChunkerModel .
        public ParserModel UpdateChunkerModel(ChunkerModel chunkModel) {
            return new ParserModel(Language, BuildModel, CheckModel, AttachModel, ParserTaggerModel, chunkModel, HeadRules, ParserType);
        }
        #endregion

        #region . ValidateArtifactMap .
        /// <summary>
        /// Validates the parsed artifacts.
        /// </summary>
        /// <exception cref="InvalidFormatException">Unable to find the manifest entry.</exception>
        /// <remarks>Subclasses should generally invoke super.validateArtifactMap at the beginning of this method.</remarks>
        protected override void ValidateArtifactMap() {
            base.ValidateArtifactMap();

            if (artifactMap.ContainsKey(BUILD_MODEL_ENTRY_NAME) &&
                (artifactMap[BUILD_MODEL_ENTRY_NAME] as AbstractModel) == null) {
                throw new InvalidFormatException("Missing the build model!");
            }

            if (ParserType == ParserType.Chunking) {
                if (AttachModel != null)
                    throw new InvalidFormatException("attachModel must be null for chunking parser!");
            } else if (ParserType == ParserType.TreeInsert) {
                if (AttachModel == null)
                    throw new InvalidFormatException("attachModel must not be null!");
            } else {
                throw new InvalidFormatException("Unknown parser type.");
            }

            if (CheckModel == null)
                throw new InvalidFormatException("Missing the check model!");

            if (ParserChunkerModel == null)
                throw new InvalidFormatException("Missing the chunker model!");

            if (ParserTaggerModel == null)
                throw new InvalidFormatException("Missing the tagger model!");

            if (HeadRules == null)
                throw new InvalidFormatException("Missing the head rules!");

        }
        #endregion

    }
}