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

namespace SharpNL.Utility {
    /// <summary>
    /// Utility class to store common parameters.
    /// </summary>
    public static class Parameters {
        /// <summary>
        /// The algorithm parameter.
        /// </summary>
        public const string Algorithm = "Algorithm";

        /// <summary>
        /// The beam size parameter.
        /// </summary>
        public const string BeamSize = "BeamSize";

        /// <summary>
        /// The cutoff parameter.
        /// </summary>
        public const string Cutoff = "Cutoff";

        /// <summary>
        /// The data indexer parameter.
        /// </summary>
        public const string DataIndexer = "DataIndexer";

        /// <summary>
        /// The iterations parameter.
        /// </summary>
        public const string Iterations = "Iterations";
        /// <summary>
        /// The tolerance parameter.
        /// </summary>
        public const string Tolerance = "Tolerance";
        /// <summary>
        /// The trainer type parameter.
        /// </summary>
        public const string TrainerType = "TrainerType";

        /// <summary>
        /// The training eventhash parameter.
        /// </summary>
        public const string TrainingEventhash = "Training-Eventhash";

        /// <summary>
        /// The step size decrease parameter.
        /// </summary>
        public const string StepSizeDecrease = "StepSizeDecrease";

        /// <summary>
        /// The use average parameter
        /// </summary>
        public const string UseAverage = "UseAverage";

        /// <summary>
        /// The use skipped averaging parameter.
        /// </summary>
        public const string UseSkippedAveraging = "UseSkippedAveraging";

        /// <summary>
        /// The threads parameter.
        /// </summary>
        public const string Threads = "Threads";

        /// <summary>
        /// Represents the supported algorithms.
        /// </summary>
        public static class Algorithms {
            /// <summary>
            /// The perceptron algorithm.
            /// </summary>
            public const string Perceptron = "PERCEPTRON";

            /// <summary>
            /// The maximum entropy algorithm.
            /// </summary>
            public const string MaxEnt = "MAXENT";

        }

        /// <summary>
        /// Represents the supported data indexers.
        /// </summary>
        public static class DataIndexers {
            /// <summary>
            /// The one pass data indexer.
            /// </summary>
            public const string OnePass = "OnePass";

            /// <summary>
            /// The two pass data indexer.
            /// </summary>
            public const string TwoPass = "TwoPass";

        }
    }
}