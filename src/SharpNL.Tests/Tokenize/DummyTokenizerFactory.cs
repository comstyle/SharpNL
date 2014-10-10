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
using SharpNL.Dictionary;
using SharpNL.Tokenize;
using SharpNL.Utility;
using SharpNL.Utility.Serialization;

using Dict = SharpNL.Dictionary.Dictionary;

namespace SharpNL.Tests.Tokenize {
    [TypeClass("this.is.aWeSoMe.DummyTokenizerFactory")]
    internal class DummyTokenizerFactory : TokenizerFactory {

        private const string DummyDict = "dict.dummy";
        private DummyDictionary dict;

        // required!
        public DummyTokenizerFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerFactory"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="abbreviationDictionary">The abbreviation dictionary.</param>
        /// <param name="useAlphaNumericOptimization">if true alpha numerics are skipped.</param>
        public DummyTokenizerFactory(string languageCode, SharpNL.Dictionary.Dictionary abbreviationDictionary,
            bool useAlphaNumericOptimization)
            : base(
                languageCode, abbreviationDictionary != null ? new DummyDictionary(abbreviationDictionary) : null,
                useAlphaNumericOptimization) {

            dict = base.AbbreviationDictionary as DummyDictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerFactory"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="abbreviationDictionary">The abbreviation dictionary.</param>
        /// <param name="useAlphaNumericOptimization">if true alpha numerics are skipped.</param>
        /// <param name="alphaNumericPattern">The alpha numeric pattern.</param>
        public DummyTokenizerFactory(string languageCode, SharpNL.Dictionary.Dictionary abbreviationDictionary,
            bool useAlphaNumericOptimization, string alphaNumericPattern)
            : base(
                languageCode, abbreviationDictionary != null ? new DummyDictionary(abbreviationDictionary) : null,
                useAlphaNumericOptimization, alphaNumericPattern) {
            dict = base.AbbreviationDictionary as DummyDictionary;
        }

        /// <summary>
        /// Creates the artifact serializers for the <see cref="BaseToolFactory.ArtifactProvider"/>.
        /// The subclasses should call the <see cref="SharpNL.Utility.Serialization.ArtifactProvider.RegisterArtifactType"/>
        /// </summary>
        /// <param name="provider">The provider.</param>
        public override void CreateArtifactSerializers(ArtifactProvider provider) {
            provider.RegisterArtifactType(Path.GetExtension(DummyDict), (artifact, stream) => {
                var dictionary = artifact as DummyDictionary;
                if (dictionary == null)
                    throw new InvalidOperationException();

                dictionary.Serialize(stream);
            }, stream => new DummyDictionary(stream));
        }

        /// <summary>
        /// Creates a dictionary with pairs of keys and objects. 
        /// The models implementation should call this constructor that creates a model programmatically.
        /// </summary>
        public override Dictionary<string, object> CreateArtifactMap() {
            var map = base.CreateArtifactMap();

            if (dict != null)
                map.Add(DummyDict, dict);

            return map;
        }

        #region . AbbreviationDictionary .
        /// <summary>
        /// Gets the abbreviation dictionary.
        /// </summary>
        /// <value>The abbreviation dictionary.</value>
        public override Dict AbbreviationDictionary {
            get {
                if (dict == null && ArtifactProvider != null)
                    dict = ArtifactProvider.GetArtifact<DummyDictionary>(DummyDict);

                return dict;
            }
        }
        #endregion

        #region . ContextGenerator .
        /// <summary>
        /// Gets the context generator.
        /// </summary>
        /// <value>The context generator.</value>
        public override ITokenContextGenerator ContextGenerator {
            get { return new DummyContextGenerator(AbbreviationDictionary.ToList()); }
        }

        #endregion

        internal class DummyDictionary : Dict {
            public DummyDictionary(IEnumerable<Entry> dict) {
                foreach (var entry in dict) {
                    Add(entry);
                }
            }
            public DummyDictionary(Stream inputStream) : this(new Dict(inputStream)) {}
        }

        internal class DummyContextGenerator : DefaultTokenContextGenerator {
            public DummyContextGenerator(List<String> inducedAbbreviations) : base(inducedAbbreviations) {}
        }
    }
}