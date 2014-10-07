using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SharpNL.Gui.Properties;

namespace SharpNL.Gui.Forms {
    public partial class frmMain : Form {
        private readonly Dictionary<Control, List<Tuple<Control, bool>>> disabledStates;

        public frmMain() {
            InitializeComponent();

            disabledStates = new Dictionary<Control, List<Tuple<Control, bool>>>();
        }

        private static void SelectLang(ComboBox combo, string langCode) {
            for (int i = 0; i < combo.Items.Count; i++) {
                var token = combo.Items[i] as string;
                if (token != null && token.IndexOf(' ') > 0) {
                    if (token.Split(' ')[0].Equals(langCode, StringComparison.InvariantCultureIgnoreCase)) {
                        combo.SelectedIndex = i;
                        return;
                    }
                }
            }
            combo.SelectedIndex = -1;
        }

        private static List<Control> GetControls(Control container, List<Control> parent = null) {
            if (parent == null)
                parent = new List<Control>();

            parent.Add(container);
            if (container.HasChildren) {
                foreach (Control child in container.Controls) {
                    GetControls(child, parent);
                }
            }

            return parent;
        }

        private void Disable(Control control) {

            if (control.InvokeRequired) {
                control.Invoke(new Action<Control>(Disable), control);
                return;
            }

            if (!disabledStates.ContainsKey(control)) {
                Cursor = Cursors.WaitCursor;

                var controls = GetControls(control);

                var list = new List<Tuple<Control, bool>>(controls.Count);

                foreach (var item in controls) {
                    list.Add(new Tuple<Control, bool>(item, item.Enabled));
                    item.Enabled = false;
                }

                disabledStates.Add(control, list);

                Cursor = Cursors.Default;
            }
        }

        private void Restore(Control control) {

            if (control.InvokeRequired) {
                control.Invoke(new Action<Control>(Restore), control);
                return;
            }

            if (disabledStates.ContainsKey(control)) {
                Cursor = Cursors.WaitCursor;

                var list = disabledStates[control];

                foreach (var tuple in list) {
                    tuple.Item1.Enabled = tuple.Item2; // restore the enable state;

                    tuple.Item1.Enabled = true;

                }

                disabledStates.Remove(control);

                Cursor = Cursors.Default;
            }
        }


        private void frmMain_Load(object sender, EventArgs e) {
            
            SelectLang(cmbSentLang, Program.Lang);

            cmbSentEncoding.Items.AddRange(Program.Encodings);
            cmbSentEncoding.SelectedItem = "utf-8";
        }

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {

            var form = new frmAbout();
            form.ShowDialog();

        }

        private void btnSentData_Click(object sender, EventArgs e) {
            var open = new OpenFileDialog {
                CheckFileExists = true,
                RestoreDirectory = true, // remember the last directory ;-)
                Filter = @"Árvore Deitada (*.ad)|*.ad"
            };

            if (open.ShowDialog() == DialogResult.OK) {
                btnSentData.Text = open.FileName;
                btnSentData.ForeColor = Color.DarkGreen;
            } else {
                btnSentData.Text = Resources.btnSentData_Select;
                btnSentData.ForeColor = Color.DimGray;
            }

        }

        private enum SentOptions {
            Convert, 
            Detect,
            Eval,
            Train,
            Validate
        }

        private SentOptions SentOption {
            get {
                if (rdbSentConvert.Checked)
                    return SentOptions.Convert;

                if (rdbSentDetection.Checked)
                    return SentOptions.Detect;

                if (rdbSentEval.Checked)
                    return SentOptions.Eval;

                if (rdbSentTraining.Checked)
                    return SentOptions.Train;

                if (rdbSentValidate.Checked)
                    return SentOptions.Validate;

                return 0;
            }
        }

        private void btnSentExec_Click(object sender, EventArgs e) {
            switch (SentOption) {
                case SentOptions.Convert:

                    if (btnSentData.ForeColor != Color.DarkGreen) {
                        MessageBox.Show(Resources.RequiredInputData, Resources.MessageTitleRequired,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        btnSentData.Focus();
                        return;
                    }

                    var convert = new SaveFileDialog {
                        RestoreDirectory = true,
                        CreatePrompt = true,
                        OverwritePrompt = true
                    };

                    if (convert.ShowDialog() == DialogResult.OK) {

                        tabs.Enabled = false;
                    }

                    break;
                case SentOptions.Detect:
                    break;
                case SentOptions.Eval:
                    break;
                case SentOptions.Train:
                    break;
                case SentOptions.Validate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
