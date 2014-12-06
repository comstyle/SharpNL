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

using System.Text;

namespace SharpNL.ML.Model {
    /// <summary>
    /// The context of a decision point during training.
    /// This includes contextual predicates and an outcome.
    /// </summary>
    public class Event {

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> with a outcome and its context.
        /// </summary>
        /// <param name="outcome">The event outcome.</param>
        /// <param name="context">The event context.</param>
        public Event(string outcome, string[] context) {
            Outcome = outcome;
            Context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> with the specified outcome, context and values.
        /// </summary>
        /// <param name="outcome">The event outcome.</param>
        /// <param name="context">The event context.</param>
        /// <param name="values">The event values.</param>
        public Event(string outcome, string[] context, float[] values) : this(outcome, context) {
            Values = values;
        }

        #region . Context .
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public string[] Context { get; private set; }
        #endregion

        #region . Outcome .
        /// <summary>
        /// Gets the outcome.
        /// </summary>
        /// <value>The outcome.</value>
        public string Outcome { get; private set; }
        #endregion

        #region . Values .

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public float[] Values { get; private set; }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {

            var sb = new StringBuilder(Outcome + " [");

            if (Context.Length > 0) {
                sb.Append(Context[0]);
                if (Values != null) {
                    sb.AppendFormat("={0}", Values[0]);
                }
            }
            for (int i = 0; i < Context.Length; i++) {
                sb.AppendFormat(" {0}", Context[i]);
                if (Values != null && Values.Length >= i) {
                    sb.AppendFormat("={0}", Values[i]);
                }
            }
            sb.Append("]");
            return sb.ToString();
        }
        #endregion


    }
}