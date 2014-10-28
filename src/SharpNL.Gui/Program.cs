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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpNL.Gui.Forms;

namespace SharpNL.Gui {
    internal static class Program {
        internal static string Lang { get; private set; }

        /// <summary>
        /// Gets the encodings available in the current environment.
        /// </summary>
        /// <value>The encodings.</value>
        internal static string[] Encodings { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread, SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // handle errors in the gui
            Application.ThreadException += OnThreadException;

            // handle all the errors in the current app domain
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            LoadInfo();

            Application.Run(new frmMain());
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs args) {
            var form = new frmException(args.Exception);
            form.ShowDialog();
        }
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs args) {
            var ex = args.ExceptionObject as Exception;
            if (ex != null) {
                var form = new frmException(ex);
                form.ShowDialog();
            }
        }

        internal static string Path {
            get {
                return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        private static void LoadInfo() {
            Lang = "en";
            
            var culture = new CultureInfo(Lang);
            Application.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Encodings = Encoding.GetEncodings().Select(encodingInfo => encodingInfo.Name).ToArray();
        }

        public static void InvokeIfRequired(this ISynchronizeInvoke obj, Action action) {
            if (obj.InvokeRequired) {
                var args = new object[0];
                obj.Invoke(action, args);
            } else {
                action();
            }
        }


    }
}