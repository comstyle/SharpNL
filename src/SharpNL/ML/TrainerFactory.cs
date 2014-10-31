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
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.Model;
using SharpNL.ML.Perceptron;
using SharpNL.Utility;

namespace SharpNL.ML {
    /// <summary>
    /// Represents the trainer factory.
    /// </summary>
    public class TrainerFactory {
        private static readonly Dictionary<string, Type> BuiltInTrainers;
        private static readonly Dictionary<string, Type> CustomTrainers;

        static TrainerFactory() {
            BuiltInTrainers = new Dictionary<string, Type>();
            CustomTrainers = new Dictionary<string, Type>();

            BuiltInTrainers[GIS.MaxEntropy] = typeof (GIS);
            BuiltInTrainers[PerceptronTrainer.PerceptronValue] = typeof (PerceptronTrainer);
        }

        private static T CreateCustomTrainer<T>(string type, Monitor monitor) {
            if (CustomTrainers.ContainsKey(type)) {
                return (T)Activator.CreateInstance(CustomTrainers[type], monitor);
            }
            return default(T);
        }
        private static T CreateBuiltinTrainer<T> (string type, Monitor monitor) {
            if (BuiltInTrainers.ContainsKey(type)) {
                return (T) Activator.CreateInstance(BuiltInTrainers[type], monitor);
                
            }
            return default(T);
        }

        #region . GetEventTrainer .

        /// <summary>
        /// Gets the event trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The <see cref="IEventTrainer"/> trainer object.</returns>
        public static IEventTrainer GetEventTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var algorithm = parameters.Get(TrainingParameters.AlgorithmParam);

            if (algorithm == null) {
                AbstractEventTrainer trainer = new GIS(monitor);
                trainer.Init(parameters, reportMap);
                return trainer;
            }

            var trainerType = GetTrainerType(parameters);
            if (trainerType.HasValue && trainerType.Value == TrainerType.EventModelTrainer) {
                var type = GetTrainer(algorithm);

                var trainer = (IEventTrainer) Activator.CreateInstance(type, monitor);
                trainer.Init(parameters, reportMap);
                return trainer;
            }

            return null;
        }

        #endregion

        #region . GetEventModelSequenceTrainer .

        /// <summary>
        /// Gets the event model sequence trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The <see cref="IEventModelSequenceTrainer"/> trainer object.</returns>
        /// <exception cref="System.InvalidOperationException">Trainer type couldn't be determined!</exception>
        public static IEventModelSequenceTrainer GetEventModelSequenceTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var trainerType = parameters.Get(Parameters.Algorithm);
            if (!string.IsNullOrEmpty(trainerType)) {
                if (BuiltInTrainers.ContainsKey(trainerType)) {
                    var trainer = CreateBuiltinTrainer<IEventModelSequenceTrainer>(trainerType, monitor);
                    trainer.Init(parameters, reportMap);
                    return trainer;
                }

                if (CustomTrainers.ContainsKey(trainerType)) {
                    var type = CustomTrainers[trainerType];
                    var trainer2 = (IEventModelSequenceTrainer)Activator.CreateInstance(type, monitor);
                    trainer2.Init(parameters, reportMap);
                    return trainer2;
                }
            }

