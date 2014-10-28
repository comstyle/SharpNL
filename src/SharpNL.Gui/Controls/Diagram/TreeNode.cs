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

using System.Drawing;
using System.Collections.Generic;

namespace SharpNL.Gui.Controls.Diagram {
    internal abstract class TreeNode {
        private const float hOffset = 12;
        private const float vOffset = 18;

        public List<TreeNode> Children = new List<TreeNode>();

        protected TreeNode() {
            Font = new Font("Verdana", 8);
            Border = Color.Black;
            ForeColor = SystemColors.ControlText;
            BackColor = Color.White;
            LinkColor = Color.Black;
        }

        public void AddChild(TreeNode child) {
            Children.Add(child);
        }

        #region + Properties .

        #region . BackColor .
        /// <summary>
        /// Gets or sets the background color of this node.
        /// </summary>
        /// <value>The background color of this node.</value>
        public Color BackColor { get; set; }
        #endregion

        #region . Border .
        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        /// <value>The border color.</value>
        public Color Border { get; set; }
        #endregion

        #region . Center .
        /// <summary>
        /// Gets the center position.
        /// </summary>
        /// <value>The center position.</value>
        protected PointF Center { get; private set; }
        #endregion

        #region . ForeColor .
        /// <summary>
        /// Gets or sets the color of the legend text.
        /// </summary>
        /// <value>The color of the legend text.</value>
        public Color ForeColor { get; set; }
        #endregion

        #region . Font .
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public Font Font { get; set; }
        #endregion

        #region . LinkColor .
        /// <summary>
        /// Gets or sets the color of the link line.
        /// </summary>
        /// <value>The color of the link line.</value>
        public Color LinkColor { get; set; }
        #endregion

        #endregion

        #region . IsHovered .
        /// <summary>
        /// Determines whether the node is hovered.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns><c>true</c> if the node is hovered; otherwise, <c>false</c>.</returns>
        protected abstract bool IsHovered(PointF pos);
        #endregion

        #region . GetSize .
        /// <summary>
        /// Gets the size of the node.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <returns>SizeF.</returns>
        protected abstract SizeF GetSize(Graphics g);
        #endregion

        #region . Draw .
        /// <summary>
        /// Draws the node.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object.</param>
        protected abstract void Draw(Graphics g);
        #endregion

        #region . Arrange .
        /// <summary>
        /// Arranges the node and its children.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="posX">The right edge.</param>
        /// <param name="posY">The bottom edge.</param>
        internal void Arrange(Graphics g, ref float posX, ref float posY) {
            var size = GetSize(g);

            var x = posX;
            var biggestYMin = posY + size.Height;
            var subYMin = posY + size.Height + vOffset;
            foreach (var child in Children) {
                var yMin = subYMin;
                child.Arrange(g, ref x, ref yMin);

                if (biggestYMin < yMin) biggestYMin = yMin;

                x += hOffset;
            }

            if (Children.Count > 0) x -= hOffset;

            var subWidth = x - posX;
            if (size.Width > subWidth) {
                x = posX + (size.Width - subWidth)/2;
                foreach (var child in Children) {
                    child.Arrange(g, ref x, ref subYMin);
                    x += hOffset;
                }
                subWidth = size.Width;
            }

            Center = new PointF(posX + subWidth/2, posY + size.Height/2);
            posX += subWidth;
            posY = biggestYMin;
        }

        #endregion

        #region + DrawTree .
        public void DrawTree(Graphics g) {
            DrawLinks(g);
            DrawNodes(g);
        }
        public void DrawTree(Graphics g, ref float x, ref float y) {
            Arrange(g, ref x, ref y);
            DrawTree(g);
        }
        #endregion

        #region . DrawLinks .
        /// <summary>
        /// Draws the links between the nodes.
        /// </summary>
        private void DrawLinks(Graphics g) {
            foreach (var child in Children) {
                DrawLink(g, child);

                child.DrawLinks(g);
            }
        }
        private void DrawLink(Graphics g, TreeNode child) {
            var h2 = (child.Center.Y - Center.Y) / 2;

            var pen = new Pen(LinkColor);

            g.DrawLine(pen, Center, new PointF(Center.X, Center.Y + h2));
            g.DrawLine(pen, child.Center, new PointF(child.Center.X, child.Center.Y - h2));
            g.DrawLine(pen, new PointF(Center.X, Center.Y + h2), new PointF(child.Center.X, child.Center.Y - h2));

        }
        #endregion

        #region . DrawNodes .
        private void DrawNodes(Graphics g) {

            Draw(g);

            foreach (var child in Children) {
                child.DrawNodes(g);
            }
        }
        #endregion

        #region . GetNodeAt .
        public TreeNode GetNodeAt(PointF pos) {
            if (IsHovered(pos))
                return this;

            foreach (var child in Children) {
                var node = child.GetNodeAt(pos);
                if (node != null)
                    return node;
            }

            return null;
        }
        #endregion

    }
}