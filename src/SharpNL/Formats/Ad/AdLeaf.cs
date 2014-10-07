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

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Represents the AD leaf.
    /// </summary>
    public class AdLeaf : AdTreeElement {

        #region + Properties .

        #region . FunctionalTag .

        /// <summary>
        /// Gets or sets the functional tag.
        /// </summary>
        /// <value>The functional tag.</value>
        public string FunctionalTag { get; set; }

        #endregion

        #region . IsLeaf .

        /// <summary>
        /// Gets a value indicating whether this element is a leaf.
        /// </summary>
        /// <value><c>true</c> if this element is a leaf; otherwise, <c>false</c>.</value>
        public override bool IsLeaf {
            get { return true; }
        }

        #endregion

        #region . Lemma .

        /// <summary>
        /// Gets or sets the lemma.
        /// </summary>
        /// <value>The lemma.</value>
        public string Lemma { get; set; }

        #endregion

        #region . Lexme .

        /// <summary>
        /// Gets or sets the lexeme.
        /// </summary>
        /// <value>The lexeme.</value>
        public string Lexeme { get; set; }

        #endregion

        #region . SecondaryTag .

        /// <summary>
        /// Gets or sets the secondary tag.
        /// </summary>
        /// <value>The secondary tag.</value>
        public string SecondaryTag { get; set; }

        #endregion

        #endregion

        #region . EmptyOrString .

        private static string EmptyOrString(string value, string prefix, string suffix) {
            if (string.IsNullOrEmpty(value)) {
                return string.Empty;
            }
            return value + prefix + suffix;
        }

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current leaf.
        /// </summary>
        /// <returns>
        /// A string that represents the current leaf.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            sb.Append(new string('=', Level));

            if (!string.IsNullOrEmpty(SyntacticTag)) {
                sb.AppendFormat("{0}:{1}({2}{3}{4}) ",
                    SyntacticTag,
                    FunctionalTag,
                    EmptyOrString(Lemma, "'", "' "),
                    EmptyOrString(SecondaryTag, string.Empty, " "),
                    MorphologicalTag);
            }
            sb.AppendLine(Lexeme);

            return sb.ToString();
        }
        #endregion

    }
}