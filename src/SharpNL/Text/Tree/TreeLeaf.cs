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

namespace SharpNL.Text.Tree {
    /// <summary>
    /// Represents a leaf element.
    /// </summary>
    public class TreeLeaf : TreeElement {

        private string word;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeLeaf"/> class.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="lemma">The lemma.</param>
        public TreeLeaf(string word, string[] lemma) {
            this.word = word;
            Lemma = lemma;
        }

        #region + Static .

        #region . CreateLeafs .
        /// <summary>
        /// Creates the leaf list from the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <returns>The leaf list.</returns>
        internal static List<TreeElement> CreateLeafs(ISentence sentence) {
            var tokens = sentence.Tokens;
            var leafs = new List<TreeElement>(tokens.Count);

            foreach (var token in tokens) {
                leafs.Add(new TreeLeaf(token.Lexeme, token.Lemmas) {
                    Level = 3,
                    MorphologicalTag = token.POSTag,
                    FeatureTag = token.Features
                });
            }

            return leafs;
        }
        #endregion

        #endregion

        #region + Properties .

        #region . Lexeme .
        /// <summary>
        /// Gets or sets the lexeme.
        /// </summary>
        /// <value>The lexeme.</value>
        public string Lexeme {
            get { return word; }
            set { word = value; }
        }

        #endregion

        #region . Lemma .
        /// <summary>
        /// Gets or sets the lemma.
        /// </summary>
        /// <value>The lemma.</value>
        public string[] Lemma { get; set; }
        #endregion

        #region . SyntaxTree .

        private string syntaxTree;
        /// <summary>
        /// Gets the leaf syntax tree.
        /// </summary>
        /// <value>The leaf syntax tree.</value>
        public override string SyntaxTree {
            get {
                if (syntaxTree != null)
                    return syntaxTree;

                syntaxTree = string.Format("[{0} {1}]", MorphologicalTag, word);

                return syntaxTree;
            }
        }
        #endregion

        #region . TreeBank .

        private string treeBank;
        /// <summary>
        /// Gets the leaf tree bank.
        /// </summary>
        /// <value>The leaf tree bank.</value>
        public override string TreeBank {
            get {
                if (treeBank != null)
                    return treeBank;

                return (treeBank = string.Format("({0} {1})", CreateTag(), word));
            }
        }
        #endregion

        #endregion

        #region . CreateTag .
        /// <summary>
        /// Creates the leaf tag.
        /// </summary>
        /// <returns>The leaf tag.</returns>
        private string CreateTag() {

            if (Lexeme == MorphologicalTag)
                return "PUNCT";

            var hasLemma = Lemma != null && Lemma.Length > 0;
            var hasFeats = FeatureTag != null && FeatureTag != "-";

            var sb = new StringBuilder(MorphologicalTag.Replace("-", string.Empty));

            if (hasLemma || hasFeats) {
                sb.Append("-");
                sb.Append(hasFeats ? FeatureTag : "*");

                if (!hasLemma) 
                    return sb.ToString();

                sb.Append("-");
                sb.Append(string.Join("|", Lemma));
            }

            return sb.ToString();
        }
        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current leaf element.
        /// </summary>
        /// <returns>
        /// A string that represents the current leaf element.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            sb.Append('=', Level);

            if (SyntacticTag != null)
                sb.Append(SyntacticTag).Append("(").Append(MorphologicalTag).Append(") ");

            sb.AppendLine(word);

            return sb.ToString();
        }

        #endregion

    }
}