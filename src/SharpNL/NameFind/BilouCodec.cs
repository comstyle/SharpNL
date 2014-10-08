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
using SharpNL.Java;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Represents a BILOU (Begin, Inside, Last, Outside, Unit) sequence codec.
    /// </summary>
    [JavaClass("opennlp.tools.namefind.BilouCodec")]
    public class BilouCodec : ISequenceCodec<string> {

        internal const string START = "start";
        internal const string CONTINUE = "cont";
        internal const string LAST = "last";
        internal const string UNIT = "unit";
        internal const string OTHER = "other";

        private static readonly Regex typedOutcomePattern;

        static BilouCodec() {
            typedOutcomePattern = new Regex("(.+)-\\w+", RegexOptions.Compiled);
        }

        #region . Decode .
        /// <summary>
        /// Decodes a sequence string objects into its respective <see cref="Span"/> objects.
        /// </summary>
        /// <param name="objectList">The object list.</param>
        /// <returns>A array with the decoded objects.</returns>
        public Span[] Decode(string[] objectList) {
            int start = -1;
            int end = -1;
            var spans = new List<Span>(objectList.Length);
            for (int li = 0; li < objectList.Length; li++) {

                if (objectList[li].EndsWith(START)) {
                    start = li;
                    end = li + 1;
                } else if (objectList[li].EndsWith(CONTINUE)) {
                    end = li + 1;
                } else if (objectList[li].EndsWith(LAST)) {
                    if (start != -1) {
                        spans.Add(new Span(start, end + 1, ExtractNameType(objectList[li - 1])));
                        start = -1;
                        end = -1;
                    }
                } else if (objectList[li].EndsWith(UNIT)) {
                    spans.Add(new Span(li, li + 1, ExtractNameType(objectList[li])));
                } else if (objectList[li].EndsWith(OTHER)) {
                    // in this case do nothing
                }
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
            for (int i = 0; i < outcomes.Length; i++) {
                outcomes[i] = OTHER;
            }
            foreach (Span name in names) {
                if (name.Type == null) {
                    outcomes[name.Start] = "default" + "-" + START;
                } else {
                    outcomes[name.Start] = name.Type + "-" + START;
                }
                // now iterate from begin + 1 till end
                for (int i = name.Start + 1; i < name.End; i++) {
                    if (name.Type == null) {
                        outcomes[i] = "default" + "-" + CONTINUE;
                    } else {
                        outcomes[i] = name.Type + "-" + CONTINUE;
                    }
                }
            }

            return outcomes;
        }
        #endregion

        #region . ExtractNameType .
        internal static string ExtractNameType(string outcome) {
            var matches = typedOutcomePattern.Match(outcome);
            if (matches.Success) {
                return matches.Groups[1].Value;
            }

            return null;
        }
        #endregion

        #region . CreateSequenceValidator .
        /// <summary>
        /// Creates a sequence validator which can validate a sequence of outcomes.
        /// </summary>
        /// <returns>A sequence validator which can validate a sequence of outcomes.</returns>
        public ISequenceValidator<string> CreateSequenceValidator() {
            return new BilouNameFinderSequenceValidator();
        }
        #endregion

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

            var start = new List<String>();
            var cont = new List<String>();

            foreach (var outcome in outcomes) {
                if (outcome.EndsWith(START)) {
                    start.Add(outcome.Substring(0, outcome.Length - START.Length));
                } else if (outcome.EndsWith(CONTINUE)) {
                    cont.Add(outcome.Substring(0, outcome.Length - CONTINUE.Length));
                } else if (outcome.Equals(OTHER)) {
                    // don't fail anymore if couldn't find outcome named OTHER
                } else {
                    // got unexpected outcome
                    return false;
                }
            }

            if (start.Count == 0) {
                return false;
            }

            foreach (string contPrefix in cont) {
                if (!start.Contains(contPrefix)) {
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}