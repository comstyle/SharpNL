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

namespace SharpNL.ML {
    using Model;
    /// <summary>
    /// Represents a sequence trainer that uses a <see cref="ISequenceStream"/> to train a <see cref="T:Model.ISequenceClassificationModel{string}"/> model.
    /// </summary>
    public interface ISequenceTrainer : ITrainer {

        /// <summary>
        /// Trains a sequence classification model using a event sequence stream.
        /// </summary>
        /// <param name="events">The sequence event stream.</param>
        /// <returns>A trained sequence classification model.</returns>
        Model.ISequenceClassificationModel<string> Train(ISequenceStream events);

    }
}