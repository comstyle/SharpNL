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
using System.Linq;

namespace SharpNL.Parser.TreeInsert {
    public class AttachContextGenerator : AbstractContextGenerator {
        public AttachContextGenerator(List<string> punctSet) {
            this.punctSet = punctSet;
        }

        #region . ContainsPunct .
        private static bool ContainsPunct(IEnumerable<Parse> puncts, string punct) {
            if (puncts != null) {
                return puncts.Any(p => p.Type == punct);
            }
            return false;
        }
        #endregion

        #region + GetContext .

        /*
        public string[] getContext(object o) {
            var parts = (object[])o;
            return GetContext((Parse[])parts[0], (int)parts[1], (List<Parse>)parts[2], (int)parts[3]);
        }
         * */

        /// <summary>
        /// Gets the context features about this attachment.
        /// </summary>
        /// <param name="constituents">The constituents as they have been constructed so far.</param>
        /// <param name="index">The constituent index of the node being attached.</param>
        /// <param name="rightFrontier">The nodes which have been not attach to so far.</param>
        /// <param name="rfi">The rfi.</param>
        /// <returns>A set of contextual features about this attachment.</returns>
        public string[] GetContext(Parse[] constituents, int index, List<Parse> rightFrontier, int rfi) {
            var features = new List<string>();
            var nodeDistance = rfi;
            var fn = rightFrontier[rfi];
            Parse fp = null;
            if (rfi + 1 < rightFrontier.Count) {
                fp = rightFrontier[rfi + 1];
            }
            Parse p_1 = null;
            if (rightFrontier.Count > 0) {
                p_1 = rightFrontier[0];
            }
            var p0 = constituents[index];
            Parse p1 = null;
            if (index + 1 < constituents.Length) {
                p1 = constituents[index + 1];
            }

            var punct_1fs =  fn.PreviousPunctuationSet;
            var punct_1s = p0.PreviousPunctuationSet;
            var punct1s = p0.NextPunctuationSet;

            var consfp = Cons(fp, -3);
            var consf = Cons(fn, -2);
            var consp_1 = Cons(p_1, -1);
            var consp0 = Cons(p0, 0);
            var consp1 = Cons(p1, 1);

            var consbofp = ConsBo(fp, -3);
            var consbof = ConsBo(fn, -2);
            var consbop_1 = ConsBo(p_1, -1);
            var consbop0 = ConsBo(p0, 0);
            var consbop1 = ConsBo(p1, 1);

            var cfp = new Cons(consfp, consbofp, -3, true);
            var cf = new Cons(consf, consbof, -2, true);
            var c_1 = new Cons(consp_1, consbop_1, -1, true);
            var c0 = new Cons(consp0, consbop0, 0, true);
            var c1 = new Cons(consp1, consbop1, 1, true);

            //default
            features.Add("default");

            //unigrams
            features.Add(consfp);
            features.Add(consbofp);
            features.Add(consf);
            features.Add(consbof);
            features.Add(consp_1);
            features.Add(consbop_1);
            features.Add(consp0);
            features.Add(consbop0);
            features.Add(consp1);
            features.Add(consbop1);

            //productions
            var prod = Production(fn, false);
            //String punctProd = production(fn,true,punctSet);
            features.Add("pn=" + prod);
            features.Add("pd=" + prod + "," + p0.Type);
            features.Add("ps=" + fn.Type + "->" + fn.Type + "," + p0.Type);

            /*
            if (punct_1s != null) {
                var punctBuf = new StringBuilder();
                for (Iterator<Parse> pi = punct_1s.iterator(); pi.hasNext(); ) {
                    Parse punct = pi.next();
                    punctBuf.append(punct.getType()).append(",");
                }
                //features.Add("ppd="+punctProd+","+punctBuf.toString()+p0.getType());
                //features.Add("pps="+fn.getType()+"->"+fn.getType()+","+punctBuf.toString()+p0.getType());
            }
            */

            //bi-grams
            //cons(fn),cons(0)
            Cons2(features, cfp, c0, punct_1s, true);
            Cons2(features, cf, c0, punct_1s, true);
            Cons2(features, c_1, c0, punct_1s, true);
            Cons2(features, c0, c1, punct1s, true);
            Cons3(features, cf, c_1, c0, null, punct_1s, true, true, true);
            Cons3(features, cf, c0, c1, punct_1s, punct1s, true, true, true);
            Cons3(features, cfp, cf, c0, null, punct_1s, true, true, true);
            /*
            for (int ri=0;ri<rfi;ri++) {
              Parse jn = (Parse) rightFrontier.get(ri);
              features.Add("jn="+jn.getType());
            }
            */
            var headDistance = (p0.HeadIndex - fn.HeadIndex);
            features.Add("hd=" + headDistance);
            features.Add("nd=" + nodeDistance);

            features.Add("nd=" + p0.Type + "." + nodeDistance);
            features.Add("hd=" + p0.Type + "." + headDistance);
            //features.Add("fs="+rightFrontier.size());
            //paired punct features
            if (ContainsPunct(punct_1s, "''")) {
                if (ContainsPunct(punct_1fs, "``")) {
                    features.Add("quotematch"); //? not generating feature correctly
                } /* else {
                    //features.Add("noquotematch");
                } */
            }
            return features.ToArray();
        }

        #endregion

    }
}