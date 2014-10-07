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
using System.Text;

namespace SharpNL.Parser {
    /// <summary>
    /// Abstract class containing many of the methods used to generate contexts for parsing.
    /// </summary>
    public abstract class AbstractContextGenerator {
        protected const string EOS = "eos";

        protected List<string> punctSet;
        protected bool useLabel;
        protected bool zeroBackOff;

        #region . Cons .

        protected string Cons(Parse punctuation, int index) {
            var feat = new StringBuilder();
            feat.Append(index).Append("=");
            if (punctuation != null) {
                if (useLabel && index < 0) {
                    feat.Append(punctuation.Label).Append("|");
                }
                feat.Append(punctuation.Type).Append("|").Append(punctuation.Head.CoveredText);
            } else {
                feat.Append(EOS);
            }
            return feat.ToString();
        }

        #endregion

        #region . ConsBo .

        protected String ConsBo(Parse p, int i) {
            //cons back-off
            var feat = new StringBuilder();
            feat.Append(i).Append("*=");
            if (p != null) {
                if (useLabel && i < 0) {
                    feat.Append(p.Label).Append("|");
                }
                feat.Append(p.Type);
            } else {
                feat.Append(EOS);
            }
            return feat.ToString();
        }

        #endregion

        #region . Punct .

        /// <summary>
        /// Creates punctuation feature for the specified punctuation at the specified index based on the punctuation mark.
        /// </summary>
        /// <param name="punctuation">The punctuation which is in context.</param>
        /// <param name="index">The index of the punctuation with relative to the parse.</param>
        /// <returns>Punctuation feature for the specified parse and the specified punctuation at the specified index.</returns>
        protected string Punct(Parse punctuation, int index) {
            return string.Format("{0}={1}", index, punctuation.CoveredText);
        }

        #endregion

        #region . PunctBo .

        /// <summary>
        /// Creates punctuation feature for the specified punctuation at the specfied index based on the punctuation's tag.
        /// </summary>
        /// <param name="punctuation">The punctuation which is in context.</param>
        /// <param name="index">The index of the punctuation relative to the parse.</param>
        /// <returns>Punctuation feature for the specified parse and the specified punctuation at the specfied index.</returns>
        protected string PunctBo(Parse punctuation, int index) {
            return string.Format("{0}={1}", index, punctuation.Type);
        }

        #endregion

        #region . Production .

        /// <summary>
        /// Generates a string representing the grammar rule production that the specified parse is starting.
        /// </summary>
        /// <param name="p">The parse which stats teh production.</param>
        /// <param name="includePunctuation">if set to <c>true</c> punctuation will be included in the production.</param>
        /// <returns>A string representing the grammar rule production that the specified parse is starting</returns>
        protected string Production(Parse p, bool includePunctuation) {
            var sb = new StringBuilder();
            sb.Append(p.Type).Append("->");
            var children = AbstractBottomUpParser.CollapsePunctuation(p.Children, punctSet);
            for (var ci = 0; ci < children.Length; ci++) {
                sb.Append(children[ci].Type);
                if (ci + 1 != children.Length) {
                    sb.Append(",");
                    var nextPunct = children[ci].NextPunctuationSet;
                    if (includePunctuation && nextPunct != null) {
                        //TODO: make sure multiple punctuation comes out the same

                        for (var pit = nextPunct.GetEnumerator(); pit.MoveNext();) {
                            if (pit.Current != null)
                                sb.Append(pit.Current.Type).Append(",");
                        }
                    }
                }
            }
            return sb.ToString();
        }

        #endregion

        #region . Cons2 .

        protected void Cons2(List<String> features, Cons c0, Cons c1, List<Parse> punct1s, bool bigram) {
            if (punct1s != null) {
                foreach (var p in punct1s) {
                    var punctbo = PunctBo(p, c1.index <= 0 ? c1.index - 1 : c1.index);

                    //punctbo(1);
                    features.Add(punctbo);
                    if (c0.index == 0) {
                        //TODO look at removing case
                        //cons(0)punctbo(1)
                        if (c0.unigram) features.Add(c0.cons + "," + punctbo);
                        features.Add(c0.consbo + "," + punctbo);
                    }
                    if (c1.index == 0) {
                        //TODO look at removing case
                        //punctbo(1)cons(1)
                        if (c1.unigram) features.Add(punctbo + "," + c1.cons);
                        features.Add(punctbo + "," + c1.consbo);
                    }

                    //cons(0)punctbo(1)cons(1)
                    if (bigram) features.Add(c0.cons + "," + punctbo + "," + c1.cons);
                    if (c1.unigram) features.Add(c0.consbo + "," + punctbo + "," + c1.cons);
                    if (c0.unigram) features.Add(c0.cons + "," + punctbo + "," + c1.consbo);
                    features.Add(c0.consbo + "," + punctbo + "," + c1.consbo);
                }
            } else {
                //cons(0),cons(1)
                if (bigram) features.Add(c0.cons + "," + c1.cons);
                if (c1.unigram) features.Add(c0.consbo + "," + c1.cons);
                if (c0.unigram) features.Add(c0.cons + "," + c1.consbo);
                features.Add(c0.consbo + "," + c1.consbo);
            }
        }

