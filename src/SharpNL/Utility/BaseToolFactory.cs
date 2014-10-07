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

namespace SharpNL.Utility {

    using Serialization;

    public abstract class BaseToolFactory : IDisposable {

        protected BaseToolFactory() {
            Name = GetType().Name;
        }

        /// <summary>
        /// Gets the artifact provider.
        /// </summary>
        /// <value>The artifact provider.</value>
        protected ArtifactProvider ArtifactProvider { get; private set; }

        #region . Initialize .

        /// <summary>
        /// Initializes the tool factory with the specified artifact provider.
        /// </summary>
        /// <param name="artifactProvider">The artifact provider.</param>
        internal void Initialize(ArtifactProvider artifactProvider) {
            ArtifactProvider = artifactProvider;
        }

        #endregion

        #region . Name .
        /// <summary>
        /// Gets the tool factory name.
        /// </summary>
        /// <value>The tool factory name.</value>
        public string Name { get; private set; }
        #endregion

        /// <summary>
        /// Creates the artifact serializers for the <see cref="ArtifactProvider"/>.
        /// The subclasses should call the <see cref="Serialization.ArtifactProvider.RegisterArtifactType"/> method to register an new artifact type.
        /// </summary>
        /// <param name="provider">The artifact provider.</param>
        public virtual void CreateArtifactSerializers(ArtifactProvider provider) {
            // nothing to do here...
        }

        /// <summary>
        /// Validates the parsed artifacts. 
        /// </summary>
        /// <exception cref="InvalidFormatException">Invalid artifact map.</exception>
        public virtual void ValidateArtifactMap() {
            // no additional artifacts
        }

        /// <summary>
        /// Creates a dictionary with pairs of keys and objects. 
        /// The models implementation should call this constructor that creates a model programmatically.
        /// </summary>
        public virtual Dictionary<string, object> CreateArtifactMap() {
            return new Dictionary<string, object>();
        }


        //public virtual Dictionary<string, > 


        /// <summary>
        /// Creates the manifest entries that will be added to the model manifest
        /// </summary>
        /// <returns>The manifest entries to added to the model manifest.</returns>
        public virtual Dictionary<string, string> CreateManifestEntries() {
            return new Dictionary<string, string>();
        }


        #region . Free .
        /// <summary>
        /// Release all resources associated with this tool factory. This method is called by the object disposal.
        /// </summary>
        protected virtual void Free() {
            ArtifactProvider = null;
        }
        #endregion

        #region . Dispose .
        //internal static BaseToolFactory Create()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            try {
                Free();
            } catch { }
        }
        #endregion

    }
}