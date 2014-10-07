using System;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Class for creating a maximum-entropy-based name finder.
    /// </summary>
    public class NameFinderME : ITokenNameFinder {
        public const int DefaultBeamSize = 3;

        internal const string START = "start";
        internal const string CONTINUE = "cont";
        internal const string LAST = "last";
        internal const string UNIT = "unit";
        internal const string OTHER = "other";

        public NameFinderME(TokenNameFinderModel model) {
            if (model == null)
                throw new ArgumentNullException("model");
           

        }

        /// <summary>
        /// Generates name tags for the given sequence, typically a sentence, returning token spans for any identified names.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence, typically a sentence.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string[] tokens) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forgets all adaptive data which was collected during previous calls to one of the find methods.
        /// </summary>
        /// <remarks>This method is typical called at the end of a document.</remarks>
        public void ClearAdaptiveData() {
            throw new NotImplementedException();
        }
    }
}
