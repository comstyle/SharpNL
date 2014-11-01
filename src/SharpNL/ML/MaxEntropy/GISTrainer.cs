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
using System.Threading;
using System.Threading.Tasks;
using SharpNL.ML.MaxEntropy.IO;
using SharpNL.Utility;

namespace SharpNL.ML.MaxEntropy {
    using Model;

    /// <summary>
    /// An implementation of Generalized Iterative Scaling.
    /// </summary>
    /// <remarks>
    /// The reference paper for this implementation was Adwait Ratnaparkhi's tech report at the
    /// University of Pennsylvania's Institute for Research in Cognitive Science, and is available at 
    /// <see href="ftp://ftp.cis.upenn.edu/pub/ircs/tr/97-08.ps.Z" />.
    /// 
    /// The slack parameter used in the above implementation has been removed by default from the computation and a method
    /// for updating with Gaussian smoothing has been  added per Investigating GIS and Smoothing for Maximum Entropy Taggers,
    /// Clark and Curran (2002).
    /// <see href="http://acl.ldc.upenn.edu/E/E03/E03-1071.pdf" />
    /// 
    /// A prior can be used to train models which converge to the distribution which minimizes the
    /// relative entropy between the distribution specified by the empirical constraints of the training
    /// data and the specified prior.  By default, the uniform distribution is used as the prior.
    /// </remarks>
    public class GISTrainer {
        private const double LLThreshold = 0.0001;

        /// <summary>
        /// Specified whether parameter updates should prefer a distribution of parameters which is gaussian.
        /// </summary>
        private bool useGaussianSmoothing;

        private double sigma = 2.0;

        /// <summary>
        /// Number of unique events which occurred in the event set.
        /// </summary>
        private int numUniqueEvents;

        /// <summary>
        /// Number of predicates.
        /// </summary>
        private int numPreds;

        /// <summary>
        /// The number outcomes.
        /// </summary>
        private int numOutcomes;

        /// <summary>
        /// Records the array of predicates seen in each event.
        /// </summary>
        private int[][] contexts;
        
        /// <summary>
        /// The value associated with each context. If null then context values are assumes to be 1.
        /// </summary>
        private float[][] values;

        /// <summary>
        /// List of outcomes for each event i, in context[i].
        /// </summary>
        private int[] outcomeList;

        /// <summary>
        /// Records the num of times an event has been seen for each event i, in context[i].
        /// </summary>
        private int[] numTimesEventsSeen;

        /// <summary>
        /// Records the last iteration accuracy.
        /// </summary>
        private double lastAccuracy;

        /// <summary>
        /// The number of times a predicate occurred in the training data.
        /// </summary>
        private int[] predicateCounts;

        private int cutoff;
        
        /// <summary>
        /// Stores the String names of the outcomes. The GIS only tracks outcomes as 
        /// ints, and so this array is needed to save the model to disk and thereby
        /// allow users to know what the outcome was in human understandable terms.
        /// </summary>
        private string[] outcomeLabels;

        /// <summary>
        /// Stores the String names of the predicates. The GIS only tracks predicates
        /// as ints, and so this array is needed to save the model to disk and thereby
        /// allow users to know what the outcome was in human understandable terms.
        /// </summary>
        private string[] predLabels;

        /// <summary>
        /// Stores the observed expected values of the features based on training data.
        /// </summary>
        private MutableContext[] observedExpects;

        /// <summary>
        /// Stores the estimated parameter value of each predicate during iteration.
        /// </summary>
        private MutableContext[] param;

        /// <summary>
        /// Stores the expected values of the features based on the current models.
        /// </summary>
        private MutableContext[][] modelExpects;

        /// <summary>
        /// This is the prior distribution that the model uses for training.
        /// </summary>
        private IPrior prior;

        /// <summary>
        /// Initial probability for all outcomes.
        /// </summary>
        private EvalParameters evalParams;

        /// <summary>
        /// The evaluation monitor
        /// </summary>
        private readonly Monitor monitor;

        /// <summary>
        /// The training information.
        /// </summary>
        private readonly TrainingInfo info;

        private int[] threadNumEvents;
        private int[] threadNumCorrect;
        private double[] threadLoglikelihood;

