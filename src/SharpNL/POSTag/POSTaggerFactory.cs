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
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.Serialization;

namespace SharpNL.POSTag {
    /// <summary>
    /// The factory that provides POS Tagger default implementations and resources.
    /// </summary>
    [TypeClass("opennlp.tools.postag.POSTaggerFactory")]
    public class POSTaggerFactory : BaseToolFactory {
        internal const string TagDictionaryEntryName = "tags.tagdict";
        internal const string NgramDictionaryEntryName = "ngram.dictionary";

        protected Dictionary.Dictionary dictionary;
        protected ITagDictionary tagDictionary;

        protected void Init(Dictionary.Dictionary ngramDic, ITagDictionary posDic) {
            dictionary = ngramDic;
            tagDictionary = posDic;
        }

        #region + Properties .

        #region . Dictionary .
        /// <summary>
        /// Gets or sets ngram the dictionary.
        /// </summary>
        /// <value>The ngram dictionary.</value>
        /// <exception cref="System.InvalidOperationException">Can not set ngram dictionary while using artifact provider.</exception>
        public Dictionary.Dictionary Dictionary {
            get {
                if (dictionary == null && ArtifactProvider != null) {
                    dictionary = ArtifactProvider.GetArtifact<Dictionary.Dictionary>(NgramDictionaryEntryName);
                }
                return dictionary;
            }
            set {
                if (ArtifactProvider != null) {
                    throw new InvalidOperationException("Can not set ngram dictionary while using artifact provider.");
                }
                dictionary = value;
            }
        }

        #endregion

        #region . TagDictionary .

        /// <summary>
        /// Gets or sets the tag dictionary.
        /// </summary>
        /// <value>The tag dictionary.</value>
        /// <exception cref="System.InvalidOperationException">Can not set tag dictionary while using artifact provider.</exception>
        public ITagDictionary TagDictionary {
            get {
                if (tagDictionary == null && ArtifactProvider != null) {
                    tagDictionary = ArtifactProvider.GetArtifact<ITagDictionary>(TagDictionaryEntryName);
                }
                return tagDictionary;
            }
            set {
                if (ArtifactProvider != null) {
                    throw new InvalidOperationException("Can not set tag dictionary while using artifact provider.");
                }
                tagDictionary = value;
            }
        }

        #endregion

        #region . Model .

        /// <summary>
        /// Gets the model from the artifact provider.
        /// </summary>
        /// <value>The model from the artifact provider..</value>
        private AbstractModel Model {
            get {
                return ArtifactProvider != null 
                    ? ArtifactProvider.GetArtifact<AbstractModel>(POSModel.EntryName) 
                    : null;
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

            if (tagDictionary != null)
                map.Add(TagDictionaryEntryName, tagDictionary);

            if (dictionary != null)
                map.Add(NgramDictionaryEntryName, dictionary);

            return map;
        }

        #endregion

        /// <summary>
        /// Creates the artifact serializers for the <see cref="BaseToolFactory.ArtifactProvider"/>.
        /// The subclasses should call the <see cref="SharpNL.Utility.Serialization.ArtifactProvider.RegisterArtifactType"/> method to register an new artifact type.
        /// </summary>
        /// <param name="provider">The artifact provider.</param>
        public override void CreateArtifactSerializers(ArtifactProvider provider) {
            base.CreateArtifactSerializers(provider);

            // TODO: the ITagDictionary is not extensive :(

            provider.RegisterArtifactType(".tagdict", 
                (artifact, stream) => ((POSDictionary) artifact).Serialize(stream), 
                stream => new POSDictionary(stream)
                );

        }

        #region . CreateEmptyTagDictionary .

        internal ITagDictionary CreateEmptyTagDictionary() {
            tagDictionary = new POSDictionary(true);
            return tagDictionary;
        }

        #endregion

        #region + GetPOSContextGenerator .

        public IPOSContextGenerator GetPOSContextGenerator() {
            return new DefaultPOSContextGenerator(0, Dictionary);
        }

        public IPOSContextGenerator GetPOSContextGenerator(int cacheSize) {
            return new DefaultPOSContextGenerator(cacheSize, Dictionary);
        }

        #endregion

        #region . GetSequenceValidator .

        public ISequenceValidator<String> GetSequenceValidator() {
            return new DefaultPOSSequenceValidator(TagDictionary);
        }

        #endregion

        #region . ValidateArtifactMap .

        /// <summary>
        /// Validates the parsed artifacts. 
        /// </summary>
        /// <exception cref="InvalidFormatException">Invalid artifact map.</exception>
        public override void ValidateArtifactMap() {
            // Ensure that the tag dictionary is compatible with the model

            var tagDicEntry = ArtifactProvider.GetArtifact<ITagDictionary>(TagDictionaryEntryName);

            if (tagDicEntry != null && !ArtifactProvider.IsLoadedFromSerialized) {
                ValidatePOSDictionary((POSDictionary) tagDicEntry, Model);
            }

            dictionary = ArtifactProvider.GetArtifact<Dictionary.Dictionary>(NgramDictionaryEntryName);
        }

        #endregion

        #region . ValidatePOSDictionary .

        protected void ValidatePOSDictionary(POSDictionary posDic, AbstractModel posModel) {
            if (!posModel.ContainsOutcomes(posDic)) {
                throw new InvalidFormatException("Tag dictionary contains tags which are unknown by the model!");
            }
        }

        #endregion

    }
}