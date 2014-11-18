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

using System.Diagnostics;
using SharpNL.Utility;

namespace SharpNL.Parser {
    /// <summary>
    /// Class used to hold constituents when reading parses.
    /// </summary>
#if DEBUG
    [DebuggerDisplay("{Label} - {Span}")]
#endif
    public class Constituent {

        /// <summary>
        /// Initializes a new instance of the <see cref="Constituent"/>.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="span">The span.</param>
        public Constituent(string label, Span span) {
            Label = label;
            Span = span;
        }

        #region . Label .

        /// <summary>
        /// Gets or sets the label of the constituent.
        /// </summary>
        /// <value>The label of the constituent.</value>
        public string Label { get; set; }

        #endregion

        #region . Span .

        /// <summary>
        /// Gets the span of the constituent.
        /// </summary>
        /// <value>The span of the constituent.</value>
        public Span Span { get; private set; }

        #endregion
    }
}