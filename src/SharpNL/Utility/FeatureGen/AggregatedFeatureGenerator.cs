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
using System.Collections.ObjectModel;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// The <see cref="AggregatedFeatureGenerator"/> aggregates a set of <see cref="IAdaptiveFeatureGenerator"/>s
    /// and calls them to generate the features.
    /// </summary>
    /// <seealso cref="IAdaptiveFeatureGenerator"/>
    internal class AggregatedFeatureGenerator : IAdaptiveFeatureGenerator {


        private readonly ReadOnlyCollection <IAdaptiveFeatureGenerator> featureGenerators;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedFeatureGenerator"/> class.
        /// </summary>
        /// <param name="featureGenerators">The feature generators.</param>
        public AggregatedFeatureGenerator(IEnumerable<IAdaptiveFeatureGenerator> featureGenerators)
            : this(featureGenerators.ToArray()) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedFeatureGenerator"/> class.
        /// </summary>
        /// <param name="featureGenerators">The feature generators.</param>
        /// <exception cref="System.ArgumentException">null values in generators are not permitted!</exception>
        public AggregatedFeatureGenerator(params IAdaptiveFeatureGenerator[] featureGenerators) {
            foreach (var generator in featureGenerators) {
                if (generator == null)
                    throw new ArgumentException("null values in generators are not permitted!");
            }

            this.featureGenerators = new ReadOnlyCollection<IAdaptiveFeatureGenerator>(featureGenerators);

        }
        #endregion

        #region . Generators .

        /// <summary>
        /// Gets a readonly collection of all aggregated feature generators.
        /// </summary>
        /// <value>All aggregated generators.</value>
        public ReadOnlyCollection<IAdaptiveFeatureGenerator> Generators {
            get { return featureGenerators; }
        }
        #endregion

        #region . ClearAdaptiveData .
        /// <summary>
        /// Informs the aggregated feature generators that the context of the adaptive data (typically a document)
        /// is no longer valid.
        /// </summary>
        public void ClearAdaptiveData() {
            foreach (var generator in featureGenerators) {
                generator.ClearAdaptiveData();
            }
        }
        #endregion

        #region . CreateFeatures .
        /// <summary>
        /// Adds the appropriate features for the token at the specified index with the
        /// specified array of previous outcomes to the specified list of features for all
        /// aggregated feature generators.
        /// </summary>
        /// <param name="features">The list of features to be added to.</param>
        /// <param name="tokens">The tokens of the sentence or other text unit being processed.</param>
        /// <param name="index">The index of the token which is currently being processed.</param>
        /// <param name="previousOutcomes">The outcomes for the tokens prior to the specified index.</param>
        public void CreateFeatures(List<string> features, string[] tokens, int index, string[] previousOutcomes) {
            foreach (var generator in featureGenerators) {
                generator.CreateFeatures(features, tokens, index, previousOutcomes);
            }
        }
        #endregion

        #region . UpdateAdaptiveData .
        /// <summary>
        /// Informs the aggregated feature generators that the specified tokens have been classified with the
        /// corresponding set of specified outcomes.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence or other text unit which has been processed.</param>
        /// <param name="outcomes">The outcomes associated with the specified tokens.</param>
        public void UpdateAdaptiveData(string[] tokens, string[] outcomes) {
            foreach (var generator in featureGenerators) {
                generator.UpdateAdaptiveData(tokens, outcomes);
            }
        }
        #endregion



    }
}