            throw new InvalidOperationException("Trainer type couldn't be determined!");
        }

        #endregion

        #region . GetSequenceModelTrainer .

        /// <summary>
        /// Gets the sequence model trainer.
        /// </summary>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="reportMap">The report map.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The <see cref="ISequenceTrainer"/> trainer object.</returns>
        /// <exception cref="System.InvalidOperationException">Trainer type couldn't be determined!</exception>
        public static ISequenceTrainer GetSequenceModelTrainer(TrainingParameters parameters, Dictionary<string, string> reportMap, Monitor monitor) {

            var trainerType = parameters.Get(AbstractTrainer.ALGORITHM_PARAM);

            ISequenceTrainer trainer = null;

            if (trainerType != null) {
                if (BuiltInTrainers.ContainsKey(trainerType)) {
                    trainer = CreateBuiltinTrainer<ISequenceTrainer>(trainerType, monitor);
                }
                if (CustomTrainers.ContainsKey(trainerType)) {
                    trainer = CreateCustomTrainer<ISequenceTrainer>(trainerType, monitor);
                }
            } 

            if (trainer == null) {
                throw new InvalidOperationException("Trainer type couldn't be determined!");
            }

            trainer.Init(parameters, reportMap);
            return trainer;
        }
        #endregion

        #region . GetTrainer .
        internal static Type GetTrainer(string algorithm) {
            if (BuiltInTrainers.ContainsKey(algorithm)) {
                return BuiltInTrainers[algorithm];
            }
            if (CustomTrainers.ContainsKey(algorithm)) {
                return CustomTrainers[algorithm];
            }
            return null;
        }
        #endregion

        #region . GetTrainerType .

        public static TrainerType? GetTrainerType(TrainingParameters trainParams) {

            string algorithm = trainParams.Get(TrainingParameters.AlgorithmParam);

            if (algorithm == null) {
                return TrainerType.EventModelTrainer;
            }

            Type trainerType = null;

            if (BuiltInTrainers.ContainsKey(algorithm)) {
                trainerType = BuiltInTrainers[algorithm];
            } else if (CustomTrainers.ContainsKey(algorithm)) {
                trainerType = CustomTrainers[algorithm];
            }

            return GetTrainerType(trainerType);
        }

        private static TrainerType? GetTrainerType(Type trainerType) {
            if (trainerType != null) {
                if (typeof(IEventTrainer).IsAssignableFrom(trainerType)) {
                    return TrainerType.EventModelTrainer;
                }

                if (typeof(IEventModelSequenceTrainer).IsAssignableFrom(trainerType)) {
                    return TrainerType.EventModelSequenceTrainer;
                }

                if (typeof(ISequenceTrainer).IsAssignableFrom(trainerType)) {
                    return TrainerType.SequenceTrainer;
                }
            }

            return null;
        }


        #endregion

        #region . IsValid .

        /// <summary>
        /// Determines whether the specified train parameters are valid.
        /// </summary>
        /// <param name="trainParams">The train parameters.</param>
        /// <returns><c>true</c> if the specified train parameters are valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid(TrainingParameters trainParams) {

            if (!trainParams.IsValid()) {
                return false;
            }

            var algorithmName = trainParams.Get(TrainingParameters.AlgorithmParam);
            if (!(BuiltInTrainers.ContainsKey(algorithmName) || GetTrainerType(trainParams) != null)) {
                return false;
            }

            var dataIndexer = trainParams.Get(AbstractEventTrainer.DataIndexerParam);
            if (dataIndexer != null) {
                if (!(AbstractEventTrainer.DataIndexerOnePass.Equals(dataIndexer) ||
                      AbstractEventTrainer.DataIndexerTwoPass.Equals(dataIndexer))) {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region . RegisterTrainer .
        /// <summary>
        /// Registers a custom trainer with the given name.
        /// </summary>
        /// <param name="trainerName">Name of the trainer.</param>
        /// <param name="trainerType">The trainer type.</param>
        /// <exception cref="System.ArgumentNullException">trainerName
        /// or
        /// trainerType</exception>
        /// <exception cref="System.ArgumentException">The specified trainer name is an built in trainer.</exception>
        /// <exception cref="System.ArgumentException">The specified trainer name is already registered.</exception>
        /// <exception cref="System.InvalidOperationException">The specified trainer type does not implement an valid interface.</exception>
        public static void RegisterTrainer(string trainerName, Type trainerType) {
            if (string.IsNullOrEmpty(trainerName)) {
                throw new ArgumentNullException("trainerName");
            }
            if (trainerType == null) {
                throw new ArgumentNullException("trainerType");
            }

            if (BuiltInTrainers.ContainsKey(trainerName)) {
                throw new ArgumentException(@"The specified trainer name is an built in trainer.", "trainerName");
            }

            if (CustomTrainers.ContainsKey(trainerName)) {
                throw new ArgumentException(@"The specified trainer name is already registered.", "trainerName");
            }

            TrainerType? type = GetTrainerType(trainerType);

            if (!type.HasValue) {
                throw new InvalidOperationException("The specified trainer type does not implement an valid interface.");
            }

            CustomTrainers.Add(trainerName, trainerType);
        }

        #endregion

    }
}