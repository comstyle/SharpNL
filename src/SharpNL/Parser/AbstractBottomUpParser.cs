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
using System.Diagnostics;
using System.Text;
using SharpNL.Chunker;
using SharpNL.NGram;
using SharpNL.Parser.Chunking;
using SharpNL.POSTag;
using SharpNL.Utility;


using Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.Parser {
    /// <summary>
    /// Abstract class which contains code to tag and chunk parses for bottom up parsing and
    /// leaves implementation of advancing parses and completing parses to extend class.
    /// </summary>
    /// <remarks>
    /// The nodes within the returned parses are shared with other parses and therefore their parent 
    /// node references will not be consistent with their child node reference.
    /// </remarks>
    public abstract class AbstractBottomUpParser : IParser {

        #region + Constants .

        /// <summary>
        /// The label for the top node.
        /// </summary>
        public const string TOP_NODE = "TOP";

        /// <summary>
        /// The label for the top if an incomplete node.
        /// </summary>
        public const string INC_NODE = "INC";

        /// <summary>
        /// The label for a token node.
        /// </summary>
        public const string TOK_NODE = "TK";

        /// <summary>
        /// Prefix for outcomes starting a constituent.
        /// </summary>
        public const string START = "S-";

        /// <summary>
        /// Prefix for outcomes continuing a constituent.
        /// </summary>
        public const string CONT = "C-";

        /// <summary>
        /// Outcome for token which is not contained in a basal constituent.
        /// </summary>
        public const string OTHER = "O";

        /// <summary>
        /// Outcome used when a constituent is complete.
        /// </summary>
        public const string COMPLETE = "c";

        /// <summary>
        /// Outcome used when a constituent is incomplete.
        /// </summary>
        public const string INCOMPLETE = "i";

        /// <summary>
        /// The integer 0.
        /// </summary>
        public static int ZERO = 0;

        #endregion

        /// <summary>
        /// The default amount of probability mass required of advanced outcomes.
        /// </summary>
        public static readonly double DefaultAdvancePercentage = 0.95;

        /// <summary>
        /// The default beam size used if no beam size is given.
        /// </summary>
        public static readonly int DefaultBeamSize = 20;

        /// <summary>
        /// The maximum number of parses advanced from all preceding parses at each derivation step.
        /// </summary>
        protected int M;
 
        /// <summary>
        /// The maximum number of parses to advance from a single preceding parse.
        /// </summary>
        protected int K;

        /// <summary>
        ///  The minimum total probability mass of advanced outcomes.
        /// </summary>
        protected double Q;

        /// <summary>
        /// Completed parses.
        /// </summary>
        protected IHeap<Parse> completeParses;

        /// <summary>
        /// Incomplete parses which will be advanced.
        /// </summary>
        protected IHeap<Parse> odh;

        /// <summary>
        /// Incomplete parses which have been advanced.
        /// </summary>
        protected IHeap<Parse> ndh;

        /// <summary>
        /// The head rules for the parser.
        /// </summary>
        protected AbstractHeadRules headRules;

        /// <summary>
        /// The set strings which are considered punctuation for the parser.
        /// </summary>
        /// <remarks>
        /// Punctuation is not attached, but floats to the top of the parse as attachment
        /// decisions are made about its non-punctuation sister nodes.
        /// </remarks>
        protected List<string> punctSet;

        /// <summary>
        /// The pos-tagger that the parser uses.
        /// </summary>
        protected IPOSTagger tagger;

        /// <summary>
        /// The chunker that the parser uses to chunk non-recursive structures.
        /// </summary>
        protected IChunker chunker;



        /// <summary>
        /// Specifies whether a derivation string should be created during parsing.
        /// </summary>
        protected bool createDerivationString = false;

        /// <summary>
        /// Turns debug print on or off.
        /// </summary>
        protected bool debugOn = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBottomUpParser"/>.
        /// </summary>
        /// <param name="tagger">The pos-tagger that the parser uses.</param>
        /// <param name="chunker">The chunker that the parser uses to chunk non-recursive structures.</param>
        /// <param name="headRules">The head rules for the parser.</param>
        /// <param name="beamSize">Size of the beam.</param>
        /// <param name="advancePercentage">The advance percentage.</param>
        protected AbstractBottomUpParser(IPOSTagger tagger, IChunker chunker, AbstractHeadRules headRules, int beamSize,
            double advancePercentage) {
            this.tagger = tagger;
            this.chunker = chunker;
            M = beamSize;
            K = beamSize;
            Q = advancePercentage;
            ReportFailedParse = true;
            this.headRules = headRules;
            punctSet = headRules.PunctuationTags;
            odh = new ListHeap<Parse>(K);
            ndh = new ListHeap<Parse>(K);
            completeParses = new ListHeap<Parse>(K);
        }

        #region + Properties .

        #region . ReportFailedParse .
        /// <summary>
        /// Gets or sets a value indicating whether the parser should report when it was unable to 
        /// find a parse for a particular sentence.
        /// </summary>
        /// <value>if <c>true</c> then un-parsed sentences are reported; otherwise, <c>false</c>.</value>
        public bool ReportFailedParse { get; set; }
        #endregion

        #endregion

        #region . AdvanceChunks .

        /// <summary>
        /// Returns the top chunk sequences for the specified parse.
        /// </summary>
        /// <param name="p">A pos-tag assigned parse.</param>
        /// <param name="minChunkScore">A minimum score below which chunks should not be advanced.</param>
        /// <returns>The top chunk assignments to the specified parse.</returns>
        protected virtual Parse[] AdvanceChunks(Parse p, double minChunkScore) {
            // chunk
            var children = p.Children;
            var words = new string[children.Length];
            var pTags = new string[words.Length];
            //var probs = new double[words.Length];

            for (int i = 0, il = children.Length; i < il; i++) {
                words[i] = children[i].Head.CoveredText;
                pTags[i] = children[i].Type;
            }

            //System.err.println("adjusted mcs = "+(minChunkScore-p.getProb()));

            var cs = chunker.TopKSequences(words, pTags, minChunkScore - p.Probability);

            var newParses = new Parse[cs.Length];
            for (var si = 0; si < cs.Length; si++) {
                newParses[si] = (Parse) p.Clone(); //copies top level

                if (createDerivationString) 
                    newParses[si].Derivation.Append(si).Append(".");

                var tags = cs[si].Outcomes.ToArray();

                var start = -1;
                var end = 0;
                string type = null;

                for (var j = 0; j <= tags.Length; j++) {

                    if (j != tags.Length) {
                        newParses[si].AddProbability(Math.Log(cs[si].Probabilities[j]));
                    }
                    if (j != tags.Length && tags[j].StartsWith(CONT)) {
                        // if continue just update end chunking tag don't use contTypeMap
                        end = j;
                    } else {
                        //make previous constituent if it exists
                        if (type != null) {

                            var p1 = p.Children[start];
                            var p2 = p.Children[end];

                            var cons = new Parse[end - start + 1];
                            cons[0] = p1;
                            //cons[0].label="Start-"+type;
                            if (end - start != 0) {
                                cons[end - start] = p2;
                                //cons[end-start].label="Cont-"+type;
                                for (var ci = 1; ci < end - start; ci++) {
                                    cons[ci] = p.Children[ci + start];
                                    //cons[ci].label="Cont-"+type;
                                }
                            }
                            var chunk = new Parse(p1.Text, new Span(p1.Span.Start, p2.Span.End), type, 1, headRules.GetHead(cons, type)) {
                                    IsChunk = true
                                };

                            newParses[si].Insert(chunk);
                        }
                        if (j != tags.Length) {
                            // update for new constituent
                            if (tags[j].StartsWith(START)) {
                                // don't use startTypeMap these are chunk tags
                                type = tags[j].Substring(START.Length);
                                start = j;
                                end = j;
                            } else {
                                // other
                                type = null;
                            }
                        }
                    }
                }
            }
            return newParses;
        }

        #endregion

        #region . AdvanceParses .
        /// <summary>
        /// Advances the specified parse and returns the an array advanced parses whose probability accounts for
        /// more than the specified amount of probability mass.
        /// </summary>
        /// <param name="p">The parse to advance.</param>
        /// <param name="probMass">The amount of probability mass that should be accounted for by the advanced parses.</param>
        protected abstract Parse[] AdvanceParses(Parse p, double probMass);
        #endregion

        #region . AdvanceTags .

        /// <summary>
        /// Advances the parse by assigning it POS tags and returns multiple tag sequences.
        /// </summary>
        /// <param name="p">The parse to be tagged.</param>
        /// <returns>Parses with different POS-tag sequence assignments.</returns>
        /// <exception cref="System.InvalidOperationException">No tag sequence.</exception>
        protected Parse[] AdvanceTags(Parse p) {
            var children = p.Children;
            var words = new string[children.Length];
            for (int i = 0, il = children.Length; i < il; i++) {
                words[i] = children[i].CoveredText;
            }
            var ts = tagger.TopKSequences(words);

            if (ts.Length == 0)
                throw new InvalidOperationException("No tag sequence.");

            var newParses = new Parse[ts.Length];
            for (var i = 0; i < ts.Length; i++) {
                var tags = ts[i].Outcomes.ToArray();

                newParses[i] = (Parse) p.Clone(); //copies top level

                if (createDerivationString)
                    newParses[i].Derivation.Append(i).Append(".");

                for (var j = 0; j < words.Length; j++) {
                    var word = children[j];

                    //System.err.println("inserting tag "+tags[j]);

                    var prob = ts[i].Probabilities[j];
                    newParses[i].Insert(new Parse(word.Text, word.Span, tags[j], prob, j));
                    newParses[i].AddProbability(Math.Log(prob));
                    //newParses[i].show();
                }
            }
            return newParses;
        }

        #endregion

        #region . AdvanceTop .
        /// <summary>
        /// Adds the "TOP" node to the specified parse.
        /// </summary>
        /// <param name="parse">The complete parse.</param>
        protected abstract void AdvanceTop(Parse parse);
        #endregion

        #region . BuildDictionary .
        /// <summary>
        /// Creates a n-gram dictionary from the specified data stream using the specified head rule and specified cut-off.
        /// </summary>
        /// <param name="data">The data stream of parses.</param>
        /// <param name="rules">The head rules for the parses.</param>
        /// <param name="parameters">Can contain a cutoff, the minimum number of entries required for the n-gram to be saved as part of the dictionary.</param>
        /// <returns>A dictionary object.</returns>
        public static Dic BuildDictionary(IObjectStream<Parse> data, AbstractHeadRules rules, TrainingParameters parameters) {
            var cutoff = parameters.Get("dict", Parameters.Cutoff, 5);
            var dict = new NGramModel();

            Parse p;
            while ((p = data.Read()) != null) {
                p.UpdateHeads(rules);
                var pWords = p.GetTagNodes();
                var words = new string[pWords.Length];
                //add all uni-grams
                for (var wi = 0; wi < words.Length; wi++) {
                    words[wi] = pWords[wi].CoveredText;
                }

                dict.Add(new StringList(words), 1, 1);
                //add tri-grams and bi-grams for initial sequence
                var chunks = CollapsePunctuation(AbstractParserEventStream.GetInitialChunks(p), rules.PunctuationTags);
                var cWords = new string[chunks.Length];
                for (var wi = 0; wi < cWords.Length; wi++) {
                    cWords[wi] = chunks[wi].Head.CoveredText;
                }
                dict.Add(new StringList(cWords), 2, 3);

                //emulate reductions to produce additional n-grams
                var ci = 0;
                while (ci < chunks.Length) {
                    /*
                    if (chunks[ci].Parent == null) {
                        chunks[ci].Show();
                    } */
                    if (LastChild(chunks[ci], chunks[ci].Parent, rules.PunctuationTags)) {
                        //perform reduce
                        var reduceStart = ci;
                        while (reduceStart >= 0 && Equals(chunks[reduceStart].Parent, chunks[ci].Parent)) {
                            reduceStart--;
                        }
                        reduceStart++;
                        chunks = ParserEventStream.ReduceChunks(chunks, ref ci, chunks[ci].Parent);
                        ci = reduceStart;
                        if (chunks.Length != 0) {
                            var window = new string[5];
                            var wi = 0;
                            if (ci - 2 >= 0) window[wi++] = chunks[ci - 2].Head.CoveredText;
                            if (ci - 1 >= 0) window[wi++] = chunks[ci - 1].Head.CoveredText;
                            window[wi++] = chunks[ci].Head.CoveredText;
                            if (ci + 1 < chunks.Length) window[wi++] = chunks[ci + 1].Head.CoveredText;
                            if (ci + 2 < chunks.Length) window[wi++] = chunks[ci + 2].Head.CoveredText;
                            if (wi < 5) {
                                var subWindow = new string[wi];
                                for (var swi = 0; swi < wi; swi++) {
                                    subWindow[swi] = window[swi];
                                }
                                window = subWindow;
                            }
                            if (window.Length >= 3) {
                                dict.Add(new StringList(window), 2, 3);
                            } else if (window.Length == 2) {
                                dict.Add(new StringList(window), 2, 2);
                            }
                        }
                        ci = reduceStart - 1; //ci will be incremented at end of loop
                    }
                    ci++;
                }
            }
            dict.CutOff(cutoff, int.MaxValue);
            return dict.ToDictionary(true);
        }
        #endregion

        #region . CollapsePunctuation .

        /// <summary>
        /// Removes the punctuation from the specified set of chunks, adds it to the parses
        /// adjacent to the punctuation is specified, and returns a new array of parses with the punctuation.
        /// </summary>
        /// <param name="chunks">A set of parses.</param>
        /// <param name="punctSet">The set of punctuation which is to be removed.</param>
        /// <returns>An array of parses which is a subset of chunks with punctuation removed.</returns>
        public static Parse[] CollapsePunctuation(Parse[] chunks, List<string> punctSet) {
            var collapsedParses = new List<Parse>(chunks.Length);
            var lastNonPunct = -1;
            
            for (int ci = 0, cn = chunks.Length; ci < cn; ci++) {
                if (punctSet.Contains(chunks[ci].Type)) {
                    if (lastNonPunct >= 0) {
                        chunks[lastNonPunct].AddNextPunctuation(chunks[ci]);
                    }
                    int nextNonPunct;
                    for (nextNonPunct = ci + 1; nextNonPunct < cn; nextNonPunct++) {
                        if (!punctSet.Contains(chunks[nextNonPunct].Type)) {
                            break;
                        }
                    }
                    if (nextNonPunct < cn) {
                        chunks[nextNonPunct].AddPreviousPunctuation(chunks[ci]);
                    }
                } else {
                    collapsedParses.Add(chunks[ci]);
                    lastNonPunct = ci;
                }
            }
            return collapsedParses.Count == chunks.Length ? chunks : collapsedParses.ToArray();
        }

        #endregion

        #region . LastChild .
        private static bool LastChild(Parse child, Parse parent, List<string> punctSet) {
            if (parent == null) {
                return false;
            }

            var kids = CollapsePunctuation(parent.Children, punctSet);
            return (kids[kids.Length - 1].Equals(child));
        }
        #endregion

        #region . MapParseIndex .

        /// <summary>
        /// Determines the mapping between the specified index into the specified parses without punctuation to
        /// the corresponding index into the specified parses.
        /// </summary>
        /// <param name="index">An index into the parses without punctuation.</param>
        /// <param name="nonPunctParses">The parses without punctuation.</param>
        /// <param name="parses">The parses wit punctuation.</param>
        /// <returns>An index into the specified parses which corresponds to the same node the specified index into the parses with punctuation.</returns>
        protected int MapParseIndex(int index, Parse[] nonPunctParses, Parse[] parses) {
            int parseIndex = index;
            while (!parses[parseIndex].Equals(nonPunctParses[index])) {
                parseIndex++;
            }
            return parseIndex;
        }

        #endregion

        

        #region + Parse .

        /// <summary>
        /// Returns the specified number of parses or fewer for the specified tokens.
        /// </summary>
        /// <param name="tokens">A parse containing the tokens with a single parent node.</param>
        /// <param name="numParses">The number of parses desired.</param>
        /// <returns>The specified number of parses for the specified tokens.</returns>
        /// <remarks>
        /// The nodes within the returned parses are shared with other parses and therefore their 
        /// parent node references will not be consistent with their child node reference. 
        /// <see cref="SharpNL.Parser.Parse.Parent"/> can be used to make the parents consistent with a 
        /// particular parse, but subsequent calls to this property can invalidate the results of earlier
        /// calls.
        /// </remarks>
        public Parse[] Parse(Parse tokens, int numParses) {
            if (createDerivationString)
                tokens.Derivation = new StringBuilder();

            odh.Clear();
            ndh.Clear();
            completeParses.Clear();
            var derivationStage = 0; //derivation length
            var maxDerivationLength = 2*tokens.ChildCount + 3;
            odh.Add(tokens);
            Parse guess = null;
            double minComplete = 2;
            double bestComplete = -100000; //approximating -infinity/0 in ln domain

            while (odh.Size() > 0 && 
                  (completeParses.Size() < M || odh.First().Probability < minComplete) &&
                   derivationStage < maxDerivationLength) {

                ndh = new ListHeap<Parse>(K);

                var derivationRank = 0;
                // foreach derivation
                for (var pi = odh.GetEnumerator(); pi.MoveNext() && derivationRank < K; derivationRank++) {
                    var tp = pi.Current;

                    //TODO: Need to look at this for K-best parsing cases
                    /*
                     if (tp.getProb() < bestComplete) { //this parse and the ones which follow will never win, stop advancing.
                     break;
                     }
                     */

                    if (guess == null && derivationStage == 2) {
                        guess = tp;
                    }
                    if (debugOn) {
                        Console.Out.WriteLine(derivationStage + " " + derivationRank + " " + tp.Probability);
                        Console.Out.WriteLine(tp.ToString());
                        Console.Out.WriteLine();
                    }

                    Parse[] nd;

                    if (derivationStage == 0) {
                        nd = AdvanceTags(tp);

                    } else if (derivationStage == 1) {
                        nd = AdvanceChunks(tp, ndh.Size() < K ? bestComplete : ndh.Last().Probability);
                    } else {
                        // i > 1
                        nd = AdvanceParses(tp, Q);
                    }

                    if (nd != null) {
                        for (int k = 0, kl = nd.Length; k < kl; k++) {

                            if (nd[k].Complete) {
                                AdvanceTop(nd[k]);
                                if (nd[k].Probability > bestComplete) {
                                    bestComplete = nd[k].Probability;
                                }
                                if (nd[k].Probability < minComplete) {
                                    minComplete = nd[k].Probability;
                                }
                                completeParses.Add(nd[k]);
                            } else {
                                ndh.Add(nd[k]);
                            }
                        }
                    } else {
                        if (ReportFailedParse) {
                            Console.Error.WriteLine("Couldn't advance parse " + derivationStage + " stage " +
                                                    derivationRank + "!\n");
                        }
                        AdvanceTop(tp);
                        completeParses.Add(tp);
                    }
                }
                derivationStage++;
                odh = ndh;
            }
            if (completeParses.Size() == 0) {
                if (ReportFailedParse)
                    Console.Error.WriteLine("Couldn't find parse for: " + tokens);
                //Parse r = (Parse) odh.first();
                //r.show();
                //System.out.println();
                return new[] {guess};
            }
            if (numParses == 1) {
                return new[] {completeParses.First()};
            }
            var topParses = new List<Parse>();
            while (!completeParses.IsEmpty() && topParses.Count < numParses) {
                var tp = completeParses.Extract();
                topParses.Add(tp);
                //parses.remove(tp);
            }
            return topParses.ToArray();
        }

        /// <summary>
        /// Returns a parse for the specified parse of tokens.
        /// </summary>
        /// <param name="tokens">The root node of a flat parse containing only tokens.</param>
        /// <returns>A full parse of the specified tokens or the flat chunks of the tokens if a full parse could not be found.</returns>
        public Parse Parse(Parse tokens) {
            if (tokens.ChildCount > 0) {
                var p = Parse(tokens, 1)[0];
                SetParents(p);
                return p;
            }
            return tokens;
        }
        #endregion

        #region . SetParents .

        /// <summary>
        /// Assigns parent references for the specified parse so that they
        /// are consistent with the children references.
        /// </summary>
        /// <param name="p">The parse whose parent references need to be assigned.</param>
        public static void SetParents(Parse p) {
            foreach (var c in p.Children) {
                c.Parent = p;
                SetParents(c);
            }
        }

        #endregion

        protected void Debug(string message) {

            if (debugOn && Debugger.IsAttached)
                System.Diagnostics.Debug.Print(message);
            else {
                Console.Out.WriteLine(message);
            }
            
        }

    }
}