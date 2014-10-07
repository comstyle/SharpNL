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
using System.Text.RegularExpressions;
using SharpNL.Text;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Analyzer {
    /// <summary>
    /// The <see cref="Tokenizer"/> class separates every word in a given sentence and 
    /// allocates them in a list of tokens.
    /// </summary>
    public class Tokenizer : IAnalyzer {

        private static readonly Regex OpenQuotation;
        private static readonly Regex CloseQuotation;

        static Tokenizer() {
            OpenQuotation = new Regex("[«“]", RegexOptions.Compiled);
            CloseQuotation = new Regex("[»”]", RegexOptions.Compiled);
        }


        private readonly ITokenizer tokenizer;
        private readonly bool normalize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class with the sentence normalization enabled.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        public Tokenizer(ITokenizer tokenizer) : this(tokenizer, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="normalize">if set to <c>true</c> the sentences in the document will be normalized before being tokenized.</param>
        /// <exception cref="System.ArgumentNullException">tokenizer</exception>
        public Tokenizer(ITokenizer tokenizer, bool normalize) {
            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            this.tokenizer = tokenizer;
            this.normalize = normalize;
        }


        /// <summary>
        /// Analyzes the specified document.
        /// </summary>
        /// <param name="document">
        /// The the whole text given by the user. 
        /// After an analysis it can store the text's sentences, words or its tags.
        /// </param>
        public void Analyze(Document document) {
            var sentences = document.Sentences;

            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            foreach (var sentence in sentences) {
                var text = normalize ? Normalize(sentence.Text) : sentence.Text;

                Span[] spans;
                lock (tokenizer) {
                    spans = tokenizer.TokenizePos(text);
                }

                var tokens = new List<Token>(spans.Length);
                for (var i = 0; i < spans.Length; i++) {
                    tokens.Add(new Token(spans[i].Start, spans[i].End, spans[i].GetCoveredText(text)));
                }
                sentence.Tokens = new ReadOnlyCollection<Token>(tokens);
            }


        }

        private static string Normalize(string sentence) {
            return CloseQuotation.Replace(
                    OpenQuotation.Replace(sentence, "\""), "\"");
        }
    }
}