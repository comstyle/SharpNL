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
using System.Globalization;
using SharpNL.Chunker;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Parser.TreeInsert {
    /// <summary>
    /// Represents a built/attach parser.
    /// </summary>
    /// <remarks>
    /// Nodes are built when their left-most child is encountered. Subsequent children are attached as
    /// daughters. Attachment is based on node in the right-frontier of the tree.  After each attachment
    /// or building, nodes are assessed as either complete or incomplete.  Complete nodes are no longer
    /// eligible for daughter attachment. Complex modifiers which produce additional node levels of the
    /// same type are attached with sister-adjunction. Attachment can not take place higher in the 
    /// right-frontier than an incomplete node.
    /// </remarks>
    public class Parser : AbstractBottomUpParser {
        /// <summary>
        /// Outcome used when a constituent needs an no additional parent node/building.
        /// </summary>
        public const string DONE = "d";

        /// <summary>
        /// Outcome used when a node should be attached as a sister to another node.
        /// </summary>
        public const string ATTACH_SISTER = "s";

        /// <summary>
        /// Outcome used when a node should be attached as a daughter to another node.
        /// </summary>
        public const string ATTACH_DAUGHTER = "d";

        /// <summary>
        /// Outcome used when a node should not be attached to another node.
        /// </summary>
        public const string NON_ATTACH = "n";

        /// <summary>
        /// Label used to distinguish build nodes from non-built nodes.
        /// </summary>
        public static readonly string BUILT = "built";

        public static bool checkComplete = false;

        private readonly double[] aProbs;

        private readonly AttachContextGenerator attachContextGenerator;
        private readonly IMaxentModel attachModel;
        private readonly int[] attachments;

        private readonly double[] bProbs;
        private readonly BuildContextGenerator buildContextGenerator;
        private readonly IMaxentModel buildModel;
        private readonly CheckContextGenerator checkContextGenerator;
        private readonly IMaxentModel checkModel;
        private readonly int completeIndex;
        private double[] cProbs;

        private readonly int daughterAttachIndex;
        private readonly int doneIndex;
        //private int nonAttachIndex;
        private readonly int sisterAttachIndex;

        public Parser(ParserModel model, int beamSize, double advancePercentage) :
            this(model.BuildModel, model.AttachModel, model.CheckModel,
                new POSTaggerME(model.ParserTaggerModel),
                new ChunkerME(model.ParserChunkerModel),
                model.HeadRules,
                beamSize, advancePercentage) {}


        public Parser(ParserModel model) : this(model, DefaultBeamSize, DefaultAdvancePercentage) {}


        private Parser(IMaxentModel buildModel, IMaxentModel attachModel, IMaxentModel checkModel, IPOSTagger tagger,
            IChunker chunker, AbstractHeadRules headRules, int beamSize, double advancePercentage)
            : base(tagger, chunker, headRules, beamSize, advancePercentage) {
            this.buildModel = buildModel;
            this.attachModel = attachModel;
            this.checkModel = checkModel;

            buildContextGenerator = new BuildContextGenerator();
            attachContextGenerator = new AttachContextGenerator(punctSet);
            checkContextGenerator = new CheckContextGenerator(punctSet);

            bProbs = new double[buildModel.GetNumOutcomes()];
            aProbs = new double[attachModel.GetNumOutcomes()];
            cProbs = new double[checkModel.GetNumOutcomes()];

            doneIndex = buildModel.GetIndex(DONE);
            sisterAttachIndex = attachModel.GetIndex(ATTACH_SISTER);
            daughterAttachIndex = attachModel.GetIndex(ATTACH_DAUGHTER);
            // nonAttachIndex = attachModel.GetIndex(NON_ATTACH);
            attachments = new[] {daughterAttachIndex, sisterAttachIndex};
            completeIndex = checkModel.GetIndex(COMPLETE);
        }


        #region . GetRightFrontier .

        /// <summary>
        /// Returns the right frontier of the specified parse tree with nodes ordered from deepest to shallowest.
        /// </summary>
        /// <param name="root">The root of the parse tree.</param>
        /// <param name="punctSet">The punctuation set.</param>
        /// <returns>The right frontier of the specified parse tree.</returns>
        public static List<Parse> GetRightFrontier(Parse root, List<string> punctSet) {
            var rf = new LinkedList<Parse>();
            Parse top;
            if (root.Type == TOP_NODE ||
                root.Type == INC_NODE) {
                top = CollapsePunctuation(root.Children, punctSet)[0];
            } else {
                top = root;
            }
            while (!top.IsPosTag) {
                rf.AddFirst(top);
                var kids = top.Children;
                top = kids[kids.Length - 1];
            }
            return new List<Parse>(rf);
        }

        #endregion

        #region . SetComplete .
        private void SetComplete(Parse p) {
            if (!IsBuilt(p)) {
                p.Label = COMPLETE;
            } else {
                p.Label = BUILT + "." + COMPLETE;
            }
        }
        #endregion

        #region . SetIncomplete .
        private void SetIncomplete(Parse p) {
            if (!IsBuilt(p)) {
                p.Label = INCOMPLETE;
            } else {
                p.Label = BUILT + "." + INCOMPLETE;
            }
        }
        #endregion

        #region . IsBuilt .
        private static bool IsBuilt(Parse p) {
            return p.Label != null && p.Label.StartsWith(BUILT);
        }
        #endregion

        #region . IsComplete .
        private static bool IsComplete(Parse p) {
            return p.Label != null && p.Label.EndsWith(COMPLETE);
        }
        #endregion

        #region . AdvanceChunks .
        protected override Parse[] AdvanceChunks(Parse p, double minChunkScore) {
            var parses = base.AdvanceChunks(p, minChunkScore);
            foreach (var parse in parses) {
                var chunks = parse.Children;
                foreach (var chunk in chunks) {
                    SetComplete(chunk);
                }
            }
            return parses;
        }
        #endregion

        #region . AdvanceParses .

        protected override Parse[] AdvanceParses(Parse p, double probMass) {
            var q = 1 - probMass;
            /** The index of the node which will be labeled in this iteration of advancing the parse. */
            int advanceNodeIndex;
            /** The node which will be labeled in this iteration of advancing the parse. */
            Parse advanceNode = null;
            var originalChildren = p.Children;
            var children = CollapsePunctuation(originalChildren, punctSet);
            var numNodes = children.Length;
            if (numNodes == 0) {
                return null;
            }
            if (numNodes == 1) {
                //put sentence initial and final punct in top node
                if (children[0].IsPosTag) {
                    return null;
                }
                p.ExpandTopNode(children[0]);
                return new[] {p};
            }
            //determines which node needs to advanced.
            for (advanceNodeIndex = 0; advanceNodeIndex < numNodes; advanceNodeIndex++) {
                advanceNode = children[advanceNodeIndex];
                if (!IsBuilt(advanceNode)) {
                    break;
                }
            }

            if (advanceNode == null)
                throw new InvalidOperationException("advanceNode is null.");

            var originalZeroIndex = MapParseIndex(0, children, originalChildren);
            var originalAdvanceIndex = MapParseIndex(advanceNodeIndex, children, originalChildren);
            var newParsesList = new List<Parse>();
            //call build model
            buildModel.Eval(buildContextGenerator.GetContext(children, advanceNodeIndex), bProbs);
            var doneProb = bProbs[doneIndex];

            Debug("adi=" + advanceNodeIndex + " " + advanceNode.Type + "." + advanceNode.Label + " " + advanceNode + " choose build=" + (1 - doneProb) + " attach=" + doneProb);

            if (1 - doneProb > q) {
                double bprobSum = 0;
                while (bprobSum < probMass) {
                    /** The largest un advanced labeling. */
                    var max = 0;
                    for (var pi = 1; pi < bProbs.Length; pi++) {
                        //for each build outcome
                        if (bProbs[pi] > bProbs[max]) {
                            max = pi;
                        }
                    }
                    if (bProbs[max].Equals(0d)) {
                        break;
                    }
                    var bprob = bProbs[max];
                    bProbs[max] = 0; //zero out so new max can be found
                    bprobSum += bprob;
                    var tag = buildModel.GetOutcome(max);
                    if (!tag.Equals(DONE)) {
                        var newParse1 = (Parse) p.Clone();
                        var newNode = new Parse(p.Text, advanceNode.Span, tag, bprob, advanceNode.Head);
                        newParse1.Insert(newNode);
                        newParse1.AddProbability(Math.Log(bprob));
                        newParsesList.Add(newParse1);
                        if (checkComplete) {
                            cProbs =
                                checkModel.Eval(checkContextGenerator.GetContext(newNode, children, advanceNodeIndex,
                                    false));

                            Debug("building " + tag + " " + bprob + " c=" + cProbs[completeIndex]);

                            if (cProbs[completeIndex] > probMass) {
                                //just complete advances
                                SetComplete(newNode);
                                newParse1.AddProbability(Math.Log(cProbs[completeIndex]));

                                Debug("Only advancing complete node");
                            } else if (1 - cProbs[completeIndex] > probMass) {
                                //just incomplete advances
                                SetIncomplete(newNode);
                                newParse1.AddProbability(Math.Log(1 - cProbs[completeIndex]));
                                Debug("Only advancing incomplete node");
                            } else {
                                //both complete and incomplete advance
                                Debug("Advancing both complete and incomplete nodes");
                                SetComplete(newNode);
                                newParse1.AddProbability(Math.Log(cProbs[completeIndex]));

                                var newParse2 = (Parse) p.Clone();
                                var newNode2 = new Parse(p.Text, advanceNode.Span, tag, bprob, advanceNode.Head);
                                newParse2.Insert(newNode2);
                                newParse2.AddProbability(Math.Log(bprob));
                                newParsesList.Add(newParse2);
                                newParse2.AddProbability(Math.Log(1 - cProbs[completeIndex]));
                                SetIncomplete(newNode2); //set incomplete for non-clone
                            }
                        } else {
                            Debug("building " + tag + " " + bprob);
                        }
                    }
                }
            }
            //advance attaches
            if (doneProb > q) {
                var newParse1 = (Parse) p.Clone(); //clone parse
                //mark nodes as built
                if (checkComplete) {
                    if (IsComplete(advanceNode)) {
                        newParse1.SetChild(originalAdvanceIndex, BUILT + "." + COMPLETE);
                        //replace constituent being labeled to create new derivation
                    } else {
                        newParse1.SetChild(originalAdvanceIndex, BUILT + "." + INCOMPLETE);
                        //replace constituent being labeled to create new derivation
                    }
                } else {
                    newParse1.SetChild(originalAdvanceIndex, BUILT);
                    //replace constituent being labeled to create new derivation
                }
                newParse1.AddProbability(Math.Log(doneProb));
                if (advanceNodeIndex == 0) {
                    //no attach if first node.
                    newParsesList.Add(newParse1);
                } else {
                    var rf = GetRightFrontier(p, punctSet);
                    for (int fi = 0, fs = rf.Count; fi < fs; fi++) {
                        var fn = rf[fi];
                        attachModel.Eval(attachContextGenerator.GetContext(children, advanceNodeIndex, rf, fi), aProbs);
                        if (debugOn) {
                            //List cs = java.util.Arrays.asList(attachContextGenerator.getContext(children, advanceNodeIndex,rf,fi,punctSet));
                            Debug("Frontier node(" + fi + "): " + fn.Type + "." + fn.Label + " " + fn + " <- " +
                                  advanceNode.Type + " " + advanceNode + " d=" + aProbs[daughterAttachIndex] + " s=" +
                                  aProbs[sisterAttachIndex] + " ");

                        }
                        foreach (int attachment in attachments) {
                            var prob = aProbs[attachment];
                            //should we try an attach if p > threshold and
                            // if !checkComplete then prevent daughter attaching to chunk
                            // if checkComplete then prevent daughter attaching to complete node or
                            //    sister attaching to an incomplete node
                            if (prob > q && (
                                (!checkComplete && (attachment != daughterAttachIndex || !IsComplete(fn)))
                                ||
                                (checkComplete &&
                                 ((attachment == daughterAttachIndex && !IsComplete(fn)) ||
                                  (attachment == sisterAttachIndex && IsComplete(fn)))))) {
                                var newParse2 = newParse1.CloneRoot(fn, originalZeroIndex);
                                var newKids = CollapsePunctuation(newParse2.Children, punctSet);
                                //remove node from top level since were going to attach it (including punct)
                                for (var ri = originalZeroIndex + 1; ri <= originalAdvanceIndex; ri++) {
                                    //System.out.println(at"-removing "+(originalZeroIndex+1)+" "+newParse2.getChildren()[originalZeroIndex+1]);
                                    newParse2.Remove(originalZeroIndex + 1);
                                }
                                var crf = GetRightFrontier(newParse2, punctSet);
                                Parse updatedNode;
                                if (attachment == daughterAttachIndex) {
                                    //attach daughter
                                    updatedNode = crf[fi];
                                    updatedNode.Add(advanceNode, headRules);
                                } else {
                                    //attach sister
                                    Parse psite;
                                    if (fi + 1 < crf.Count) {
                                        psite = crf[fi + 1];
                                        updatedNode = psite.AdJoin(advanceNode, headRules);
                                    } else {
                                        psite = newParse2;
                                        updatedNode = psite.AdJoinRoot(advanceNode, headRules, originalZeroIndex);
                                        newKids[0] = updatedNode;
                                    }
                                }
                                //update spans affected by attachment
                                for (var ni = fi + 1; ni < crf.Count; ni++) {
                                    var node = crf[ni];
                                    node.UpdateSpan();
                                }
                                //if (debugOn) {System.out.print(ai+"-result: ");newParse2.show();System.out.println();}
                                newParse2.AddProbability(Math.Log(prob));
                                newParsesList.Add(newParse2);
                                if (checkComplete) {
                                    cProbs =
                                        checkModel.Eval(checkContextGenerator.GetContext(updatedNode, newKids,
                                            advanceNodeIndex, true));
                                    if (cProbs[completeIndex] > probMass) {
                                        SetComplete(updatedNode);
                                        newParse2.AddProbability(Math.Log(cProbs[completeIndex]));

                                        Debug("Only advancing complete node");
                                    } else if (1 - cProbs[completeIndex] > probMass) {
                                        SetIncomplete(updatedNode);
                                        newParse2.AddProbability(Math.Log(1 - cProbs[completeIndex]));
                                        Debug("Only advancing incomplete node");
                                    } else {
                                        SetComplete(updatedNode);
                                        var newParse3 = newParse2.CloneRoot(updatedNode, originalZeroIndex);
                                        newParse3.AddProbability(Math.Log(cProbs[completeIndex]));
                                        newParsesList.Add(newParse3);
                                        SetIncomplete(updatedNode);
                                        newParse2.AddProbability(Math.Log(1 - cProbs[completeIndex]));
                                        Debug("Advancing both complete and incomplete nodes; c=" + cProbs[completeIndex]);
                                    }
                                }
                            } else {
                                Debug("Skipping " + fn.Type + "." + fn.Label + " " + fn + " daughter=" +
                                      (attachment == daughterAttachIndex) + " complete=" + IsComplete(fn) +
                                      " prob=" + prob);
                            }
                        }
                        if (checkComplete && !IsComplete(fn)) {
                            Debug("Stopping at incomplete node(" + fi + "): " + fn.Type + "." + fn.Label + " " + fn);
                            break;
                        }
                    }
                }
            }
            return newParsesList.ToArray();
        }

        #endregion

        #region . AdvanceTop .
        protected override void AdvanceTop(Parse p) {
            p.Type = TOP_NODE;
        }
        #endregion

        #region + Train .

        public static ParserModel Train(string languageCode,
            IObjectStream<Parse> parseSamples, AbstractHeadRules rules, TrainingParameters mlParams) {
            var manifestInfoEntries = new Dictionary<string, string>();

            System.Diagnostics.Debug.Print("Building dictionary");

            var dictionary = BuildDictionary(parseSamples, rules, mlParams);

            parseSamples.Reset();

            // tag
            var posModel = POSTaggerME.Train(
                languageCode,
                new PosSampleStream(parseSamples),
                mlParams.GetNamespace("tagger"),
                new POSTaggerFactory());

            parseSamples.Reset();

            // chunk
            var chunkModel = ChunkerME.Train(
                languageCode,
                new ChunkSampleStream(parseSamples),
                mlParams.GetNamespace("chunker"),
                new ChunkerFactory());

            parseSamples.Reset();

            // build
            System.Diagnostics.Debug.Print("Training builder");
            var bes = new ParserEventStream(parseSamples, rules, ParserEventTypeEnum.Build, dictionary);
            var buildReportMap = new Dictionary<string, string>();
            var buildTrainer = TrainerFactory.GetEventTrainer(mlParams.GetNamespace("build"), buildReportMap);

            var buildModel = buildTrainer.Train(bes);

            Chunking.Parser.MergeReportIntoManifest(manifestInfoEntries, buildReportMap, "build");

            parseSamples.Reset();

            // check
            System.Diagnostics.Debug.Print("Training checker");
            var kes = new ParserEventStream(parseSamples, rules, ParserEventTypeEnum.Check);
            var checkReportMap = new Dictionary<string, string>();

            var checkTrainer = TrainerFactory.GetEventTrainer(mlParams.GetNamespace("check"), checkReportMap);

            var checkModel = checkTrainer.Train(kes);

            Chunking.Parser.MergeReportIntoManifest(manifestInfoEntries, checkReportMap, "check");

            parseSamples.Reset();

            // attach
            System.Diagnostics.Debug.Print("Training attacher");
            var attachEvents = new ParserEventStream(parseSamples, rules, ParserEventTypeEnum.Attach);
            var attachReportMap = new Dictionary<string, string>();

            var attachTrainer = TrainerFactory.GetEventTrainer(mlParams.GetNamespace("attach"), attachReportMap);

            var attachModel = attachTrainer.Train(attachEvents);

            Chunking.Parser.MergeReportIntoManifest(manifestInfoEntries, attachReportMap, "attach");

            return new ParserModel(
                languageCode,
                buildModel,
                checkModel,
                attachModel,
                posModel,
                chunkModel,
                rules,
                ParserType.TreeInsert,
                manifestInfoEntries);
        }

        public static ParserModel Train(
            string languageCode,
            IObjectStream<Parse> parseSamples, 
            AbstractHeadRules rules, 
            int iterations,
            int cut) {
            var param = new TrainingParameters();

            param.Set("dict", Parameters.Cutoff, cut.ToString(CultureInfo.InvariantCulture));


            param.Set("tagger", Parameters.Cutoff, cut.ToString(CultureInfo.InvariantCulture));
            param.Set("tagger", Parameters.Iterations, iterations.ToString(CultureInfo.InvariantCulture));


            param.Set("chunker", Parameters.Cutoff, cut.ToString(CultureInfo.InvariantCulture));
            param.Set("chunker", Parameters.Iterations, iterations.ToString(CultureInfo.InvariantCulture));

            param.Set("check", Parameters.Cutoff, cut.ToString(CultureInfo.InvariantCulture));
            param.Set("check", Parameters.Iterations, iterations.ToString(CultureInfo.InvariantCulture));

            param.Set("build", Parameters.Cutoff, cut.ToString(CultureInfo.InvariantCulture));
            param.Set("build", Parameters.Iterations, iterations.ToString(CultureInfo.InvariantCulture));

            return Train(languageCode, parseSamples, rules, param);
        }

        #endregion

    }
}