using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SharpNL.Gui.Controls {
    internal class RichLog : RichTextBox {

#if WIN
        // Windows shenanigans to prevent the control flickering and the default (and poorly implemented) autoscroll.
        [DllImport("user32", EntryPoint = "SendMessage")]
        private static extern int SendMessage(HandleRef hWnd, uint msg, uint wParam, ref POINT lp);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        const int WM_SETREDRAW = 11;
        const int EM_SETEVENTMASK = 1073;
        const uint EM_GETSCROLLPOS = 0x0400 + 221;
        const uint EM_SETSCROLLPOS = 0x0400 + 222;

        struct POINT {
            public long X;
            public long Y;
        }

        private int updating;
        private int oldEventMask;
        private POINT ScrollPosition;
       
        public void BeginUpdate() {
            ++updating;
            if (updating > 1) {
                return;
            }
            SendMessage(new HandleRef(this, Handle), EM_GETSCROLLPOS, 0, ref ScrollPosition);
            oldEventMask = SendMessage(new HandleRef(this, Handle), EM_SETEVENTMASK, 0, 0);
            SendMessage(new HandleRef(this, Handle), WM_SETREDRAW, 0, 0);
        }

        public void EndUpdate() {
            --updating;
            if (updating > 0) {
                return;
            }
            SendMessage(new HandleRef(this, Handle), EM_SETSCROLLPOS, 0, ref ScrollPosition);    
            SendMessage(new HandleRef(this, Handle), WM_SETREDRAW, 1, 0);
            SendMessage(new HandleRef(this, Handle), EM_SETEVENTMASK, 0, oldEventMask);
            //Refresh();
            Invalidate();
        }


#endif

        #region . AppendText .

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AppendText(string text, Color foreColor) {

#if WIN
            // prevent  and the stupid auto scroll
            BeginUpdate();
#endif

            var start = SelectionStart;
            var len = SelectionLength;
            var end = !Focused || start == TextLength;

            SelectionStart = TextLength;
            SelectionLength = 0;
            SelectionColor = foreColor;
            AppendText(text);

            //SelectedText = text;
            SelectionColor = foreColor;

            if (end) {
                SelectionStart = TextLength;
                SelectionLength = 0;
            } else {
                SelectionStart = start;
                SelectionLength = len;    
            }
           
#if WIN
            EndUpdate();
#endif

            if (end) {
                ScrollToCaret();
            }

        }

        #endregion

        public void Highlight(int start, int length, Color color) {
            int sPos = 0;
            Color sColor = Color.White;
            try {
                NativeMethods.LockWindowUpdate(Handle);
                sPos = SelectionStart;

                SelectionStart = start;
                SelectionLength = length;
                SelectionBackColor = color;

            } finally {
                SelectionStart = sPos;
                SelectionLength = 0;
                SelectionBackColor = sColor;
                NativeMethods.LockWindowUpdate(IntPtr.Zero);
            }



        }


    }
}
