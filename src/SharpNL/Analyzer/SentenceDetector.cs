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
using SharpNL.Sentence;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Represents a sentence analyzer.
    /// </summary>
    public class SentenceDetector : IAnalyzer {
        private readonly ISentenceDetector sentenceDetector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceDetector"/> with the specified <see cref="ISentenceDetector"/>.
        /// </summary>
        /// <param name="sentenceDetector">The sentence detector.</param>
        /// <exception cref="System.ArgumentNullException">sentenceDetector</exception>
        public SentenceDetector(ISentenceDetector sentenceDetector) {
            if (sentenceDetector == null)
                throw new ArgumentNullException("sentenceDetector");

            this.sentenceDetector = sentenceDetector;
        }

        /// <summary>
        /// Analyzes the sentences in the specified document.
        /// </summary>
        /// <param name="document">The document to be analyzed.</param>
        public void Analyze(Document document) {

            Span[] spans;
            lock (sentenceDetector) {
                spans = sentenceDetector.SentPosDetect(document.Text);
            }

            var sentences = new List<Text.Sentence>(spans.Length);
            foreach (var span in spans) {
                sentences.Add(new Text.Sentence(span.Start, span.End, document));
            }
            document.Sentences = new ReadOnlyCollection<Text.Sentence>(sentences);

        }
    }
}