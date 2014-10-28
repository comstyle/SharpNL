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
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using SharpNL.Gui.Controls;
using SharpNL.Gui.Properties;

namespace SharpNL.Gui.Forms {
    public partial class frmMain : Form {
        public frmMain() {
            InitializeComponent();

            foreach (var control in Controls) {
                var mdiClient = control as MdiClient;
                if (mdiClient != null) {
                    mdiClient.SetBevel(false);
                    mdiClient.BackColor = Color.White;
                    break;
                }
            }
        }
        
        private void frmGui_Load(object sender, EventArgs e) {
            

        }

        private void mnuAbout_Click(object sender, EventArgs e) {
            var form = new frmAbout();
            form.ShowDialog();
        }

        private void mnuNewProject_Click(object sender, EventArgs e) {
            var form = new frmProject {MdiParent = this};
            form.Show();
        }

        private void menuStrip_ItemAdded(object sender, ToolStripItemEventArgs e) {
            if (e.Item.GetType().Name == "SystemMenuItem") {
                e.Item.Visible = false;
            }
        }

        #region + Menu Edit .


        private void mnuEditCut_Click(object sender, EventArgs e) {
            var ac = NativeMethods.GetActiveControl();

            if (ac == null) return;

            var richLog = ac as RichLog;
            if (richLog != null) {
                richLog.Cut();
                return;
            }

            var textBox = ac as TextBox;
            if (textBox != null) {
                textBox.Cut();
                return;
            }
        }

        private void mnuEditCopy_Click(object sender, EventArgs e) {
            var ac = NativeMethods.GetActiveControl();

            if (ac == null) return;

            var richLog = ac as RichLog;
            if (richLog != null) {
                richLog.Copy();
                return;
            }

            var textBox = ac as TextBox;
            if (textBox != null) {
                textBox.Copy();
                return;
            }
        }

        private void mnuEditSelect_Click(object sender, EventArgs e) {
            var ac = NativeMethods.GetActiveControl();

            if (ac == null) return;

            var richLog = ac as RichLog;
            if (richLog != null) {
                richLog.SelectAll();
                return;
            }

            var textBox = ac as TextBox;
            if (textBox != null) {
                textBox.SelectAll();
                return;
            }
        }


        private void mnuEditPaste_Click(object sender, EventArgs e) {
            var ac = NativeMethods.GetActiveControl();

            if (ac == null) return;

            var richLog = ac as RichLog;
            if (richLog != null) {
                richLog.Paste();
                return;
            }

            var textBox = ac as TextBox;
            if (textBox != null) {
                textBox.Paste();
                return;
            }
        }

        private void mnuEdit_DropDownOpening(object sender, EventArgs e) {
            var control = NativeMethods.GetActiveControl();

            var readOnly = false;
            var supported = false;


            var text = control as TextBox;
            if (text != null) {
                readOnly = text.ReadOnly;
                supported = true;
                goto done;
            }

            var rich = control as RichLog;
            if (rich != null) {
                readOnly = rich.ReadOnly;
                supported = true;
                goto done;
            }

            done:
            mnuEditCopy.Enabled = supported;
            mnuEditSelect.Enabled = supported;

            mnuEditCut.Enabled = supported && !readOnly;
            mnuEditPaste.Enabled = supported && !readOnly;
        }

        #endregion

        private void mnuAboutDonate_Click(object sender, EventArgs e) {
            try {
                //
                // >>> don't be a dick !!!
                //
                Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7SWNPAPJNSARC");
            } catch { }    
        }
        private void mnuAboutProject_Click(object sender, EventArgs e) {
            try {
                Process.Start("https://github.com/knuppe/SharpNL");
            } catch { }            
        }
        private void mnuAboutWiki_Click(object sender, EventArgs e) {
            try {
                Process.Start("https://github.com/knuppe/SharpNL/wiki");
            } catch { }                       
        }

        private void mnuFile_DropDownOpening(object sender, EventArgs e) {
            var form = ActiveMdiChild as frmProject;

            if (form != null && form.project != null) {
                mnuSaveProject.Enabled = form.project.IsDirty;
            } else {
                mnuSaveProject.Enabled = false;
            }

            mnuCloseProject.Enabled = ActiveMdiChild != null;

        }

        private void mnuCloseProject_Click(object sender, EventArgs e) {
            if (ActiveMdiChild != null) {
                ActiveMdiChild.Close();
            }
        }

        private void mnuOpenProject_Click(object sender, EventArgs e) {
            var open = new OpenFileDialog {
                Filter = @"SharpNL Project File (*.snl)|*.snl"
            };

            if (open.ShowDialog() != DialogResult.OK) return;

            Project.Project project = null;
            try {
                using (var file = new FileStream(open.FileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    project = Project.Project.Deserialize(file);
                }
            } catch {
                MessageBox.Show(Resources.ProjectOpenError, Resources.ErrorTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }

            if (project != null) {
                var form = new frmProject(project) {
                    MdiParent = this
                };
                form.Show();
            }           
        }

        private void mnuExit_Click(object sender, EventArgs e) {
            if (ActiveMdiChild == null && MdiChildren.Length == 0) {
                Close();
                return;
            }

            if (MessageBox.Show(Resources.ApplicationExit, Resources.ApplicationExitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Retry)
                return;

            Close();
        }
    }
}