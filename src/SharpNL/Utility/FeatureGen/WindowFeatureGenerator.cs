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

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Generates previous and next features for a given <see cref="IAdaptiveFeatureGenerator"/>
    /// The window size can be specified.
    /// </summary>
    /// <remarks>
    /// Features:
    /// Current token is always included unchanged
    /// Previous tokens are prefixed with p distance
    /// Next tokens are prefix with n distance
    /// </remarks>
    [TypeClass("opennlp.tools.util.featuregen.WindowFeatureGenerator")]
    public class WindowFeatureGenerator : IAdaptiveFeatureGenerator {
        public const string PREV_PREFIX = "p";
        public const string NEXT_PREFIX = "n";

        private readonly IAdaptiveFeatureGenerator generator;

        private readonly int nextWindowSize;
        private readonly int prevWindowSize;

        #region + Constructors .

        /// <summary>
        /// Initializes the current instance. The previous and next window size is 5.
        /// </summary>
        /// <param name="generator">The feature generator.</param>
        public WindowFeatureGenerator(IAdaptiveFeatureGenerator generator) : this(generator, 5, 5) {}

        /// <summary>
        /// Initializes the current instance with the given parameters.
        /// </summary>
        /// <param name="generators">The feature generators.</param>
        public WindowFeatureGenerator(params IAdaptiveFeatureGenerator[] generators)
            : this(new AggregatedFeatureGenerator(generators), 5, 5) {}

        /// <summary>
        /// Initializes the current instance with the given parameters.
        /// </summary>
        /// <param name="prevWindowSize">Size of the window to the left of the current token.</param>
        /// <param name="nextWindowSize">Size of the window to the right of the current token.</param>
        /// <param name="generators">The feature generators.</param>
        public WindowFeatureGenerator(int prevWindowSize, int nextWindowSize,
            params IAdaptiveFeatureGenerator[] generators)
            : this(new AggregatedFeatureGenerator(generators), prevWindowSize, nextWindowSize) {}

        /// <summary>
        /// Initializes the current instance with the given parameters.
        /// </summary>
        /// <param name="generator">Feature generator to apply to the window.</param>
        /// <param name="prevWindowSize">Size of the window to the left of the current token.</param>
        /// <param name="nextWindowSize">Size of the window to the right of the current token.</param>
        public WindowFeatureGenerator(IAdaptiveFeatureGenerator generator, int prevWindowSize, int nextWindowSize) {
            this.generator = generator;
            this.prevWindowSize = prevWindowSize;
            this.nextWindowSize = nextWindowSize;
        }

        #endregion

        #region . ClearAdaptiveData .

        /// <summary>
        /// Informs the feature generator that the context of the adaptive data (typically a document)
        /// is no longer valid.
        /// </summary>
        public void ClearAdaptiveData() {
            generator.ClearAdaptiveData();
        }

        #endregion

        #region . CreateFeatures .

        /// <summary>
        /// Adds the appropriate features for the token at the specified index with the
        /// specified array of previous outcomes to the specified list of features.
        /// </summary>
        /// <param name="features">The list of features to be added to.</param>
        /// <param name="tokens">The tokens of the sentence or other text unit being processed.</param>
        /// <param name="index">The index of the token which is currently being processed.</param>
        /// <param name="previousOutcomes">The outcomes for the tokens prior to the specified index.</param>
        public void CreateFeatures(List<string> features, string[] tokens, int index, string[] previousOutcomes) {
            generator.CreateFeatures(features, tokens, index, previousOutcomes);

            // previous features
            for (var i = 1; i < prevWindowSize + 1; i++) {
                if (index - i >= 0) {
                    var prevFeatures = new List<string>();

                    generator.CreateFeatures(prevFeatures, tokens, index - i, previousOutcomes);

                    foreach (var prevFeature in prevFeatures) {
                        features.Add(PREV_PREFIX + i + prevFeature);
                    }
                }
            }

            // next features
            for (var i = 1; i < nextWindowSize + 1; i++) {
                if (i + index < tokens.Length) {
                    var nextFeatures = new List<string>();

                    generator.CreateFeatures(nextFeatures, tokens, index + i, previousOutcomes);

                    foreach (var nextFeature in nextFeatures) {
                        features.Add(NEXT_PREFIX + i + nextFeature);
                    }
                }
            }
        }

        #endregion

        #region . UpdateAdaptiveData .

        /// <summary>
        /// Informs the feature generator that the specified tokens have been classified with the
        /// corresponding set of specified outcomes.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence or other text unit which has been processed.</param>
        /// <param name="outcomes">The outcomes associated with the specified tokens.</param>
        public void UpdateAdaptiveData(string[] tokens, string[] outcomes) {
            generator.UpdateAdaptiveData(tokens, outcomes);
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("{0}: Prev window size: {1}, Next window size: {2}", base.ToString(), prevWindowSize,
                nextWindowSize);
        }

        #endregion

    }
}