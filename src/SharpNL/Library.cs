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

using Ver = SharpNL.Utility.Version;
namespace SharpNL {
    /// <summary>
    /// Represents the SharpNL library.
    /// </summary>
    public static class Library {

        #region + Properties .

        #region . IsInitialized .

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region . OpenNlpVersion .

        /// <summary>
        /// Gets the supported OpenNLP version.
        /// </summary>
        /// <value>The supported OpenNLP version.</value>
        public static Ver OpenNlpVersion { get; private set; }

        #endregion

        #region . Version .

        /// <summary>
        /// Gets the version of the SharpNL library.
        /// </summary>
        /// <value>The version of the SharpNLP library.</value>
        public static Version Version { get; private set; }

        #endregion

        #endregion

        static Library() {
            Version = typeof (Library).Assembly.GetName().Version;

            OpenNlpVersion = new Ver(1, 5, 3, false);
        }

        /// <summary>
        /// Initializes the SharpNL library.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The library is already initialized.</exception>
        public static void Initialize() {
            if (IsInitialized) {
                throw new InvalidOperationException("The library is already initialized.");
            }

            IsInitialized = true;
        }

        #region . Millis .

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static long CurrentTimeMillis() {
            return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        #endregion
    }
}