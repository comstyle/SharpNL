using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpNL.Utility {

    /// <summary>
    /// Represents a weighted sequence of outcomes.
    /// </summary>
    public class Sequence : IComparable<Sequence>, ICloneable {
        private readonly List<string> outcomes;
        private readonly List<double> probs;

        #region + Constructors .

        public Sequence() {
            outcomes = new List<string>();
            probs = new List<double>();
        }

        public Sequence(Sequence sequence, string outcome, double probability) : this(sequence) {
            outcomes.Add(outcome);
            probs.Add(probability);
            Score += Math.Log(probability);           
        }

        public Sequence(IEnumerable<string> outcomes) {
            this.outcomes = new List<string>(outcomes);
            probs = new List<double>(Enumerable.Repeat(1d, this.outcomes.Count));
        }

        private Sequence(Sequence sequence) {
            outcomes = new List<string>(sequence.outcomes);
            probs = new List<double>(sequence.probs);
            Score = sequence.Score;
        }

        #endregion

        #region + Properties .

        #region . Outcomes .
        /// <summary>
        /// Gets the outcomes for this sequence.
        /// </summary>
        /// <value>The outcomes for this sequence.</value>
        public List<string> Outcomes {
            get { return outcomes; }
        }
        #endregion

        #region . Probabilities .
        /// <summary>
        /// Gets the probabilities associated with the outcomes of this sequence.
        /// </summary>
        /// <value>The probabilities associated with the outcomes of this sequence.</value>
        public List<double> Probabilities {
            get { return probs; }
        }
        #endregion

        #region . Score .
        /// <summary>
        /// Gets the score of this sequence.
        /// </summary>
        /// <value>The score of this sequence.</value>
        public double Score { get; private set; }
        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified outcome with its probability to this sequence.
        /// </summary>
        /// <param name="outcome">The outcome to be added.</param>
        /// <param name="probability">The probability associated with this outcome.</param>
        public void Add(string outcome, double probability) {
            outcomes.Add(outcome);
            probs.Add(probability);
            Score += Math.Log(probability);
        }
        #endregion

        #region . Clone .
        /// <summary>
        /// Creates a new <see cref="T:Sequence"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Sequence"/> that is a copy of this instance.
        /// </returns>
        public object Clone() {
            return new Sequence(this);
        }
        #endregion

        #region . CompareTo .
        /// <summary>
        /// Compares the current sequence with another sequence.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the sequence being compared. The return value has the following meanings: Value Meaning Less than zero This sequence is less than the <paramref name="other"/> parameter.Zero This sequence is equal to <paramref name="other"/>. Greater than zero This sequence is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Sequence other) {
            return other.Score.CompareTo(Score);
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("{0} {1}", Score, outcomes.ToDisplay());
        }
        #endregion

    }
}
