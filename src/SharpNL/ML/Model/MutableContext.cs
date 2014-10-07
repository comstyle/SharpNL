namespace SharpNL.ML.Model {
    public class MutableContext : Context {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MutableContext"/> with the specified parameters associated with the specified outcome pattern.
        /// </summary>
        /// <param name="outcomes">The outcomes outcomes for which parameters exists for this context.</param>
        /// <param name="parameters">The parameters for the outcomes specified.</param>
        public MutableContext(int[] outcomes, double[] parameters) : base(outcomes, parameters) {}



        /// <summary>
        /// Assigns the parameter or expected value at the specified outcomeIndex the specified value.
        /// </summary>
        /// <param name="index">The index of the parameter or expected value to be updated.</param>
        /// <param name="value">The value to be assigned.</param>
        public void SetParameter(int index, double value) {
            Parameters[index] = value;
        }


        /// <summary>
        /// Updates the parameter or expected value at the specified index by adding the specified value to its current value.
        /// </summary>
        /// <param name="index">The index of the parameter or expected value to be updated.</param>
        /// <param name="value">The value to be added.</param>
        public void UpdateParameter(int index, double value) {
            Parameters[index] += value;
        }

    }
}