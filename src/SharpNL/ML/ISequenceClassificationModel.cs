using SharpNL.Utility;
using SharpNL.Utility.Model;
using SharpNL.Utility.Serialization;

namespace SharpNL.ML {
    public interface ISequenceClassificationModel<T> {

        /// <summary>
        /// Finds the sequence with the highest probability.
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="additionalContext"></param>
        /// <param name="beamSearch"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        Sequence BestSequence(
            T[] sequence, 
            object[] additionalContext, 
            IBeamSearchContextGenerator<T> beamSearch,
            ISequenceValidator<T> validator);



        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="minSequenceScore">The minimum sequence score.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        Sequence[] BestSequences(
            int numSequences, 
            T[] sequence, 
            object[] additionalContext, 
            double minSequenceScore,
            IBeamSearchContextGenerator<T> beamSearch, 
            ISequenceValidator<T> validator);


        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        Sequence[] BestSequences(
            int numSequences, 
            T[] sequence,
            object[] additionalContext,
            IBeamSearchContextGenerator<T> beamSearch, 
            ISequenceValidator<T> validator);


        /// <summary>
        /// Gets all possible outcomes.
        /// </summary>
        /// <returns>all possible outcomes.</returns>
        string[] GetOutcomes();

    }
}
