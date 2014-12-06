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
using SharpNL.Text;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Represents a abstract analyzer.
    /// </summary>
    public abstract class AbstractAnalyzer : IAnalyzer {

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAnalyzer"/> with the specified weight.
        /// </summary>
        /// <param name="weight">The analyzer weight.</param>
        protected AbstractAnalyzer(float weight) {
            Weight = weight;
        }

        #region + Analyze .
        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// The <see cref="DefaultTextFactory"/> will be used to create the objects in the <see cref="IDocument"/>.
        /// </summary>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="document"/>
        /// </exception>
        public void Analyze(IDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            Evaluate(DefaultTextFactory.Instance, document);
        }

        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// </summary>
        /// <param name="factory">The text factory. if this argument is <c>null</c> the <see cref="DefaultTextFactory" /> will be used during the analysis.</param>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="document"/>
        /// </exception>
        public void Analyze(ITextFactory factory, IDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            Evaluate(factory ?? DefaultTextFactory.Instance, document);
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected abstract void Evaluate(ITextFactory factory, IDocument document);
        #endregion

        #region . Weight .
        /// <summary>
        /// Property used to control the influence of a analyzer during the execution in the <see cref="AggregateAnalyzer"/>.
        /// The lower values will be executed first.
        /// </summary>
        /// <value>Returns a floating point value indicating the relative weight a analyzer.</value>
        public float Weight { get; private set; }
        #endregion

    }
}