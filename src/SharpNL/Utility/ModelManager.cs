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
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SharpNL.Utility.Model;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a model manager that provides convenient access to culture-specific models at run time.
    /// </summary>
    public class ModelManager : IEnumerable<ModelInfo>, IDisposable {
        private readonly Dictionary<string, ModelInfo> infos;
        private readonly List<Models> available;

        /// <summary>
        /// Raised when a model is added or removed from this manager.
        /// </summary>
        public EventHandler<ModelInfoEventArgs> Changed;

        public ModelManager() {
            infos = new Dictionary<string, ModelInfo>();
            available = new List<Models>();
        }

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of models contained in the <see cref="ModelManager"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ModelManager"/>.</value>
        public int Count {
            get {
                return infos.Count;
            }
        }
        #endregion

        #endregion

        #region . Add .

        /// <summary>
        /// Adds the specified model information. The model key will be de filename without extension.
        /// </summary>
        /// <param name="modelInfo">The model information.</param>
        /// <exception cref="System.ArgumentNullException">modelInfo</exception>
        /// <exception cref="System.IO.FileNotFoundException">The model file does not exist.</exception>
        /// <exception cref="System.ArgumentException">The model <i>name</i> is already in this manager.</exception>
        public void Add(ModelInfo modelInfo) {
            if (modelInfo == null)
                throw new ArgumentNullException("modelInfo");

            if (!modelInfo.File.Exists)
                throw new FileNotFoundException("The model file does not exist.", modelInfo.File.FullName);

            infos[modelInfo.Name] = modelInfo;

            if (!available.Contains(modelInfo.ModelType))
                available.Add(modelInfo.ModelType);

            if (Changed != null)
                Changed(this, new ModelInfoEventArgs(modelInfo));

        }

        #endregion

        #region . Available .
        /// <summary>
        /// Gets the available models in this instance.
        /// </summary>
        /// <value>The available models in this instance.</value>
        public Models[] Available {
            get { return available.ToArray(); }
        }
        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether this instance contains the specified model name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if model name is in this instance; otherwise, <c>false</c>.</returns>
        public bool Contains(string name) {
            return infos.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether this instance contains the specified <see cref="ModelInfo"/>.
        /// </summary>
        /// <param name="modelInfo">The model information.</param>
        /// <returns><c>true</c> if <see cref="ModelInfo"/> is in this instance; otherwise, <c>false</c>.</returns>
        public bool Contains(ModelInfo modelInfo) {
            return infos.ContainsValue(modelInfo);
        }

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            infos.Clear();
        }
        #endregion

        #region . Remove .
        /// <summary>                                                        
        /// Removes the model with the specified name.
        /// </summary>
        /// <param name="name">The model name to be removed.</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The model <paramref name="name"/> is not in this instance.</exception>
        public void Remove(string name) {
            if (!infos.ContainsKey(name))
                throw new KeyNotFoundException("The model " + name + " is not in this instance.");

            if (infos.ContainsKey(name)) {
                var info = infos[name];

                infos.Remove(name);

                CheckRemovedType(info.ModelType);

                if (Changed != null)
                    Changed(this, new ModelInfoEventArgs(info));
            }
        }
        /// <summary>
        /// Removes the specified model information.
        /// </summary>
        /// <param name="modelInfo">The model information.</param>
        /// <exception cref="System.ArgumentNullException">modelInfo</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The model <see cref="ModelInfo.Name"/> is not in this instance.</exception>
        public void Remove(ModelInfo modelInfo) {
            if (modelInfo == null)
                throw new ArgumentNullException("modelInfo");

            if (!infos.ContainsKey(modelInfo.Name))
                throw new KeyNotFoundException("The model " + modelInfo.Name + " is not in this instance.");

            if (!infos.ContainsKey(modelInfo.Name)) 
                return;

            infos.Remove(modelInfo.Name);

            CheckRemovedType(modelInfo.ModelType);

            if (Changed != null)
                Changed(this, new ModelInfoEventArgs(modelInfo));
        }
        #endregion

        #region . CheckRemovedType .
        /// <summary>
        /// Removes the model type from the available list if there are no more 
        /// models of the same <paramref name="type"/>.
        /// </summary>
        private void CheckRemovedType(Models type) {
            var another = infos.Values.Any(info => info.ModelType == type);
            if (!another) {
                available.Remove(type);
            }
        }
        #endregion

        #region . GetModel .
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <typeparam name="T">The desired model type.</typeparam>
        /// <param name="name">The model name/key.</param>
        /// <returns>The model as the requested type or a <c>null</c> value.</returns>
        /// <exception cref="FileNotFoundException">The model file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Unable to detect the model type.</exception>
        public T GetModel<T>(string name) where T : BaseModel {
            return infos[name].OpenModel() as T;
        }

        #endregion

        #region . GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the model manager.
        /// </summary>
        /// <returns>
        /// A <see cref="T:IEnumerator{string}"/> that contains all the model names in this manager.
        /// </returns>
        public IEnumerator<ModelInfo> GetEnumerator() {
            return infos.Values.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

    }
}