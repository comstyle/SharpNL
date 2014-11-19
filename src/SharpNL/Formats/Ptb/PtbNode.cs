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
using System.Linq;
using System.Text;
using SharpNL.Formats.Ptb.Lang;
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {

    /// <summary>
    /// Represents a Penn Treebank node. This class cannot be inherited.
    /// </summary>
    public sealed class PtbNode {
        private List<PtbNode> children;

        #region + Properties .

        #region . Children .
        /// <summary>
        /// Gets a list of the children of this node.
        /// </summary>
        /// <value>Returns a list of the children of this node.</value>
        public List<PtbNode> Children {
            get { return children ?? (children = new List<PtbNode>()); }
        }
        #endregion

        #region . GetTokens .
        /// <summary>
        /// Returns a list of tokens in this node (including its child nodes).
        /// </summary>
        /// <param name="list">The token list.</param>
        private void GetTokens(ICollection<string> list) {
            if (Token != null)
                list.Add(Token);

            if (!HasChildren) 
                return;

            foreach (var child in children)
                child.GetTokens(list);
        }
        #endregion

        #region . HasChildren .
        /// <summary>
        /// Gets a value indicating whether the node contains one or more child nodes.
        /// </summary>
        /// <value><c>true</c> if the control node one or more child nodes; otherwise, <c>false</c>.</value>
        public bool HasChildren {
            get {
                if (children == null)
                    return false;

                return children.Count > 0;
            }
        }
        #endregion

        #region . IsChunk .
        /// <summary>
        /// Gets a value indicating whether all children nodes are tagged as part of speech or if this node has no children
        /// and is tagged as part of speech.
        /// </summary>
        /// <value><c>true</c> if this node is a chunk node; otherwise, <c>false</c>.</value>
        public bool IsChunk {
            get {
                return HasChildren 
                    ? Children.All(child => child.IsPosTag) 
                    : IsPosTag;
            }
        }
        #endregion

        #region . IsPosTag .
        /// <summary>
        /// Gets a value indicating whether this node is tagged as part of speech.
        /// </summary>
        /// <value><c>true</c> if this node is tagged as part of speech; otherwise, <c>false</c>.</value>
        public bool IsPosTag {
            get { return Token != null && Type != "-NONE-"; }
        }
        #endregion
        
        #region . Span .
        /// <summary>
        /// Gets or sets the node span.
        /// </summary>
        /// <value>The node span.</value>
        public Span Span { get; set; }
        #endregion

        #region . Show .
        private void Show(StringBuilder sb) {
            if (HasChildren) {
                sb.AppendFormat("({0} ", Type);
                foreach (var child in Children) {
                    child.Show(sb);
                }
                sb.Append(')');
            } else {
                if (Token != null)
                    sb.AppendFormat("({0} {1})", Type, DefaultResolver.EncodeToken(Token));
                else
                    sb.AppendFormat("({0})", Type);
            }
        }
        #endregion

        #region . Type .
        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        /// <value>The node type.</value>
        public string Type { get; set; }
        #endregion

        #region . Types .
        /// <summary>
        /// Gets the child tags.
        /// </summary>
        /// <value>The child tags.</value>
        public string[] Types {
            get {
                return HasChildren
                    ? children.Select(child => child.Type).ToArray()
                    : null;
            }
        }
        #endregion

        #region . Token .
        /// <summary>
        /// Gets or sets the node token.
        /// </summary>
        /// <value>The node token.</value>
        public string Token { get; set; }
        #endregion

        #region . Tokens .
        /// <summary>
        /// Gets the tokens in this node.
        /// </summary>
        /// <value>The tokens in this node.</value>
        public string[] Tokens {
            get {
                if (Token == null && !HasChildren)
                    return null;

                var list = new List<string>();

                GetTokens(list);

                return list.ToArray();
            }
        }
        #endregion

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current node.
        /// </summary>
        /// <returns>
        /// A string that represents the current node.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();

            Show(sb);

            return sb.ToString();
        }
        #endregion

    }
}