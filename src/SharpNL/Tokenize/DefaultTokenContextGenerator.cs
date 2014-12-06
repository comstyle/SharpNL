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

namespace SharpNL.Tokenize {
    /// <summary>
    /// Generate events for maxent decisions for tokenization.
    /// </summary>
    public class DefaultTokenContextGenerator : ITokenContextGenerator {
        protected readonly List<string> inducedAbbreviations;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTokenContextGenerator"/> class.
        /// </summary>
        public DefaultTokenContextGenerator() : this(new List<string>()) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTokenContextGenerator"/> class.
        /// </summary>
        /// <param name="inducedAbbreviations">The induced abbreviations.</param>
        public DefaultTokenContextGenerator(List<string> inducedAbbreviations) {
            this.inducedAbbreviations = inducedAbbreviations;
        }

        /// <summary>
        /// Returns an array of features for the specified sentence string at the specified index. 
        /// </summary>
        /// <param name="sentence">The string for a sentence.</param>
        /// <param name="index">The index to consider splitting as a token.</param>
        /// <returns>An array of features for the specified sentence string at the specified index.</returns>
        public string[] GetContext(string sentence, int index) {
            return CreateContext(sentence, index).ToArray();
        }

        #region . CreateContext .

        /// <summary>
        /// Returns an array of features for the specified sentence string at the specified index. 
        /// Extensions of this class can override this method to create a customized <see cref="ITokenContextGenerator"/>
        /// </summary>
        /// <param name="sentence">The string for a sentence.</param>
        /// <param name="index">The index to consider splitting as a token.</param>
        /// <returns>An array of features for the specified sentence string at the specified index.</returns>
        protected virtual List<string> CreateContext(string sentence, int index) {
            var preds = new List<string>();
            var prefix = sentence.Substring(0, index);
            var suffix = sentence.Substring(index);
            preds.Add("p=" + prefix);
            preds.Add("s=" + suffix);
            if (index > 0) {
                AddCharPreds("p1", sentence[index - 1], preds);
                if (index > 1) {
                    AddCharPreds("p2", sentence[index - 2], preds);
                    preds.Add("p21=" + sentence[index - 2] + sentence[index - 1]);
                } else {
                    preds.Add("p2=bok");
                }
                preds.Add("p1f1=" + sentence[index - 1] + sentence[index]);
            } else {
                preds.Add("p1=bok");
            }
            AddCharPreds("f1", sentence[index], preds);
            if (index + 1 < sentence.Length) {
                AddCharPreds("f2", sentence[index + 1], preds);
                preds.Add("f12=" + sentence[index] + sentence[index + 1]);
            } else {
                preds.Add("f2=bok");
            }
            if (sentence[0] == '&' && sentence[sentence.Length - 1] == ';') {
                preds.Add("cc"); //character code
            }

            if (index == sentence.Length - 1 && inducedAbbreviations.Contains(sentence)) {
                preds.Add("pabb");
            }

            return preds;
        }

        #endregion

        #region . AddCharPreds .

        /// <summary>
        /// Adds the character preds. Helper function for getContext.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="c">The c.</param>
        /// <param name="preds">The preds.</param>
        protected virtual void AddCharPreds(String key, char c, List<string> preds) {
            preds.Add(key + "=" + c);
            if (char.IsLetter(c)) {
                preds.Add(key + "_alpha");
                if (char.IsUpper(c)) {
                    preds.Add(key + "_caps");
                }
            } else if (char.IsDigit(c)) {
                preds.Add(key + "_num");
            } else if (char.IsWhiteSpace(c)) {
                preds.Add(key + "_ws");
            } else {
                if (c == '.' || c == '?' || c == '!') {
                    preds.Add(key + "_eos");
                } else if (c == '`' || c == '"' || c == '\'') {
                    preds.Add(key + "_quote");
                } else if (c == '[' || c == '{' || c == '(') {
                    preds.Add(key + "_lp");
                } else if (c == ']' || c == '}' || c == ')') {
                    preds.Add(key + "_rp");
                }
            }
        }

        #endregion

    }
}