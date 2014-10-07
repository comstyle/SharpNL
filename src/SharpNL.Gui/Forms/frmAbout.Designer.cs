namespace SharpNL.Gui.Forms {
    partial class frmAbout {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.picDonate = new System.Windows.Forms.PictureBox();
            this.picAwesomeness = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lnkLicense = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.lnkSharpNL = new System.Windows.Forms.LinkLabel();
            this.lnkOpenNLP = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblVerGui = new System.Windows.Forms.Label();
            this.lblVerLib = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblNote = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picDonate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAwesomeness)).BeginInit();
            this.SuspendLayout();
            // 
            // picDonate
            // 
            this.picDonate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picDonate.Image = global::SharpNL.Gui.Properties.Resources.paypal;
            this.picDonate.Location = new System.Drawing.Point(350, 387);
            this.picDonate.Name = "picDonate";
            this.picDonate.Size = new System.Drawing.Size(48, 29);
            this.picDonate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picDonate.TabIndex = 1;
            this.picDonate.TabStop = false;
            this.picDonate.Click += new System.EventHandler(this.picDonate_Click);
            this.picDonate.MouseEnter += new System.EventHandler(this.picDonate_MouseEnter);
            this.picDonate.MouseLeave += new System.EventHandler(this.picDonate_MouseLeave);
            // 
            // picAwesomeness
            // 
            this.picAwesomeness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picAwesomeness.Image = global::SharpNL.Gui.Properties.Resources.avatarMrBean;
            this.picAwesomeness.Location = new System.Drawing.Point(0, 331);
            this.picAwesomeness.Name = "picAwesomeness";
            this.picAwesomeness.Size = new System.Drawing.Size(96, 96);
            this.picAwesomeness.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picAwesomeness.TabIndex = 0;
            this.picAwesomeness.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "Licensed under the Apache License, Version 2.0 (the \"License\");\r\nyou may not use " +
    "this file except in compliance with the License.\r\nYou may obtain a copy of the L" +
    "icense at";
            // 
            // lnkLicense
            // 
            this.lnkLicense.AutoSize = true;
            this.lnkLicense.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkLicense.Location = new System.Drawing.Point(8, 203);
            this.lnkLicense.Name = "lnkLicense";
            this.lnkLicense.Size = new System.Drawing.Size(252, 15);
            this.lnkLicense.TabIndex = 3;
            this.lnkLicense.TabStop = true;
            this.lnkLicense.Text = "http://www.apache.org/licenses/LICENSE-2.0";
            this.lnkLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLicense_LinkClicked);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(497, 66);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Lucida Sans Unicode", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(153)))));
            this.label3.Location = new System.Drawing.Point(6, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(167, 28);
            this.label3.TabIndex = 4;
            this.label3.Text = "SharpNL.Gui";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(9, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(500, 31);
            this.label4.TabIndex = 2;
            this.label4.Text = "SharpNL is an independent reimplementation of the Apache OpenNLP software library" +
    " which is used as a machine learning based toolkit for the processing of natural" +
    " language text.";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.Location = new System.Drawing.Point(9, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 1);
            this.panel1.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.DimGray;
            this.label5.Location = new System.Drawing.Point(7, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(204, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Copyright © 2014, Gustavo J KNUPPE";
            // 
            // lnkSharpNL
            // 
            this.lnkSharpNL.AutoSize = true;
            this.lnkSharpNL.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkSharpNL.Location = new System.Drawing.Point(296, 96);
            this.lnkSharpNL.Name = "lnkSharpNL";
            this.lnkSharpNL.Size = new System.Drawing.Size(209, 15);
            this.lnkSharpNL.TabIndex = 7;
            this.lnkSharpNL.TabStop = true;
            this.lnkSharpNL.Text = "https://github.com/knuppe/SharpNL";
            this.lnkSharpNL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSharpNL_LinkClicked);
            // 
            // lnkOpenNLP
            // 
            this.lnkOpenNLP.AutoSize = true;
            this.lnkOpenNLP.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkOpenNLP.Location = new System.Drawing.Point(296, 76);
            this.lnkOpenNLP.Name = "lnkOpenNLP";
            this.lnkOpenNLP.Size = new System.Drawing.Size(163, 15);
            this.lnkOpenNLP.TabIndex = 7;
            this.lnkOpenNLP.TabStop = true;
            this.lnkOpenNLP.Text = "https://opennlp.apache.org/";
            this.lnkOpenNLP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkOpenNLP_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(190, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Apache OpenNLP:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label7.Location = new System.Drawing.Point(30, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 15);
            this.label7.TabIndex = 8;
            this.label7.Text = "Gui version:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.label8.Location = new System.Drawing.Point(10, 96);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 15);
            this.label8.TabIndex = 9;
            this.label8.Text = "Library version:";
            // 
            // lblVerGui
            // 
            this.lblVerGui.AutoSize = true;
            this.lblVerGui.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblVerGui.Location = new System.Drawing.Point(102, 76);
            this.lblVerGui.Name = "lblVerGui";
            this.lblVerGui.Size = new System.Drawing.Size(64, 15);
            this.lblVerGui.TabIndex = 10;
            this.lblVerGui.Text = "0.0.0 (dev)";
            // 
            // lblVerLib
            // 
            this.lblVerLib.AutoSize = true;
            this.lblVerLib.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.lblVerLib.Location = new System.Drawing.Point(102, 96);
            this.lblVerLib.Name = "lblVerLib";
            this.lblVerLib.Size = new System.Drawing.Size(64, 15);
            this.lblVerLib.TabIndex = 11;
            this.lblVerLib.Text = "0.0.0 (dev)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(237, 96);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 15);
            this.label9.TabIndex = 12;
            this.label9.Text = "SharpNL:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.Location = new System.Drawing.Point(5, 299);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(500, 1);
            this.panel2.TabIndex = 13;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(429, 388);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 27);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblNote
            // 
            this.lblNote.Location = new System.Drawing.Point(102, 308);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(407, 116);
            this.lblNote.TabIndex = 2;
            this.lblNote.Text = resources.GetString("lblNote.Text");
            // 
            // frmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(525, 427);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblVerLib);
            this.Controls.Add(this.lblVerGui);
            this.Controls.Add(this.lnkOpenNLP);
            this.Controls.Add(this.lnkSharpNL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lnkLicense);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picDonate);
            this.Controls.Add(this.picAwesomeness);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblNote);
            this.Font = new System.Drawing.Font("Lucida Sans Unicode", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.frmAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picDonate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAwesomeness)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picAwesomeness;
        private System.Windows.Forms.PictureBox picDonate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel lnkLicense;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.LinkLabel lnkSharpNL;
        private System.Windows.Forms.LinkLabel lnkOpenNLP;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblVerGui;
        private System.Windows.Forms.Label lblVerLib;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblNote;
    }
}