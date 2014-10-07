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
using System.Threading.Tasks;
using SharpNL.ML.MaxEntropy.IO;

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
    /// <history>
    ///  Knuppe 2014-09-18: Ported from OpenNLP 1.5.3
    ///         2014-09-26: This class is giving me a lot of work :(
    /// </history>
    public class GISTrainer {
        private const double LLThreshold = 0.0001;

        /// <summary>
        /// Specified whether parameter updates should prefer a distribution of parameters which is gaussian.
        /// </summary>
        private bool useGaussianSmoothing;

        private double sigma = 2.0;

        /// <summary>
        /// Number of unique events which occured in the event set.
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

        private int[] threadNumEvents;
        private int[] threadNumCorrect;
        private double[] threadLoglikelihood;

        public GISTrainer() : this(false) { }

        public GISTrainer(bool printMessages) {
            PrintMessages = printMessages;
            Smoothing = false;
            SmoothingObservation = 0.1;
        }

        #region + Properties .

        #region . PrintMessages .
        public bool PrintMessages { get; private set; }
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

            // TODO: Improve the output report...
            if (PrintMessages) 
                Console.Out.WriteLine(message);

            System.Diagnostics.Debug.Print(message);
        }
        #endregion

        #region . NextIteration .

        /// <summary>
        /// Compute one iteration of GIS and return the log-likelihood.
        /// </summary>
        /// <param name="correctionConstant">The correction constant.</param>
        /// <remarks>Compute contribution of p(a|b_i) for each feature and the new correction parameter.</remarks>
        private double NextIteration(double correctionConstant) {
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

            Display(".");

            // merge the results of the two computations
            for (int pi = 0; pi < numPreds; pi++) {
                int[] activeOutcomes = param[pi].Outcomes;

                for (int aoi = 0; aoi < activeOutcomes.Length; aoi++) {
                    for (int i = 1; i < modelExpects.Length; i++) {
                        modelExpects[0][pi].UpdateParameter(aoi, modelExpects[i][pi].Parameters[aoi]);
                    }
                }
            }

            Display(".");

            // compute the new parameter values
            for (int pi = 0; pi < numPreds; pi++) {
                double[] observed = observedExpects[pi].Parameters;
                double[] model = modelExpects[0][pi].Parameters;
                int[] activeOutcomes = param[pi].Outcomes;
                for (int aoi = 0; aoi < activeOutcomes.Length; aoi++) {
                    if (useGaussianSmoothing) {
                        param[pi].UpdateParameter(aoi, GaussianUpdate(pi, aoi, correctionConstant));
                    } else {
                        if (model[aoi].Equals(0d)) {
                            Console.Error.WriteLine("Model expects == 0 for " + predLabels[pi] + " " +
                                                    outcomeLabels[aoi]);
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

            Display(". loglikelihood=" + loglikelihood + "\t" + ((double)numCorrect / numEvents) + "\n");

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
            double prevLL = 0.0;
            Display("Performing " + iterations + " iterations.\n");
            for (int i = 1; i <= iterations; i++) {
                if (i < 10)
                    Display("  " + i + ":  ");
                else if (i < 100)
                    Display(" " + i + ":  ");
                else
                    Display(i + ":  ");

                double currLL = NextIteration(correctionConstant);
                if (i > 1) {
                    if (prevLL > currLL) {
                        Console.Error.WriteLine("Model Diverging: loglikelihood decreased");
                        break;
                    }
                    if (currLL - prevLL < LLThreshold) {
                        break;
                    }
                }
                prevLL = currLL;
            }

            // kill a bunch of these big objects now that we don't need them
            observedExpects = null;
            modelExpects = null;
            numTimesEventsSeen = null;
            contexts = null;
        }
        #endregion

        #region . GaussianUpdate .
        /// <remarks>Modeled on implementation in Zhang Le's maxent kit.</remarks>
        private double GaussianUpdate(int predicate, int oid, double correctionConstant) {
            var p = param[predicate].Parameters[oid];
            double x0 = 0.0;
            double modelValue = modelExpects[0][predicate].Parameters[oid];
            double observedValue = observedExpects[predicate].Parameters[oid];
            for (int i = 0; i < 50; i++) {
                double tmp = modelValue*Math.Exp(correctionConstant*x0);
                double f = tmp + (p + x0)/sigma - observedValue;
                double fp = tmp*correctionConstant + 1/sigma;
                if (fp.Equals(0d)) {
                    break;
                }
                double x = x0 - f/fp;
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

            for (int ei = startIndex; ei < startIndex + length; ei++) {

                if (values != null) {
                    prior.LogPrior(modelDistribution, contexts[ei], values[ei]);
                    GISModel.Eval(contexts[ei], values[ei], modelDistribution, evalParams);
                } else {
                    prior.LogPrior(modelDistribution, contexts[ei]);
                    GISModel.Eval(contexts[ei], modelDistribution, evalParams);
                }

                for (int j = 0; j < contexts[ei].Length; j++) {
                    int pi = contexts[ei][j];

                    if (predicateCounts[pi] >= cutoff) {
                        int[] activeOutcomes = modelExpects[threadIndex][pi].Outcomes;
                        for (int aoi = 0; aoi < activeOutcomes.Length; aoi++) {
                            int oi = activeOutcomes[aoi];

                            if (values != null && values[ei] != null) {
                                modelExpects[threadIndex][pi].UpdateParameter(aoi, modelDistribution[oi] * values[ei][j] * numTimesEventsSeen[ei]);
                            } else {
                                modelExpects[threadIndex][pi].UpdateParameter(aoi, modelDistribution[oi] * numTimesEventsSeen[ei]);
                            }

                        }
                    }
                }
                
                threadLoglikelihood[threadIndex] += Math.Log(modelDistribution[outcomeList[ei]]) * numTimesEventsSeen[ei];
                threadNumEvents[threadIndex] += numTimesEventsSeen[ei];

                if (PrintMessages) {
                    int max = 0;
                    for (int oi = 0; oi < numOutcomes; oi++) {
                        if (modelDistribution[oi] > modelDistribution[max]) {
                            max = oi;
                        }
                    }
                    if (max == outcomeList[ei]) {
                        threadNumCorrect[threadIndex] += numTimesEventsSeen[ei];
                    }
                }
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

            if (threads <= 0) {
                throw new ArgumentOutOfRangeException("threads", threads, @"Threads must be at least one or greater.");
            }

            modelExpects = new MutableContext[threads][];

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
                    float cl = values[ci][0];
                    for (int vi = 1; vi < values[ci].Length; vi++) {
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

            Display("\tNumber of Event Tokens: " + numUniqueEvents + "\n");
            Display("\t    Number of Outcomes: " + numOutcomes + "\n");
            Display("\t  Number of Predicates: " + numPreds + "\n");

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
            for (int oi = 0; oi < numOutcomes; oi++) {
                allOutcomesPattern[oi] = oi;
            }
            for (int pi = 0; pi < numPreds; pi++) {
                int numActiveOutcomes = 0;
                int[] outcomePattern;
                if (Smoothing) {
                    numActiveOutcomes = numOutcomes;
                    outcomePattern = allOutcomesPattern;
                } else {
                    //determine active outcomes
                    for (int oi = 0; oi < numOutcomes; oi++) {
                        if (predCount[pi][oi] > 0 && predicateCounts[pi] >= cutoff) {
                            activeOutcomes[numActiveOutcomes] = oi;
                            numActiveOutcomes++;
                        }
                    }
                    if (numActiveOutcomes == numOutcomes) {
                        outcomePattern = allOutcomesPattern;
                    } else {
                        outcomePattern = new int[numActiveOutcomes];
                        for (int aoi = 0; aoi < numActiveOutcomes; aoi++) {
                            outcomePattern[aoi] = activeOutcomes[aoi];
                        }
                    }
                }
                param[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);

                foreach (MutableContext[] me in modelExpects)
                    me[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);

                observedExpects[pi] = new MutableContext(outcomePattern, new double[numActiveOutcomes]);
                for (int aoi = 0; aoi < numActiveOutcomes; aoi++) {
                    int oi = outcomePattern[aoi];
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

            Display("...done.\n");

            /***************** Find the parameters ************************/
            if (threads == 1)
                Display("Computing model parameters ...\n");
            else
                Display("Computing model parameters in " + threads + " threads...\n");

            FindParameters(iterations, correctionConstant);

            /*************** Create and return the model ******************/
            // To be compatible with old models the correction constant is always 1
            return new GISModel(
                Array.ConvertAll(param, input => (Context)input),
                predLabels,
                outcomeLabels,
                1, 
                evalParams.CorrectionParam);
        }

        #endregion


    }
}