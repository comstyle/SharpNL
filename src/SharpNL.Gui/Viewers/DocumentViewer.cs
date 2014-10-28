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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpNL.Gui.Controls.Diagram;
using SharpNL.Parser;
using SharpNL.Text;
using TreeNode = SharpNL.Gui.Controls.Diagram.TreeNode;

namespace SharpNL.Gui.Viewers {
    public partial class DocumentViewer : Viewer {
        
        private enum Modes {
            None = 0,
            Sentence,
            Tokens,
            Entity,
            Chunker,
            Parse,
            PoS
        }
        

        private readonly Color[] colors = { Color.FromArgb(210, 243, 190), Color.FromArgb(181, 237, 255) };

        private List<Tuple<int, int, int, int>> PoS;
        public DocumentViewer(Document document) : base(document) {
            if (document == null)
                throw new ArgumentNullException("document");

            InitializeComponent();

            mnuDocSentences.Enabled = document.Sentences != null && document.Sentences.Count > 0;
            mnuDocTokens.Enabled = document.Tokenized;
            mnuDocEntities.Enabled = document.GetEntityCount > 0;
            mnuDocChunker.Enabled = document.Tokenized;
            mnuDocPoS.Enabled = document.PoS;
        }

        private Modes mode = Modes.None;
        
        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        protected Document Document {
            get { return (Document) SelectedObject; }
        }
        private void mnuDocSentences_Click(object sender, EventArgs e) {           
            try {
                Cursor = Cursors.WaitCursor;

                richText.BeginUpdate();

                ResetRichText();

                richText.Text = Document.Text;


                if (mnuShowColors.Checked) {
                    
                    var index = 0;
                    foreach (var sentence in Document.Sentences) {
                        richText.Highlight(sentence.Start, sentence.Length, colors[index++ % colors.Length]);
                    }
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.Sentence;
                richText.EndUpdate();
            }
        }
        private void mnuDocTokens_Click(object sender, EventArgs e) {
            try {
                Cursor = Cursors.WaitCursor;

                richText.BeginUpdate();

                ResetRichText();

                richText.Text = Document.Text;

                if (mnuShowColors.Checked) {

                    var index = 0;
                    foreach (var sentence in Document.Sentences) {
                        foreach (var token in sentence.Tokens) {
                            richText.Highlight(
                                sentence.Start + token.Start, token.Length, colors[index++ % colors.Length]);
                        }                       
                    }
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.Tokens;
                richText.EndUpdate();
            }
        }

        private void mnuDocEntities_Click(object sender, EventArgs e) {
            try {
                Cursor = Cursors.WaitCursor;

                richText.BeginUpdate();

                ResetRichText();

                richText.Text = Document.Text;

                if (mnuShowColors.Checked) {

                    var index = 0;
                    foreach (var sentence in Document.Sentences) {
                        foreach (var entity in sentence.Entities) {
                            var pos = entity.Span.GetSpanPosition(sentence);
                            
                            richText.Highlight(sentence.Start + pos.Start, pos.Length, colors[index++%colors.Length]);
                        }
                    }
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.Entity;
                richText.EndUpdate();
            }
        }

        private void ResetRichText(bool showDiagram = false) {
            diagram.Visible = showDiagram;

            richText.ResetForeColor();
            richText.ResetText();

            // richText.ResetBackColor(); mess with the docking position :(

            richText.BackColor = Color.White;
            richText.SelectionBackColor = Color.White;
            richText.SelectionColor = SystemColors.WindowText;
         
        }

        private void mnuDocChunker_Click(object sender, EventArgs e) {
            try {
                Cursor = Cursors.WaitCursor;
               
                richText.BeginUpdate();

                ResetRichText();

                richText.Text = Document.Text;

                if (mnuShowColors.Checked) {

                    var index = 0;
                    foreach (var sentence in Document.Sentences) {
                        foreach (var chunk in sentence.Chunks) {
                            var pos = chunk.Span.GetSpanPosition(sentence);

                            richText.Highlight(sentence.Start + pos.Start, pos.Length, colors[index++ % colors.Length]);
                        }                       
                    }
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.Chunker;
                richText.EndUpdate();
            }
        }
        private void mnuDocPoS_Click(object sender, EventArgs e) {
            try {
                Cursor = Cursors.WaitCursor;

                richText.BeginUpdate();

                ResetRichText();
                
                PoS = new List<Tuple<int, int, int, int>>();

                var colorDic = new Dictionary<string, Color>();

                for (var s = 0; s < Document.Sentences.Count; s++) {
                    for (var t = 0; t < Document.Sentences[s].Tokens.Count; t++) {
                        if (mnuShowColors.Checked) {
                            var tag = Document.Sentences[s].Tokens[t].POSTag ?? "?";
                            richText.SelectionBackColor = colorDic.ContainsKey(tag)
                                ? colorDic[tag]
                                : colorDic[tag] = GetRandomColor();
                        }

                        var tStart = richText.TextLength;

                        richText.AppendText(string.Format("{0}/ {1}",
                            Document.Sentences[s].Tokens[t].POSTag,
                            Document.Sentences[s].Tokens[t].Lexeme));

                        richText.SelectionBackColor = Color.White;
                        richText.AppendText(" ");

                        PoS.Add(new Tuple<int, int, int, int>(tStart, richText.TextLength, s, t));
                    }
                    richText.AppendText("\n\n");
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.PoS;
                richText.EndUpdate();
            }
        }
        private void mnuDocParser_Click(object sender, EventArgs e) {
            if (!Document.Parsed)
                return;

            try {
                Cursor = Cursors.WaitCursor;

                richText.BeginUpdate();

                ResetRichText(true);

                richText.Text = Document.Text;

                if (!mnuShowColors.Checked) 
                    return;

                var index = 0;
                foreach (var sentence in Document.Sentences) {
                    richText.Highlight(sentence.Start, sentence.Length, colors[index++ % colors.Length]);
                }
            } finally {
                Cursor = DefaultCursor;
                mode = Modes.Parse;
                richText.EndUpdate();
            }
        }
        private void richText_SelectionChanged(object sender, EventArgs e) {
            var pos = richText.SelectionStart;

            Text.Sentence sentence;
            switch (mode) {
                case Modes.Sentence:
                    for (var i = 0; i < Document.Sentences.Count; i++) {
                        sentence = Document.Sentences[i];
                        if (!pos.Between(sentence.Start, sentence.End, true))
                            continue;

                        lblStatus.Text = string.Format("Sentence {0} of {1} [{2}-{3} = {4} chars]", i + 1,
                            Document.Sentences.Count, sentence.Start, sentence.End, sentence.Length);
                        return;
                    }
                    break;
                case Modes.Tokens:
                    for (int i = 0; i < Document.Sentences.Count; i++) {
                        sentence = Document.Sentences[i];
                        if (!pos.Between(sentence.Start, sentence.End, true))
                            continue;

                        pos -= sentence.Start;

                        for (var t = 0; t < sentence.Tokens.Count; t++) {
                            var token = sentence.Tokens[t];
                            if (!pos.Between(token.Start, token.End, true))
                                continue;

                            lblStatus.Text = string.Format(
                                "Sentence {0} of {1} - token {2} of {3} (token probability: {4})",
                                i + 1,
                                Document.Sentences.Count,
                                t + 1,
                                sentence.Tokens.Count,
                                token.Probability);
                            break;
                        }
                        return; // further sentences
                    }
                    break;
                case Modes.Entity:
                    var entities = 1;

                    for (int i = 0; i < Document.Sentences.Count; i++) {
                        sentence = Document.Sentences[i];

                        if (!pos.Between(sentence.Start, sentence.End, true)) {
                            entities += sentence.Entities.Count;
                            continue;
                        }

                        pos -= sentence.Start;

                        foreach (var entity in sentence.Entities) {
                            var sp = entity.Span.GetSpanPosition(sentence);

                            if (!pos.Between(sp.Start, sp.End, true)) {
                                entities += 1;
                                continue;
                            }

                            var ent = entity as Entity;
                            if (ent != null) {
                                lblStatus.Text = string.Format(
                                    "Sentence {0} of {1} - Entity {2} of {3} (entity probability: {4})",
                                    i + 1,
                                    Document.Sentences.Count,
                                    entities,
                                    Document.GetEntityCount,
                                    ent.Probability);

                                return;
                            }
                            lblStatus.Text = string.Format(
                                "Sentence {0} of {1} - Entity {2} of {3} ({4})",
                                i + 1,
                                Document.Sentences.Count,
                                entities,
                                Document.GetEntityCount,
                                entity);
                        }
                        return; // further sentences
                    }
                    break;
                case Modes.Chunker:
                    for (int i = 0; i < Document.Sentences.Count; i++) {
                        sentence = Document.Sentences[i];

                        if (!pos.Between(sentence.Start, sentence.End, true))
                            continue;

                        pos -= sentence.Start;

                        foreach (var chunk in sentence.Chunks) {
                            var sp = chunk.Span.GetSpanPosition(sentence);

                            if (!pos.Between(sp.Start, sp.End, true))
                                continue;

                            lblStatus.Text = string.Format(
                                "Sentence {0} of {1} - {2}",
                                i + 1,
                                Document.Sentences.Count,
                                chunk);

                        }
                        return; // further sentences
                    }
                    break;    
                case Modes.Parse:
                    for (var i = 0; i < Document.Sentences.Count; i++) {
                        sentence = Document.Sentences[i];
                        if (!pos.Between(sentence.Start, sentence.End, true))
                            continue;

                        if (sentence.Parse == null) {
                            lblStatus.Text = @"Not parsed";
                            return;
                        }

                        var root = new TextNode(sentence.Parse.Type) {
                            BackColor = Color.SteelBlue
                        };

                        AddParseNode(root, sentence.Parse.Children );

                        diagram.Root = root;

                        lblStatus.Text = string.Format("Sentence {0} of {1} [{2}-{3} = {4} chars]", i + 1,
                            Document.Sentences.Count, sentence.Start, sentence.End, sentence.Length);
                        return;
                    }
                    break;
                case Modes.PoS:
                    if (PoS == null || PoS.Count == 0)
                        return;

                    foreach (var p in PoS) {                       
                        if (!pos.Between(p.Item1, p.Item2))
                            continue;

                        var token = Document.Sentences[p.Item3].Tokens[p.Item4];
                        
                        lblStatus.Text = string.Format("Part-of-Speech: {0}/ {1} - (Sentence: #{2}  Token: #{3}  Probability: {4})", 
                            token.POSTag,
                            token.Lexeme,
                            p.Item3 + 1,
                            p.Item4 + 1,
                            token.POSTagProbability);

                        return;
                    }

                    lblStatus.Text = string.Empty;

                    return;

            }
        }

        private void AddParseNode(TreeNode node, IEnumerable<Parse> parses) {
            foreach (var parse in parses) {
                if (parse.ChildCount > 0) {
                    var child = new TextNode(parse.Type) {
                        BackColor = parse.IsPosTag ? Color.DarkGoldenrod : Color.SteelBlue
                    };

                    node.AddChild(child);

                    AddParseNode(child, parse.Children);
                } else {
                    node.AddChild(new TextNode(parse.CoveredText) { BackColor = Color.Ivory });    
                }               
            }
        }

        private void mnuShowColors_Click(object sender, EventArgs e) {
            switch (mode) {
                case Modes.None:
                    return;
                case Modes.Sentence:
                    mnuDocSentences_Click(this, EventArgs.Empty);
                    return;
                case Modes.Tokens:
                    mnuDocTokens_Click(this, EventArgs.Empty);
                    return;
                case Modes.Entity:
                    mnuDocEntities_Click(this, EventArgs.Empty);
                    return;
                case Modes.Chunker:
                    mnuDocChunker_Click(this, EventArgs.Empty);
                    return;
                case Modes.Parse:
                    mnuDocParser_Click(this, EventArgs.Empty);
                    return;
                case Modes.PoS:
                    mnuDocPoS_Click(this, EventArgs.Empty);
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        

        private static readonly Random rnd = new Random();
        private static Color GetRandomColor() {
            // plastic colors should be good :)
            return Color.FromArgb(rnd.Next(64, 128) + 127, rnd.Next(64, 128) + 127, rnd.Next(64, 128) + 127);
        }

        private void diagram_DoubleClick(object sender, EventArgs e) {
            if (ParentForm == null || diagram.Root == null)
                return;

            var control = new Diagram {
                    Dock = DockStyle.Fill,
                    BackColor = diagram.BackColor,
                    Root = diagram.Root,
                    Visible = true
                };

            control.DoubleClick += (o, args) => {
                var d = o as Diagram;
                if (d != null) {
                    var form = d.Parent as Form;
                    if (form != null) {
                        form.Controls.Remove(d);

                        d.Visible = false;
                        d.Dispose();
                    }
                }
            };

            ParentForm.Controls.Add(control);

            control.BringToFront();
        }
    }
}