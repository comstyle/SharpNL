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

using System.Collections.Generic;
using SharpNL.Java;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Doccat {
    /// <summary>
    /// Generates a feature for each word in a document.
    /// </summary>
    [JavaClass("opennlp.tools.doccat.BagOfWordsFeatureGenerator")]
    public class BagOfWordsFeatureGenerator : IFeatureGenerator {

        private readonly bool useOnlyAllLetterTokens;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="BagOfWordsFeatureGenerator"/> using only all letter tokens.
        /// </summary>
        public BagOfWordsFeatureGenerator() : this(true) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BagOfWordsFeatureGenerator"/> class.
        /// </summary>
        /// <param name="useOnlyAllLetterTokens">if set to <c>true</c> only all letter tokens will be added to the feature list.</param>
        public BagOfWordsFeatureGenerator(bool useOnlyAllLetterTokens) {
            this.useOnlyAllLetterTokens = useOnlyAllLetterTokens;
        }
        #endregion

        #region + ExtractFeatures .
        /// <summary>
        /// Extracts the features from the given words.
        /// </summary>
        /// <param name="text">The words array.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>The list of features.</returns>
        public List<string> ExtractFeatures(string[] text, Dictionary<string, object> extraInformation) {
            var bag = new List<string>(text.Length);

            foreach (var word in text) {
                if (useOnlyAllLetterTokens) {
                    if (StringPattern.Recognize(word).AllLetter)
                        bag.Add("bow=" + word);
                } else {
                    bag.Add("bow=" + word);
                }
            }

            return bag;
        }
        #endregion

    }
}