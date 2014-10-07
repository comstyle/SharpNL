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
using System.Collections.Generic;
using System.Security.Permissions;

namespace ShapNL.Cmd {

    /// <summary>
    /// Utility class that makes my life easier. =D
    /// </summary>
    internal static class Extensions {

        #region . GetSubclasses .

        /// <summary>
        /// Gets a list containing all subclasses for the given type.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A list containing all subclasses for the given type.</returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        internal static List<Type> GetSubclasses(this Type input) {
            var list = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var modules = assembly.GetModules();
                foreach (var module in modules) {
                    var types = module.GetTypes();
                    foreach (var type in types) {
                        if (type.IsSubclassOf(input)) {
                            list.Add(type);
                        }
                    }
                }
            }
            return list;
        }

        #endregion

    }
}