        #region  + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="GISTrainer"/> class.
        /// </summary>
        public GISTrainer() {
            Smoothing = false;
            SmoothingObservation = 0.1;

            info = new TrainingInfo();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GISTrainer"/> class with a specified evaluation monitor.
        /// </summary>
        /// <param name="monitor">The evaluation monitor.</param>
        public GISTrainer(Monitor monitor)
            : this() {
            this.monitor = monitor;
        }
        #endregion

        #region + Properties .

        #region . TrainingInfo .
        /// <summary>
        /// Gets the training information.
        /// </summary>
        /// <value>The training information.</value>
        public TrainingInfo TrainingInfo {
            get { return info; }
        }
        #endregion

        #region . Smoothing .
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GISTrainer"/> will use smoothing while training the model.
        /// This can improve model accuracy, though training will potentially take longer and use more memory. Model size will also be larger.
        /// </summary>
        /// <value><c>true</c> if smoothing; otherwise, <c>false</c>.</value>
        public bool Smoothing { get; set; }
        #endregion

        #region . SmoothingObservation .
        /// <summary>
        /// Gets or sets the "number" of times we want the trainer to imagine it saw a feature that it actually didn't see.
        /// </summary>
        /// <remarks>Default value is 0.1.</remarks>
        public double SmoothingObservation { get; set; }
        #endregion

        #endregion

        #region . Display .
        private void Display(string message) {

            if (monitor != null) 
                monitor.OnMessage(message);              

#if DEBUG
            System.Diagnostics.Debug.Print(message);
#endif
        }
        #endregion

        #region . NextIteration .
        /// <summary>
        /// Compute one iteration of GIS and return the log-likelihood.
        /// </summary>
        /// <param name="iteration">The iteration number.</param>
        /// <param name="correctionConstant">The correction constant.</param>
        /// <remarks>Compute contribution of p(a|b_i) for each feature and the new correction parameter.</remarks>
        private double NextIteration(int iteration, double correctionConstant) {
            double loglikelihood = 0.0;
            int numEvents = 0;
            int numCorrect = 0;

            int numberOfThreads = modelExpects.Length;

            int taskSize = numUniqueEvents / numberOfThreads;
            int leftOver = numUniqueEvents % numberOfThreads;

            var tasks = new Task[numberOfThreads];

            /* 
            Knuppe 2014-10-01: 
             * The following three lines of code cost me four days, of work (I'm not even kidding) :(
             * i had forgotten to clear the memory over the interactions, so the newer interactions were using 
             * the old junk in memory!!!
             * 
             * Personal note: Keep calm and stop being a NooB! #lol
             * 
             */
            threadNumEvents = new int[numberOfThreads];
            threadNumCorrect = new int[numberOfThreads];
            threadLoglikelihood = new double[numberOfThreads];

            for (int i = 0; i < numberOfThreads; i++) {
                int id = i;
                if (id != numberOfThreads - 1) {
                    tasks[id] = new Task(() => Compute(id, id * taskSize, taskSize));
                } else {
                    tasks[id] = new Task(() => Compute(id, id * taskSize, taskSize + leftOver));
                }
                tasks[id].Start();
            }

            Task.WaitAll(tasks);

            for (int i = 0; i < numberOfThreads; i++) {
                numEvents += threadNumEvents[i];
                numCorrect += threadNumCorrect[i];
                loglikelihood += threadLoglikelihood[i];
            }

            // merge the results of the two computations
            for (int pi = 0; pi < numPreds; pi++) {
                for (int aoi = 0; aoi < param[pi].Outcomes.Length; aoi++) {
                    for (int i = 1; i < modelExpects.Length; i++) {
                        modelExpects[0][pi].UpdateParameter(aoi, modelExpects[i][pi].Parameters[aoi]);
                    }
                }
            }

            // compute the new parameter values
            for (var pi = 0; pi < numPreds; pi++) {
                var observed = observedExpects[pi].Parameters;
                var model = modelExpects[0][pi].Parameters;
                var activeOutcomes = param[pi].Outcomes;
                for (var aoi = 0; aoi < activeOutcomes.Length; aoi++) {
                    if (useGaussianSmoothing) {
                        param[pi].UpdateParameter(aoi, GaussianUpdate(pi, aoi, correctionConstant));
                    } else {
                        if (model[aoi].Equals(0d) && monitor != null) {
                            monitor.OnError("Model expects == 0 for " + predLabels[pi] + " " + outcomeLabels[aoi]);
                        }
                        //params[pi].updateParameter(aoi,(Math.log(observed[aoi]) - Math.log(model[aoi])));
                        param[pi].UpdateParameter(aoi,
                            ((Math.Log(observed[aoi]) - Math.Log(model[aoi]))/correctionConstant));
                    }

                    foreach (var modelExpect in modelExpects) {
                        modelExpect[pi].SetParameter(aoi, 0.0); // re-initialize to 0.0's
                    }

                }
            }

            lastAccuracy = ((double) numCorrect/numEvents);

            Display(string.Format("{0,-5} loglikelihood = {1,-20:0.00000000000} {2,20:0.00000000000}", 
                iteration, 
                loglikelihood,
                lastAccuracy));

            return loglikelihood;
        }
        #endregion

        #region . UseGaussianSigma .
        /// <summary>
        /// Sets this trainer to use smoothing while training the model.
        /// This can improve model accuracy, though training will potentially take
        /// longer and use more memory. Model size will also be larger.
        /// </summary>
        /// <param name="sigmaValue">The sigma value.</param>
        public void UseGaussianSigma(double sigmaValue) {
            useGaussianSmoothing = true;
            sigma = sigmaValue;
        }
        #endregion

        #region . FindParameters .
        /// <summary>
        /// Estimate and return the model parameters.
        /// </summary>
        /// <param name="iterations">The iterations.</param>
        /// <param name="correctionConstant">The correction constant.</param>
        private void FindParameters(int iterations, double correctionConstant) {
            var prevLL = 0d;

            Display("Performing " + iterations + " iterations.\n");

            info.Append("  Number of Iterations: {0}\n", iterations);

            for (var i = 1; i <= iterations; i++) {
                var currLL = NextIteration(i, correctionConstant);
                if (i > 1) {
                    if (prevLL > currLL) {
                        if (monitor != null)
                            monitor.OnError("Model Diverging: loglikelihood decreased");

                        break;
                    }
                    if (currLL - prevLL < LLThreshold) {
                        break;
                    }
                }
                prevLL = currLL;
            }
            
            info.Append("  Final Log-likelihood: {0}  {1}\n", prevLL, lastAccuracy);

            // kill a bunch of these big objects now that we don't need them
            observedExpects = null;
            modelExpects = null;
            numTimesEventsSeen = null;
            contexts = null;
        }
        #endregion

        #region . GaussianUpdate .

        /// <remarks>
        /// Modeled on implementation in Zhang Le's maxent kit.
        /// </remarks>
        private double GaussianUpdate(int predicate, int oid, double correctionConstant) {
            var p = param[predicate].Parameters[oid];
            var x0 = 0.0d;
            var modelValue = modelExpects[0][predicate].Parameters[oid];
            var observedValue = observedExpects[predicate].Parameters[oid];
            for (var i = 0; i < 50; i++) {
                var tmp = modelValue * Math.Exp(correctionConstant*x0);
                var f = tmp + (p + x0)/sigma - observedValue;
                var fp = tmp * correctionConstant + 1 / sigma;

                if (fp.Equals(0d))
                    break;
                
                var x = x0 - f/fp;
                if (Math.Abs(x - x0) < 0.000001) {
                    x0 = x;
                    break;
                }
                x0 = x;
            }
            return x0;
        }

        #endregion

        #region . Compute .

        private void Compute(int threadIndex, int startIndex, int length) {
            var modelDistribution = new double[numOutcomes];

            // to avoid unnecessary comparisons during the loop 
            var token = monitor != null
                ? monitor.Token
                : new CancellationToken();

            for (var ei = startIndex; ei < startIndex + length; ei++) {

                token.ThrowIfCancellationRequested();

                if (values != null) {
                    prior.LogPrior(modelDistribution, contexts[ei], values[ei]);
                    GISModel.Eval(contexts[ei], values[ei], modelDistribution, evalParams);
                } else {
                    prior.LogPrior(modelDistribution, contexts[ei]);
                    GISModel.Eval(contexts[ei], modelDistribution, evalParams);
                }

                for (var j = 0; j < contexts[ei].Length; j++) {
                    var pi = contexts[ei][j];

                    if (predicateCounts[pi] < cutoff) 
                        continue;

                    var activeOutcomes = modelExpects[threadIndex][pi].Outcomes;
                    for (var aoi = 0; aoi < activeOutcomes.Length; aoi++) {
                        var oi = activeOutcomes[aoi];

                        if (values != null && values[ei] != null) {
                            modelExpects[threadIndex][pi].UpdateParameter(aoi, modelDistribution[oi] * values[ei][j] * numTimesEventsSeen[ei]);
                        } else {
                            modelExpects[threadIndex][pi].UpdateParameter(aoi, modelDistribution[oi] * numTimesEventsSeen[ei]);
                        }

                    }
                }
                
                threadLoglikelihood[threadIndex] += Math.Log(modelDistribution[outcomeList[ei]]) * numTimesEventsSeen[ei];
                threadNumEvents[threadIndex] += numTimesEventsSeen[ei];
                
#if !DEBUG
                // only when we have a monitor to report...
                if (monitor != null) {
#endif
                    var max = 0;
                    for (var oi = 0; oi < numOutcomes; oi++) {
                        if (modelDistribution[oi] > modelDistribution[max]) {
                            max = oi;
                        }
                    }
                    if (max == outcomeList[ei]) {
                        threadNumCorrect[threadIndex] += numTimesEventsSeen[ei];
                    }
#if !DEBUG
                }
#endif

            }
        }


        #endregion

        #region . TrainModel .

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="di">The data indexer used to compress events in memory.</param>
        /// <param name="modelCutoff">The number of times a feature must occur to be included.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using an <see cref="GISModelWriter"/> object.</returns>
        public GISModel TrainModel(int iterations, IDataIndexer di, int modelCutoff) {
            return TrainModel(iterations, di, new UniformPrior(), modelCutoff, 1);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="di">The data indexer used to compress events in memory.</param>
        /// <param name="modelPrior">The prior distribution used to train this model.</param>
        /// <param name="modelCutoff">The number of times a feature must occur to be included.</param>
        /// <param name="threads">The number of threads used to train this model.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using an <see cref="GISModelWriter"/> object.</returns>
        public GISModel TrainModel(int iterations, IDataIndexer di, IPrior modelPrior, int modelCutoff, int threads) {

            if (threads <= 0)
                throw new ArgumentOutOfRangeException("threads", threads, @"Threads must be at least one or greater.");

            modelExpects = new MutableContext[threads][];

            info.Append("Trained using GIS algorithm.\n\n");

            // Executes the data indexer
            di.Execute();

            // Incorporate all of the needed info.
            Display("Incorporating indexed data for training...");
            contexts = di.GetContexts();
            values = di.Values;
            cutoff = modelCutoff;
            predicateCounts = di.GetPredCounts();
            numTimesEventsSeen = di.GetNumTimesEventsSeen();
            numUniqueEvents = contexts.Length;
            prior = modelPrior;

            // determine the correction constant and its inverse
            double correctionConstant = 0;
            for (int ci = 0; ci < contexts.Length; ci++) {
                if (values == null || values[ci] == null) {
                    if (contexts[ci].Length > correctionConstant) {
                        correctionConstant = contexts[ci].Length;
                    }
                } else {
                    var cl = values[ci][0];
                    for (var vi = 1; vi < values[ci].Length; vi++) {
                        cl += values[ci][vi];
                    }

                    if (cl > correctionConstant) {
                        correctionConstant = cl;
                    }
                }
            }

            Display("done.");

            outcomeLabels = di.GetOutcomeLabels();
            outcomeList = di.GetOutcomeList();
            numOutcomes = outcomeLabels.Length;

            predLabels = di.GetPredLabels();
            prior.SetLabels(outcomeLabels, predLabels);
            numPreds = predLabels.Length;

            info.Append("Number of Event Tokens: {0}\n", numUniqueEvents);
            info.Append("    Number of Outcomes: {0}\n", numOutcomes);
            info.Append("  Number of Predicates: {0}\n", numPreds);

            Display("\tNumber of Event Tokens: " + numUniqueEvents);
            Display("\t    Number of Outcomes: " + numOutcomes);
            Display("\t  Number of Predicates: " + numPreds);

            // set up feature arrays
            //var predCount = new float[numPreds][numOutcomes];

            var predCount = new float[numPreds][];

            for (int ti = 0; ti < numUniqueEvents; ti++) {
                for (int j = 0; j < contexts[ti].Length; j++) {

                    if (predCount[contexts[ti][j]] == null) {
                        predCount[contexts[ti][j]] = new float[numOutcomes];
                    }

                    if (values != null && values[ti] != null) {
                        predCount[contexts[ti][j]][outcomeList[ti]] += numTimesEventsSeen[ti]*values[ti][j];
                    } else {
                        predCount[contexts[ti][j]][outcomeList[ti]] += numTimesEventsSeen[ti];
                    }
                }
            }

            // ReSharper disable once RedundantAssignment
            di = null;

            // Get the observed expectations of the features. Strictly speaking,
            // we should divide the counts by the number of Tokens, but because of
            // the way the model's expectations are approximated in the
            // implementation, this is canceled out when we compute the next
            // iteration of a parameter, making the extra divisions wasteful.
            param = new MutableContext[numPreds];
            for (var i = 0; i < modelExpects.Length; i++)
                modelExpects[i] = new MutableContext[numPreds];

            observedExpects = new MutableContext[numPreds];


            // The model does need the correction constant and the correction feature. The correction constant
            // is only needed during training, and the correction feature is not necessary.
            // For compatibility reasons the model contains form now on a correction constant of 1,
            // and a correction param 0.
            // ReSharper disable once CoVariantArrayConversion
            evalParams = new EvalParameters(param, 0, 1, numOutcomes);

            var activeOutcomes = new int[numOutcomes];
            var allOutcomesPattern = new int[numOutcomes];
            for (var oi = 0; oi < numOutcomes; oi++) {
                allOutcomesPattern[oi] = oi;
            }
            for (var pi = 0; pi < numPreds; pi++) {
                var numActiveOutcomes = 0;
                int[] outcomePattern;
                if (Smoothing) {
                    numActiveOutcomes = numOutcomes;
                    outcomePattern = allOutcomesPattern;
                } else {
                    //determine active outcomes
                    for (var oi = 0; oi < numOutcomes; oi++) {
                        if (predCount[pi][oi] > 0 && predicateCounts[pi] >= cutoff) {
                            activeOutcomes[numActiveOutcomes] = oi;
                            numActiveOutcomes++;
                        }
                    }
                    if (numActiveOutcomes == numOutcomes) {
                        outcomePattern = allOutcomesPattern;
                    } else {
                        outcomePattern = new int[numActiveOutcomes];
                        for (var aoi = 0; aoi < numActiveOutcomes; aoi++) {
                            outcomePattern[aoi] = activeOutcomes[aoi];
                        }
                    }
                }
                param[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);

                foreach (MutableContext[] me in modelExpects)
                    me[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);

                observedExpects[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);
                for (var aoi = 0; aoi < numActiveOutcomes; aoi++) {
                    var oi = outcomePattern[aoi];
                    param[pi].SetParameter(aoi, 0.0);

                    foreach (var modelExpect in modelExpects) {
                        modelExpect[pi].SetParameter(aoi, 0.0);
                    }

                    if (predCount[pi][oi] > 0) {
                        observedExpects[pi].SetParameter(aoi, predCount[pi][oi]);
                    } else if (Smoothing) {
                        observedExpects[pi].SetParameter(aoi, SmoothingObservation);
                    }
                }
            }

            Display("...done.");

            /***************** Find the parameters ************************/
            if (threads == 1)
                Display("Computing model parameters ...");
            else
                Display("Computing model parameters in " + threads + " threads...");

            FindParameters(iterations, correctionConstant);

            /*************** Create and return the model ******************/

            // To be compatible with old models the correction constant is always 1

            // ReSharper disable once CoVariantArrayConversion
            return new GISModel(param, predLabels, outcomeLabels, 1, evalParams.CorrectionParam) {
                info = TrainingInfo
            };
        }

        #endregion


    }
}