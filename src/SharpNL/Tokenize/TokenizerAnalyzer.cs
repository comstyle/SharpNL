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
using System.ComponentModel;
using System.Text.RegularExpressions;
using SharpNL.Analyzer;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// Represents a tokenizer analyzer which allows the easy abstraction of the tokenizer. This class is thread-safe.
    /// </summary>
    public class TokenizerAnalyzer : AbstractAnalyzer {

        private static Regex ptQuotes;

        /// <summary>
        /// Gets the tokenizer.
        /// </summary>
        /// <value>The tokenizer.</value>
        protected ITokenizer Tokenizer { get; private set; }

        #region + Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerAnalyzer"/> using the default analyzer weight;
        /// </summary>
        /// <param name="tokenizer">The tokenizer used by this analyzer.</param>
        public TokenizerAnalyzer(ITokenizer tokenizer) : this(tokenizer, 1f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAnalyzer" /> using the specified tokenizer and the analyzer weight.
        /// </summary>
        /// <param name="tokenizer">The tokenizer used by this analyzer.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">tokenizer</exception>
        public TokenizerAnalyzer(ITokenizer tokenizer, float weight)
            : base(weight) {
            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            Tokenizer = tokenizer;
            PreProcess = true;
        }

        #endregion

        #region . PreProcess .
        /// <summary>
        /// Gets or sets a value indicating whether the sentences will be preprocessed. The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the sentences will be preprocessed; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool PreProcess { get; set; }
        #endregion

        #region . PreProcessor .
        /// <summary>
        /// Gets or sets the sentence preprocess function which is called during de analysis 
        /// for each sentence in the document. If the result of this function is a <c>null</c> value
        /// the sentence will be ignored by the analyzer.
        /// </summary>
        /// <value>The sentence preprocess function.</value>
        public Func<string, string> PreProcessor { get; set; }
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
                var text = sentence.Text;

                if (string.IsNullOrWhiteSpace(text) || (PreProcess && (text = PreProcessSentence(document.Language, text)) == null)) {
                    sentence.Tokens = new ReadOnlyCollection<IToken>(new IToken[] { });
                    continue;
                }

                Span[] spans;
                lock (Tokenizer) {
                    spans = Tokenizer.TokenizePos(text);
                }

                if (spans == null || spans.Length == 0) {
                    sentence.Tokens = new ReadOnlyCollection<IToken>(new IToken[] {});
                } else {
                    var list = new List<IToken>(spans.Length);

                    foreach (var span in spans) {
                        var token = factory.CreateToken(
                            span.Start, 
                            span.End, 
                            span.GetCoveredText(text),
                            span.Probability);
                        
                        if (token != null)
                            list.Add(token);
                    }
                    sentence.Tokens = new ReadOnlyCollection<IToken>(list);
                }
            }
        }

        #endregion

        #region . PreProcessSentence .
        /// <summary>
        /// Preprocesses the sentence before it is analyzed by the tokenizer.
        /// </summary>
        /// <param name="language">The language of the sentence.</param>
        /// <param name="sentence">The sentence string.</param>
        /// <returns>The processed sentence. If this value is <c>null</c> the sentence will be ignored.</returns>
        protected virtual string PreProcessSentence(string language, string sentence) {
            // preprocess the portuguese quotations.
            if (language != null && language.StartsWith("pt", StringComparison.OrdinalIgnoreCase)) {
                // ptQuotes will be initialized only when needed.
                return (ptQuotes ?? (ptQuotes = new Regex("[«“»”]", RegexOptions.Compiled))).Replace(sentence, "\"");
            }

            return PreProcessor != null 
                ? PreProcessor(sentence)
                : sentence;
        }
        #endregion

    }
}