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
using SharpNL.Gui.Viewers;

namespace SharpNL.Gui {
    public static class Manager {
        private static readonly Dictionary<Type, Type> viewers;

        static Manager() {
            viewers = new Dictionary<Type, Type>();

            // automatically loads the viewers :)
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var module in assembly.GetModules()) {
                    foreach (var type in module.GetTypes()) {
                        if (type.IsSubclassOf(typeof (Viewer))) {
                            var cons = type.GetConstructors();
                            foreach (var info in cons) {
                                var p = info.GetParameters();
                                if (p.Length == 1 && !viewers.ContainsKey(p[0].ParameterType)) {
                                    viewers.Add(p[0].ParameterType, type);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static Type GetViewer(Type type) {
            if (viewers.ContainsKey(type))
                return viewers[type];

            return null;
        }
    }
}