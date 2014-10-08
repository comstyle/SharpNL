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

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpNL.Java;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    [JavaClass("opennlp.tools.namefind.BioCodec")]
    internal class BioCodec : ISequenceCodec<string> {
        public const string Start = "start";
        public const string Continue = "cont";
        public const string Other = "other";

        private static readonly Regex TypedOutcomeRegex;

        static BioCodec() {
            TypedOutcomeRegex = new Regex("(.+)-\\w+", RegexOptions.Compiled);
        }

        #region . AreOutcomesCompatible .
        /// <summary>
        /// Checks if the outcomes of the model are compatible with the codec.
        /// </summary>
        /// <param name="outcomes">All possible model outcomes</param>
        /// <returns><c>true</c> if the outcomes of the model are compatible with the codec, <c>false</c> otherwise.</returns>
        public bool AreOutcomesCompatible(string[] outcomes) {

            // We should have *optionally* one outcome named "other", some named xyz-start and sometimes
            // they have a pair xyz-cont. We should not have any other outcome
            // To validate the model we check if we have one outcome named "other", at least
            // one outcome with suffix start. After that we check if all outcomes that ends with
            // "cont" have a pair that ends with "start".
            var start = new List<string>();
            var cont = new List<string>();

            foreach (var outcome in outcomes) {

                if (outcome.EndsWith(NameFinderME.START)) {
                    start.Add(outcome.Substring(0, outcome.Length - NameFinderME.START.Length));
                } else if (outcome.EndsWith(NameFinderME.Continue)) {
                    cont.Add(outcome.Substring(0, outcome.Length - NameFinderME.Continue.Length));
                } else if (outcome.Equals(NameFinderME.Other)) {
                    // don't fail anymore if couldn't find outcome named OTHER
                } else {
                    // got unexpected outcome
                    return false;
                }
            }

            return start.Count != 0 && cont.All(start.Contains);
        }
        #endregion

        #region . CreateSequenceValidator .
        /// <summary>
        /// Creates a sequence validator which can validate a sequence of outcomes.
        /// </summary>
        /// <returns>A sequence validator which can validate a sequence of outcomes.</returns>
        public NameFinderSequenceValidator CreateSequenceValidator() {
            return new NameFinderSequenceValidator();
        }
        ISequenceValidator<string> ISequenceCodec<string>.CreateSequenceValidator() {
            return CreateSequenceValidator();
        }
        #endregion

        #region . Decode .

        /// <summary>
        /// Decodes a sequence string objects into its respective <see cref="Span"/> objects.
        /// </summary>
        /// <param name="objectList">The object list.</param>
        /// <returns>A array with the decoded objects.</returns>
        public Span[] Decode(string[] objectList) {
            var start = -1;
            var end = -1;
            var spans = new List<Span>(objectList.Length);
            for (var li = 0; li < objectList.Length; li++) {
                var chunkTag = objectList[li];
                if (chunkTag.EndsWith(Start)) {
                    if (start != -1) {
                        spans.Add(new Span(start, end, ExtractNameType(objectList[li - 1])));
                    }

                    start = li;
                    end = li + 1;
                } else if (chunkTag.EndsWith(Continue)) {
                    end = li + 1;
                } else if (chunkTag.EndsWith(Other)) {
                    if (start != -1) {
                        spans.Add(new Span(start, end, ExtractNameType(objectList[li - 1])));
                        start = -1;
                        end = -1;
                    }
                }
            }

            if (start != -1) {
                spans.Add(new Span(start, end, ExtractNameType(objectList[objectList.Length - 1])));
            }

            return spans.ToArray();
        }

        #endregion

        #region . Encode .
        /// <summary>
        /// Encodes the specified names.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="length">The length.</param>
        /// <returns>T[].</returns>
        public string[] Encode(Span[] names, int length) {
            var outcomes = new string[length];
            for (var i = 0; i < outcomes.Length; i++) {
                outcomes[i] = Other;
            }
            foreach (var name in names) {
                if (name.Type == null) {
                    outcomes[name.Start] = "default" + "-" + Start;
                } else {
                    outcomes[name.Start] = name.Type + "-" + Start;
                }
                // now iterate from begin + 1 till end
                for (var i = name.Start + 1; i < name.End; i++) {
                    if (name.Type == null) {
                        outcomes[i] = "default" + "-" + Continue;
                    } else {
                        outcomes[i] = name.Type + "-" + Continue;
                    }
                }
            }

            return outcomes;
        }
        #endregion

        #region . ExtractNameType .
        internal static string ExtractNameType(string outcome) {
            if (string.IsNullOrEmpty(outcome))
                return null;

            var match = TypedOutcomeRegex.Match(outcome);

            return match.Success ? match.Groups[1].Value : null;
        }
        #endregion

    }
}