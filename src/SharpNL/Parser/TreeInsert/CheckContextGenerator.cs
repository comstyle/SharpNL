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

namespace SharpNL.Parser.TreeInsert {
    public class CheckContextGenerator : AbstractContextGenerator {
        private readonly Parse[] leftNodes;

        public CheckContextGenerator(List<string> punctSet) {
            this.punctSet = punctSet;

            leftNodes = new Parse[2];
        }

        public string[] GetContext(Parse parent, Parse[] constituents, int index, bool trimFrontier) {
            var features = new List<string> {"default"};
            var children = AbstractBottomUpParser.CollapsePunctuation(parent.Children, punctSet);
            var pStart = children[0];
            var pend = children[children.Length - 1];
            var type = parent.Type;
            CheckCons(pStart, "begin", type, features);
            CheckCons(pend, "last", type, features);
            var production = "p=" + Production(parent, false);
            var punctProduction = "pp=" + Production(parent, true);
            features.Add(production);
            features.Add(punctProduction);


            Parse p1 = null;
            Parse p2 = null;
            var p1s = constituents[index].NextPunctuationSet;
            SortedSet<Parse> p2s = null;
            var p_1s = constituents[index].PreviousPunctuationSet;
            SortedSet<Parse> p_2s = null;
            List<Parse> rf;
            if (index == 0) {
                rf = new List<Parse>();
            } else {
                rf = Parser.GetRightFrontier(constituents[0], punctSet);
                if (trimFrontier) {
                    var pi = rf.IndexOf(parent);
                    if (pi == -1) {
                        throw new InvalidOperationException("Parent not found in right frontier:" + parent + " rf=" + rf);
                    }
                    for (var ri = 0; ri <= pi; ri++) {
                        //System.err.println(pi+" removing "+((Parse)rf.get(0)).getType()+" "+rf.get(0)+" "+(rf.size()-1)+" remain");
                        rf.RemoveAt(0);
                    }
                }
            }

            GetFrontierNodes(rf, leftNodes);
            var p_1 = leftNodes[0];
            var p_2 = leftNodes[1];
            var ps = constituents.Length;
            if (p_1 != null) {
                p_2s = p_1.PreviousPunctuationSet;
            }
            if (index + 1 < ps) {
                p1 = constituents[index + 1];
                p2s = p1.NextPunctuationSet;
            }
            if (index + 2 < ps) {
                p2 = constituents[index + 2];
            }
            Surround(p_1, -1, type, p_1s, features);
            Surround(p_2, -2, type, p_2s, features);
            Surround(p1, 1, type, p1s, features);
            Surround(p2, 2, type, p2s, features);

            return features.ToArray();
        }
    }
}