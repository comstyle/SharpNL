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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpNL.Stemmer.RSLP {
    /// <summary>
    /// Represents a RSLP step. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{StepName} ({Count} rules)")]
    public sealed class RSLPStep : IEnumerable<RSLPRule> {

        internal RSLPStep FlowPass;
        internal RSLPStep FlowFail;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="RSLPStep"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="minWordLen">Minimum length of the word.</param>
        /// <param name="entireWord">if set to <c>true</c> [entire word].</param>
        /// <param name="endWords">The end words.</param>
        public RSLPStep(string name, uint minWordLen, bool entireWord, params string[] endWords) {
            StepName = name;
            MinWordLen = minWordLen;
            EntireWord = entireWord;
            EndWords = endWords;
            Rules = new List<RSLPRule>();
        }
        #endregion

        #region + Properties .

        #region . StepName .
        /// <summary>
        /// Gets or sets the name of the step.
        /// </summary>
        public string StepName { get; private set; }
        #endregion

        #region . Count .
        /// <summary>
        /// Gets the number of rules in this step.
        /// </summary>
        /// <value>The number of rules in this step.</value>
        public int Count {
            get { return Rules.Count; }
        }
        #endregion

        #region . MinWordLen .
        /// <summary>
        /// Gets or sets the minimum length of the word.
        /// </summary>
        public uint MinWordLen { get; private set; }
        #endregion

        #region . EntireWord .
        /// <summary>
        /// Gets or sets a value indicating whether is needed to compare the entire word.
        /// </summary>
        /// <value><c>true</c> if is needed to compare the entire word; otherwise, <c>false</c>.</value>
        public bool EntireWord { get; private set; }
        #endregion

        #region . EndWords .
        /// <summary>
        /// Gets or sets the end of word conditions array: check words that end with these strings.
        /// </summary>
        public string[] EndWords { get; private set; }
        #endregion

        #region . Rules .
        /// <summary>
        /// Gets the rules.
        /// </summary>
        /// <value>The rules.</value>
        public List<RSLPRule> Rules { get; private set; }
        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <exception cref="System.ArgumentNullException">rule</exception>
        public void Add(RSLPRule rule) {
            if (rule == null)
                throw new ArgumentNullException("rule");

            Rules.Add(rule);
        }
        #endregion

        #region . GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<RSLPRule> GetEnumerator() {
            return Rules.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

    }
}