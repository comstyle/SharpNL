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

using System.Collections.Generic;
using System.IO;
using SharpNL.Java;

namespace SharpNL.Parser.Lang.en {

    /// <summary>
    /// Class HeadRules.
    /// </summary>
    public class HeadRules : AbstractHeadRules, IGapLabeler {

        #region . Constructor .

        public HeadRules(StreamReader reader) {
            ReadHeadRules(reader);
            PunctuationTags = new List<string> {".", ",", "``", "''"}; // ":"
        }

        #endregion

        #region . GetHead .

        /// <summary>
        /// Returns the head constituent for the specified constituents of the specified type.
        /// </summary>
        /// <param name="constituents">The constituents which make up a constituent of the specified type.</param>
        /// <param name="type">The type of a constituent which is made up of the specified constituents.</param>
        /// <returns>The constituent which is the head.</returns>
        public override Parse GetHead(Parse[] constituents, string type) {
            if (constituents[0].Type == AbstractBottomUpParser.TOK_NODE) {
                return null;
            }


            if (type == "NP" || type == "NX") {
                string[] tags1 = {"NN", "NNP", "NNPS", "NNS", "NX", "JJR", "POS"};
                for (var ci = constituents.Length - 1; ci >= 0; ci--) {
                    for (var ti = tags1.Length - 1; ti >= 0; ti--) {
                        if (constituents[ci].Type == tags1[ti]) {
                            return constituents[ci].Head;
                        }
                    }
                }
                for (var ci = 0; ci < constituents.Length; ci++) {
                    if (constituents[ci].Type == "NP") {
                        return constituents[ci].Head;
                    }
                }
                string[] tags2 = {"$", "ADJP", "PRN"};
                for (var ci = constituents.Length - 1; ci >= 0; ci--) {
                    for (var ti = tags2.Length - 1; ti >= 0; ti--) {
                        if (constituents[ci].Type == tags2[ti]) {
                            return constituents[ci].Head;
                        }
                    }
                }
                string[] tags3 = {"JJ", "JJS", "RB", "QP"};
                for (var ci = constituents.Length - 1; ci >= 0; ci--) {
                    for (var ti = tags3.Length - 1; ti >= 0; ti--) {
                        if (constituents[ci].Type == tags3[ti]) {
                            return constituents[ci].Head;
                        }
                    }
                }
                return constituents[constituents.Length - 1].Head;
            }

            if (HeadRules.ContainsKey(type)) {
                var hr = HeadRules[type];

                var tags = hr.Tags;
                var cl = constituents.Length;
                var tl = tags.Length;
                if (hr.LeftToRight) {
                    for (var ti = 0; ti < tl; ti++) {
                        for (var ci = 0; ci < cl; ci++) {
                            if (constituents[ci].Type == tags[ti]) {
                                return constituents[ci].Head;
                            }
                        }
                    }
                    return constituents[0].Head;
                }
                for (var ti = 0; ti < tl; ti++) {
                    for (var ci = cl - 1; ci >= 0; ci--) {
                        if (constituents[ci].Type == tags[ti]) {
                            return constituents[ci].Head;
                        }
                    }
                }
                return constituents[cl - 1].Head;
            }
            return constituents[constituents.Length - 1].Head;
        }

        #endregion

        #region . GetPunctuationTags .

        /// <summary>
        /// Returns the set of punctuation tags.
        /// Attachment decisions for these tags will not be modeled.
        /// </summary>
        /// <returns>The list of punctuation tags.</returns>
        public List<string> GetPunctuationTags() {
            return PunctuationTags;
        }

        #endregion

        #region . LabelGaps .

        /// <summary>
        /// Labels the constituents found in the stack with gap labels if appropriate.
        /// </summary>
        /// <param name="list">The list of un-completed constituents.</param>
        public void LabelGaps(Stack<Constituent> stack) {
            if (stack.Count > 4) {

                var list = stack.ToArray(4);

                //var con0 = list[list.Count - 1];
                var con1 = list[list.Length - 2];
                var con2 = list[list.Length - 3];
                var con3 = list[list.Length - 4];
                var con4 = list[list.Length - 5];

                // subject extraction
                if (con1.Label == "NP" && con2.Label == "S" && con3.Label == "SBAR") {
                    con1.Label = con1.Label + "-G";
                    con2.Label = con2.Label + "-G";
                    con3.Label = con3.Label + "-G";
                    return;
                }
                // object extraction
                if (con1.Label == "NP" && con2.Label == "VP" && con3.Label == "S" && con4.Label == "SBAR") {
                    con1.Label = con1.Label + "-G";
                    con2.Label = con2.Label + "-G";
                    con3.Label = con3.Label + "-G";
                    con4.Label = con4.Label + "-G";
                }
            }
        }

        #endregion

        #region . ReadHeadRules .

        private void ReadHeadRules(StreamReader reader) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                var st = new StringTokenizer(line);
                var num = int.Parse(st.NextToken);
                var type = st.NextToken;
                var dir = st.NextToken;
                var tags = new string[num - 2];
                var ti = 0;
                while (st.HasMoreTokens) {
                    tags[ti] = st.NextToken;
                    ti++;
                }
                HeadRules[type] = new HeadRule(dir == "1", tags);
            }
        }

        #endregion
        
    }
}