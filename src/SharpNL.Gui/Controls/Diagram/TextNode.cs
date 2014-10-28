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

namespace SharpNL.Gui.Controls.Diagram {
    internal class TextNode : TreeNode {

        private RectangleF nodeRect;

        public TextNode(string text) {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            Text = text;
        }

        public string Text { get; private set; }


        protected override bool IsHovered(PointF pos) {
            return nodeRect.Contains(pos);
        }

        protected override SizeF GetSize(Graphics g) {
            return g.MeasureString(Text, Font) + new SizeF(5, 5);
        }

        protected override void Draw(Graphics g) {
            var x = Center.X;
            var y = Center.Y;

            var size = GetSize(g);

            nodeRect = new RectangleF(new PointF(x - size.Width / 2, y - size.Height / 2), size);

            using (var path = RoundRect(g, new Pen(Border), x - size.Width / 2, y - size.Height / 2, size.Width, size.Height, 5)) {

                // shadow
                using (var p2 = (GraphicsPath) path.Clone()) {
                    var matrix = new Matrix();
                    matrix.Translate(3, 3);
                    p2.Transform(matrix);

                    g.FillPath(new SolidBrush(Color.FromArgb(120, 185, 185, 185)), p2);
                }

                g.FillPath(new SolidBrush(Color.White), path);
                g.FillPath(new LinearGradientBrush(nodeRect, BackColor, Color.FromArgb(50, BackColor), LinearGradientMode.Vertical), path);

                g.DrawPath(new Pen(Border), path);              
            }

            // Draw the text.
            using (var sf = new StringFormat()) {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.DrawString(Text, Font, new SolidBrush(ForeColor), x, y, sf);
            }
        }

        internal GraphicsPath RoundRect(Graphics g, Pen p, float x, float y, float width, float height, float radius) {
            var path = new GraphicsPath();
            path.AddLine(x + radius, y, x + width - (radius*2), y); // Line
            path.AddArc(x + width - (radius*2), y, radius*2, radius*2, 270, 90); // Corner
            path.AddLine(x + width, y + radius, x + width, y + height - (radius*2)); // Line
            path.AddArc(x + width - (radius*2), y + height - (radius*2), radius*2, radius*2, 0, 90); // Corner
            path.AddLine(x + width - (radius*2), y + height, x + radius, y + height); // Line
            path.AddArc(x, y + height - (radius*2), radius*2, radius*2, 90, 90); // Corner
            path.AddLine(x, y + height - (radius*2), x, y + radius); // Line
            path.AddArc(x, y, radius*2, radius*2, 180, 90); // Corner
            path.CloseFigure();

            return path;
        }
    }
}