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
using System.Text;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Represents a Ad node.
    /// </summary>
    public class AdNode : AdTreeElement {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdNode"/> class.
        /// </summary>
        public AdNode() {
            Elements = new List<AdTreeElement>();
        }

        #region . Elements .
        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <value>The elements.</value>
        public List<AdTreeElement> Elements { get; private set; }
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

            sb.Append(new string('=', Level));
            sb.Append(SyntacticTag);
            if (!string.IsNullOrEmpty(MorphologicalTag)) {
                sb.Append(MorphologicalTag);
            }
            sb.Append("\n");
            foreach (var element in Elements) {
                sb.Append(element);
            }

            return sb.ToString();
        }

        #endregion

        #region . AddElement .
        /// <summary>
        /// Adds an element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AddElement(AdTreeElement element) {
            Elements.Add(element);
        }
        #endregion

    }
}