using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.Model;

namespace SharpNL.NameFind {
    // TODO: Fix the model validation, on loading via constructors and input streams
    public class TokenNameFinderModel : BaseModel {

        internal const string ComponentName = "NameFinderME";

        internal const string MaxentModelEntry = "nameFinder.model";
        internal const string GeneratorDescriptorEntry = "generator.featuregen";
        internal const string SequenceCodecNameParam = "sequenceCodecImplName";

        #region + Constructors .

        public TokenNameFinderModel(
            string languageCode, 
            ISequenceClassificationModel<string> nameFinderModel, 
            byte[] generatorDescriptor,
            Dictionary<string, object> resources,
            Dictionary<string, string> manifestInfoEntries,
            ISequenceCodec<string> sequenceCodec) 
            : base (ComponentName, languageCode, manifestInfoEntries) {

            Init(nameFinderModel, generatorDescriptor, resources, manifestInfoEntries, sequenceCodec);

            if (!sequenceCodec.AreOutcomesCompatible(nameFinderModel.GetOutcomes())) {
                throw new InvalidOperationException("Model not compatible with name finder!");
            }
        }

        public TokenNameFinderModel(
            string languageCode,
            IMaxentModel nameFinderModel,
            int beamSize,
            byte[] generatorDescriptor,
            Dictionary<string, object> resources,
            Dictionary<string, string> manifestInfoEntries,
            ISequenceCodec<string> sequenceCodec)
            : base(ComponentName, languageCode, manifestInfoEntries) {

            Manifest[Parameters.BeamSize] = beamSize.ToString(CultureInfo.InvariantCulture);

            Init(nameFinderModel, generatorDescriptor, resources, manifestInfoEntries, sequenceCodec);

            

        }

        #endregion

        #region + Properties .

        #region . Factory .

        public TokenNameFinderFactory Factory {
            get { return (TokenNameFinderFactory) ToolFactory; }
        }
        #endregion

        #endregion


        private void Init(object nameFinderModel,
                          byte[] generatorDescriptor,
                          Dictionary<string, object> resources,
                          Dictionary<string, string> manifestInfoEntries,
                          ISequenceCodec<string> seqCodec) {


            // TODO: make compatible !
            Manifest[SequenceCodecNameParam] = seqCodec.GetType().Name;

            artifactMap[MaxentModelEntry] = nameFinderModel;

            if (generatorDescriptor != null && generatorDescriptor.Length > 0)
                artifactMap[GeneratorDescriptorEntry] = generatorDescriptor;

            if (resources != null) {
                // The resource map must not contain key which are already taken
                // like the name finder maxent model name

                if (resources.ContainsKey(MaxentModelEntry) ||
                    resources.ContainsKey(GeneratorDescriptorEntry)) {
                    throw new ArgumentException();
                }

                // TODO: Add checks to not put resources where no serializer exists,
                //       make that case fail here, should be done in the BaseModel
                foreach (var resource in resources) {
                    artifactMap.Add(resource.Key, resource.Value);
                }
            }
            CheckArtifactMap();
        }

        private bool IsModelValid(IMaxentModel model) {

            var outcomes = new string[model.GetNumOutcomes()];

            for (int i = 0; i < model.GetNumOutcomes(); i++) {
                outcomes[i] = model.GetOutcome(i);
            }

            return Factory.CreateSequenceCodec().AreOutcomesCompatible(outcomes);
        }

        /// <summary>
        /// Gets the default tool factory.
        /// </summary>
        /// <returns>The default tool factory.</returns>
        protected override Type GetDefaultFactory() {
            throw new NotImplementedException();
        }
    }
}
