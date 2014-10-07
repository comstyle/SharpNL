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

using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Interface for streams of sequences used to train sequence models.
    /// </summary>
    public interface ISequenceStream : IObjectStream<Sequence> {
        /// <summary>
        /// Creates a new event array based on the outcomes predicted by the specified parameters for the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence to be evaluated.</param>
        /// <param name="model">The model.</param>
        /// <returns>The event array.</returns>
        Event[] UpdateContext(Sequence sequence, AbstractModel model);
    }
}