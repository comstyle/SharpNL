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
        public const string Algorithm = "Algorithm";
        public const string BeamSize = "BeamSize";
        public const string Cutoff = "Cutoff";
        public const string DataIndexer = "DataIndexer";

        ///public const string Dictionary = "dict";
        public const string Iterations = "Iterations";
        public const string Tolerance = "Tolerance";
        public const string TrainerType = "TrainerType";
        public const string StepSizeDecrease = "StepSizeDecrease";
        public const string UseAverage = "UseAverage";
        public const string UseSkippedAveraging = "UseSkippedAveraging";

        public const string Threads = "Threads";

        public static class Algorithms {

            public const string Perceptron = "PERCEPTRON";
            public const string MaxEnt = "MAXENT";

        }

        public static class DataIndexers {
            public const string OnePass = "OnePass";
            public const string TwoPass = "TwoPass";

        }
    }
}