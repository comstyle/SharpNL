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
using System.IO;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents the training parameters.
    /// </summary>
    public class TrainingParameters {

        private readonly Properties properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingParameters"/> class.
        /// </summary>
        public TrainingParameters() {
            properties = new Properties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrainingParameters"/> by deserializing the input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        public TrainingParameters(Stream inputStream) : this() {
            properties.Load(inputStream);
        }

        #region . DefaultParameters .
        /// <summary>
        /// Gets the default parameters.
        /// </summary>
        /// <returns>The default parameters.</returns>
        public static TrainingParameters DefaultParameters() {
            var p = new TrainingParameters();

            p.Set(Parameters.Algorithm, "MAXENT");
            p.Set(Parameters.TrainerType, "Event");
            p.Set(Parameters.Iterations, "100");
            p.Set(Parameters.Cutoff, "5");

            return p;
        }
        #endregion

        #region + Contains .
        /// <summary>
        /// Determines whether the training parameters contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance contains the specified key; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool Contains(string key) {
            if (key == null)
                throw new ArgumentNullException("key");
            return properties.Contains(key);
        }

        /// <summary>
        /// Determines whether the training parameters contains the key in the specified namespace.
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if this instance contains the key in the specified namespace; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="ns" /> is null.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public bool Contains(string ns, string key) {
            if (string.IsNullOrEmpty(ns))
                throw new ArgumentNullException("ns");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            return properties.Contains(string.Format("{0}.{1}", ns, key));
        }

        #endregion

        #region + Get .
        /// <summary>
        /// Gets the assigned value with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The assigned value with the specified key.</returns>
        public string Get(string key) {
            return properties[key];
        }

        /// <summary>
        /// Gets the assigned value with the specified key and namespace.
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <param name="key">The key.</param>
        /// <returns>The assigned value with the specified key and namespace.</returns>
        public string Get(string ns, string key) {
            return properties[string.Format("{0}.{1}", ns, key)];
        }

        /// <summary>
        /// Gets the value of the specified key as a type. 
        /// If the key does not exist or the value can't be converted, the <paramref name="defaultValue"/> 
        /// will be returned as result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The <paramref name="defaultValue"/> if value does not exist or if the value can't be
        /// converted; otherwise, the value associated with the specified <paramref name="key"/>.
        /// </returns>
        public T Get<T>(string key, T defaultValue) {
            return properties.Get(key, defaultValue);
        }

        /// <summary>
        /// Gets the value of the specified namespace + key as a type. 
        /// If the key does not exist or the value can't be converted, the <paramref name="defaultValue"/> 
        /// will be returned as result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="ns">The namespace.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The <paramref name="defaultValue"/> if value does not exist or if the value can't be
        /// converted; otherwise, the value associated with the specified <paramref name="key"/>.
        /// </returns>
        public T Get<T>(string ns, string key, T defaultValue) {
            return properties.Get(string.Format("{0}.{1}", ns, key), defaultValue);
        }

        #endregion

        #region . GetNamespace .

        /// <summary>
        /// Gets the properties in the specified namespace.
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <returns>A <see cref="TrainingParameters"/> containing the properties in the specified namespace.</returns>
        public TrainingParameters GetNamespace(string ns) {
            if (!ns.EndsWith("."))
                ns = ns + ".";

            var p = new TrainingParameters();
            foreach (var pk in properties) {
                if (pk.StartsWith(ns)) {
                    string key = pk.Substring(ns.Length);
                    p.Set(key, properties[pk]);
                }
            }
            return p;
        }

        #endregion

        #region + Set .
        /// <summary>
        /// Sets the specified value to an given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public void Set(string key, string value) {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            properties[key] = value;
        }
        /// <summary>
        /// Sets the specified value to an given key and namespace.
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="ns" /> is null.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="key" /> is null.</exception>
        public void Set(string ns, string key, string value) {
            if (string.IsNullOrEmpty(ns))
                throw new ArgumentNullException("ns");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            properties[string.Format("{0}.{1}", ns, key)] = value;
        }
        #endregion

        #region . IsValid .
        /// <summary>
        /// Determines whether the basic parameters (algorithm, cutoff and iterations) are valid.
        /// </summary>
        /// <returns><c>true</c> if the basic parameters are valid; otherwise, <c>false</c>.</returns>
        internal bool IsValid() {
            var value = 0;

            if (properties[Parameters.Cutoff] != null && !int.TryParse(properties[Parameters.Cutoff], out value)) {
                return false;
            }
            if (value < 0) {
                return false;
            }

            value = 0;
            if (properties[Parameters.Iterations] != null && !int.TryParse(properties[Parameters.Iterations], out value)) {
                return false;
            }

            if (value < 0) {
                return false;
            }

            return true;
        }
        #endregion

        #region . Serialize .
        /// <summary>
        /// Serializes the properties into a given stream.
        /// </summary>
        /// <param name="output">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">output</exception>
        /// <exception cref="System.ArgumentException">The stream is not writable.</exception>
        public void Serialize(Stream output) {
            if (output == null) {
                throw new ArgumentNullException("output");
            }
            if (!output.CanWrite) {
                throw new ArgumentException(@"The stream is not writable.", "output");
            }

            properties.Save(output);
        }
        #endregion

    }
}