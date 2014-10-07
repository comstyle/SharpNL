using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNL.Utility.Model {

    /// <summary>
    /// Manages the models on the OpenNL library.
    /// </summary>
    public static class ModelManager {

        private static readonly object syncLock;

        private static readonly Dictionary<string, Type> models;

        static ModelManager() {
            models = new Dictionary<string, Type>();
            syncLock = new object();
        }


        #region . Register .

        /// <summary>
        /// Registers an model type with the model manager.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <exception cref="System.ArgumentNullException">
        /// modelName
        /// or
        /// modelType
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The specified model type is not a valid model.
        /// or
        /// The specified model name is already registered.
        /// </exception>
        public static void Register(string modelName, Type modelType) {

            if (string.IsNullOrEmpty(modelName)) {
                throw new ArgumentNullException("modelName");
            }

            if (modelType == null) {
                throw new ArgumentNullException("modelType");
            }



            if (!modelType.IsSubclassOf(typeof (BaseModel))) {
                throw new ArgumentException("The specified model type is not a valid model.");
            }

            lock (syncLock) {
                if (models.ContainsKey(modelName)) {
                    throw new ArgumentException("The specified model name is already registered.");
                }
                models[modelName] = modelType;    
            }
        }

        #endregion

    }
}
