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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace SharpNL.Gui.Controls.Diagram {
    /// <summary>
    /// Represents a tree diagram
    /// </summary>
    internal partial class Diagram : ScrollableControl {

        // Knuppe:
        // If you are going to copy my work, at least give-me a donation :)
        // Just remember the "prayer" above... and don't be a dick !

        private TreeNode hoverNode;

        public event EventHandler<TreeNode> HoverNode; 

        public Diagram() {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        #region + Properties .

        #region . NodeHovered .
        /// <summary>
        /// Gets the hovered node.
        /// </summary>
        /// <value>The hovered node.</value>
        public TreeNode NodeHovered {
            get { return hoverNode; }
        }
        #endregion

        #region . Root .

        private TreeNode root;

        /// <summary>
        /// Gets or sets the root node.
        /// </summary>
        /// <value>The root node.</value>
        public TreeNode Root {
            get {
                return root;
            }
            set {
                root = value;
                ArrangeTree();
            }
        }
        #endregion

        #endregion

        #region . ArrangeTree .
        private void ArrangeTree() {
            if (Root == null)
                return;

            using (var g = CreateGraphics()) {
                float xMin = 0, yMin = 0;
                Root.Arrange(g, ref xMin, ref yMin);

                AutoScrollMinSize = new Size((int)xMin + 20, (int)yMin + 20);

                if (Width > xMin + 20)
                    xMin = (ClientSize.Width - xMin)/2;
                else
                    xMin = 10;
                
                yMin = 10;
                Root.Arrange(g, ref xMin, ref yMin);
            }
            Refresh();
        }
        #endregion

        #region . OnPaint .
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            e.Graphics.Clear(BackColor);

            if (Root != null) {

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.TranslateTransform(
                    AutoScrollPosition.X,
                    AutoScrollPosition.Y);

                Root.DrawTree(e.Graphics);


            }
        }
        #endregion

        #region . OnMouseMove .
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (Root != null && HoverNode != null) {
                var node = Root.GetNodeAt(e.Location);
                if (node != null) {
                    if (hoverNode == null || hoverNode != node) {
                        hoverNode = node;

                        HoverNode(this, node);
                        return;
                    }
                } else {
                    hoverNode = null;
                }
            }
        }
        #endregion

        #region . OnMouseLeave .
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            hoverNode = null;
        }
        #endregion

        #region . OnResize .
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            ArrangeTree();
        }
        #endregion
        
    }
}