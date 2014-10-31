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
using SharpNL.ML.Model;

namespace SharpNL.ML.Perceptron {
    /// <summary>
    /// Trains models for sequences using the perceptron algorithm.
    /// </summary>
    /// <remarks>
    /// Each outcome is represented as a binary perceptron classifier. 
    /// This supports standard (integer) weighting as well average weighting. 
    /// Sequence information is used in a simplified was to that described in:
    /// Discriminative Training Methods for Hidden Markov Models: Theory and Experiments
    /// with the Perceptron Algorithm. Michael Collins, EMNLP 2002.
    /// Specifically only updates are applied to tokens which were incorrectly tagged by a sequence tagger
    /// rather than to all feature across the sequence which differ from the training sequence.
    /// </remarks>
    public class SimplePerceptronSequenceTrainer : AbstractEventModelSequenceTrainer {
        private const int EVENT = 2;
        private const int ITER = 1;
        private const int VALUE = 0;
        public static readonly string PERCEPTRON_SEQUENCE_VALUE = "PERCEPTRON_SEQUENCE";
        private int[] allOutcomesPattern;
        private MutableContext[] averageParams;

        private int iterations;
        /** Number of events in the event set. */
        private int numEvents;

        /** Number of predicates. */
        private int numOutcomes;
        private int numPreds;
        private int numSequences;

        /** List of outcomes for each event i, in context[i]. */
        private Dictionary<string, int> oMap;
        private string[] outcomeLabels;
        private int[] outcomeList;

        /** Stores the estimated parameter value of each predicate during iteration. */
        private MutableContext[] param;
        private IndexHashTable<string> pMap;

