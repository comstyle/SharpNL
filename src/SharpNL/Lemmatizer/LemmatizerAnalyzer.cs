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
using SharpNL.Analyzer;
using SharpNL.Text;

namespace SharpNL.Lemmatizer {
    /// <summary>
    /// Represents a lemmatizer analyzer. This class cannot be inherited.
    /// </summary>
    public sealed class LemmatizerAnalyzer : IAnalyzer {

        private readonly ILemmatizer lemmatizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="LemmatizerAnalyzer"/> class.
        /// </summary>
        /// <param name="lemmatizer">The lemmatizer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="lemmatizer"/>
        /// </exception>
        public LemmatizerAnalyzer(ILemmatizer lemmatizer) {
            if (lemmatizer == null)
                throw new ArgumentNullException("lemmatizer");

            this.lemmatizer = lemmatizer;
        }

        #region . Analyze .
        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// </summary>
        /// <param name="factory">The text factory. if this argument is <c>null</c> the <see cref="DefaultTextFactory"/> must 
        /// be used during the analysis.</param>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        public void Analyze(ITextFactory factory, IDocument document) {
            if (document == null)
                throw new ArgumentNullException("document");

            if (document.Sentences == null || document.Sentences.Count == 0)
                throw new InvalidOperationException("The document does not have any sentences detected.");

            if (document.Sentences[0].Tokens == null || document.Sentences[0].Tokens.Count == 0)
                throw new InvalidOperationException("The document is not tokenized.");

            foreach (var sentence in document.Sentences)
                foreach (var token in sentence.Tokens)
                    token.Lemmas = lemmatizer.Lemmatize(token.Lexeme, token.POSTag);

        }
        #endregion

        #region . Weight .
        /// <summary>
        /// Property used to control the influence of a analyzer during the execution in the <see cref="AggregateAnalyzer"/>.
        /// The lower values will be executed first.
        /// </summary>
        /// <value>Returns a floating point value indicating the relative weight a task.</value>
        /// <remarks>
        /// The standard weights are:
        /// <list type="table">
        ///  <listheader>
        ///   <term>Weight</term><description>Analyzer</description>
        ///  </listheader>
        ///  <item><term>0.0</term><description>Sentence detection.</description></item>
        ///  <item><term>1.0</term><description>Tokenization.</description></item>
        ///  <item><term>2.0</term><description>Document categorizer.</description></item>
        ///  <item><term>3.0</term><description>Entity recognition.</description></item>
        ///  <item><term>4.0</term><description>Part-Of-Speech tagging.</description></item>
        ///  <item><term>5.0</term><description>Chunking</description></item>
        ///  <item><term>6.0</term><description>Parsing</description></item>
        /// </list>
        /// </remarks>
        public float Weight {
            get { return 4.1f; }
        }
        #endregion

    }
}