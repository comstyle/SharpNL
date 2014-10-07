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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SharpNL.Gui.Properties;

namespace SharpNL.Gui.Forms {
    public partial class frmAbout : Form {
        private readonly Bitmap hoveDonation;

        public frmAbout() {
            InitializeComponent();

            hoveDonation = ConvertToGrayscale(Resources.paypal);
        }

        public Bitmap ConvertToGrayscale(Bitmap source) {
            var bm = new Bitmap(source.Width, source.Height);
            for (var y = 0; y < bm.Height; y++) {
                for (var x = 0; x < bm.Width; x++) {
                    var c = source.GetPixel(x, y);
                    var luma = (int) (c.R*0.3 + c.G*0.59 + c.B*0.11);
                    bm.SetPixel(x, y, Color.FromArgb(luma, luma, luma));
                }
            }
            return bm;
        }

        private void frmAbout_Load(object sender, EventArgs e) {
            picDonate.Image = hoveDonation;

#if DEBUG
            lblVerGui.Text = Assembly.GetExecutingAssembly().GetName().Version + @" (dev)";
#else
            lblVerGui.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#endif

#if DEBUG
            lblVerLib.Text = Library.Version + @" (dev)";
#else
            lblVerLib.Text =  Library.Version.ToString();
#endif

        }

        private void picDonate_Click(object sender, EventArgs e) {
            //
            // >>> don't be a dick !!!
            //
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7SWNPAPJNSARC");
        }

        private void picDonate_MouseEnter(object sender, EventArgs e) {
            picDonate.Image = Resources.paypal;
        }
        private void picDonate_MouseLeave(object sender, EventArgs e) {
            picDonate.Image = hoveDonation;
        }
        private void lnkLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(lnkLicense.Text);
        }
        private void lnkOpenNLP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(lnkOpenNLP.Text);
        }
        private void lnkSharpNL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(lnkSharpNL.Text);
        }
    }
}