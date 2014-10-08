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
using System.Threading;


namespace SharpNL.Utility {
    /// <summary>
    /// Represents a delegate, which resolves the type name into a <see cref="Type" /> object..
    /// </summary>
    /// <param name="type">The string type.</param>
    /// <returns>The <see cref="Type" /> object or a <c>null</c> value if not recognized.</returns>
    public delegate Type ReadTypeDelegate(string type);


    /// <summary>
    /// Represents a delegate, which resolves the type name by the given <paramref name="type"/> object.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The resolved type name, or a <c>null</c> value if it can not be resolved.</returns>
    public delegate string WriteTypeDelegate(Type type);

    /// <summary>
    /// The type resolver is responsible to translate a string representation of a type into a <see cref="Type"/> 
    /// object and vice versa.
    /// </summary>
    public static class TypeResolver {

        private static readonly Dictionary<string, Type> types;
        private static readonly ReaderWriterLockSlim lockSlim;

        static TypeResolver() {
            types = new Dictionary<string, Type>();
            lockSlim = new ReaderWriterLockSlim();
        }

        #region . IsRegistered .
        /// <summary>
        /// Determines whether the specified type name is registered.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        public static bool IsRegistered(string name) {
            lockSlim.EnterReadLock();
            try {
                return types.ContainsKey(name);
            } finally {
                lockSlim.ExitReadLock();
            }           
        }
        #endregion

        #region . Overwrite .
        /// <summary>
        /// Overwrites an specified type.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <param name="type">The type value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/>
        /// or
        /// <paramref name="type"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified name is not registered.</exception>
        /// <remarks>This method locks this entire instance! Use it wisely.</remarks>
        public static void Overwrite(string name, Type type) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (type == null)
                throw new ArgumentNullException("type");

            // this also prevents people to use this function as a set register.
            if (!types.ContainsKey(name))
                throw new ArgumentException("The specified name is not registered.");

            lockSlim.EnterUpgradeableReadLock();
            try {
                types[name] = type;
            } finally {
                lockSlim.ExitUpgradeableReadLock();               
            }
        }
        #endregion

        #region . Register .
        /// <summary>
        /// Registers the specified type object with its string representation.
        /// </summary>
        /// <param name="name">A string representation of the given type.</param>
        /// <param name="type">The type.</param>
        public static void Register(string name, Type type) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (type == null)
                throw new ArgumentNullException("type");

            if (types.ContainsKey(name))
                throw new ArgumentException("The specified name is already registered.");

            lockSlim.EnterWriteLock();
            try {
                types.Add(name, type);
            } finally {
                lockSlim.ExitWriteLock();
            }
        }
        #endregion

        #region . ResolveType .
        /// <summary>
        /// Resolves the type name into a <see cref="Type" /> object.
        /// </summary>
        /// <param name="name">The type name.</param>
        /// <returns>The <see cref="Type" /> object or a <c>null</c> value if not recognized.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public static Type ResolveType(string name) {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            lockSlim.EnterReadLock();
            try {
                if (types.ContainsKey(name))
                    return types[name];
            } finally {
                lockSlim.ExitReadLock();
            }
            return null;
        }
        #endregion

        #region . ResolveName .
        /// <summary>
        /// Resolves the type name by the given <paramref name="type"/> object.
        /// </summary>
        /// <param name="type">The type name.</param>
        /// <returns>The resolved type name, or a <c>null</c> value if it can not be resolved.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public static string ResolveName(Type type) {
            if (type == null)
                throw new ArgumentNullException("type");

            lockSlim.EnterReadLock();
            try {
                if (types.ContainsValue(type))
                    return types.GetKey(type);
            } finally {
                lockSlim.ExitReadLock();
            }
            return null;
        }
        #endregion

    }
}