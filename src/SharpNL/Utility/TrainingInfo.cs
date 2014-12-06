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

namespace SharpNL.Utility {
    /// <summary>
    /// This class is a temporary artifact that holds the training information,
    /// it is used to store some information as a comment in the model file.
    /// </summary>
    /// <remarks>This is a SharpNL implementation.</remarks>
    public sealed class TrainingInfo {
        private readonly StringBuilder sb;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingInfo"/> class.
        /// </summary>
        public TrainingInfo() {
            sb = new StringBuilder();
        }

        #region . Value .
        /// <summary>
        /// Gets the training information.
        /// </summary>
        /// <value>The training information.</value>
        public string Value {
            get { return sb.ToString(); }
        }
        #endregion

        #region . Append .
        /// <summary>
        /// Appends the string returned by processing a composite format string, which contains zero or more format items, to this instance. Each format item is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An array of objects to format.</param>
        public void Append(string format, params object[] args) {
            if (args.Length == 0)
                sb.Append(format);
            else
                sb.AppendFormat(format, args);
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current <see cref="TrainingInfo"/> object.
        /// </summary>
        /// <returns>
        /// A string that represents the current <see cref="TrainingInfo"/> object.
        /// </returns>
        public override string ToString() {
            return Value;
        }
        #endregion

    }
}