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

using SharpNL.Text;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Defines a method to analyze a <see cref="IDocument"/> object 
    /// which can be several sentences, a sentence or even a single word.
    /// </summary>
    public interface IAnalyzer {

        #region . Analyze .
        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// </summary>
        /// <param name="factory">The text factory. if this argument is <c>null</c> the <see cref="DefaultTextFactory"/> must 
        /// be used during the analysis.</param>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        void Analyze(ITextFactory factory, IDocument document);
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
        float Weight { get; }
        
        #endregion

    }
}