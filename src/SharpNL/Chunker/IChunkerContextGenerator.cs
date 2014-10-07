using SharpNL.Utility;

namespace SharpNL.Chunker {

    /// <summary>
    /// Interface for the context generator used in syntactic chunking.
    /// </summary>
    public interface IChunkerContextGenerator : IBeamSearchContextGenerator<string> {

        /// <summary>
        /// Gets the contexts for chunking of the specified index.
        /// </summary>
        /// <param name="index">The index of the token in the specified tokens array for which the context should be constructed.</param>
        /// <param name="tokens">The tokens of the sentence. The <code>ToString</code> methods of these objects should return the token text.</param>
        /// <param name="tags">The POS tags for the the specified tokens.</param>
        /// <param name="prevDecisions">The previous decisions made in the tagging of this sequence.</param>
        /// <returns>An array of predictive contexts on which a model basis its decisions.</returns>
        string[] GetContext(int index, string[] tokens, string[] tags, string[] prevDecisions);

    }
}
