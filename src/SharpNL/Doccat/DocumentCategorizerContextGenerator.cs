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

namespace SharpNL.Doccat {
    /// <summary>
    /// Represents the document categorizer context generator.
    /// </summary>
    public class DocumentCategorizerContextGenerator {

        private readonly IFeatureGenerator[] featureGenerators;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerContextGenerator"/> with the given generators.
        /// </summary>
        /// <param name="featureGenerators">The feature generators.</param>
        /// <exception cref="System.ArgumentNullException">featureGenerators</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The feature generators must not be empty.</exception>
        public DocumentCategorizerContextGenerator(params IFeatureGenerator[] featureGenerators) {
            if (featureGenerators == null)
                throw new ArgumentNullException("featureGenerators");

            if (featureGenerators.Length == 0)
                throw new ArgumentOutOfRangeException("featureGenerators", 0, @"The feature generators must not be empty.");

            this.featureGenerators = featureGenerators;
        }
        #endregion

        #region . GetContext .
        /// <summary>
        /// Gets the context for the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>The contexts for the specified text.</returns>
        public string[] GetContext(string[] text, Dictionary<string, object> extraInformation) {
            var context = new List<string>();

            foreach (var fg in featureGenerators) {
                context.AddRange(fg.ExtractFeatures(text, extraInformation));
            }


            return context.ToArray();
        }
        #endregion

    }
}