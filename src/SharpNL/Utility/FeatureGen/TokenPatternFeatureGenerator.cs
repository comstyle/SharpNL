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
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpNL.Tokenize;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Partitions tokens into sub-tokens based on character classes and generates
    /// class features for each of the sub-tokens and combinations of those sub-tokens.
    /// </summary>
    public class TokenPatternFeatureGenerator : FeatureGeneratorAdapter {

        private static readonly Regex noLetters;
        private readonly ITokenizer tokenizer;

        static TokenPatternFeatureGenerator() {
            noLetters = new Regex("[^a-zA-Z]", RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenPatternFeatureGenerator"/> using
        /// the <see cref="SimpleTokenizer"/> as the default tokenizer.
        /// </summary>
        public TokenPatternFeatureGenerator() : this(SimpleTokenizer.Instance) {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenPatternFeatureGenerator"/> using a
        /// specified <see cref="ITokenizer"/> object.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <exception cref="System.ArgumentNullException">tokenizer</exception>
        public TokenPatternFeatureGenerator(ITokenizer tokenizer) {
            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            this.tokenizer = tokenizer;
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
            var tokenized = tokenizer.Tokenize(tokens[index]);

            if (tokenized.Length == 1) {
                features.Add("st=" + tokens[index].ToLowerInvariant());
                return;
            }

            features.Add("stn=" + tokenized.Length);

            var sb = new StringBuilder();

            for (int i = 0; i < tokenized.Length; i++) {

                if (i < tokenized.Length - 1) {
                    features.Add("pt2=" + FeatureGeneratorUtil.TokenFeature(tokenized[i]) +
                        FeatureGeneratorUtil.TokenFeature(tokenized[i + 1]));
                }

                if (i < tokenized.Length - 2) {
                    features.Add("pt3=" + 
                        FeatureGeneratorUtil.TokenFeature(tokenized[i]) +
                        FeatureGeneratorUtil.TokenFeature(tokenized[i + 1]) +
                        FeatureGeneratorUtil.TokenFeature(tokenized[i + 2]));
                }

                sb.Append(FeatureGeneratorUtil.TokenFeature(tokenized[i]));

                if (!noLetters.IsMatch(tokenized[i])) {
                    features.Add("st=" + tokenized[i].ToLowerInvariant());
                }
            }
            features.Add("pta=" + sb);
        }
    }
}