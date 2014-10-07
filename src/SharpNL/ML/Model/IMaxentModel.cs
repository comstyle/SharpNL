using SharpNL.Utility.Model;
using SharpNL.Utility.Serialization;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Interface for maximum entropy models.
    /// </summary>
    public interface IMaxentModel {
        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together..</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context);

        /// <summary>
        /// Evaluates a context.
        /// </summary>
        /// <param name="context">A list of string names of the contextual predicates which are to be evaluated together..</param>
        /// <param name="probs">An array which is populated with the probabilities for each of the different outcomes, all of which sum to 1.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context, double[] probs);

        /// <summary>
        /// Evaluates a contexts with the specified context values.
        /// </summary>
        /// <param name="context">A list of String names of the contextual predicates which are to be evaluated together.</param>
        /// <param name="probs">The values associated with each context.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Eval(string[] context, float[] probs);


        /// <summary>
        /// Simple function to return the outcome associated with the index containing the highest probability in the double[].
        /// </summary>
        /// <param name="outcomes"></param>
        /// <returns>The string name of the best outcome.</returns>
        string GetBestOutcome(double[] outcomes);

        /*
        /// <summary>
        /// Return a string matching all the outcome names with all the probabilities produced by the <code>eval(String[] context)</code> method.
        /// </summary>
        /// <param name="outcomes">A <code>double[]</code> as returned by the <code>eval(String[] context)</code> method.</param>
        /// <returns>A string containing outcome names paired with the normalized probability (contained in the <code>double[] ocs</code>) for each one.</returns>
        /// TODO: This should be removed, can't be used anyway without format spec.
        string GetAllOutcomes(double[] outcomes);
        */

        /// <summary>
        /// Gets the string name of the outcome associated with the index,
        /// </summary>
        /// <param name="index">The index for which the name of the associated outcome is desired.</param>
        /// <returns>The string name of the outcome.</returns>
        string GetOutcome(int index);


        /// <summary>
        /// Gets the index associated with the String name of the given outcome.
        /// </summary>
        /// <param name="outcome">The string name of the outcome for which the index is desired.</param>
        /// <returns>The index if the given outcome label exists for this model, -1 if it does not.</returns>
        int GetIndex(string outcome);


        /// <summary>
        /// Gets the data structures relevant to storing the model.
        /// </summary>
        /// <returns>The data structures relevant to storing the model.</returns>
        int GetNumOutcomes();

    }
}