        #endregion

        #region . Cons3 .

        /// <summary>
        /// Creates cons features involving the 3 specified nodes and adds them to the specified feature list.
        /// </summary>
        /// <param name="features">The list of features.</param>
        /// <param name="c0">The first node.</param>
        /// <param name="c1">The second node.</param>
        /// <param name="c2">The third node.</param>
        /// <param name="punct1s">The punctuation between the first and second node.</param>
        /// <param name="punct2s">The punctuation between the second and third node.</param>
        /// <param name="trigram">Specifies whether lexical tri-gram features between these nodes should be generated.</param>
        /// <param name="bigram1">Specifies whether lexical bi-gram features between the first and second node should be generated.</param>
        /// <param name="bigram2">Specifies whether lexical bi-gram features between the second and third node should be generated.</param>
        protected void Cons3(List<String> features, Cons c0, Cons c1, Cons c2, List<Parse> punct1s,
            List<Parse> punct2s, bool trigram, bool bigram1, bool bigram2) {
            //  features.add("stage=cons(0),cons(1),cons(2)");
            if (punct1s != null) {
                if (c0.index == -2) {
                    foreach (var p in punct1s) {
                        var punctbo = PunctBo(p, c1.index <= 0 ? c1.index - 1 : c1.index);
                        //punct(-2)
                        //TODO consider changing
                        //features.add(punct);

                        //punctbo(-2)
                        features.Add(punctbo);
                    }
                }
            }
            if (punct2s != null) {
                if (c2.index == 2) {
                    for (int i = 0; i < punct2s.Count; i++) {
                        var p = punct2s[i];
                        //          String punct = punct(p,c2.index);
                        var punctbo = PunctBo(p, c2.index <= 0 ? c2.index - 1 : c2.index);
                        //punct(2)
                        //TODO consider changing
                        //features.add(punct);

                        //punctbo(2)
                        features.Add(punctbo);
                    }
                }
                if (punct1s != null) {
                    //cons(0),punctbo(1),cons(1),punctbo(2),cons(2)
                    for (int i = 0; i < punct2s.Count; i++) {
                        var pi2 = punct2s[i];
                        var punctbo2 = PunctBo(pi2, c2.index <= 0 ? c2.index - 1 : c2.index);

                        foreach (var pi1 in punct1s) {
                            var punctbo1 = PunctBo(pi1, c1.index <= 0 ? c1.index - 1 : c1.index);
                            if (trigram)
                                features.Add(c0.cons + "," + punctbo1 + "," + c1.cons + "," + punctbo2 + "," + c2.cons);

                            if (bigram2)
                                features.Add(c0.consbo + "," + punctbo1 + "," + c1.cons + "," + punctbo2 + "," + c2.cons);
                            if (c0.unigram && c2.unigram)
                                features.Add(c0.cons + "," + punctbo1 + "," + c1.consbo + "," + punctbo2 + "," + c2.cons);
                            if (bigram1)
                                features.Add(c0.cons + "," + punctbo1 + "," + c1.cons + "," + punctbo2 + "," + c2.consbo);

                            if (c2.unigram)
                                features.Add(c0.consbo + "," + punctbo1 + "," + c1.consbo + "," + punctbo2 + "," +
                                             c2.cons);
                            if (c1.unigram)
                                features.Add(c0.consbo + "," + punctbo1 + "," + c1.cons + "," + punctbo2 + "," +
                                             c2.consbo);
                            if (c0.unigram)
                                features.Add(c0.cons + "," + punctbo1 + "," + c1.consbo + "," + punctbo2 + "," +
                                             c2.consbo);

                            features.Add(c0.consbo + "," + punctbo1 + "," + c1.consbo + "," + punctbo2 + "," + c2.consbo);
                            if (zeroBackOff) {
                                if (bigram1) features.Add(c0.cons + "," + punctbo1 + "," + c1.cons + "," + punctbo2);
                                if (c1.unigram)
                                    features.Add(c0.consbo + "," + punctbo1 + "," + c1.cons + "," + punctbo2);
                                if (c0.unigram)
                                    features.Add(c0.cons + "," + punctbo1 + "," + c1.consbo + "," + punctbo2);
                                features.Add(c0.consbo + "," + punctbo1 + "," + c1.consbo + "," + punctbo2);
                            }
                        }
                    }
                } else {
                    //punct1s == null

                    foreach (var pi2 in punct2s) {
                        var punctbo2 = PunctBo(pi2, c2.index <= 0 ? c2.index - 1 : c2.index);
                        if (trigram) features.Add(c0.cons + "," + c1.cons + "," + punctbo2 + "," + c2.cons);

                        if (bigram2) features.Add(c0.consbo + "," + c1.cons + "," + punctbo2 + "," + c2.cons);
                        if (c0.unigram && c2.unigram)
                            features.Add(c0.cons + "," + c1.consbo + "," + punctbo2 + "," + c2.cons);

                        if (bigram1) features.Add(c0.cons + "," + c1.cons + "," + punctbo2 + "," + c2.consbo);

                        if (c2.unigram) features.Add(c0.consbo + "," + c1.consbo + "," + punctbo2 + "," + c2.cons);
                        if (c1.unigram) features.Add(c0.consbo + "," + c1.cons + "," + punctbo2 + "," + c2.consbo);
                        if (c0.unigram) features.Add(c0.cons + "," + c1.consbo + "," + punctbo2 + "," + c2.consbo);

                        features.Add(c0.consbo + "," + c1.consbo + "," + punctbo2 + "," + c2.consbo);

                        if (zeroBackOff) {
                            if (bigram1) features.Add(c0.cons + "," + c1.cons + "," + punctbo2);
                            if (c1.unigram) features.Add(c0.consbo + "," + c1.cons + "," + punctbo2);
                            if (c0.unigram) features.Add(c0.cons + "," + c1.consbo + "," + punctbo2);
                            features.Add(c0.consbo + "," + c1.consbo + "," + punctbo2);
                        }
                    }
                }
            } else {
                if (punct1s != null) {
                    //cons(0),punctbo(1),cons(1),cons(2)
                    foreach (var pi1 in punct1s) {
                        var punctbo1 = PunctBo(pi1, c1.index <= 0 ? c1.index - 1 : c1.index);
                        if (trigram) features.Add(c0.cons + "," + punctbo1 + "," + c1.cons + "," + c2.cons);

                        if (bigram2) features.Add(c0.consbo + "," + punctbo1 + "," + c1.cons + "," + c2.cons);
                        if (c0.unigram && c2.unigram)
                            features.Add(c0.cons + "," + punctbo1 + "," + c1.consbo + "," + c2.cons);
                        if (bigram1) features.Add(c0.cons + "," + punctbo1 + "," + c1.cons + "," + c2.consbo);

                        if (c2.unigram) features.Add(c0.consbo + "," + punctbo1 + "," + c1.consbo + "," + c2.cons);
                        if (c1.unigram) features.Add(c0.consbo + "," + punctbo1 + "," + c1.cons + "," + c2.consbo);
                        if (c0.unigram) features.Add(c0.cons + "," + punctbo1 + "," + c1.consbo + "," + c2.consbo);

                        features.Add(c0.consbo + "," + punctbo1 + "," + c1.consbo + "," + c2.consbo);

                        //zero backoff case covered by cons(0)cons(1)
                    }
                } else {
                    //cons(0),cons(1),cons(2)
                    if (trigram) features.Add(c0.cons + "," + c1.cons + "," + c2.cons);

                    if (bigram2) features.Add(c0.consbo + "," + c1.cons + "," + c2.cons);
                    if (c0.unigram && c2.unigram) features.Add(c0.cons + "," + c1.consbo + "," + c2.cons);
                    if (bigram1) features.Add(c0.cons + "," + c1.cons + "," + c2.consbo);

                    if (c2.unigram) features.Add(c0.consbo + "," + c1.consbo + "," + c2.cons);
                    if (c1.unigram) features.Add(c0.consbo + "," + c1.cons + "," + c2.consbo);
                    if (c0.unigram) features.Add(c0.cons + "," + c1.consbo + "," + c2.consbo);

                    features.Add(c0.consbo + "," + c1.consbo + "," + c2.consbo);
                }
            }
        }

