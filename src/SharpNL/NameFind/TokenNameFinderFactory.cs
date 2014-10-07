using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.NameFind {
    public class TokenNameFinderFactory : BaseToolFactory {


        private const string MaxentModelEntry = "nameFinder.model";

        private byte[] featureGeneratorBytes;

        public TokenNameFinderFactory() {
            SequenceCodec = new BioCodec();
        }

        public TokenNameFinderFactory(
            byte[] featureGeneratorBytes,
            Dictionary<string, object> resources,
            ISequenceCodec<string> seqCodec) {

            this.featureGeneratorBytes = featureGeneratorBytes;
            Resources = resources;
            SequenceCodec = seqCodec;
        }

        #region + Properties .

        #region . Resources .
        /// <summary>
        /// Gets the resources dictionary.
        /// </summary>
        /// <value>The resources dictionary.</value>
        protected Dictionary<string, object> Resources { get; private set; }
        #endregion

        #region . SequenceCodec .
        /// <summary>
        /// Gets the sequence codec.
        /// </summary>
        /// <value>The sequence codec.</value>
        protected ISequenceCodec<string> SequenceCodec { get; private set; }
        #endregion

        #endregion

        #region .  .

        public IAdaptiveFeatureGenerator CreateFeatureGenerators() {
            /*
    byte[] descriptorBytes;
    if (featureGeneratorBytes == null && ArtifactProvider != null) {
        descriptorBytes = ArtifactProvider.GetArtifact<byte[]>(TokenNameFinderModel.GeneratorDescriptorEntry);
    } else {
      descriptorBytes = featureGeneratorBytes;
    }

    if (descriptorBytes != null) {
      var descriptorIn = new ByteArrayInputStream(descriptorBytes);

      IAdaptiveFeatureGenerator generator = null;
      try {
        generator = GeneratorFactory.create(descriptorIn, new FeatureGeneratorResourceProvider() {

          public Object getResource(String key) {
            if (artifactProvider != null) {
              return artifactProvider.getArtifact(key);
            }
            else {
              return resources.get(key);
            }
          }
        });
      } catch (InvalidFormatException e) {
        // It is assumed that the creation of the feature generation does not
        // fail after it succeeded once during model loading.

        // But it might still be possible that such an exception is thrown,
        // in this case the caller should not be forced to handle the exception
        // and a Runtime Exception is thrown instead.

        // If the re-creation of the feature generation fails it is assumed
        // that this can only be caused by a programming mistake and therefore
        // throwing a Runtime Exception is reasonable

        throw new FeatureGeneratorCreationError(e);
      } catch (IOException e) {
        throw new IllegalStateException("Reading from mem cannot result in an I/O error", e);
      }

      return generator;
    }
    else {
      return null;
    }*/
            throw new NotImplementedException();
        }
        #endregion

        #region . CreateSequenceCodec .

        public ISequenceCodec<string> CreateSequenceCodec() {

            if (ArtifactProvider != null) {
                string codecName = ArtifactProvider.Manifest[TokenNameFinderModel.SequenceCodecNameParam];
                if (codecName != null) {

                    // will work?
                    var type = Type.GetType(codecName);

                    if (type != null && typeof (ISequenceCodec<string>).IsAssignableFrom(type)) {
                        return (ISequenceCodec<string>)Activator.CreateInstance(type);
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
