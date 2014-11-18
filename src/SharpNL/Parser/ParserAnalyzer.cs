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

namespace SharpNL.Parser {
    /// <summary>
    /// Represents a parser analyzer which allows the easy abstraction of the parsing operation. 
    /// This class is thread-safe.
    /// </summary>
    public class ParserAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <value>The parser.</value>
        protected IParser Parser { get; private set; }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserAnalyzer"/> using the default weight. 
        /// </summary>
        /// <param name="parser">The parser.</param>
        public ParserAnalyzer(IParser parser) : this(parser, 6f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserAnalyzer" /> with the specified weight.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">parser</exception>
        public ParserAnalyzer(IParser parser, float weight)
            : base(weight) {
            if (parser == null)
                throw new ArgumentNullException("parser");

            Parser = parser;
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected override void Evaluate(ITextFactory factory, IDocument document) {
            if (document.Sentences == null)
                throw new AnalyzerException(this, "The document does not have the sentences detected.");

            foreach (var sentence in document.Sentences) {
                var p = ParserTool.ParseLine(sentence, Parser, 1);
                if (p != null && p.Length > 0)
                    sentence.Parse = p[0];
            }
        }
        #endregion

    }
}