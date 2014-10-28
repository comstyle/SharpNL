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

namespace SharpNL.Parser.Chunking {
    /// <summary>
    /// Class for a shift reduce style parser based on Adwait Ratnaparkhi's 1998 thesis.
    /// </summary>
    public class Parser : AbstractBottomUpParser {
        private const string TOP_START = START + TOP_NODE;
        private readonly double[] bProbs;
        private readonly BuildContextGenerator buildContextGenerator;
        private readonly IMaxentModel buildModel;
        private readonly CheckContextGenerator checkContextGenerator;
        private readonly IMaxentModel checkModel;

        private readonly int completeIndex;
        private readonly Dictionary<string, string> contTypeMap;
        private readonly double[] cProbs;
        private readonly int incompleteIndex;
        private readonly Dictionary<string, string> startTypeMap;
        private readonly int topStartIndex;

        #region + Constructors .

        public Parser(ParserModel model) : this(model, DefaultBeamSize, DefaultAdvancePercentage) {}

        public Parser(ParserModel model, int beamSize, double advancePercentage)
            : this(
                model.BuildModel,
                model.CheckModel, 
                new POSTaggerME(model.ParserTaggerModel, 10, 0),
                new ChunkerME(
                    model.ParserChunkerModel, 
                    ChunkerME.DEFAULT_BEAM_SIZE, 
                    new ParserChunkerSequenceValidator(model.ParserChunkerModel),
                    new ChunkContextGenerator(ChunkerME.DEFAULT_BEAM_SIZE)), 
                model.HeadRules, 
                beamSize, 
                advancePercentage) {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBottomUpParser"/>.
        /// </summary>
        /// <param name="tagger">The pos-tagger that the parser uses.</param>
        /// <param name="chunker">The chunker that the parser uses to chunk non-recursive structures.</param>
        /// <param name="headRules">The head rules for the parser.</param>
        /// <param name="beamSize">Size of the beam.</param>
        /// <param name="advancePercentage">The advance percentage.</param>
        public Parser(IPOSTagger tagger, IChunker chunker, AbstractHeadRules headRules, int beamSize,
            double advancePercentage)
            : base(tagger, chunker, headRules, beamSize, advancePercentage) {}

        private Parser(IMaxentModel buildModel, IMaxentModel checkModel, IPOSTagger tagger, IChunker chunker,
            AbstractHeadRules headRules, int beamSize, double advancePercentage) :
                base(tagger, chunker, headRules, beamSize, advancePercentage) {
            this.buildModel = buildModel;
            this.checkModel = checkModel;
            bProbs = new double[buildModel.GetNumOutcomes()];
            cProbs = new double[checkModel.GetNumOutcomes()];
            buildContextGenerator = new BuildContextGenerator();
            checkContextGenerator = new CheckContextGenerator();
            startTypeMap = new Dictionary<string, string>();
            contTypeMap = new Dictionary<string, string>();
            for (int boi = 0, bon = buildModel.GetNumOutcomes(); boi < bon; boi++) {
                var outcome = buildModel.GetOutcome(boi);
                if (outcome.StartsWith(START)) {
                    startTypeMap[outcome] = outcome.Substring(START.Length);
                } else if (outcome.StartsWith(CONT)) {
                    contTypeMap[outcome] = outcome.Substring(CONT.Length);
                }
            }
            topStartIndex = buildModel.GetIndex(TOP_START);
            completeIndex = checkModel.GetIndex(COMPLETE);
            incompleteIndex = checkModel.GetIndex(INCOMPLETE);
        }

        #endregion

        #region . AdvanceParses .

        /// <summary>
        /// Advances the specified parse and returns the an array advanced parses whose probability accounts for
        /// more than the specified amount of probability mass.
        /// </summary>
        /// <param name="p">The parse to advance.</param>
        /// <param name="probMass">The amount of probability mass that should be accounted for by the advanced parses.</param>
        protected override Parse[] AdvanceParses(Parse p, double probMass) {
            var q = 1 - probMass;
            /** The closest previous node which has been labeled as a start node. */
            Parse lastStartNode = null;
            /** The index of the closest previous node which has been labeled as a start node. */
            var lastStartIndex = -1;
            /** The type of the closest previous node which has been labeled as a start node. */
            string lastStartType = null;
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
            //determines which node needs to be labeled and prior labels.
            for (advanceNodeIndex = 0; advanceNodeIndex < numNodes; advanceNodeIndex++) {
                advanceNode = children[advanceNodeIndex];
                if (advanceNode.Label == null) {
                    break;
                }
                if (startTypeMap.ContainsKey(advanceNode.Label)) {
                    lastStartType = startTypeMap[advanceNode.Label];
                    lastStartNode = advanceNode;
                    lastStartIndex = advanceNodeIndex;
                    //System.err.println("lastStart "+i+" "+lastStart.label+" "+lastStart.prob);
                }
            }
            var originalAdvanceIndex = MapParseIndex(advanceNodeIndex, children, originalChildren);
            var newParsesList = new List<Parse>(buildModel.GetNumOutcomes());

            //call build
            buildModel.Eval(buildContextGenerator.GetContext(children, advanceNodeIndex), bProbs);
            var bProbSum = 0d;
            while (bProbSum < probMass) {
                // The largest un-advanced labeling.
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
                var bProb = bProbs[max];
                bProbs[max] = 0; //zero out so new max can be found
                bProbSum += bProb;
                var tag = buildModel.GetOutcome(max);
                //System.out.println("trying "+tag+" "+bprobSum+" lst="+lst);
                if (max == topStartIndex) {
                    // can't have top until complete
                    continue;
                }
                //System.err.println(i+" "+tag+" "+bprob);
                if (startTypeMap.ContainsKey(tag)) {
                    //update last start
                    lastStartIndex = advanceNodeIndex;
                    lastStartNode = advanceNode;
                    lastStartType = startTypeMap[tag];
                } else if (contTypeMap.ContainsKey(tag)) {
                    if (lastStartNode == null || !lastStartType.Equals(contTypeMap[tag])) {
                        continue; //Cont must match previous start or continue
                    }
                }
                var newParse1 = (Parse) p.Clone(); //clone parse

                if (createDerivationString) 
                    newParse1.Derivation.Append(max).Append("-");

                newParse1.SetChild(originalAdvanceIndex, tag); //replace constituent being labeled to create new derivation
                newParse1.AddProbability(Math.Log(bProb));

                //check
                //String[] context = checkContextGenerator.getContext(newParse1.getChildren(), lastStartType, lastStartIndex, advanceNodeIndex);
                checkModel.Eval(
                    checkContextGenerator.GetContext(
                        CollapsePunctuation(newParse1.Children, punctSet), 
                        lastStartType,
                        lastStartIndex, 
                        advanceNodeIndex),
                    cProbs);

                //System.out.println("check "+lastStartType+" "+cprobs[completeIndex]+" "+cprobs[incompleteIndex]+" "+tag+" "+java.util.Arrays.asList(context));
                
                if (cProbs[completeIndex] > q) {
                    //make sure a reduce is likely
                    var newParse2 = (Parse) newParse1.Clone();

                    if (createDerivationString) 
                        newParse2.Derivation.Append(1).Append(".");

                    newParse2.AddProbability(Math.Log(cProbs[completeIndex]));
                    var cons = new Parse[advanceNodeIndex - lastStartIndex + 1];
                    var flat = true;

                    if (lastStartNode == null)
                        throw new InvalidOperationException("lastStartNode is null.");

                    //first
                    cons[0] = lastStartNode;
                    flat &= cons[0].IsPosTag;
                    //last
                    cons[advanceNodeIndex - lastStartIndex] = advanceNode;
                    flat &= cons[advanceNodeIndex - lastStartIndex].IsPosTag;
                    //middle
                    for (var ci = 1; ci < advanceNodeIndex - lastStartIndex; ci++) {
                        cons[ci] = children[ci + lastStartIndex];
                        flat &= cons[ci].IsPosTag;
                    }
                    if (!flat) {
                        //flat chunks are done by chunker
                        if (lastStartIndex == 0 && advanceNodeIndex == numNodes - 1) {
                            //check for top node to include end and beginning punctuation
                            //System.err.println("ParserME.advanceParses: reducing entire span: "+new Span(lastStartNode.getSpan().getStart(), advanceNode.getSpan().getEnd())+" "+lastStartType+" "+java.util.Arrays.asList(children));
                            newParse2.Insert(new Parse(p.Text, p.Span, lastStartType, cProbs[1],
                                headRules.GetHead(cons, lastStartType)));
                        } else {
                            newParse2.Insert(new Parse(p.Text, new Span(lastStartNode.Span.Start, advanceNode.Span.End),
                                lastStartType, cProbs[1], headRules.GetHead(cons, lastStartType)));
                        }
                        newParsesList.Add(newParse2);
                    }
                }
                if (cProbs[incompleteIndex] > q) {
                    //make sure a shift is likely
                    if (createDerivationString) 
                        newParse1.Derivation.Append(0).Append(".");

                    if (advanceNodeIndex != numNodes - 1) {
                        //can't shift last element
                        newParse1.AddProbability(Math.Log(cProbs[incompleteIndex]));
                        newParsesList.Add(newParse1);
                    }
                }
            }
            return newParsesList.ToArray();
        }

        #endregion

        #region . AdvanceTop .

        /// <summary>
        /// Adds the "TOP" node to the specified parse.
        /// </summary>
        /// <param name="parse">The complete parse.</param>
        protected override void AdvanceTop(Parse parse) {
            buildModel.Eval(buildContextGenerator.GetContext(parse.Children, 0), bProbs);
            parse.AddProbability(Math.Log(bProbs[topStartIndex]));

            checkModel.Eval(checkContextGenerator.GetContext(parse.Children, TOP_NODE, 0, 0), cProbs);
            parse.AddProbability(Math.Log(cProbs[completeIndex]));

            parse.Type = TOP_NODE;
        }

        #endregion

        #region . MergeReportIntoManifest .

        internal static void MergeReportIntoManifest(
            Dictionary<string, string> manifest,
            Dictionary<string, string> report,
            string ns) {
            foreach (var entry in report) {
                manifest[ns + "." + entry.Key] = entry.Value;
            }
        }

        #endregion

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

        public static ParserModel Train(
            string languageCode, 
            IObjectStream<Parse> parseSamples, 
            AbstractHeadRules rules,
            TrainingParameters mlParams) {
            //System.err.println("Building dictionary");

            var dict = BuildDictionary(parseSamples, rules, mlParams);

            parseSamples.Reset();

            var manifestInfoEntries = new Dictionary<string, string>();

            // build
            //System.err.println("Training builder");
            var bes = new ParserEventStream(parseSamples, rules, ParserEventTypeEnum.Build, dict);
            var buildReportMap = new Dictionary<string, string>();
            var buildTrainer = TrainerFactory.GetEventTrainer(mlParams.GetNamespace("build"), buildReportMap);


            var buildModel = buildTrainer.Train(bes);

            MergeReportIntoManifest(manifestInfoEntries, buildReportMap, "build");

            parseSamples.Reset();

            // tag
            var posModel = POSTaggerME.Train(languageCode, new PosSampleStream(parseSamples),
                mlParams.GetNamespace("tagger"), new POSTaggerFactory());

            parseSamples.Reset();

            // chunk
            var chunkModel = ChunkerME.Train(languageCode, new ChunkSampleStream(parseSamples),
                mlParams.GetNamespace("chunker"), new ChunkerFactory());

            parseSamples.Reset();

            // check
            //System.err.println("Training checker");
            var kes = new ParserEventStream(parseSamples, rules, ParserEventTypeEnum.Check);
            var checkReportMap = new Dictionary<string, string>();
            var checkTrainer = TrainerFactory.GetEventTrainer(mlParams.GetNamespace("check"), checkReportMap);

            var checkModel = checkTrainer.Train(kes);
            MergeReportIntoManifest(manifestInfoEntries, checkReportMap, "check");

            return new ParserModel(languageCode, buildModel, checkModel, posModel, chunkModel, rules,
                ParserType.Chunking, manifestInfoEntries);
        }
    }
}