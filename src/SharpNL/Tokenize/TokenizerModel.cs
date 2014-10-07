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
using SharpNL.Utility.Model;

namespace SharpNL.Tokenize {
    /// <summary>
    /// The <see cref="TokenizerModel"/> is the model used by a learnable <see cref="ITokenizer"/>.
    /// </summary>
    public class TokenizerModel : BaseModel {
        private const string ComponentName = "TokenizerME";
        private const string TokenizerModelEntry = "token.model";

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerModel"/> class.
        /// </summary>
        /// <param name="tokenizerModel">The tokenizer model.</param>
        /// <param name="manifestInfoEntries">The manifest information entries.</param>
        /// <param name="tokenizerFactory">The tokenizer factory.</param>
        public TokenizerModel(IMaxentModel tokenizerModel, Dictionary<string, string> manifestInfoEntries, TokenizerFactory tokenizerFactory)
            : base(ComponentName, tokenizerFactory.LanguageCode, manifestInfoEntries, tokenizerFactory) {
            
            artifactMap.Add(TokenizerModelEntry, tokenizerModel);
            CheckArtifactMap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="stream">The input stream containing the model.</param>
        public TokenizerModel(Stream stream) : base(ComponentName, stream) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModel"/> class.
        /// </summary>
        /// <param name="fileName">The model filename.</param>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        public TokenizerModel(string fileName) : base(ComponentName, fileName) {
            
        }

        #endregion

        #region + Properties .

        #region . Abbreviations .
        /// <summary>
        /// Gets the abbreviation dictionary.
        /// </summary>
        /// <value>The abbreviation dictionary.</value>
        public Dictionary.Dictionary Abbreviations {
            get {
                if (Factory != null) {
                    return Factory.AbbreviationDictionary;
                }
                return null;
            }
        }
        #endregion

        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type DefaultFactory {
            get { return typeof(TokenizerFactory); }
        }

        #region . Factory .
        /// <summary>
        /// Gets the tokenizer factory.
        /// </summary>
        /// <value>The tokenizer factory.</value>
        public TokenizerFactory Factory {
            get { return (TokenizerFactory) ToolFactory; }
        }
        #endregion

        #region . MaxentModel .
        /// <summary>
        /// Gets the maxent model.
        /// </summary>
        /// <value>The maxent model.</value>
        public IMaxentModel MaxentModel {
            get { return (IMaxentModel) artifactMap[TokenizerModelEntry]; }
        }
        #endregion

        #region . UseAlphaNumericOptimization .
        /// <summary>
        /// Gets a value indicating whether to use alpha numeric optimization.
        /// </summary>
        /// <value><c>true</c> if alpha numeric optimization will be used; otherwise, <c>false</c>.</value>
        public bool UseAlphaNumericOptimization {
            get {
                if (Factory != null) {
                    return Factory.UseAlphaNumericOptimization;
                }
                return false;
            }
        }
        #endregion

        #endregion

    }
}