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
using SharpNL.Text;

namespace SharpNL.Analyzer {
    /// <summary>
    /// The <see cref="T:Pipe"/> class contains a sequence of analyzers.
    /// <para>
    /// It follows the composite pattern to manage the analyzers. 
    /// Use the method <see cref="Add"/> to add analyzers into the pipe.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The <see cref="Analyze"/> method is thread-safe, if all analyzers in it
    /// are also thread-safe, which is the case of the default analyzers.
    /// </remarks>
    public class Pipe : IAnalyzer {
        private readonly List<IAnalyzer> childAnalyzers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipe"/> class.
        /// </summary>
        public Pipe() {
            childAnalyzers = new List<IAnalyzer>();
        }

        #region . Add .

        /// <summary>
        /// Adds the specified analyzer into the pipe.
        /// </summary>
        /// <param name="analyzer">The analyzer to be added in the pipe.</param>
        /// <exception cref="System.ArgumentNullException">analyzer</exception>
        public void Add(IAnalyzer analyzer) {
            if (analyzer == null)
                throw new ArgumentNullException("analyzer");

            childAnalyzers.Add(analyzer);
        }

        #endregion

        #region . Analyze .
        /// <summary>
        /// Analyzes the specified document.
        /// </summary>
        /// <param name="document">
        /// The the whole text given by the user. 
        /// After an analysis it can store the text's sentences, words or its tags.
        /// </param>
        public void Analyze(Document document) {
            lock (childAnalyzers) {
                foreach (var analyzer in childAnalyzers) {
                    try {
                        analyzer.Analyze(document);
                    } catch (Exception ex) {
                        throw new AnalyzerException("An analyzer raised an exception during the analysis.", ex);
                    }
                }
            }
        }
        #endregion

    }
}