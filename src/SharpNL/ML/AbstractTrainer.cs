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
using System.Globalization;

namespace SharpNL.ML {
    using Utility;

    /// <summary>
    /// Represents an abstract trainer.
    /// </summary>
    public abstract class AbstractTrainer {
        /// <summary>
        /// The default cutoff value.
        /// </summary>
        public static readonly int DefaultCutoff = 5;

        /// <summary>
        /// The default iterations value.
        /// </summary>
        public static readonly int DefaultIterations = 100;

        private TrainingParameters trainParams;
        private Dictionary<string, string> reportMap;

        /// <summary>
        /// The training information field.
        /// </summary>
        protected readonly TrainingInfo info;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTrainer"/> class.
        /// </summary>
        /// <param name="monitor">The evaluation monitor. This value can be null.</param>
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
            get { return GetStringParam(Parameters.Algorithm, "MAXENT"); }        
        }
        #endregion

        #region . Cutoff .
        /// <summary>
        /// Gets the trainer cutoff.
        /// </summary>
        /// <value>The trainer cutoff.</value>
        public int Cutoff { get { return GetIntParam(Parameters.Cutoff, DefaultCutoff); } }
        #endregion

        #region . Iterations .
        /// <summary>
        /// Gets the number of iterations of this trainer.
        /// </summary>
        /// <value>The the number of iterations of this trainer.</value>
        public int Iterations { get { return GetIntParam(Parameters.Iterations, DefaultIterations); } }

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
        /// <summary>
        /// Adds the specified key and value to report map.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void AddToReport(string key, string value) {
            if (reportMap != null) {
                reportMap.Add(key, value);
            }
        }
        #endregion

        #region . Display .

        /// <summary>
        /// Displays the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void Display(string message) {
            if (Monitor != null)
                Monitor.OnMessage(message);
#if DEBUG
            System.Diagnostics.Debug.Print(message);
#endif
        }

        #endregion

        #region . GetStringParam .
        /// <summary>
        /// Gets the parameter from the train parameters. 
        /// </summary>
        /// <param name="key">The param key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A value or the <paramref name="defaultValue"/>.</returns>
        protected string GetStringParam(string key, string defaultValue) {
            var value = trainParams.Get(key) ?? defaultValue;
            if (reportMap != null) {
                reportMap[key] = value;
            }

            return value;
        }
        #endregion

        #region . GetIntParam .
        /// <summary>
        /// Gets the int parameter from the train parameters. 
        /// </summary>
        /// <param name="key">The param key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A int value or the <paramref name="defaultValue"/>.</returns>
        protected int GetIntParam(string key, int defaultValue) {
            return trainParams.Get(key) != null
                ? int.Parse(trainParams.Get(key), CultureInfo.InvariantCulture)
                : defaultValue;
        }

        #endregion

        #region . GetDoubleParam .
        /// <summary>
        /// Gets the double parameter from the train parameters. 
        /// </summary>
        /// <param name="key">The param key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A double value or the <paramref name="defaultValue"/>.</returns>
        protected double GetDoubleParam(string key, double defaultValue) {
            return trainParams.Get(key) != null
                ? double.Parse(trainParams.Get(key), CultureInfo.InvariantCulture)
                : defaultValue;
        }

        #endregion

        #region . GetBoolParam .
        /// <summary>
        /// Gets the bool parameter from the train parameters. 
        /// </summary>
        /// <param name="key">The param key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>A bool value or the <paramref name="defaultValue"/>.</returns>
        protected bool GetBoolParam(string key, bool defaultValue) {
            return trainParams.Get(key) != null
                ? bool.Parse(trainParams.Get(key))
                : defaultValue;
        }

        #endregion

        #region . Init .
        /// <summary>
        /// Initializes abstract trainer using the specified train parameters.
        /// </summary>
        /// <param name="trainParameters">The train parameters.</param>
        /// <param name="reportMapping">The report map.</param>
        public void Init(TrainingParameters trainParameters, Dictionary<string, string> reportMapping) {
            trainParams = trainParameters;
            reportMap = reportMapping;
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