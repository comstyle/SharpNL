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

namespace SharpNL.Parser.TreeInsert {
    /// <summary>
    /// Creates the features or contexts for the building phase of parsing.
    /// This phase builds constituents from the left-most node of these constituents.
    /// </summary>
    public class BuildContextGenerator : AbstractContextGenerator {
        private readonly Parse[] leftNodes;

        public BuildContextGenerator() {
            leftNodes = new Parse[2];
        }

        /*
        public string[] getContext(Object o) {
            object[] parts = (Object[])o;
            return getContext((Parse[])parts[0], (Integer)parts[1]);
        }
        */

        #region . GetContext .
        /// <summary>
        /// Returns the contexts/features for the decision to build a new constituent for the 
        /// specified parse at the specified index.
        /// </summary>
        /// <param name="constituents">The constituents of the parse so far.</param>
        /// <param name="index">The index of the constituent where a build decision is being made.</param>
        /// <returns>The contexts/features for the decision to build a new constituent.</returns>
        public string[] GetContext(Parse[] constituents, int index) {
            Parse p0 = constituents[index];
            Parse p1 = null;
            Parse p2 = null;
            var ps = constituents.Length;

            if (index + 1 < ps) {
                p1 = constituents[index + 1];
            }
            if (index + 2 < ps) {
                p2 = constituents[index + 2];
            }

            SortedSet<Parse> punct2s = null;
            SortedSet<Parse> punct_2s = null;

            var punct_1s = p0.PreviousPunctuationSet;
            var punct1s = p0.NextPunctuationSet;
            if (p1 != null) {
                punct2s = p1.NextPunctuationSet;
            }

            List<Parse> rf;
            if (index == 0) {
                rf = new List<Parse>();
            } else {
                //this isn't a root node so, punctSet won't be used and can be passed as empty.
                var emptyPunctSet = new List<string>();
                rf = Parser.GetRightFrontier(constituents[0], emptyPunctSet);
            }
            GetFrontierNodes(rf, leftNodes);
            var p_1 = leftNodes[0];
            var p_2 = leftNodes[1];

            if (p_1 != null) {
                punct_2s = p_1.PreviousPunctuationSet;
            }

            var consp_2 = Cons(p_2, -2);
            var consp_1 = Cons(p_1, -1);
            var consp0 = Cons(p0, 0);
            var consp1 = Cons(p1, 1);
            var consp2 = Cons(p2, 2);

            var consbop_2 = ConsBo(p_2, -2);
            var consbop_1 = ConsBo(p_1, -1);
            var consbop0 = ConsBo(p0, 0);
            var consbop1 = ConsBo(p1, 1);
            var consbop2 = ConsBo(p2, 2);

            var c_2 = new Cons(consp_2, consbop_2, -2, true);
            var c_1 = new Cons(consp_1, consbop_1, -1, true);
            var c0 = new Cons(consp0, consbop0, 0, true);
            var c1 = new Cons(consp1, consbop1, 1, true);
            var c2 = new Cons(consp2, consbop2, 2, true);

            var features = new List<string> {
                "default",

                //uni-grams
                consp_2,
                consbop_2,
                consp_1,
                consbop_1,
                consp0,
                consbop0,
                consp1,
                consbop1,
                consp2,
                consbop2
            };

            //cons(0),cons(1)
            Cons2(features, c0, c1, punct1s, true);
            //cons(-1),cons(0)
            Cons2(features, c_1, c0, punct_1s, true);
            //features.add("stage=cons(0),cons(1),cons(2)");
            Cons3(features, c0, c1, c2, punct1s, punct2s, true, true, true);
            Cons3(features, c_2, c_1, c0, punct_2s, punct_1s, true, true, true);
            Cons3(features, c_1, c0, c1, punct_1s, punct_1s, true, true, true);

            if (rf.Count == 0) {
                features.Add(EOS + "," + consp0);
                features.Add(EOS + "," + consbop0);
            }

            return features.ToArray();
        }

        #endregion

    }
}