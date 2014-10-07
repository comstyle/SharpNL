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
using Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.POSTag {
    /// <summary>
    /// The factory that provides POS Tagger default implementations and resources.
    /// </summary>
    public class POSTaggerFactory : BaseToolFactory {
        private const string FactoryName = "POSTaggerFactory";

        internal const string TagDictionaryEntryName = "tags.tagdict";
        internal const string NgramDictionaryEntryName = "ngram.dictionary";

        protected Dic dictionary;
        protected ITagDictionary tagDictionary;

        protected void Init(Dic ngramDic, ITagDictionary posDic) {
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
        public Dic Dictionary {
            get {
                if (dictionary == null && ArtifactProvider != null) {
                    dictionary = ArtifactProvider.GetArtifact<Dic>(NgramDictionaryEntryName);
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
        internal AbstractModel Model {
            get {
                if (ArtifactProvider != null) {
                    return ArtifactProvider.GetArtifact<AbstractModel>(POSModel.EntryName);
                }
                return null;
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

            dictionary = ArtifactProvider.GetArtifact<Dic>(NgramDictionaryEntryName);
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