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
using SharpNL.Utility.FeatureGen;

namespace SharpNL.NameFind {
    /// <summary>
    /// Class for determining contextual features for a tag/chunk style named-entity recognizer.
    /// </summary>
    public class DefaultNameContextGenerator : INameContextGenerator {

        private readonly List<IAdaptiveFeatureGenerator> featureGenerators;

        private DefaultNameContextGenerator() {
            featureGenerators = new List<IAdaptiveFeatureGenerator>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNameContextGenerator"/> class.
        /// </summary>
        /// <param name="featureGenerators">The feature generators.</param>
        public DefaultNameContextGenerator(params IAdaptiveFeatureGenerator[] featureGenerators) : this() {
            if (featureGenerators.Length > 0)
                this.featureGenerators.AddRange(featureGenerators);
        }

        #region . AddFeatureGenerator .
        /// <summary>
        /// Adds a feature generator to this set of feature generators.
        /// </summary>
        /// <param name="featureGenerator">The feature generator to add.</param>
        public void AddFeatureGenerator(IAdaptiveFeatureGenerator featureGenerator) {
            if (featureGenerator == null)
                throw new ArgumentNullException("featureGenerator");

            featureGenerators.Add(featureGenerator);
        }
        #endregion

        #region . ClearAdaptiveData .
        /// <summary>
        /// Informs all the feature generators for a name finder that the context of the adaptive data (typically a document) is no longer valid.
        /// </summary>
        public void ClearAdaptiveData() {
            foreach (var featureGenerator in featureGenerators) {
                featureGenerator.ClearAdaptiveData();
            }
        }
        #endregion

        #region . GetContext .
        /// <summary>Gets the context for finding names at the specified index.</summary>
        /// <param name="index">The index of the token in the specified toks array for which the context should be constructed.</param>
        /// <param name="tokens">The sequence of items over which the beam search is performed.</param>
        /// <param name="preds">The previous decisions made in the tagging of this sequence. Only indices less than i will be examined.</param>
        /// <param name="additionalContext">Addition features which may be based on a context outside of the sentence.</param>
        /// <returns>The context for finding names at the specified index.</returns>
        public string[] GetContext(int index, string[] tokens, string[] preds, object[] additionalContext) {

            var features = new List<string>();
            foreach (var featureGenerator in featureGenerators) {
                featureGenerator.CreateFeatures(features, tokens, index, preds);
            }

            //previous outcome features
            var po = NameFinderME.Other;
            var ppo = NameFinderME.Other;

            // TODO: These should be moved out here in its own feature generator!
            if (preds != null) {
                if (index > 1) {
                    ppo = preds[index - 2];
                }

                if (index > 0) {
                    po = preds[index - 1];
                }
                features.Add("po=" + po);
                features.Add("pow=" + po + "," + tokens[index]);
                features.Add("powf=" + po + "," + FeatureGeneratorUtil.TokenFeature(tokens[index]));
                features.Add("ppo=" + ppo);
            }

            return features.ToArray();
        }
        #endregion

        #region . UpdateAdaptiveData .
        /// <summary>
        /// Informs all the feature generators for a name finder that the specified tokens have been classified with the coorisponds set of specified outcomes.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence or other text unit which has been processed.</param>
        /// <param name="outcomes">The outcomes associated with the specified tokens.</param>
        public void UpdateAdaptiveData(string[] tokens, string[] outcomes) {
            if (tokens != null && outcomes != null && tokens.Length != outcomes.Length) {
                throw new ArgumentException("The tokens and outcome arrays MUST have the same size!");
            }
            foreach (var featureGenerator in featureGenerators) {
                featureGenerator.UpdateAdaptiveData(tokens, outcomes);
            }
        }
        #endregion

    }
}