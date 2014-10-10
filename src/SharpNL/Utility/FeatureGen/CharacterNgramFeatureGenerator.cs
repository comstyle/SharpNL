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
using SharpNL.NGram;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// The <see cref="CharacterNgramFeatureGenerator"/> uses character ngrams to generate features about each token.
    /// The minimum and maximum length can be specified.
    /// </summary>
    [TypeClass("opennlp.tools.util.featuregen.CharacterNgramFeatureGenerator")]
    public class CharacterNgramFeatureGenerator : FeatureGeneratorAdapter {
        private readonly int max;
        private readonly int min;


        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterNgramFeatureGenerator"/> 
        /// with min 2 length and max 5 length of ngrams.
        /// </summary>
        public CharacterNgramFeatureGenerator() : this(2, 5) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterNgramFeatureGenerator"/> 
        /// with the specified minimum and maximum length values.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        public CharacterNgramFeatureGenerator(int minLength, int maxLength) {
            min = minLength;
            max = maxLength;
        }


        /// <summary>
        /// Adds the appropriate features for the token at the specified index with the
        /// specified array of previous outcomes to the specified list of features.
        /// </summary>
        /// <param name="features">The list of features to be added to.</param>
        /// <param name="tokens">The tokens of the sentence or other text unit being processed.</param>
        /// <param name="index">The index of the token which is currently being processed.</param>
        /// <param name="previousOutcomes">The outcomes for the tokens prior to the specified index.</param>
        public override void CreateFeatures(List<string> features, string[] tokens, int index, string[] previousOutcomes) {
            var model = new NGramModel {{tokens[index], min, max}};
            foreach (var token in model) {
                if (token.Count > 0)
                    features.Add("ng=" + token[0].ToLowerInvariant());
            }
        }
    }
}