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

namespace SharpNL.ML.Model {
    /// <summary>
    /// Class used to store parameters or expected values associated with this context which
    /// can be updated or assigned. This class cannot be inherited.
    /// </summary>
    public sealed class MutableContext : Context {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MutableContext"/> with the specified parameters associated with the specified outcome pattern.
        /// </summary>
        /// <param name="outcomes">The outcomes outcomes for which parameters exists for this context.</param>
        /// <param name="parameters">The parameters for the outcomes specified.</param>
        public MutableContext(int[] outcomes, double[] parameters) : base(outcomes, parameters) {}

        /// <summary>
        /// Assigns the parameter or expected value at the specified outcomeIndex the specified value.
        /// </summary>
        /// <param name="index">The index of the parameter or expected value to be updated.</param>
        /// <param name="value">The value to be assigned.</param>
        public void SetParameter(int index, double value) {
            Parameters[index] = value;
        }

        /// <summary>
        /// Updates the parameter or expected value at the specified index by adding the specified value to its current value.
        /// </summary>
        /// <param name="index">The index of the parameter or expected value to be updated.</param>
        /// <param name="value">The value to be added.</param>
        public void UpdateParameter(int index, double value) {
            Parameters[index] += value;
        }
    }
}