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
using SharpNL.NameFind;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Analyzer {
    /// <summary>
    /// The <see cref="NameFinder"/> class searches for subsequent proper nouns in the
    /// document sentences and gathers each of these sets in one word.
    /// </summary>
    public class NameFinder : IAnalyzer {

        private readonly ITokenNameFinder nameFinder;

        public NameFinder(ITokenNameFinder nameFinder) {
            if (nameFinder == null) {
                throw new ArgumentNullException("nameFinder");
            }

            this.nameFinder = nameFinder;
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

                Span[] spans;
                lock (nameFinder) {
                    spans = nameFinder.Find(TextUtils.TokensToString(sentence.Tokens));
                }

                var tokens = new List<Token>(sentence.Tokens);

                foreach (var span in spans) {
                    var start = span.Start;
                    var end = span.End;

                    var chStart = tokens[start].Start;
                    var chEnd = tokens[end - 1].End;

                    var name = sentence.Substring(chStart, chEnd - chStart).Replace(" ", "_");
                    tokens.RemoveAt(end - 1);

                    for (int j = end - 2; j >= start; j--) {
                        tokens.RemoveAt(j);
                    }
                    var token = new Token(chStart, chEnd, name);
                    
                    token.SetAdditionalContext(GetType(), "P");

                    tokens.Insert(start, token);
                }

                sentence.Tokens = tokens.AsReadOnly();;
            }

        }
    }
}