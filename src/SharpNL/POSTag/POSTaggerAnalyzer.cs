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
using System.Linq;
using SharpNL.Analyzer;
using SharpNL.Text;

namespace SharpNL.POSTag {
    /// <summary>
    /// Represents a part-of-speech analyzer which allows the easy abstraction of 
    /// the part-of-speech tagging. This class is thread-safe.
    /// </summary>
    public class POSTaggerAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Delegate that provides the additional context for the sentences to tag the part-of-speech tags.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="sentence">The sentence.</param>
        /// <returns>The return value must be a <c>string[][]</c> array or a <c>null</c> value.</returns>
        public delegate object[] dAddContextProvider(IDocument document, ISentence sentence);

        /// <summary>
        /// Gets the part-of-speech tagger.
        /// </summary>
        /// <value>The part-of-speech tagger.</value>
        protected POSTaggerME Tagger { get; private set; }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="POSTaggerAnalyzer"/> using the default analyzer weight.
        /// </summary>
        /// <param name="tagger">The part-of-speech tagger used by this analyzer.</param>
        public POSTaggerAnalyzer(POSTaggerME tagger) : this(tagger, 4f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAnalyzer" /> using a custom analyzer weight.
        /// </summary>
        /// <param name="tagger">The part-of-speech tagger used by this analyzer.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">tagger</exception>
        public POSTaggerAnalyzer(POSTaggerME tagger, float weight)
            : base(weight) {
            if (tagger == null)
                throw new ArgumentNullException("tagger");

            Tagger = tagger;
        }
        #endregion

        #region . AddContextProvider .
        /// <summary>
        /// Gets or sets the additional context provider.
        /// </summary>
        /// <value>The additional context provider.</value>
        public dAddContextProvider AddContextProvider { get; set; }
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
                if (sentence.Tokens == null)
                    throw new AnalyzerException(this, "The document have a sentence without the tokenization.");

                string[] tags;
                double[] probs;
                object[]  ac = AddContextProvider != null
                    ? AddContextProvider(document, sentence)
                    : null;

                lock (Tagger) {
                    tags = Tagger.Tag(sentence.Tokens.ToTokenArray(), ac);
                    probs = Tagger.Probabilities;
                }

                var prob = probs.Sum(p => p);
                if (probs.Length > 0)
                    prob /= probs.Length;

                for (var i = 0; i < tags.Length; i++) {
                    sentence.Tokens[i].POSTag = tags[i];
                    sentence.Tokens[i].POSTagProbability = probs[i];
                }

                // TODO: Add the ability to pre/post process each sentence during the analysis.

                sentence.TagProbability = prob;
            }
        }
        #endregion

    }
}