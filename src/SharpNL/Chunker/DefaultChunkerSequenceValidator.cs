using System;

namespace SharpNL.Chunker {

    using Utility;

    /// <summary>
    /// Represents a default chunker sequence validator.
    /// </summary>
    public class DefaultChunkerSequenceValidator : ISequenceValidator<string> {


        private bool ValidOutcome(String outcome, String prevOutcome) {
            if (outcome.StartsWith("I-")) {
                if (prevOutcome == null) {
                    return false;
                }
                if (prevOutcome.Equals("O")) {
                    return false;
                }
                if (!prevOutcome.Substring(2).Equals(outcome.Substring(2))) {
                    return false;
                }
            }
            return true;
        }

        protected virtual bool ValidOutcome(String outcome, String[] sequence) {
            String prevOutcome = null;
            if (sequence.Length > 0) {
                prevOutcome = sequence[sequence.Length - 1];
            }
            return ValidOutcome(outcome, prevOutcome);
        }


        /// <summary>
        /// Determines whether a particular continuation of a sequence is valid.
        /// This is used to restrict invalid sequences such as those used in start/continue tag-based chunking or could be used to implement tag dictionary restrictions.
        /// </summary>
        /// <param name="index">The index in the input sequence for which the new outcome is being proposed.</param>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="outcomesSequence">The outcomes so far in this sequence.</param>
        /// <param name="outcome">The next proposed outcome for the outcomes sequence.</param>
        /// <returns><c>true</c> if the sequence would still be valid with the new outcome, <c>false</c> otherwise.</returns>
        public bool ValidSequence(int index, string[] inputSequence, string[] outcomesSequence, string outcome) {
            return ValidOutcome(outcome, outcomesSequence);

        }
    }
}
