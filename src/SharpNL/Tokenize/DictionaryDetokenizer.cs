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
using System.IO;
using System.Text;

namespace SharpNL.Tokenize {
    /// <summary>
    /// A rule based detokenizer. Simple rules which indicate in which direction a token should be 
    /// moved are looked up in a <see cref="DetokenizationDictionary"/> object.
    /// </summary>
    public class DictionaryDetokenizer : IDetokenizer {
        private readonly DetokenizationDictionary dictionary;

        public DictionaryDetokenizer(DetokenizationDictionary dictionary) {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryDetokenizer"/> class.
        /// </summary>
        /// <param name="dictionaryFile">The dictionary file.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="dictionaryFile"/></exception>
        /// <exception cref="System.IO.FileNotFoundException">The dictionary file does not exist.</exception>
        public DictionaryDetokenizer(FileInfo dictionaryFile) {
            if (dictionaryFile == null)
                throw new ArgumentNullException("dictionaryFile");

            if (!dictionaryFile.Exists)
                throw new FileNotFoundException("The dictionary file does not exist.", dictionaryFile.FullName);

            dictionary = new DetokenizationDictionary(dictionaryFile.OpenRead());
        }

        /// <summary>Detokenizes the specified tokens.</summary>
        /// <param name="tokens">The tokens to detokenize.</param>
        /// <returns>The merge operations to detokenize the input tokens.</returns>
        public DetokenizationOperation[] Detokenize(string[] tokens) {
            var operations = new DetokenizationOperation[tokens.Length];

            var matchingTokens = new HashSet<string>();

            for (var i = 0; i < tokens.Length; i++) {
                var dictOperation = dictionary[tokens[i]];

                switch (dictOperation) {
                    case DetokenizationDictionary.Operation.MoveRight:
                        operations[i] = DetokenizationOperation.MergeToRight;
                        break;
                    case DetokenizationDictionary.Operation.MoveLeft:
                        operations[i] = DetokenizationOperation.MergeToLeft;
                        break;
                    case DetokenizationDictionary.Operation.MoveBoth:
                        operations[i] = DetokenizationOperation.MergeBoth;
                        break;
                    case DetokenizationDictionary.Operation.RightLeftMatching:

                        if (matchingTokens.Contains(tokens[i])) {
                            // The token already occurred once, move it to the left
                            // and clear the occurrence flag
                            operations[i] = DetokenizationOperation.MergeToLeft;
                            matchingTokens.Remove(tokens[i]);
                        } else {
                            // First time this token is seen, move it to the right
                            // and remember it
                            operations[i] = DetokenizationOperation.MergeToRight;
                            matchingTokens.Add(tokens[i]);
                        }

                        break;
                    case null:
                        operations[i] = DetokenizationOperation.NoOperation;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown operation: " + dictionary[tokens[i]]);
                }
            }
            return operations;
        }

        /// <summary>Detokenize the input tokens into a String. Tokens which are connected without a space inbetween can be
        /// separated by a split marker.</summary>
        /// <param name="tokens">The tokens to detokenize.</param>
        /// <param name="splitMarker">The split marker or null.</param>
        /// <returns>The detokenized string.</returns>
        public string Detokenize(string[] tokens, string splitMarker) {
            var operations = Detokenize(tokens);

            if (operations.Length != tokens.Length) {
                throw new InvalidOperationException("tokens and operations array must have same length: tokens=" +
                                                    tokens.Length + ", operations=" + operations.Length + "!");
            }

            var sb = new StringBuilder();

            for (var i = 0; i < tokens.Length; i++) {
                // attach token to string buffer
                sb.Append(tokens[i]);

                bool isAppendSpace;
                bool isAppendSplitMarker;

                // if this token is the last token do not attach a space
                if (i + 1 == operations.Length) {
                    isAppendSpace = false;
                    isAppendSplitMarker = false;
                }
                    // if next token move left, no space after this token,
                    // its safe to access next token
                else if (operations[i + 1] == DetokenizationOperation.MergeToLeft ||
                         operations[i + 1] == DetokenizationOperation.MergeBoth) {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                }
                    // if this token is move right, no space
                else if (operations[i] == DetokenizationOperation.MergeToRight ||
                         operations[i] == DetokenizationOperation.MergeBoth) {
                    isAppendSpace = false;
                    isAppendSplitMarker = true;
                } else {
                    isAppendSpace = true;
                    isAppendSplitMarker = false;
                }

                if (isAppendSpace) {
                    sb.Append(' ');
                }

                if (isAppendSplitMarker && splitMarker != null) {
                    sb.Append(splitMarker);
                }
            }

            return sb.ToString();
        }
    }
}