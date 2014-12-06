// 
//  Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//   - May you do good and not evil.                                         -
//   - May you find forgiveness for yourself and forgive others.             -
//   - May you share freely, never taking more than you give.                -
//   - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  

using System;
using System.Text;

namespace SharpNL.ML.Model {
    /// <summary>
    /// A maxent predicate representation which we can use to sort based on the
    /// outcomes. This allows us to make the mapping of features to their parameters
    /// much more compact.
    /// </summary>
    public class ComparablePredicate : IComparable<ComparablePredicate> {

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparablePredicate"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="outcomes">The outcomes.</param>
        /// <param name="parameters">The parameters.</param>
        public ComparablePredicate(string name, int[] outcomes, double[] parameters) {
            Name = name;
            Outcomes = outcomes;
            Parameters = parameters;
        }
        #endregion

        #region + Properties .

        #region . Name .
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
        #endregion

        #region . Outcomes .
        /// <summary>
        /// Gets the outcomes.
        /// </summary>
        /// <value>The outcomes.</value>
        public int[] Outcomes { get; private set; }
        #endregion

        #region . Parameters .
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public double[] Parameters { get; private set; }
        #endregion

        #endregion

        #region . CompareTo .
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ComparablePredicate other) {
            var smallerLength = Math.Min(Outcomes.Length, other.Outcomes.Length);

            for (int i = 0; i < smallerLength; i++) {
                if (Outcomes[i] < other.Outcomes[i]) return -1;
                if (Outcomes[i] > other.Outcomes[i]) return 1;
            }

            if (Outcomes.Length < other.Outcomes.Length) return -1;
            if (Outcomes.Length > other.Outcomes.Length) return 1;

            return 0;
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
            var sb = new StringBuilder();
            foreach (var outcome in Outcomes) {
                sb.AppendFormat(" {0}", outcome);
            }
            return sb.ToString();
        }
        #endregion

    }
}