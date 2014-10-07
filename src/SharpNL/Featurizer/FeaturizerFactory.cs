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
using System.IO;
using SharpNL.Utility;
using SharpNL.Utility.Serialization;

namespace SharpNL.Featurizer {
    public abstract class FeaturizerFactory : BaseToolFactory {

        private const string CG_FLAGS_PROPERTY = "cgFlags";

        protected IFeatureDictionary featureDictionary;

        private List<string> poisonedDictionaryTags;
        private string cgFlags;

        protected FeaturizerFactory() { }

        protected FeaturizerFactory(IFeatureDictionary featureDictionary, string cgFlags) {
            this.featureDictionary = featureDictionary;
            this.cgFlags = cgFlags;
        }


        #region > CreateArtifactSerializers .

        /// <summary>
        /// Creates the artifact serializers for the <see cref="BaseToolFactory.ArtifactProvider"/>.
        /// The subclasses should call the <see cref="SharpNL.Utility.Serialization.ArtifactProvider.RegisterArtifactType"/> method to register an new artifact type.
        /// </summary>
        /// <param name="provider">The artifact provider.</param>
        public override void CreateArtifactSerializers(ArtifactProvider provider) {
            base.CreateArtifactSerializers(provider);

            // TODO: Remove the _ignored when the serializer is implemented.
            provider.RegisterArtifactType(".serialized_set_ignored", SerializeSet, DeserializeSet);

        }

        private static void SerializeSet(object artifact, Stream outputStream) {
            // TODO: Implement a java compatible serializer
            // Knuppe: I'm to lazy to implement this now :P

            // - BinaryWriter can't be used because the string length is stored in 4 bytes, and java
            //                stores in 2 bytes... which is correct, because there is no string 
            //                with a negative length (Duh! Microsoft)

            /*
             *  CoGrOO code
                oin = new ObjectInputStream(new UncloseableInputStream(in));
                try {
                  set = (Set<String>) oin.readObject();
                } catch (ClassNotFoundException e) {
                  System.err.println("could not restore serialied object");
                  e.printStackTrace();
                }
            */
            throw new NotImplementedException();
        }

        private static object DeserializeSet(Stream inputStream) {
            // TODO: Implement a java compatible deserializer
            // Knuppe: I'm to lazy to implement this now :P

            // - BinaryReader can't be used because the string length is stored in 4 bytes, and java
            //                stores in 2 bytes... which is correct, because there is no string 
            //                with a negative length (Duh! Microsoft)

            /*
             * CoGrOO code
                objOut = new ObjectOutputStream(out);
                objOut.writeObject(artifact);
             */
            throw new NotImplementedException();
        }

        #endregion

        #region . CreateManifestEntries .
        /// <summary>
        /// Creates the manifest entries that will be added to the model manifest
        /// </summary>
        /// <returns>The manifest entries to added to the model manifest.</returns>
        public override Dictionary<string, string> CreateManifestEntries() {
            var manifestEntries = base.CreateManifestEntries();

            if (cgFlags != null)
                manifestEntries[CG_FLAGS_PROPERTY] = cgFlags;

            return manifestEntries;
        }
        #endregion

    }
}