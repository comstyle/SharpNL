namespace SharpNL.Gui.Viewers {
    partial class DocumentViewer {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentViewer));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShowColors = new System.Windows.Forms.ToolStripButton();
            this.mnuDoc = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuDocSentences = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocTokens = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocPoS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocEntities = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocChunker = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDocParser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClose = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.diagram = new SharpNL.Gui.Controls.Diagram.Diagram();
            this.richText = new SharpNL.Gui.Controls.RichLog();
            this.splitter = new System.Windows.Forms.Splitter();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.mnuShowColors,
            this.mnuDoc,
            this.mnuClose});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(750, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // mnuShowColors
            // 
            this.mnuShowColors.Checked = true;
            this.mnuShowColors.CheckOnClick = true;
            this.mnuShowColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuShowColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.mnuShowColors.Image = global::SharpNL.Gui.Properties.Resources.rainbow;
            this.mnuShowColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuShowColors.Name = "mnuShowColors";
            this.mnuShowColors.Size = new System.Drawing.Size(23, 22);
            this.mnuShowColors.Text = "&Show colors";
            this.mnuShowColors.Click += new System.EventHandler(this.mnuShowColors_Click);
            // 
            // mnuDoc
            // 
            this.mnuDoc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDocSentences,
            this.mnuDocTokens,
            this.mnuDocPoS,
            this.mnuDocEntities,
            this.mnuDocChunker,
            this.mnuDocParser});
            this.mnuDoc.Image = global::SharpNL.Gui.Properties.Resources.document;
            this.mnuDoc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuDoc.Name = "mnuDoc";
            this.mnuDoc.Size = new System.Drawing.Size(92, 22);
            this.mnuDoc.Text = "&Document";
            // 
            // mnuDocSentences
            // 
            this.mnuDocSentences.Name = "mnuDocSentences";
            this.mnuDocSentences.Size = new System.Drawing.Size(154, 22);
            this.mnuDocSentences.Text = "&Sentences";
            this.mnuDocSentences.Click += new System.EventHandler(this.mnuDocSentences_Click);
            // 
            // mnuDocTokens
            // 
            this.mnuDocTokens.Name = "mnuDocTokens";
            this.mnuDocTokens.Size = new System.Drawing.Size(154, 22);
            this.mnuDocTokens.Text = "&Tokens";
            this.mnuDocTokens.Click += new System.EventHandler(this.mnuDocTokens_Click);
            // 
            // mnuDocPoS
            // 
            this.mnuDocPoS.Name = "mnuDocPoS";
            this.mnuDocPoS.Size = new System.Drawing.Size(154, 22);
            this.mnuDocPoS.Text = "Part-of-&Speech";
            this.mnuDocPoS.Click += new System.EventHandler(this.mnuDocPoS_Click);
            // 
            // mnuDocEntities
            // 
            this.mnuDocEntities.Name = "mnuDocEntities";
            this.mnuDocEntities.Size = new System.Drawing.Size(154, 22);
            this.mnuDocEntities.Text = "&Entities";
            this.mnuDocEntities.Click += new System.EventHandler(this.mnuDocEntities_Click);
            // 
            // mnuDocChunker
            // 
            this.mnuDocChunker.Name = "mnuDocChunker";
            this.mnuDocChunker.Size = new System.Drawing.Size(154, 22);
            this.mnuDocChunker.Text = "&Chunker";
            this.mnuDocChunker.Click += new System.EventHandler(this.mnuDocChunker_Click);
            // 
            // mnuDocParser
            // 
            this.mnuDocParser.Name = "mnuDocParser";
            this.mnuDocParser.Size = new System.Drawing.Size(154, 22);
            this.mnuDocParser.Text = "&Parser";
            this.mnuDocParser.Click += new System.EventHandler(this.mnuDocParser_Click);
            // 
            // mnuClose
            // 
            this.mnuClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuClose.Image = ((System.Drawing.Image)(resources.GetObject("mnuClose.Image")));
            this.mnuClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuClose.Name = "mnuClose";
            this.mnuClose.Size = new System.Drawing.Size(40, 22);
            this.mnuClose.Text = "&Close";
            // 
            // statusStrip
            // 
            this.statusStrip.AllowMerge = false;
            this.statusStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.statInfo});
            this.statusStrip.Location = new System.Drawing.Point(0, 326);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 17, 0);
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.statusStrip.Size = new System.Drawing.Size(750, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(716, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statInfo
            // 
            this.statInfo.Name = "statInfo";
            this.statInfo.Size = new System.Drawing.Size(16, 17);
            this.statInfo.Text = "...";
            // 
            // diagram
            // 
            this.diagram.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(241)))), ((int)(((byte)(215)))));
            this.diagram.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.diagram.Location = new System.Drawing.Point(0, 168);
            this.diagram.Name = "diagram";
            this.diagram.Root = null;
            this.diagram.Size = new System.Drawing.Size(750, 158);
            this.diagram.TabIndex = 7;
            this.diagram.Text = "Diagram";
            this.diagram.Visible = false;
            this.diagram.DoubleClick += new System.EventHandler(this.diagram_DoubleClick);
            // 
            // richText
            // 
            this.richText.BackColor = System.Drawing.Color.White;
            this.richText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richText.Location = new System.Drawing.Point(0, 25);
            this.richText.Name = "richText";
            this.richText.ReadOnly = true;
            this.richText.Size = new System.Drawing.Size(750, 143);
            this.richText.TabIndex = 9;
            this.richText.Text = "";
            this.richText.SelectionChanged += new System.EventHandler(this.richText_SelectionChanged);
            // 
            // splitter
            // 
            this.splitter.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter.Location = new System.Drawing.Point(0, 166);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(750, 2);
            this.splitter.TabIndex = 10;
            this.splitter.TabStop = false;
            // 
            // DocumentViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.richText);
            this.Controls.Add(this.diagram);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DocumentViewer";
            this.Size = new System.Drawing.Size(750, 348);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton mnuDoc;
        private System.Windows.Forms.ToolStripMenuItem mnuDocSentences;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel statInfo;
        private System.Windows.Forms.ToolStripButton mnuShowColors;
        private System.Windows.Forms.ToolStripMenuItem mnuDocTokens;
        private System.Windows.Forms.ToolStripMenuItem mnuDocEntities;
        private System.Windows.Forms.ToolStripMenuItem mnuDocChunker;
        private System.Windows.Forms.ToolStripMenuItem mnuDocParser;
        private System.Windows.Forms.ToolStripMenuItem mnuDocPoS;
        private Controls.Diagram.Diagram diagram;
        private System.Windows.Forms.ToolStripButton mnuClose;
        private Controls.RichLog richText;
        private System.Windows.Forms.Splitter splitter;
    }
}
