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
using SharpNL.Utility.Model;
using SharpNL.Utility.Serialization;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a model manager that provides convenient access to culture-specific models at run time.
    /// </summary>
    public class ModelManager : IEnumerable<string>, IDisposable {
        private readonly Dictionary<string, BaseModel> models;
        private readonly Dictionary<string, ModelInfo> infos;

        public ModelManager() {
            models = new Dictionary<string, BaseModel>();
            infos = new Dictionary<string, ModelInfo>();
        }

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of models contained in the <see cref="ModelManager"/>.
        /// </summary>
        /// <value>The number of elements contained in the <see cref="ModelManager"/>.</value>
        public int Count {
            get {
                var list = new List<string>(Math.Max(models.Count, infos.Count));
                foreach (var name in models.Keys)
                    if (!list.Contains(name)) list.Add(name);

                foreach (var name in infos.Keys)
                    if (!list.Contains(name)) list.Add(name);

                return list.Count;
            }
        }
        #endregion

        #endregion

        #region . Add .

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="name">The model name/key.</param>
        /// <param name="model">The model.</param>
        /// <exception cref="System.ArgumentNullException">model</exception>
        /// <exception cref="System.ArgumentException">
        /// The specified model does not contain a manifest.
        /// or
        /// The manifest in the specified model does not specifies the language.
        /// or
        /// The model <paramref name="name"/> is already in this manager.
        /// </exception>
        public void Add(string name, BaseModel model) {
            if (model == null)
                throw new ArgumentNullException("model");

            if (model.Manifest == null)
                throw new ArgumentException("The specified model does not contain a manifest.");

            if (!model.Manifest.Contains(ArtifactProvider.LanguageEntry))
                throw new ArgumentException("The manifest in the specified model does not specifies the language.");

            if (models.ContainsKey(name))
                throw new ArgumentException("The model " + name + " is already in this manager.");

            models[name] = model;
        }

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
           
            if (models.ContainsKey(modelInfo.Name))
                throw new ArgumentException("The model " + modelInfo.Name + " is already in this manager.");

            infos[modelInfo.Name] = modelInfo;
        }

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            models.Clear();
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
            if (!models.ContainsKey(name) && !infos.ContainsKey(name))
                throw new KeyNotFoundException("The model " + name + " is not in this instance.");

            if (models.ContainsKey(name))
                models.Remove(name); 

            if (infos.ContainsKey(name))
                infos.Remove(name);

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

            if (!infos.ContainsKey(modelInfo.Name) && !models.ContainsKey(modelInfo.Name))
                throw new KeyNotFoundException("The model " + modelInfo.Name + " is not in this instance.");

            if (infos.ContainsKey(modelInfo.Name))
                infos.Remove(modelInfo.Name);

            if (models.ContainsKey(modelInfo.Name))
                models.Remove(modelInfo.Name);
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
            if (models.ContainsKey(name))
                return models[name] as T;

            if (infos.ContainsKey(name)) {
                models[name] = infos[name].OpenModel();

                return models[name] as T;
            }

            return null;
        }

        #endregion

        #region . GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the model manager.
        /// </summary>
        /// <returns>
        /// A <see cref="T:IEnumerator{string}"/> that contains all the model names in this manager.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            var list = new List<string>(Math.Max(models.Count, infos.Count));
            foreach (var name in models.Keys)
                if (!list.Contains(name)) list.Add(name);

            foreach (var name in infos.Keys)
                if (!list.Contains(name)) list.Add(name);

            list.Sort();

            return list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

    }
}