        private string[] predLabels;
        private ISequenceStream sequenceStream;
        private int[][][] updates;
        private bool useAverage;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePerceptronSequenceTrainer"/> class
        /// without a evaluation monitor.
        /// </summary>
        public SimplePerceptronSequenceTrainer() : base(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePerceptronSequenceTrainer" /> 
        /// class with the specified evaluation monitor.
        /// </summary>
        /// <param name="monitor">The evaluation monitor. This argument can be <c>null</c>.</param>
        public SimplePerceptronSequenceTrainer(Monitor monitor) : base(monitor) { }

        #endregion

        #region + Properties .

        #endregion

        #region . DoTrain .

        protected override IMaxentModel DoTrain(ISequenceStream events) {
            var useAvg = GetBoolParam("UseAverage", true);

            return TrainModel(Iterations, events, Cutoff, useAvg);
        }

        #endregion

        #region . FindParameters .

        private void FindParameters() {
            Display("Performing " + iterations + " iterations.\n");
            for (var i = 1; i <= iterations; i++) {
                if (i < 10)
                    Display("  " + i + ":  ");
                else if (i < 100)
                    Display(" " + i + ":  ");
                else
                    Display(i + ":  ");

                NextIteration(i);
            }
            TrainingStats(useAverage ? averageParams : param);
        }

        #endregion

        #region . NextIteration .

        private void NextIteration(int iteration) {
            iteration--; //move to 0-based index
            var numCorrect = 0;
            var oei = 0;
            var si = 0;
            var featureCounts = new Dictionary<int, Dictionary<string, float>>();
            for (var oi = 0; oi < numOutcomes; oi++) {
                featureCounts[oi] = new Dictionary<string, float>();
            }
            var model = new PerceptronModel(
                Array.ConvertAll(param, input => (Context) input),
                pMap,
                outcomeLabels);

            sequenceStream.Reset();

            Sequence sequence;
            while ((sequence = sequenceStream.Read()) != null) {
                var taggerEvents = sequenceStream.UpdateContext(sequence, model);
                var update = false;
                for (var ei = 0; ei < sequence.Events.Length; ei++,oei++) {
                    if (!taggerEvents[ei].Outcome.Equals(sequence.Events[ei].Outcome)) {
                        update = true;
                        //break;
                    } else {
                        numCorrect++;
                    }
                }
                if (update) {
                    for (var oi = 0; oi < numOutcomes; oi++) {
                        featureCounts[oi].Clear();
                    }

                    //training feature count computation
                    for (var ei = 0; ei < sequence.Events.Length; ei++,oei++) {
                        var contextStrings = sequence.Events[ei].Context;
                        var values = sequence.Events[ei].Values;
                        var oi = oMap[sequence.Events[ei].Outcome];
                        for (var ci = 0; ci < contextStrings.Length; ci++) {
                            float value = 1;
                            if (values != null) {
                                value = values[ci];
                            }


                            if (featureCounts[oi].ContainsKey(contextStrings[ci])) {
                                featureCounts[oi][contextStrings[ci]] += value;
                            } else {
                                featureCounts[oi][contextStrings[ci]] = value;
                            }
                        }
                    }

                    //evaluation feature count computation
                    //System.err.print("test: ");for (int ei=0;ei<taggerEvents.length;ei++) {System.err.print(" "+taggerEvents[ei].getOutcome());} System.err.println();
                    foreach (var taggerEvent in taggerEvents) {
                        var contextStrings = taggerEvent.Context;
                        var values = taggerEvent.Values;
                        var oi = oMap[taggerEvent.Outcome];
                        for (var ci = 0; ci < contextStrings.Length; ci++) {
                            float value = 1;
                            if (values != null) {
                                value = values[ci];
                            }

                            float c;
                            if (featureCounts[oi].ContainsKey(contextStrings[ci])) {
                                c = featureCounts[oi][contextStrings[ci]] - value;
                            } else {
                                c = -1*value;
                            }

                            if (c.Equals(0f)) {
                                featureCounts[oi].Remove(contextStrings[ci]);
                            }
                        }
                    }
                    for (var oi = 0; oi < numOutcomes; oi++) {
                        foreach (var feature in featureCounts[oi].Keys) {
                            var pi = pMap[feature];
                            if (pi != -1) {
                                param[pi].UpdateParameter(oi, featureCounts[oi][feature]);
                                if (useAverage) {
                                    if (updates[pi][oi][VALUE] != 0) {
                                        averageParams[pi].UpdateParameter(oi,
                                            updates[pi][oi][VALUE]*
                                            (numSequences*(iteration - updates[pi][oi][ITER]) +
                                             (si - updates[pi][oi][EVENT])));
                                    }
                                    updates[pi][oi][VALUE] = (int) param[pi].Parameters[oi];
                                    updates[pi][oi][ITER] = iteration;
                                    updates[pi][oi][EVENT] = si;
                                }
                            }
                        }
                    }
                    model = new PerceptronModel(
                        Array.ConvertAll(param, input => (Context) input),
                        pMap,
                        outcomeLabels);
                }
                si++;
            }
            //finish average computation
            var totIterations = (double) iterations*si;
            if (useAverage && iteration == iterations - 1) {
                for (var pi = 0; pi < numPreds; pi++) {
                    var predParams = averageParams[pi].Parameters;
                    for (var oi = 0; oi < numOutcomes; oi++) {
                        if (updates[pi][oi][VALUE] != 0) {
                            predParams[oi] += updates[pi][oi][VALUE]*
                                              (numSequences*(iterations - updates[pi][oi][ITER]) -
                                               updates[pi][oi][EVENT]);
                        }

                        if (predParams[oi].Equals(0d))
                            continue;

                        predParams[oi] /= totIterations;
                        averageParams[pi].SetParameter(oi, predParams[oi]);
                    }
                }
            }
            Display(string.Format(". ({0}/{1}) {2}", numCorrect, numEvents, (double) numCorrect/numEvents));
        }

        #endregion

        #region . TrainingStats .
        private void TrainingStats(MutableContext[] paramStats) {
            var numCorrect = 0;
            var oei = 0;

            sequenceStream.Reset();

            Sequence sequence;
            while ((sequence = sequenceStream.Read()) != null) {
                var taggerEvents = sequenceStream.UpdateContext(sequence,
                    new PerceptronModel(
                        Array.ConvertAll(paramStats, input => (Context) input),
                        pMap,
                        outcomeLabels));
                for (var ei = 0; ei < taggerEvents.Length; ei++,oei++) {
                    var max = oMap[taggerEvents[ei].Outcome];
                    if (max == outcomeList[oei]) {
                        numCorrect ++;
                    }
                }
            }
            Display(string.Format(". ({0}/{1}) {2}", numCorrect, numEvents, (double) numCorrect/numEvents));
        }
        #endregion

        #region . TrainModel .
        
        public AbstractModel TrainModel(int trainIterations, ISequenceStream trainStream, int cutoff,
            bool trainUseAverage) {
            iterations = trainIterations;
            useAverage = trainUseAverage;
            sequenceStream = trainStream;

            var di = new OnePassDataIndexer(new SequenceStreamEventStream(trainStream), cutoff, false);

            trainStream.Reset();

            numSequences = 0;

            while (trainStream.Read() != null) {
                numSequences++;
            }

            outcomeList = di.GetOutcomeList();
            predLabels = di.GetPredLabels();

            pMap = new IndexHashTable<string>(predLabels, 0.7d);

            // Incorporation indexed data for training...


            numEvents = di.GetNumEvents();

            outcomeLabels = di.GetOutcomeLabels();
            oMap = new Dictionary<string, int>();
            for (var i = 0; i < outcomeLabels.Length; i++) {
                oMap.Add(outcomeLabels[i], i);
            }

            outcomeList = di.GetOutcomeList();
            numPreds = predLabels.Length;
            numOutcomes = outcomeLabels.Length;

            if (trainUseAverage) {
                updates = new int[numPreds][][];
                for (var i = 0; i < numPreds; i++) {
                    updates[i] = new int[numOutcomes][];
                    for (var j = 0; j < numOutcomes; j++) {
                        updates[i][j] = new int[3];
                    }
                }
            }

            // done.
            Display("done.\n");

            Display("\tNumber of Event Tokens: " + numEvents);
            Display("\t    Number of Outcomes: " + numOutcomes);
            Display("\t  Number of Predicates: " + numPreds);

            param = new MutableContext[numPreds];
            if (trainUseAverage)
                averageParams = new MutableContext[numPreds];

            allOutcomesPattern = new int[numOutcomes];
            for (var i = 0; i < numOutcomes; i++) {
                allOutcomesPattern[i] = i;
            }

            for (var pi = 0; pi < numPreds; pi++) {
                param[pi] = new MutableContext(allOutcomesPattern, new double[numOutcomes]);
                if (trainUseAverage)
                    averageParams[pi] = new MutableContext(allOutcomesPattern, new double[numOutcomes]);

                for (var aoi = 0; aoi < numOutcomes; aoi++) {
                    param[pi].SetParameter(aoi, 0.0d);
                    if (trainUseAverage)
                        averageParams[pi].SetParameter(aoi, 0.0d);
                }
            }

            Display("Computing model parameters...");
            FindParameters();
            Display("...done.");

            /*************** Create and return the model ******************/
            var updatedPredLabels = predLabels;

            if (trainUseAverage) {
                return new PerceptronModel(
                    Array.ConvertAll(averageParams, input => (Context) input),
                    updatedPredLabels,
                    outcomeLabels);
            }

            return new PerceptronModel(
                Array.ConvertAll(param, input => (Context) input),
                updatedPredLabels,
                outcomeLabels);
        }

        #endregion

        #region . IsValid .

        protected override bool IsValid() {
            if (!base.IsValid()) {
                return false;
            }

            return Algorithm == null || Algorithm == PERCEPTRON_SEQUENCE_VALUE;
        }

        #endregion

    }
}