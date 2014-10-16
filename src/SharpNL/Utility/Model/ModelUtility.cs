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
using System.IO;
using SharpNL.ML.Model;

namespace SharpNL.Utility.Model {
    /// <summary>
    /// Utility class for handling of <see cref="IMaxentModel"/>.
    /// </summary>
    public static class ModelUtility {

        #region . ValidateOutcomes .
        /// <summary>
        /// Checks if the expected outcomes are all contained as outcomes in the given model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="expectedOutcomes">The expected outcomes.</param>
        /// <returns><c>true</c> if all expected outcomes are the only outcomes of the model;<c>false</c> otherwise.</returns>
        public static bool ValidateOutcomes(IMaxentModel model, params string[] expectedOutcomes) {
            if (model.GetNumOutcomes() == expectedOutcomes.Length) {
                var count = model.GetNumOutcomes();
                for (int i = 0; i < count; i++) {
                    if (!expectedOutcomes.Contains(model.GetOutcome(i))) {
                        return false;
                    }
                }
            } else {
                return false;
            }

            return true;
        }
        #endregion

        #region . WriteModel .
        public static void WriteModel(AbstractModel model, Stream outStream) {
            if (model == null)
                throw new ArgumentNullException("model");

            if (outStream == null)
                throw new ArgumentNullException("outStream");

            if (!outStream.CanWrite)
                throw new ArgumentException("Stream was not writeable.");

            var writer = new GenericModelWriter(model, outStream);

            writer.Persist();
        }
        #endregion

    }
}