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
using System.Text.RegularExpressions;
using SharpNL.Utility;

namespace SharpNL.POSTag.Language.pt {
    /// <summary>
    /// Represents a portuguese POS sequence validator.
    /// </summary>
    public class POSSequenceValidator : ISequenceValidator<string> {
        private static readonly Regex PunctuationRegex;

        static POSSequenceValidator() {
            PunctuationRegex = new Regex("^[\\.,;:()?-]$", RegexOptions.Compiled);
        }

        private readonly bool bosque;
        private readonly ITagDictionary tagDictionary;
        private readonly IList<string> unknownList;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="POSSequenceValidator"/> with its respective tag dictionary.
        /// </summary>
        /// <param name="tagDictionary">The tag dictionary.</param>
        /// <param name="bosque">set true if the data is from BOSQUE file.</param>
        public POSSequenceValidator(ITagDictionary tagDictionary, bool bosque) : this(tagDictionary, bosque, null) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="POSSequenceValidator"/> with the specified dictionary and a unknown word list.
        /// </summary>
        /// <param name="tagDictionary">The tag dictionary.</param>
        /// <param name="bosque">set true if the data is from BOSQUE file.</param>
        /// <param name="unknownList">
        /// The unknown list. If specified the validator will include the unknown words to this list.
        /// </param>
        /// <exception cref="System.ArgumentNullException">tagDictionary</exception>
        public POSSequenceValidator(ITagDictionary tagDictionary, bool bosque, IList<string> unknownList) {
            if (tagDictionary == null)
                throw new ArgumentNullException("tagDictionary");

            this.bosque = bosque;
            this.tagDictionary = tagDictionary;
            this.unknownList = unknownList;
        }

        #endregion

        #region . ValidSequence .


        /// <summary>
        /// Determines whether a particular continuation of a sequence is valid.
        /// This is used to restrict invalid sequences such as those used in start/continue tag-based chunking or could be used to implement tag dictionary restrictions.
        /// </summary>
        /// <param name="index">The index in the input sequence for which the new outcome is being proposed.</param>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="outcomesSequence">The outcomes so far in this sequence.</param>
        /// <param name="outcome">The next proposed outcome for the outcomes sequence.</param>
        /// <returns><c>true</c> if the sequence would still be valid with the new outcome, <c>false</c> otherwise.</returns>
        public bool ValidSequence(int index, string[] inputSequence, string[] outcomesSequence, string outcome) {
            var word = inputSequence[index];

            if (index > 0 &&
                outcome == "mm" &&
                inputSequence[index - 1].Equals("a", StringComparison.OrdinalIgnoreCase) &&
                outcomesSequence[index - 1] == "artf") {
                return false;
            }

            outcome = GenderUtil.RemoveGender(outcome);

            if (bosque && PunctuationRegex.IsMatch(word)) {
                return outcome.Equals(word);
            }

            if (index < inputSequence.Length - 1 &&
                PunctuationRegex.IsMatch(inputSequence[index + 1]) &&
                outcome.StartsWith("B-")) {
                // we can't start a MWE here :(
                return false;
            }

            // validate B- and I-
            if (!ValidOutcome(outcome, outcomesSequence)) {
                return false;
            }

            if (tagDictionary == null)
                return true;

            if ((outcome.StartsWith("B-") || outcome.StartsWith("I-")) && inputSequence.Length > 1)
                return true;

            if (word == outcome)
                return true;

            var tagList = FilterMWE(QueryDictionary(word, true));

            if (tagList != null && tagList.Count > 0) {
                // token exists

                if (outcome == "prop" && char.IsUpper(word[0]))
                    return true;

                return Contains(tagList, outcome);
            }

            if (unknownList != null) {
                unknownList.Add(word);
            }

            return true;
        }


        #endregion

        #region . Contains .

        private bool Contains(List<string> tagList, string outcome) {
            if (tagList.Contains(outcome))
                return true;

            switch (outcome) {
                case "n-adj":
                    return tagList.Contains("n") || tagList.Contains("adj");
                case "n":
                case "adj":
                    return tagList.Contains("n-adj");
                default:
                    if (outcome.Contains("=")) {
                        var outcomeClass = outcome.Substring(0, outcome.IndexOf('='));
                        foreach (var tag in tagList) {
                            if (tag.StartsWith(outcomeClass) && (tag.Contains("/") || outcome.Contains("/"))) {
                                var outcomeParts = outcome.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                                var tagParts = tag.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                                // will only check parts without for simplicity
                                if (outcomeParts.Length != tagParts.Length) {
                                    return false;
                                }
                                for (var i = 0; i < outcomeParts.Length; i++) {
                                    var outcomePart = outcomeParts[i];
                                    var tagPart = tagParts[i];
                                    if (!outcomePart.Contains("/") && !tagPart.Contains("/")) {
                                        if (!outcomePart.Equals(tagPart)) {
                                            return false;
                                        }
                                    }
                                }
                                return true;
                            }
                        }
                    }
                    return false;
            }
        }

        #endregion

        #region . QueryDictionary .

        private string[] QueryDictionary(string word, bool recurse) {
            var tags = tagDictionary.GetTags(word) ?? tagDictionary.GetTags(word.ToLowerInvariant());
            if (recurse) {
                if (word.StartsWith("-") && word.Length > 1) {
                    tags = QueryDictionary(word.Substring(1), false);
                }
            }

            return GenderUtil.RemoveGender(tags);
        }

        #endregion

        #region . FilterMWE .

        private static List<string> FilterMWE(string[] items) {
            if (items == null)
                return null;

            var list = new List<string>(items.Length);

            foreach (var item in items) {
                if (!(item.StartsWith("B-") || item.StartsWith("I-"))) {
                    list.Add(item);
                }
            }
            return list;
        }

        #endregion

        #region + ValidOutcome .

        private static bool ValidOutcome(string outcome, string[] sequence) {
            return ValidOutcome(outcome, sequence.Length > 0 ? sequence[sequence.Length - 1] : null);
        }

        private static bool ValidOutcome(string outcome, string prevOutcome) {
            var isBoundary = false;
            var isIntermediate = false;
            var prevIsBoundary = false;
            var prevIsIntermediate = false;

            if (prevOutcome != null) {
                prevIsBoundary = prevOutcome.StartsWith("B-");
                prevIsIntermediate = prevOutcome.StartsWith("I-");
            }

            if (outcome != null) {
                isBoundary = outcome.StartsWith("B-");
                isIntermediate = outcome.StartsWith("I-");
            }

            var isSameEntity = false;
            if ((prevIsBoundary || prevIsIntermediate) && isIntermediate) {
                isSameEntity = prevOutcome.Substring(2).Equals(outcome.Substring(2));
            }

            if (isIntermediate) {
                if (prevOutcome == null) {
                    return false;
                }

                if (!isSameEntity) {
                    return false;
                }
            } else if (isBoundary) {
                if (prevIsBoundary) {
                    return false; // MWE should have at least two tokens
                }
            }

            if (prevIsBoundary && !isIntermediate) {
                return false; // MWE should have at least two tokens
            }

            return true;
        }

        #endregion
    }
}