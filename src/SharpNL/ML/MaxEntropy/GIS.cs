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

namespace SharpNL.ML.MaxEntropy {
    using IO;
    using Model;

    /// <summary>
    /// A Factory class which uses instances of <see cref="GISTrainer"/> to create and train <see cref="GISModel"/>s.
    /// </summary>
    public class GIS : AbstractEventTrainer {
        /// <summary>
        /// The maximum entropy entry value.
        /// </summary>
        public const string MaxEntropy = "MAXENT";

        /// <summary>
        /// If we are using smoothing, this is used as the "number" of times we want the trainer to imagine that it saw a feature that it actually didn't see.
        /// </summary>
        /// <remarks>The default value is 0.1.</remarks>
        public static double SmoothingObservation = 0.1;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="GIS"/> class.
        /// </summary>
        public GIS() : base(null, true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GIS"/> class with the given evaluation monitor.
        /// </summary>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        public GIS(Monitor monitor) : base(monitor, true) { } 
        #endregion    

        #region . IsValid .

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        protected override bool IsValid() {
            if (!base.IsValid()) {
                return false;
            }

            return Algorithm == null || Algorithm == MaxEntropy;
        }

        #endregion

        #region . DoTrain .
        /// <summary>
        /// Perform the training process using the specified <paramref name="indexer"/> object.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <returns>The trained <see cref="IMaxentModel"/> model.</returns>
        protected override IMaxentModel DoTrain(IDataIndexer indexer) {
            var threads = GetIntParam("Threads", 1);

            return TrainModel(Iterations, indexer, false, null, 0, threads, Monitor);
        }
        #endregion

        #region . TrainModel .

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer) {
            return TrainModel(iterations, indexer, false, null, 0);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, Monitor monitor) {
            return TrainModel(iterations, indexer, false, null, 0, monitor);
        }


        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, bool smoothing) {
            return TrainModel(iterations, indexer, smoothing, null, 0);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, bool smoothing, Monitor monitor) {
            return TrainModel(iterations, indexer, smoothing, null, 0, monitor);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <param name="modelPrior">The prior distribution for the model.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, bool smoothing, IPrior modelPrior, int cutoff, Monitor monitor) {
            return TrainModel(iterations, indexer, smoothing, modelPrior, cutoff, 1, monitor);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <param name="modelPrior">The prior distribution for the model.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, bool smoothing, IPrior modelPrior, int cutoff) {
            return TrainModel(iterations, indexer, smoothing, modelPrior, cutoff, 1);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <param name="modelPrior">The prior distribution for the model.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <param name="threads">The number of threads to use during the training.</param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(
            int iterations,
            IDataIndexer indexer,
            bool smoothing,
            IPrior modelPrior,
            int cutoff,
            int threads) {
            var trainer = new GISTrainer {
                Smoothing = smoothing,
                SmoothingObservation = SmoothingObservation
            };

            if (modelPrior == null) {
                modelPrior = new UniformPrior();
            }

            return trainer.TrainModel(iterations, indexer, modelPrior, cutoff, threads);
        }

        /// <summary>
        /// Train a model using the GIS algorithm.
        /// </summary>
        /// <param name="iterations">The number of GIS iterations to perform.</param>
        /// <param name="indexer">The object which will be used for event compilation.</param>
        /// <param name="smoothing">Defines whether the created trainer will use smoothing while training the model.</param>
        /// <param name="modelPrior">The prior distribution for the model.</param>
        /// <param name="cutoff">The number of times a predicate must occur to be used in a model.</param>
        /// <param name="threads">The number of threads to use during the training.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The newly trained model, which can be used immediately or saved to disk using a <see cref="GISModelWriter"/> object.</returns>
        public static GISModel TrainModel(int iterations, IDataIndexer indexer, bool smoothing, IPrior modelPrior, int cutoff, int threads, Monitor monitor) {
                var trainer = new GISTrainer(monitor) {
                Smoothing = smoothing,
                SmoothingObservation = SmoothingObservation
            };

            if (modelPrior == null) {
                modelPrior = new UniformPrior();
            }

            return trainer.TrainModel(iterations, indexer, modelPrior, cutoff, threads);
        }
        #endregion

    }
}