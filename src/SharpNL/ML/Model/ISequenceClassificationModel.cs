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

using RealSequence = SharpNL.Utility.Sequence;

namespace SharpNL.ML.Model {

    

    /// <summary>
    /// A classification model that can label an input sequence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequenceClassificationModel<T> {
        /// <summary>
        /// Finds the sequence with the highest probability.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="contextGenerator">The context generator.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Sequence.</returns>
        RealSequence BestSequence(T[] sequence, object[] additionalContext, IBeamSearchContextGenerator<T> contextGenerator,
            ISequenceValidator<T> validator);


        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="contextGenerator">The context generator.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Sequence[].</returns>
        RealSequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext,
            IBeamSearchContextGenerator<T> contextGenerator, ISequenceValidator<T> validator);

        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="minSequenceScore">The minimum sequence score.</param>
        /// <param name="contextGenerator">The context generator.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>Sequence[].</returns>
        RealSequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext, double minSequenceScore,
            IBeamSearchContextGenerator<T> contextGenerator, ISequenceValidator<T> validator);


        /// <summary>
        /// Gets all possible outcomes.
        /// </summary>
        /// <returns>All possible outcomes.</returns>
        string[] GetOutcomes();
    }
}