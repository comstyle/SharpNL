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
using System.Reflection;
using System.Security.Permissions;
using SharpNL.Java;
using SharpNL.Utility;
using Ver = SharpNL.Utility.Version;
using Version = System.Version;

namespace SharpNL {
    /// <summary>
    /// Represents the SharpNL library.
    /// </summary>
    public static class Library {

        /// <summary>
        /// Initializes static members of the SharpNL library.
        /// </summary>
        static Library() {
            Version = typeof(Library).Assembly.GetName().Version;

            OpenNLPVersion = new Ver(1, 5, 3, false);

            RegistersAllTheThings();
        }

        #region + Properties .

        #region . OpenNLPVersion .

        /// <summary>
        /// Gets the supported OpenNLP version.
        /// </summary>
        /// <value>The supported OpenNLP version.</value>
        public static Ver OpenNLPVersion { get; private set; }

        #endregion

        #region . Version .

        /// <summary>
        /// Gets the version of the SharpNL library.
        /// </summary>
        /// <value>The version of the SharpNLP library.</value>
        public static Version Version { get; private set; }

        #endregion

        #region . TypeResolver .
        /// <summary>
        /// Gets type resolver loaded for this library instance.
        /// </summary>
        /// <value>The type resolver.</value>
        public static TypeResolver TypeResolver { get; private set; }
        #endregion

        #endregion

        #region . RegistersAllTheThings .
        /// <summary>
        /// Registers all the things \o/ \o/ \o/
        /// </summary>
        private static void RegistersAllTheThings() {
            // have you smiled today? No? :/
            // Life is too short, then smile (=

            // lets come back to the "serious" stuff! maybe... #lol

            TypeResolver = new TypeResolver();

            RegisterTheTypes();
        }
        #endregion

        #region . RegisterTheTypes .

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private static void RegisterTheTypes() {
            // TODO: Make this loader be faster
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var modules = assembly.GetModules();
                foreach (var module in modules) {
                    var types = module.GetTypes();
                    foreach (var type in types) {
                        var attr = type.GetCustomAttribute<JavaClassAttribute>(false);
                        if (attr != null) {
                            TypeResolver.Register(attr.Name, type);                           
                        }
                    }
                }
            }
        }

        #endregion

        #region . Millis .
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal static long CurrentTimeMillis() {
            return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
        #endregion
    }
}