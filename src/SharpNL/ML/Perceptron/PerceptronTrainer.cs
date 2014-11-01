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
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.ML.Perceptron {
    /// <summary>
    /// Trains models using the perceptron algorithm.
    /// </summary>
    /// <remarks>
    /// Each outcome is represented as a binary perceptron classifier. 
    /// This supports standard (integer) weighting as well average weighting as described in:
    /// 
    /// Discriminative Training Methods for Hidden Markov Models: Theory and Experiments
    /// with the Perceptron Algorithm. Michael Collins, EMNLP 2002.
    /// </remarks>
    public class PerceptronTrainer : AbstractEventTrainer {

        public const string PerceptronValue = "PERCEPTRON";

        /// <summary>
        /// The default tolerance of the perception trainer.
        /// </summary>
        public const double DefaultTolerance = .00001;

        #region + Fields .

        /// <summary>
        /// Records the array of predicates seen in each event.
        /// </summary>
        private int[][] contexts;

        /// <summary>
        /// Number of events in the event set.
        /// </summary>
        private int numEvents;

        /// <summary>
        /// Number of outcomes.
        /// </summary>
        private int numOutcomes;

        /// <summary>
        /// Number of predicates.
        /// </summary>
        private int numPreds;

        /// <summary>
        /// Records the num of times an event has been seen for each event i, in context[i].
        /// </summary>
        private int[] numTimesEventsSeen;

        /// <summary>
        /// Number of unique events which occurred in the event set.
        /// </summary>
        private int numUniqueEvents;

        /// <summary>
        /// Stores the String names of the outcomes.
        /// </summary>
        /// <remarks>The GIS only tracks outcomes as ints, and so this array is needed to save the model to disk and thereby allow users to know what the outcome was in human understandable terms.</remarks>
        private string[] outcomeLabels;

        /// <summary>
        /// List of outcomes for each event i, in context[i].
        /// </summary>
        private int[] outcomeList;

        /// <summary>
        /// Stores the String names of the predicates.
        /// </summary>
        /// <remarks>The GIS only tracks predicates as ints, and so this array is needed to save the model to disk and thereby allow users to know what the outcome was in human understandable terms.</remarks>
        private string[] predLabels;

        /// <summary>
        /// The value associates with each context. If null then context values are assumes to be 1.
        /// </summary>
        private float[][] values;

        #endregion

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="PerceptronTrainer"/> class.
        /// </summary>
        public PerceptronTrainer() : base(null, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerceptronTrainer"/> class.
        /// </summary>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        public PerceptronTrainer(Monitor monitor) : base(monitor, false) { }
        #endregion

        #region + Properties .

        #region . StepSizeDecrease .

        private double stepSizeDecrease;

        /// <summary>
        /// Enables and sets step size decrease. 
        /// The step size is decreased every iteration by the specified value.
        /// </summary>
        /// <value>The step size decrease in percent.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">Decrease value must be between 0 and 100.</exception>
        public double StepSizeDecrease {
            get { return stepSizeDecrease; }
            set {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException("value", value, @"Decrease value must be between 0 and 100.");

                stepSizeDecrease = value;
            }
        }

        #endregion

        #region . Tolerance .

        private double tolerance = DefaultTolerance;

        /// <summary>
        /// Gets or sets the tolerance.
        /// If the change in training set accuracy is less than this, stop iterating.
        /// </summary>
        /// <value>The tolerance.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">Tolerance must be a positive number.</exception>
        public double Tolerance {
            get { return tolerance; }
            set {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", value, @"Tolerance must be a positive number.");
                    
                tolerance = value;
            }
        }

        #endregion

        #region . UseSkippedAveraging .

        /// <summary>
        /// Gets or sets a value indicating whether skipped averaging is enabled.
        /// </summary>
        /// <value><c>true</c> if skipped averaging is enabled; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If we are doing averaging, and the current iteration is one 
        /// of the first 20 or it is a perfect square, then updated the
        /// summed parameters.
        /// 
        /// The reason we don't take all of them is that the parameters change
        /// less toward the end of training, so they drown out the contributions
        /// of the more volatile early iterations. The use of perfect
        /// squares allows us to sample from successively farther apart iterations.
        /// </remarks>
        public bool UseSkippedAveraging { get; set; }

        #endregion

        #endregion

        #region . DoTrain .
        protected override IMaxentModel DoTrain(IDataIndexer indexer) {
            if (!IsValid())
                throw new InvalidOperationException("trainParams are not valid!");

            var useAverage = GetBoolParam(Parameters.UseAverage, true);

            UseSkippedAveraging = GetBoolParam(Parameters.UseSkippedAveraging, false);

            // overwrite otherwise it might not work
            if (UseSkippedAveraging)
                useAverage = true;

            StepSizeDecrease = GetDoubleParam(Parameters.StepSizeDecrease, 0d);

            Tolerance = GetDoubleParam(Parameters.Tolerance, DefaultTolerance);

            return TrainModel(Iterations, indexer, Cutoff, useAverage);
        }
        #endregion

        #region . IsValid .

        protected override bool IsValid() {
            if (!base.IsValid())
                return false;

            return Algorithm == "PERCEPTRON";
        }

        #endregion

        #region . IsPerfectSquare .
        private static bool IsPerfectSquare(int n) {
            var root = (int)Math.Sqrt(n);
            return root * root == n;
        }
        #endregion

        #region . MaxIndex .
        private static int MaxIndex(double[] indexValues) {
            var max = 0;
            for (var i = 1; i < indexValues.Length; i++)
                if (indexValues[i] > indexValues[max])
                    max = i;

            return max;
        }
        #endregion

        #region . TrainModel .
        /// <summary>
        /// Train a model using the Perceptron algorithm.
        /// </summary>
        /// <param name="iterations">The number of Perceptron iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="IO.PerceptronModelWriter"/> object.</returns>
        public AbstractModel TrainModel(int iterations, IDataIndexer indexer, int cutoff) {
            return TrainModel(iterations, indexer, cutoff, true);
        }

        /// <summary>
        /// Train a model using the Perceptron algorithm.
        /// </summary>
        /// <param name="iterations">The number of Perceptron iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <param name="useAverage"></param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="IO.PerceptronModelWriter"/> object.</returns>
        public AbstractModel TrainModel(int iterations, IDataIndexer indexer, int cutoff, bool useAverage) {
            Display("Incorporating indexed data for training...");

            info.Append("Trained using Perceptron algorithm.\n\n");

            // Executes the data indexer
            indexer.Execute();

            contexts = indexer.GetContexts();
            values = indexer.Values;
            numTimesEventsSeen = indexer.GetNumTimesEventsSeen();
            numEvents = indexer.GetNumEvents();
            numUniqueEvents = contexts.Length;

            outcomeLabels = indexer.GetOutcomeLabels();
            outcomeList = indexer.GetOutcomeList();

            predLabels = indexer.GetPredLabels();
            numPreds = predLabels.Length;
            numOutcomes = outcomeLabels.Length;

            Display("\ndone.\n");

            info.Append("Number of Event Tokens: {0}\n" +
                        "    Number of Outcomes: {1}\n" +
                        "  Number of Predicates: {2}\n", numEvents, numOutcomes, numPreds);

            Display("\tNumber of Event Tokens: " + numUniqueEvents);
            Display("\t    Number of Outcomes: " + numOutcomes);
            Display("\t  Number of Predicates: " + numPreds);

            Display("Computing model parameters.");

            var finalParameters = FindParameters(iterations, useAverage);

            Display("\ndone.\n");

            // ReSharper disable once CoVariantArrayConversion
            return new PerceptronModel(finalParameters, predLabels, outcomeLabels) {
                info = info
            };
        }

        #endregion

        #region . TrainingStats .

        private void TrainingStats(EvalParameters evalParams) {
            var numCorrect = 0;

            for (var ei = 0; ei < numUniqueEvents; ei++) {
                for (var ni = 0; ni < numTimesEventsSeen[ei]; ni++) {
                    var modelDistribution = new double[numOutcomes];

                    PerceptronModel.Eval(
                        contexts[ei], 
                        values != null ? values[ei] : null, 
                        modelDistribution, 
                        evalParams,
                        false);

                    var max = MaxIndex(modelDistribution);
                    if (max == outcomeList[ei])
                        numCorrect++;
                }
            }
            var trainingAccuracy = (double) numCorrect/numEvents;

            info.Append("        Correct Events: {0}\n" +
                        "          Total Events: {1}\n" +
                        "              Accuracy: {2}\n", numCorrect, numEvents, trainingAccuracy);

            Display("\nPerceptron training complete:\n");
            Display("\t Correct Events : " + numCorrect);
            Display("\t   Total Events : " + numEvents);
            Display("\t       Accuracy : " + trainingAccuracy);            
        }


        #endregion

        #region . FindParameters .

        private MutableContext[] FindParameters(int iterations, bool useAverage) {

            info.Append("  Number of Iterations: {0}\n", iterations);

            Display("\nPerforming " + iterations + " iterations.\n");

            var allOutcomesPattern = new int[numOutcomes];
            for (var oi = 0; oi < numOutcomes; oi++)
                allOutcomesPattern[oi] = oi;

            /** Stores the estimated parameter value of each predicate during iteration. */
            var param = new MutableContext[numPreds];
            for (var pi = 0; pi < numPreds; pi++) {
                param[pi] = new MutableContext(allOutcomesPattern, new double[numOutcomes]);
                for (var aoi = 0; aoi < numOutcomes; aoi++)
                    param[pi].SetParameter(aoi, 0.0);
            }

            // ReSharper disable once CoVariantArrayConversion
            var evalParams = new EvalParameters(param, numOutcomes);

            // Stores the sum of parameter values of each predicate over many iterations.
            var summedParams = new MutableContext[numPreds];
            if (useAverage) {
                for (var pi = 0; pi < numPreds; pi++) {
                    summedParams[pi] = new MutableContext(allOutcomesPattern, new double[numOutcomes]);
                    for (var aoi = 0; aoi < numOutcomes; aoi++)
                        summedParams[pi].SetParameter(aoi, 0.0);
                }
            }

            // Keep track of the previous three accuracies. The difference of
            // the mean of these and the current training set accuracy is used
            // with tolerance to decide whether to stop.
            var prevAccuracy1 = 0.0;
            var prevAccuracy2 = 0.0;
            var prevAccuracy3 = 0.0;

            // A counter for the denominator for averaging.
            var numTimesSummed = 0;

            double stepSize = 1;
            for (var i = 1; i <= iterations; i++) {
                // Decrease the step size by a small amount.
                if (stepSizeDecrease > 0)
                    stepSize *= 1 - stepSizeDecrease;


                if (Monitor != null && Monitor.Token.CanBeCanceled)
                    Monitor.Token.ThrowIfCancellationRequested();

                var numCorrect = 0;

                for (var ei = 0; ei < numUniqueEvents; ei++) {
                    var targetOutcome = outcomeList[ei];

                    for (var ni = 0; ni < numTimesEventsSeen[ei]; ni++) {
                        // Compute the model's prediction according to the current parameters.
                        var modelDistribution = new double[numOutcomes];

                        PerceptronModel.Eval(
                            contexts[ei], 
                            values != null ? values[ei] : null, 
                            modelDistribution,
                            evalParams, false);

                        var maxOutcome = MaxIndex(modelDistribution);

                        // If the predicted outcome is different from the target
                        // outcome, do the standard update: boost the parameters
                        // associated with the target and reduce those associated
                        // with the incorrect predicted outcome.
                        if (maxOutcome != targetOutcome) {
                            for (var ci = 0; ci < contexts[ei].Length; ci++) {
                                var pi = contexts[ei][ci];
                                if (values == null) {
                                    param[pi].UpdateParameter(targetOutcome, stepSize);
                                    param[pi].UpdateParameter(maxOutcome, -stepSize);
                                } else {
                                    param[pi].UpdateParameter(targetOutcome, stepSize*values[ei][ci]);
                                    param[pi].UpdateParameter(maxOutcome, -stepSize*values[ei][ci]);
                                }
                            }
                        }

                        // Update the counts for accuracy.
                        if (maxOutcome == targetOutcome)
                            numCorrect++;
                    }
                }

                // Calculate the training accuracy and display.
                var trainingAccuracy = (double) numCorrect/numEvents;
                Display(string.Format("{0,-4} {1} of {2} - {3}",
                    i,
                    numCorrect,
                    numEvents,
                    trainingAccuracy));
                    

                // TODO: Make averaging configurable !!!

                bool doAveraging;

                if (useAverage && UseSkippedAveraging && (i < 20 || IsPerfectSquare(i))) {
                    doAveraging = true;
                } else if (useAverage) {
                    doAveraging = true;
                } else {
                    doAveraging = false;
                }

                if (doAveraging) {
                    numTimesSummed++;
                    for (var pi = 0; pi < numPreds; pi++)
                        for (var aoi = 0; aoi < numOutcomes; aoi++)
                            summedParams[pi].UpdateParameter(aoi, param[pi].Parameters[aoi]);
                }

                // If the tolerance is greater than the difference between the
                // current training accuracy and all of the previous three
                // training accuracies, stop training.
                if (Math.Abs(prevAccuracy1 - trainingAccuracy) < tolerance &&
                    Math.Abs(prevAccuracy2 - trainingAccuracy) < tolerance && 
                    Math.Abs(prevAccuracy3 - trainingAccuracy) < tolerance) {

                    Display("Stopping: change in training set accuracy less than " + tolerance + "\n");
                    break;
                }

                // Update the previous training accuracies.
                prevAccuracy1 = prevAccuracy2;
                prevAccuracy2 = prevAccuracy3;
                prevAccuracy3 = trainingAccuracy;
            }

            // Output the final training stats.
            TrainingStats(evalParams);

            // Create averaged parameters
            if (useAverage) {
                for (var pi = 0; pi < numPreds; pi++)
                    for (var aoi = 0; aoi < numOutcomes; aoi++)
                        summedParams[pi].SetParameter(aoi, summedParams[pi].Parameters[aoi]/numTimesSummed);

                return summedParams;
            }
            return param;
        }

        #endregion
    }
}