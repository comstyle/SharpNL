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
using SharpNL.Analyzer;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Represents a sentence analyzer which allows the easy abstraction of the 
    /// sentence detection. This class is thread-safe.
    /// </summary>
    public class SentenceDetectorAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Gets the sentence detector.
        /// </summary>
        /// <value>The sentence detector.</value>
        protected ISentenceDetector SentenceDetector { get; private set; }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceDetectorAnalyzer" /> using the default weight value.
        /// </summary>
        /// <param name="sentenceDetector">The sentence detector used to analyze the sentences.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentenceDetector"/>
        /// </exception>
        public SentenceDetectorAnalyzer(ISentenceDetector sentenceDetector) : this(sentenceDetector, 0f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAnalyzer" /> with the specified weight.
        /// </summary>
        /// <param name="sentenceDetector">The sentence detector.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentenceDetector"/>
        /// </exception>
        public SentenceDetectorAnalyzer(ISentenceDetector sentenceDetector, float weight)
            : base(weight) {
            if (sentenceDetector == null)
                throw new ArgumentNullException("sentenceDetector");

            SentenceDetector = sentenceDetector;
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected override void Evaluate(ITextFactory factory, IDocument document) {

            if (string.IsNullOrEmpty(document.Text))
                throw new AnalyzerException(this, "The document text is null or empty.");

            Span[] spans;
            lock (SentenceDetector) {
                spans = SentenceDetector.SentPosDetect(document.Text);
            }

            var list = new List<ISentence>(spans.Length);
            foreach (var span in spans) {
                var sentence = factory.CreateSentence(span, document);

                if (sentence != null)
                    list.Add(sentence);
            }


            document.Sentences = new ReadOnlyCollection<ISentence>(list);
        }

        #endregion

    }
}