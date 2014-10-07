namespace SharpNL.Utility {

    /// <summary>
    /// Interface ISequenceValidator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequenceValidator<in T> {


        /// <summary>
        /// Determines whether a particular continuation of a sequence is valid.
        /// This is used to restrict invalid sequences such as those used in start/continue tag-based chunking or could be used to implement tag dictionary restrictions.
        /// </summary>
        /// <param name="index">The index in the input sequence for which the new outcome is being proposed.</param>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="outcomesSequence">The outcomes so far in this sequence.</param>
        /// <param name="outcome">The next proposed outcome for the outcomes sequence.</param>
        /// <returns><c>true</c> if the sequence would still be valid with the new outcome, <c>false</c> otherwise.</returns>
        bool ValidSequence(int index, T[] inputSequence, string[] outcomesSequence, string outcome);

    }
}
