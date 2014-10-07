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
using System.Globalization;
using System.Text;

namespace SharpNL.ML.Model {
    /// <summary>
    /// A maxent event representation which we can use to sort based on the predicates indexes contained in the events.
    /// </summary>
    public class ComparableEvent : IComparable<ComparableEvent> {
        public int outcome;
        public int[] predIndexes;
        public int seen = 1; // the number of times this event has been seen.

        public float[] values;

        public ComparableEvent(int oc, int[] pids) :this(oc, pids, null) { }

        public ComparableEvent(int oc, int[] pids, float[] values) {
            outcome = oc;
            if (values == null) {
                Array.Sort(pids);
            } else {
                Sort(pids, values);
            }
            this.values = values; // needs to be sorted like pids
            predIndexes = pids;
        }


        #region . CompareTo .
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(ComparableEvent other) {
            if (outcome < other.outcome)
                return -1;

            if (outcome > other.outcome)
                return 1;

            var smallerLength = (predIndexes.Length > other.predIndexes.Length ? other.predIndexes.Length
                : predIndexes.Length);

            for (int i = 0; i < smallerLength; i++) {
                if (predIndexes[i] < other.predIndexes[i])
                    return -1;

                if (predIndexes[i] > other.predIndexes[i])
                    return 1;

                if (values != null && other.values != null) {
                    if (values[i] < other.values[i])
                        return -1;
                    if (values[i] > other.values[i])
                        return 1;
                } else if (values != null) {
                    if (values[i] < 1)
                        return -1;
                    if (values[i] > 1)
                        return 1;
                } else if (other.values != null) {
                    if (1 < other.values[i])
                        return -1;
                    if (1 > other.values[i])
                        return 1;
                }
            }

            if (predIndexes.Length < other.predIndexes.Length)
                return -1;

            if (predIndexes.Length > other.predIndexes.Length)
                return 1;

            return 0;
        }
        #endregion

        #region . Sort .
        private static void Sort(int[] pids, float[] values) {
            for (int mi = 0; mi < pids.Length; mi++) {
                int min = mi;
                for (int pi = mi + 1; pi < pids.Length; pi++) {
                    if (pids[min] > pids[pi]) {
                        min = pi;
                    }
                }
                int pid = pids[mi];
                pids[mi] = pids[min];
                pids[min] = pid;
                float val = values[mi];
                values[mi] = values[min];
                values[min] = val;
            }
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current event.
        /// </summary>
        /// <returns>
        /// A string that represents the current event.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append(outcome.ToString(CultureInfo.InvariantCulture)).Append(":");

            for (int i = 0; i < predIndexes.Length; i++) {
                sb.Append(" ").Append(predIndexes[i]);
                if (values != null) {
                    sb.Append("=").Append(values[i]);
                }
            }
            return sb.ToString();
        }
        #endregion

    }
}