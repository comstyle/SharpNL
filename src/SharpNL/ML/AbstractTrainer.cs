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

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace SharpNL.ML {
    using Utility;

    /// <summary>
    /// Represents an abstract trainer.
    /// </summary>
    public abstract class AbstractTrainer {

        public static readonly string ALGORITHM_PARAM = "Algorithm";
        public static readonly string TRAINER_TYPE_PARAM = "TrainerType";
        public static readonly int CUTOFF_DEFAULT = 5;
        public static readonly int ITERATIONS_DEFAULT = 100;

        private TrainingParameters trainParams;
        private Dictionary<string, string> reportMap;

        protected readonly TrainingInfo info;

        protected AbstractTrainer(Monitor monitor) {
            Monitor = monitor;
            info = new TrainingInfo();
        }

        #region + Properties .

        #region . Algorithm .
        /// <summary>
        /// Gets the algorithm associated to this trainer.
        /// </summary>
        /// <value>The algorithm associated to this trainer.</value>
        public string Algorithm {
            get { return GetStringParam(TrainingParameters.AlgorithmParam, "MAXENT"); }        
        }
        #endregion

        #region . Cutoff .
        /// <summary>
        /// Gets the trainer cutoff.
        /// </summary>
        /// <value>The trainer cutoff.</value>
        public int Cutoff { get { return GetIntParam(TrainingParameters.CutoffParam, CUTOFF_DEFAULT); } }
        #endregion

        #region . Iterations .
        /// <summary>
        /// Gets the number of iterations of this trainer.
        /// </summary>
        /// <value>The the number of iterations of this trainer.</value>
        public int Iterations { get { return GetIntParam(TrainingParameters.IterationsParam, ITERATIONS_DEFAULT); } }

        #endregion

        #region . Monitor .
        /// <summary>
        /// Gets evaluation the monitor.
        /// </summary>
        /// <value>The evaluation monitor.</value>
        protected Monitor Monitor { get; private set; }
        #endregion

        #endregion

        #region . AddToReport .
        protected void AddToReport(string key, string value) {
            if (reportMap != null) {
                reportMap.Add(key, value);
            }
        }
        #endregion

        #region . Display .

        protected void Display(string message) {
            if (Monitor != null)
                Monitor.OnMessage(message);
#if DEBUG
            Debug.Print(message);
#endif
        }

        #endregion

        #region . GetStringParam .
        protected string GetStringParam(string key, string defaultValue) {
            var value = trainParams.Get(key) ?? defaultValue;
            if (reportMap != null) {
                reportMap[key] = value;
            }

            return value;
        }
        #endregion

        #region . GetIntParam .
        protected int GetIntParam(string key, int defaultValue) {
            if (trainParams.Get(key) == null) {
                return defaultValue;
            }
            return int.Parse(trainParams.Get(key), CultureInfo.InvariantCulture);
        }
        #endregion

        #region . GetDoubleParam .
        protected double GetDoubleParam(string key, double defaultValue) {
            if (trainParams.Get(key) == null) {
                return defaultValue;
            }
            return double.Parse(trainParams.Get(key), CultureInfo.InvariantCulture);
        }
        #endregion

        #region . GetBoolParam .
        protected bool GetBoolParam(string key, bool defaultValue) {
            if (trainParams.Get(key) == null) {
                return defaultValue;
            }
            return bool.Parse(trainParams.Get(key));
        }
        #endregion

        #region . Init .
        public void Init(TrainingParameters trainParameters, Dictionary<string, string> reportMap) {
            trainParams = trainParameters;
            this.reportMap = reportMap;
        }
        #endregion

        #region . IsValid .
        /// <summary>
        /// Determines whether the parameters of this trainer are valid.
        /// </summary>
        /// <returns><c>true</c> if the parameters of this trainer are valid; otherwise, <c>false</c>.</returns>
        protected virtual bool IsValid() {
            return trainParams.IsValid();
        }
        #endregion

    }
}