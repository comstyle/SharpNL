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

using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNL.Stemmer.RSLP {
    /// <summary>
    /// Represents a RSLP rule. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{Suffix}")]
    public sealed class RSLPRule {

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="RSLPRule"/> class.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <param name="minSize">The minimum size.</param>
        public RSLPRule(string suffix, int minSize) {
            Suffix = suffix;
            MinStemSize = minSize;
            Replacement = string.Empty;
            Exceptions = null;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RSLPRule"/> class.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="replacement">The replacement.</param>
        public RSLPRule(string suffix, int minSize, string replacement) {
            Suffix = suffix;
            MinStemSize = minSize;
            Replacement = replacement;
            Exceptions = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RSLPRule"/> class.
        /// </summary>
        /// <param name="suffix">The suffix.</param>
        /// <param name="minSize">The minimum size.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="exceptions">The exceptions.</param>
        public RSLPRule(string suffix, int minSize, string replacement, params string[] exceptions) {
            Suffix = suffix;
            MinStemSize = minSize;
            Replacement = replacement;
            Exceptions = new HashSet<string>(exceptions);
        }
        #endregion

        #region + Properties .

        #region . Exceptions .
        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>The exceptions.</value>
        public HashSet<string> Exceptions { get; private set; }
        #endregion

        #region . Suffix .
        /// <summary>
        /// Gets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        public string Suffix { get; private set; }
        #endregion

        #region . MinStemSize .
        /// <summary>
        /// Gets the minimum size of the stem.
        /// </summary>
        /// <value>The minimum size of the stem.</value>
        public int MinStemSize { get; private set; }
        #endregion

        #region . Replacement .
        /// <summary>
        /// Gets the replacement string.
        /// </summary>
        /// <value>The replacement string.</value>
        public string Replacement { get; private set; }
        #endregion

        #endregion

    }
}