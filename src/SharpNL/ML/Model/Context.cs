using System;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a real valued parameter or expected value associated with a particular contextual predicate or feature. 
    /// This is used to store maxent model parameters as well as model and empirical expected values.
    /// </summary>
    public class Context {

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> with the specified parameters associated with the specified outcome pattern.
        /// </summary>
        /// <param name="outcomes">The outcomes outcomes for which parameters exists for this context..</param>
        /// <param name="parameters">The parameters for the outcomes specified.</param>
        public Context(int[] outcomes, double[] parameters) {
            Outcomes = outcomes;
            Parameters = parameters;
        }


        /// <summary>
        /// Gets the outcomes for which parameters exists for this context.
        /// </summary>
        /// <value>A array of outcomes for which parameters exists for this context.</value>
        public int[] Outcomes { get; protected set; }

        /// <summary>
        /// Gets the parameters or expected values for the outcomes which occur with this context.
        /// </summary>
        /// <value>A array of parameters for the outcomes of this context.</value>
        public double[] Parameters { get; protected set; }

        /// <summary>
        /// Determines whether this context contains the specified outcome.
        /// </summary>
        /// <param name="outcome">The outcome to seek.</param>
        /// <returns><c>true</c> if the <paramref name="outcome"/> occurs within this context; otherwise, <c>false</c>.</returns>
        public bool Contains(int outcome) {
            return Array.BinarySearch(Outcomes, outcome) >= 0;
        }


    }
}
