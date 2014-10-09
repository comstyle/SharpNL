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
using SharpNL.Utility.FeatureGen;

namespace SharpNL.NameFind {
    /// <summary>
    /// Represents a tool factory for the extensions of the name finder.
    /// </summary>
    /// <remarks>
    /// The implementations of this class will work only if they are used during the training.
    /// </remarks>
    public class TokenNameFinderFactory : BaseToolFactory {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderFactory"/> that provides the 
        /// default implementation of the resources.
        /// </summary>
        public TokenNameFinderFactory() {
            SequenceCodec = new BioCodec();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderFactory"/> with the given parameters.
        /// </summary>
        /// <param name="featureGeneratorBytes">The feature generator bytes.</param>
        /// <param name="resources">The resources dictionary.</param>
        public TokenNameFinderFactory(byte[] featureGeneratorBytes, Dictionary<string, object> resources)
            : this(featureGeneratorBytes, resources, new BioCodec()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderFactory"/> with the given parameters.
        /// </summary>
        /// <param name="featureGeneratorBytes">The feature generator bytes.</param>
        /// <param name="resources">The resources dictionary.</param>
        /// <param name="seqCodec">The sequence codec.</param>
        public TokenNameFinderFactory(byte[] featureGeneratorBytes, Dictionary<string, object> resources,
            ISequenceCodec<string> seqCodec) {
            FeatureGenerator = featureGeneratorBytes;
            Resources = resources;
            SequenceCodec = seqCodec;
        }

        #endregion

        #region + Properties .

        #region . Resources .

        /// <summary>
        /// Gets the resources dictionary.
        /// </summary>
        /// <value>The resources dictionary.</value>
        public Dictionary<string, object> Resources { get; protected set; }

        #endregion

        #region . SequenceCodec .

        /// <summary>
        /// Gets the sequence codec.
        /// </summary>
        /// <value>The sequence codec.</value>
        public ISequenceCodec<string> SequenceCodec { get; protected set; }

        #endregion

        #region . FeatureGenerator .
        /// <summary>
        /// Gets feature generator bytes.
        /// </summary>
        /// <value>The feature generator bytes.</value>
        public byte[] FeatureGenerator { get; protected set; }
        #endregion

        #endregion

        #region . CreateContextGenerator .

        /// <summary>
        /// Creates the context generator.
        /// </summary>
        /// <returns>INameContextGenerator.</returns>
        public virtual INameContextGenerator CreateContextGenerator() {
            return new DefaultNameContextGenerator(CreateFeatureGenerators() ?? NameFinderME.CreateFeatureGenerator());
        }

        #endregion

        #region . CreateFeatureGenerators .

        /// <summary>
        /// Creates the <see cref="IAdaptiveFeatureGenerator"/>.
        /// Usually this is a set of generators contained in the <see cref="AggregatedFeatureGenerator"/>.
        /// </summary>
        /// <returns>The feature generator or null if there is no descriptor in the model.</returns>
        public virtual IAdaptiveFeatureGenerator CreateFeatureGenerators() {
            byte[] descriptorBytes;
            if (FeatureGenerator == null && ArtifactProvider != null) {
                descriptorBytes = ArtifactProvider.GetArtifact<byte[]>(TokenNameFinderModel.GeneratorDescriptorEntry);
            } else {
                descriptorBytes = FeatureGenerator;
            }

            if (descriptorBytes != null) {
                var descriptorIn = new MemoryStream(descriptorBytes);

                try {
                    return GeneratorFactory.Create(descriptorIn, identifier => {
                        try {
                            if (ArtifactProvider != null) {
                                return ArtifactProvider.GetArtifact<object>(identifier);
                            }
                            if (Resources.ContainsKey(identifier))
                                return Resources[identifier];

                            return null;
                        } catch (Exception ex) {
                            throw new FeatureGeneratorException("A exception with the feature generator has occured.",
                                ex);
                        }
                    });
                } catch (InvalidFormatException ex) {
                    throw new FeatureGeneratorException(
                        // It is assumed that the creation of the feature generation does not
                        // fail after it succeeded once during model loading.

                        // But it might still be possible that such an exception is thrown,
                        // in this case the caller should not be forced to handle the exception
                        // and a Runtime Exception is thrown instead.

                        // If the re-creation of the feature generation fails it is assumed
                        // that this can only be caused by a programming mistake and therefore
                        // throwing a Runtime Exception is reasonable
                        "An error during the creation or re-creation of a feature generator has occurred.", ex);
                }
            }
            return null;
        }

        #endregion

        #region . CreateSequenceCodec .

        /// <summary>
        /// Creates the sequence codec.
        /// </summary>
        /// <returns>ISequenceCodec&lt;System.String&gt;.</returns>
        public virtual ISequenceCodec<string> CreateSequenceCodec() {
            if (ArtifactProvider != null) {
                var codecName = ArtifactProvider.Manifest[TokenNameFinderModel.SequenceCodecNameParam];
                if (codecName != null) {
                    // TODO: This will not work with java code! In order to resolve this issue an type resolver must be implemented.
                    var type = Type.GetType(codecName);

                    if (type != null && typeof (ISequenceCodec<string>).IsAssignableFrom(type)) {
                        return (ISequenceCodec<string>) Activator.CreateInstance(type);
                    }
                } else {
                    return new BioCodec();
                }
            }
            return SequenceCodec;
        }

        #endregion
        
    }
}