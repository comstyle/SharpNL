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

using System.Globalization;

namespace SharpNL.Utility.Evaluation {
    /// <summary>
    /// Calculates the arithmetic mean of values added with the <see cref="M:Add"/> method.
    /// </summary>
    public class Mean {

        #region + Properties .

        #region + Count .

        /// <summary>
        /// Gets the number of times a value was added to the mean.
        /// </summary>
        /// <value>The count.</value>
        public long Count { get; private set; }

        #endregion

        #region . Sum .

        /// <summary>
        /// Gets the sum.
        /// </summary>
        /// <value>The sum.</value>
        public double Sum { get; private set; }

        #endregion

        #region . Value .

        /// <summary>
        /// Gets the mean of all values added or 0 if there are zero added values.
        /// </summary>
        /// <value>The mean of all values added or 0 if there are zero added values.</value>
        public double Value {
            get { return Count > 0 ? Sum/Count : 0; }
        }

        #endregion

        #endregion

        #region + Add .

        /// <summary>
        /// Adds a value to the arithmetic mean.
        /// </summary>
        /// <param name="value">The value which should be added to the arithmetic mean..</param>
        public void Add(double value) {
            Add(value, 1);
        }

        /// <summary>
        /// Adds a value count times to the arithmetic mean.
        /// </summary>
        /// <param name="value">The value which should be added to the arithmetic mean.</param>
        /// <param name="count">Number of times the value should be added to arithmetic mean.</param>
        public void Add(double value, long count) {
            Sum += value*count;
            Count += count;
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
            return Value.ToString(CultureInfo.InvariantCulture);
        }
        #endregion

    }
}