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
using System.Runtime.CompilerServices;

namespace SharpNL.Utility {
    public static class ToolFactoryManager {
        private static readonly Dictionary<string, Type> toolFactories;
        private static readonly object syncLock = new object();

        static ToolFactoryManager() {
            toolFactories = new Dictionary<string, Type>();
        }

        #region + Properties .

        #region . IsFactoriesLoaded .
        /// <summary>
        /// Gets a value indicating whether the factories in this application domain are loaded.
        /// </summary>
        /// <value><c>true</c> if the factories in this application domain are loaded; otherwise, <c>false</c>.</value>
        public static bool IsFactoriesLoaded { get; private set; }
        #endregion

        #endregion

        #region . CreateFactory .
        /// <summary>
        /// Creates the factory with the given name.
        /// </summary>
        /// <param name="factoryName">Name of the factory.</param>
        /// <returns>BaseToolFactory.</returns>
        public static BaseToolFactory CreateFactory(string factoryName) {
            lock (syncLock) {
                if (!toolFactories.ContainsKey(factoryName)) {
                    throw new ArgumentException(@"The requested tool factory is not registered/loaded.", "factoryName");
                }
                var type = toolFactories[factoryName];

                return (BaseToolFactory) Activator.CreateInstance(type);
            }
        }
        #endregion

        #region . IsRegistered .

        /// <summary>
        /// Determines whether the specified factory name is registered.
        /// </summary>
        /// <param name="factoryName">Name of the factory.</param>
        /// <returns><c>true</c> if the specified factory name is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered(string factoryName) {
            lock (syncLock) {

                if (!IsFactoriesLoaded)
                    LoadToolFactories();

                return toolFactories.ContainsKey(factoryName);    
            }
        }

        /// <summary>
        /// Determines whether the specified factory type is registered.
        /// </summary>
        /// <param name="factoryType">Type of the factory.</param>
        /// <returns><c>true</c> if the specified factory type is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered(Type factoryType) {
            lock (syncLock) {
                return toolFactories.ContainsValue(factoryType);
            }
        }

        #endregion

        #region . LoadToolFactories .
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LoadToolFactories() {
            lock (syncLock) {
                toolFactories.Clear();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies) {
                    var modules = assembly.GetModules();
                    foreach (var module in modules) {
                        var types = module.GetTypes();
                        foreach (var type in types) {
                            if (type.IsSubclassOf(typeof(BaseToolFactory)) && type.HasDefaultConstructor()) {
                                using (var factory = (BaseToolFactory)Activator.CreateInstance(type)) {
                                    if (!toolFactories.ContainsKey(factory.Name)) {
                                        toolFactories[factory.Name] = type;
                                    } else {
                                        Console.Error.WriteLine("The tool factory \"{0}\" is already registered.", factory.Name);
                                    }
                                }
                            }
                        }
                    }
                }
                IsFactoriesLoaded = true;
            }
        }
        #endregion

        #region . Register .
        /// <summary>
        /// Registers the specified tool factory type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="factoryType">Type of the factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>
        /// or
        /// <paramref name="factoryType"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The specified factory name is already registered.
        /// or
        /// The specified factory type does not extends the BaseToolFactory class.
        /// </exception>
        /// <remarks>
        /// Theoretically this method is not necessary, because <seealso cref="M:LoadToolFactories"/> should handle all the factories,
        /// but if an assembly is loaded during the runtime, this method allows the factory registration without reloading all the 
        /// factories.
        /// </remarks>
        public static void Register(string name, Type factoryType) {
            lock (syncLock) {
                if (string.IsNullOrEmpty(name)) {
                    throw new ArgumentNullException("name");
                }
                if (toolFactories.ContainsKey(name)) {
                    throw new ArgumentException(@"The specified factory name is already registered.", "name");
                }
                if (factoryType == null) {
                    throw new ArgumentNullException("factoryType");
                }
                if (!typeof (BaseToolFactory).IsSubclassOf(factoryType)) {
                    throw new ArgumentException(
                        @"The specified factory type does not extends the BaseToolFactory class.", "factoryType");
                }

                toolFactories.Add(name, factoryType);
            }
        }
        #endregion

    }
}