        #endregion

        #region . Surround .

        /// <summary>
        /// Generates features for nodes surrounding a completed node of the specified type.
        /// </summary>
        /// <param name="node">A surrounding node.</param>
        /// <param name="i">The index of the surrounding node with respect to the completed node.</param>
        /// <param name="type">The type of the completed node.</param>
        /// <param name="punctuation">The punctuation adjacent and between the specified surrounding node.</param>
        /// <param name="features">A list to which features are added.</param>
        protected void Surround(Parse node, int i, String type, List<Parse> punctuation, List<String> features) {
            var feat = new StringBuilder();
            feat.Append("s").Append(i).Append("=");
            if (punctuation != null) {
                foreach (var punct in punctuation) {
                    if (node != null) {
                        feat.Append(node.Head.CoveredText)
                            .Append("|")
                            .Append(type)
                            .Append("|")
                            .Append(node.Type)
                            .Append("|")
                            .Append(punct.Type);
                    } else {
                        feat.Append(type).Append("|").Append(EOS).Append("|").Append(punct.Type);
                    }
                    features.Add(feat.ToString());

                    feat = new StringBuilder();
                    feat.Append("s").Append(i).Append("*=");
                    if (node != null) {
                        feat.Append(type).Append("|").Append(node.Type).Append("|").Append(punct.Type);
                    } else {
                        feat.Append(type).Append("|").Append(EOS).Append("|").Append(punct.Type);
                    }
                    features.Add(feat.ToString());

                    feat = new StringBuilder();
                    feat.Append("s").Append(i).Append("*=");
                    feat.Append(type).Append("|").Append(punct.Type);
                    features.Add(feat.ToString());
                }
            } else {
                if (node != null) {
                    feat.Append(node.Head.CoveredText).Append("|").Append(type).Append("|").Append(node.Type);
                } else {
                    feat.Append(type).Append("|").Append(EOS);
                }
                features.Add(feat.ToString());

                feat = new StringBuilder();
                feat.Append("s").Append(i).Append("*=");
                if (node != null) {
                    feat.Append(type).Append("|").Append(node.Type);
                } else {
                    feat.Append(type).Append("|").Append(EOS);
                }
                features.Add(feat.ToString());
            }
        }

