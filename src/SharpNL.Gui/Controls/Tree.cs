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
using System.Windows.Forms;

namespace SharpNL.Gui.Controls {
    /// <summary>
    /// This TreeView removes the stupid flickering...
    /// </summary>
    internal class Tree : TreeView {
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        public Tree() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Disable default CommCtrl painting on non-Vista systems
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major < 6)
                SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            var style = 0;

            if (DoubleBuffered)
                style |= TVS_EX_DOUBLEBUFFER;

            if (style != 0)
                NativeMethods.SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)style);
        }
    }
}