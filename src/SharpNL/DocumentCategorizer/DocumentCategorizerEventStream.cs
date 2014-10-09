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

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Enumerator-like class for modeling document classification events.
    /// </summary>
    public class DocumentCategorizerEventStream : AbstractEventStream<DocumentSample> {

        private readonly DocumentCategorizerContextGenerator cg;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerEventStream"/> class 
        /// using the <see cref="BagOfWordsFeatureGenerator"/> as context generator.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public DocumentCategorizerEventStream(IObjectStream<DocumentSample> samples) : this(samples, new BagOfWordsFeatureGenerator()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerEventStream"/> class with the given feature generators.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="featureGenerators">The feature generators.</param>
        /// <exception cref="System.ArgumentNullException">featureGenerators</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">featureGenerators</exception>
        public DocumentCategorizerEventStream(IObjectStream<DocumentSample> samples, params  IFeatureGenerator[] featureGenerators) : base(samples) {
            if (featureGenerators == null)
                throw new ArgumentNullException("featureGenerators");

            if (featureGenerators.Length == 0)
                throw new ArgumentOutOfRangeException("featureGenerators");

            cg = new DocumentCategorizerContextGenerator(featureGenerators);
        }

        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(DocumentSample sample) {
            return new List<Event> {
                new Event(sample.Category, cg.GetContext(sample.Text, sample.ExtraInformation))
            }.GetEnumerator();
        }
    }
}