        #endregion

        #region + CheckCons .

        /// <summary>
        /// Produces features to determine whether the specified child node is part of
        /// a complete constituent of the specified type and adds those features to the
        /// specified list.
        /// </summary>
        /// <param name="child">The parse node to consider.</param>
        /// <param name="i">A string indicating the position of the child node.</param>
        /// <param name="type">The type of constituent being built.</param>
        /// <param name="features">List to add features to.</param>
        protected void CheckCons(Parse child, string i, string type, List<String> features) {
            var feat = new StringBuilder();
            feat.Append("c")
                .Append(i)
                .Append("=")
                .Append(child.Type)
                .Append("|")
                .Append(child.Head.CoveredText)
                .Append("|")
                .Append(type);
            features.Add(feat.ToString());

            feat = new StringBuilder();
            feat.Append("c").Append(i).Append("*=").Append(child.Type).Append("|").Append(type);
            features.Add(feat.ToString());
        }

        protected void CheckCons(Parse p1, Parse p2, String type, List<String> features) {
            var feat = new StringBuilder();
            feat.Append("cil=")
                .Append(type)
                .Append(",")
                .Append(p1.Type)
                .Append("|")
                .Append(p1.Head.CoveredText)
                .Append(",")
                .Append(p2.Type)
                .Append("|")
                .Append(p2.Head.CoveredText);
            features.Add(feat.ToString());

            feat = new StringBuilder();
            feat.Append("ci*l=")
                .Append(type)
                .Append(",")
                .Append(p1.Type)
                .Append(",")
                .Append(p2.Type)
                .Append("|")
                .Append(p2.Head.CoveredText);
            features.Add(feat.ToString());

            feat = new StringBuilder();
            feat.Append("cil*=")
                .Append(type)
                .Append(",")
                .Append(p1.Type)
                .Append("|")
                .Append(p1.Head.CoveredText)
                .Append(",")
                .Append(p2.Type);
            features.Add(feat.ToString());

            feat = new StringBuilder();
            feat.Append("ci*l*=").Append(type).Append(",").Append(p1.Type).Append(",").Append(p2.Type);
            features.Add(feat.ToString());
        }

        #endregion

        #region . GetFrontierNodes .

        /// <summary>
        /// Populates specified nodes array with left-most right frontier 
        /// node with a unique head. If the right frontier doesn't contain
        /// enough nodes, then nulls are placed in the array elements.
        /// </summary>
        /// <param name="rf">The current right frontier.</param>
        /// <param name="nodes">The array to be populated.</param>
        protected void GetFrontierNodes(List<Parse> rf, Parse[] nodes) {
            var leftIndex = 0;
            var prevHeadIndex = -1;

            // verificar se os nós são atualizados

            for (var fi = 0; fi < rf.Count; fi++) {
                var fn = rf[fi];
                var headIndex = fn.HeadIndex;
                if (headIndex != prevHeadIndex) {
                    nodes[leftIndex] = fn;
                    leftIndex++;
                    prevHeadIndex = headIndex;
                    if (leftIndex == nodes.Length) {
                        break;
                    }
                }
            }
            for (var ni = leftIndex; ni < nodes.Length; ni++) {
                nodes[ni] = null;
            }
        }

        #endregion
    }
}