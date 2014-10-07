namespace SharpNL.Chunker {
    /// <summary>
    /// Represents a default chunker context generator.
    /// </summary>
    /// <remarks>Features based on chunking model described in Fei Sha and Fernando Pereira. 
    /// Shallow parsing with conditional random fields. 
    /// In Proceedings of HLT-NAACL 2003. Association for Computational Linguistics, 2003.
    /// </remarks> 
    public class DefaultChunkerContextGenerator : IChunkerContextGenerator {

        /// <summary>
        /// Gets the context for the specified position in the specified sequence (list).
        /// </summary>
        /// <param name="index">The index of the sequence.</param>
        /// <param name="sequence">The sequence of items over which the beam search is performed.</param>
        /// <param name="priorDecisions">The sequence of decisions made prior to the context for which this decision is being made.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for the specified position in the specified sequence.</returns>
        public virtual string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext) {
            return GetContext(index, sequence, (string[]) additionalContext[0], priorDecisions);
        }

        /// <summary>
        /// Gets the contexts for chunking of the specified index.
        /// </summary>
        /// <param name="index">The index of the token in the specified tokens array for which the context should be constructed.</param>
        /// <param name="tokens">The tokens of the sentence. The <code>ToString</code> methods of these objects should return the token text.</param>
        /// <param name="tags">The POS tags for the the specified tokens.</param>
        /// <param name="preds">The previous decisions made in the tagging of this sequence.</param>
        /// <returns>An array of predictive contexts on which a model basis its decisions.</returns>
        public virtual string[] GetContext(int index, string[] tokens, string[] tags, string[] preds) {
            // Words in a 5-word window
            string w_2, w_1, w1, w2;

            // Tags in a 5-word window
            string t_2, t_1, t1, t2;

            // Previous predictions
            string p_2, p_1;

            if (index < 2) {
                w_2 = "w_2=bos";
                t_2 = "t_2=bos";
                p_2 = "p_2=bos";
            } else {
                w_2 = "w_2=" + tokens[index - 2];
                t_2 = "t_2=" + tags[index - 2];
                p_2 = "p_2" + preds[index - 2];
            }

            if (index < 1) {
                w_1 = "w_1=bos";
                t_1 = "t_1=bos";
                p_1 = "p_1=bos";
            } else {
                w_1 = "w_1=" + tokens[index - 1];
                t_1 = "t_1=" + tags[index - 1];
                p_1 = "p_1=" + preds[index - 1];
            }

            string w0 = "w0=" + tokens[index];
            string t0 = "t0=" + tags[index];

            if (index + 1 >= tokens.Length) {
                w1 = "w1=eos";
                t1 = "t1=eos";
            } else {
                w1 = "w1=" + tokens[index + 1];
                t1 = "t1=" + tags[index + 1];
            }

            if (index + 2 >= tokens.Length) {
                w2 = "w2=eos";
                t2 = "t2=eos";
            } else {
                w2 = "w2=" + tokens[index + 2];
                t2 = "t2=" + tags[index + 2];
            }

            var features = new[] {
                //add word features
                w_2,
                w_1,
                w0,
                w1,
                w2,
                w_1 + w0,
                w0 + w1,

                //add tag features
                t_2,
                t_1,
                t0,
                t1,
                t2,
                t_2 + t_1,
                t_1 + t0,
                t0 + t1,
                t1 + t2,
                t_2 + t_1 + t0,
                t_1 + t0 + t1,
                t0 + t1 + t2,

                //add pred tags
                p_2,
                p_1,
                p_2 + p_1,

                //add pred and tag
                p_1 + t_2,
                p_1 + t_1,
                p_1 + t0,
                p_1 + t1,
                p_1 + t2,
                p_1 + t_2 + t_1,
                p_1 + t_1 + t0,
                p_1 + t0 + t1,
                p_1 + t1 + t2,
                p_1 + t_2 + t_1 + t0,
                p_1 + t_1 + t0 + t1,
                p_1 + t0 + t1 + t2,

                //add pred and word
                p_1 + w_2,
                p_1 + w_1,
                p_1 + w0,
                p_1 + w1,
                p_1 + w2,
                p_1 + w_1 + w0,
                p_1 + w0 + w1
            };

            return features;
        }
    }
}