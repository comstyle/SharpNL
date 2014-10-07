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
//  Note: 
//   This class is based/inspired on code extracted from the CoGrOO (http://cogroo.sourceforge.net/)
//   under Apache V2 license.
//

using System.Collections.Generic;
using System.Text;

namespace SharpNL.Text.Tree {
    /// <summary>
    /// Represents a tree node.
    /// </summary>
    public class TreeNode : TreeElement {
       
        #region + Static .

        #region . CreateTree .
        /// <summary>
        /// Creates the tree from the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <returns>TreeNode.</returns>
        public static TreeNode CreateTree(ISentence sentence) {

            var root = new TreeNode {
                Level = 0,
                SyntacticTag = "S"
            };

            var elements = TreeLeaf.CreateLeafs(sentence);
            var originalElements = elements.ToArray();


            var chunks = sentence.Chunks;
            var syntChunks = sentence.SyntacticChunks;

            for (int i = chunks.Count - 1; i >= 0; i--) {
                var node = new TreeNode {
                    SyntacticTag = chunks[i].Tag,
                    MorphologicalTag = null,
                    Level = 2
                };

                var nodeElements = new List<TreeElement>();
                for (int j = chunks[i].Start; j < chunks[i].End; j++) {
                    elements[j].Parent = node;
                    nodeElements.Add(elements[j]);
                }
                node.Elements = nodeElements.AsReadOnly();

                for (int j = chunks[i].End - 1; j >= chunks[i].Start; j--) {
                    elements.RemoveAt(j);
                }

                elements.Insert(chunks[i].Start, node);
            }

            foreach (var syntChunk in syntChunks) {
                var node = new TreeNode {
                    SyntacticTag = syntChunk.Tag,
                    MorphologicalTag = null,
                    Level = 1
                };

                var toRemove = new List<TreeElement>();
                var sons = new List<TreeElement>();

                for (int j = syntChunk.End - 1; j >= syntChunk.Start; j--) {
                    if (originalElements[j].Parent == null) {
                        sons.Insert(0, originalElements[j]);
                        toRemove.Add(originalElements[j]);
                        originalElements[j].Parent = node;
                    } else {
                        if (sons.Count == 0 || sons[0].Parent != originalElements[j].Parent) {
                            sons.Insert(0, originalElements[j].Parent);
                            toRemove.Add(originalElements[j].Parent);
                        }
                    }
                }

                node.Elements = sons.AsReadOnly();

                int index = elements.IndexOf(toRemove[toRemove.Count - 1]);
                while (index == -1 && toRemove.Count > 1) {
                    toRemove.RemoveAt(toRemove.Count - 1);
                    index = elements.IndexOf(toRemove[toRemove.Count - 1]);
                }

                elements.RemoveAll(toRemove.Contains);

                if (index >= 0)
                    elements.Insert(index, node);

                node.Parent = root;
            }

            root.Elements = elements.AsReadOnly();

            return root;
        }

        #endregion

        #endregion

        #region . Constructor .

        private TreeNode() {
            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class with the specified elements.
        /// </summary>
        /// <param name="elements">The node elements.</param>
        public TreeNode(List<TreeElement> elements) {
            Elements = elements.AsReadOnly();
        }
        #endregion

        #region + Properties .

        #region . Elements .
        /// <summary>
        /// Gets the node elements.
        /// </summary>
        /// <value>The node elements.</value>
        public IReadOnlyList<TreeElement> Elements { get; private set; }

        #endregion

        #region . SyntaxTree .

        private string syntaxTree;
        /// <summary>
        /// Gets the syntax tree.
        /// </summary>
        /// <value>The syntax tree.</value>
        public override string SyntaxTree {
            get {
                if (syntaxTree != null) 
                    return syntaxTree;

                var sb = new StringBuilder(Elements.Count * 7);
                sb.Append("[").Append(SyntacticTag).Append(" ");
                foreach (var treeElement in Elements) {
                    sb.Append(treeElement.SyntaxTree);
                    sb.Append(" ");
                }
                sb.Append("]");

                syntaxTree = sb.ToString();

                return syntaxTree;
            }
        }
        #endregion

        #region . TreeBank .

        private string treeBank;
        /// <summary>
        /// Gets the tree bank.
        /// </summary>
        /// <value>The tree bank.</value>
        public override string TreeBank {
            get {
                if (treeBank != null)
                    return treeBank;

                var sb = new StringBuilder(Elements.Count * 7);
                sb.Append("(").Append(SyntacticTag).Append(" ");
                foreach (var treeElement in Elements) {
                    sb.Append(treeElement.TreeBank);
                    sb.Append(" ");
                }
                sb.Append(")");

                treeBank = sb.ToString();

                return treeBank;
            }
        }
        #endregion

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current tree node.
        /// </summary>
        /// <returns>
        /// A string that represents the current tree node.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            sb.Append('=', Level);

            sb.Append(SyntacticTag);

            if (MorphologicalTag != null)
                sb.Append(MorphologicalTag);

            sb.AppendLine();

            foreach (var treeElement in Elements) {
                sb.Append(treeElement);
            }

            return sb.ToString();
        }
        #endregion

    }
}