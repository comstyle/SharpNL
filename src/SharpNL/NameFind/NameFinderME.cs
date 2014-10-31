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
using System.Text.RegularExpressions;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using Sequence = SharpNL.Utility.Sequence;

namespace SharpNL.NameFind {
    /// <summary>
    /// Class for creating a maximum-entropy-based name finder.
    /// </summary>
    public class NameFinderME : ITokenNameFinder {
        /// <summary>
        /// The default beam size.
        /// </summary>
        public const int DefaultBeamSize = 3;

        /// <summary>
        /// The default span type.
        /// </summary>
        public const string DefaultType = "default";

        internal const string START = "start";
        internal const string Continue = "cont";
        internal const string LAST = "last";
        internal const string UNIT = "unit";
        internal const string Other = "other";

        /// <summary>
        /// Represents a empty additional context.
        /// </summary>
        private static readonly string[][] Empty = {};

        private static readonly Regex TypedOutcomePattern;

        private readonly ISequenceCodec<string> sequenceCodec;
        private readonly ISequenceValidator<string> sequenceValidator;
        private readonly INameContextGenerator contextGenerator;

        protected readonly ML.Model.ISequenceClassificationModel<string> model;

        private readonly AdditionalContextFeatureGenerator additionalContextFeatureGenerator;

        private Sequence bestSequence;

        static NameFinderME() {
            TypedOutcomePattern = new Regex("(.+)-\\w+", RegexOptions.Compiled);
        }

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFinderME"/> using the given <see cref="TokenNameFinderModel"/>.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="System.ArgumentNullException">model</exception>
        public NameFinderME(TokenNameFinderModel model) {
            if (model == null)
                throw new ArgumentNullException("model");

            sequenceCodec = model.Factory.CreateSequenceCodec();
            sequenceValidator = sequenceCodec.CreateSequenceValidator();

            this.model = model.NameFinderSequenceModel;

            contextGenerator = model.Factory.CreateContextGenerator();

            // TODO: We should deprecate this. And come up with a better solution!
            additionalContextFeatureGenerator = new AdditionalContextFeatureGenerator();
            contextGenerator.AddFeatureGenerator(
                new WindowFeatureGenerator(additionalContextFeatureGenerator, 8, 8));
        }

        #endregion

        #region + Properties .

        #region . Probabilities .

        /// <summary>
        /// Gets an array with the probabilities of the last decoded sequence.
        /// The sequence was determined based on the previous call to <see cref="M:Find"/>.
        /// </summary>
        /// <value>The probabilities.</value>
        public double[] Probabilities {
            get { return bestSequence.Probabilities.ToArray(); }
        }

        #endregion

        #endregion

        #region . ClearAdaptiveData .

        /// <summary>
        /// Forgets all adaptive data which was collected during previous calls to one of the find methods.
        /// </summary>
        /// <remarks>This method is typical called at the end of a document.</remarks>
        public void ClearAdaptiveData() {
            contextGenerator.ClearAdaptiveData();
        }

        #endregion

        #region . CreateFeatureGenerator .

        /// <summary>
        /// Creates the default feature generators.
        /// </summary>
        /// <returns>The <see cref="IAdaptiveFeatureGenerator"/> feature generator object.</returns>
        internal static IAdaptiveFeatureGenerator CreateFeatureGenerator() {
            return new CachedFeatureGenerator(
                new IAdaptiveFeatureGenerator[] {
                    new WindowFeatureGenerator(new TokenFeatureGenerator(), 2, 2),
                    new WindowFeatureGenerator(new TokenClassFeatureGenerator(true), 2, 2),
                    new OutcomePriorFeatureGenerator(),
                    new PreviousMapFeatureGenerator(),
                    new BigramNameFeatureGenerator(),
                    new SentenceFeatureGenerator(true, false)
                });
        }

        /// <summary>
        /// Creates the feature generators with the given parameters.
        /// </summary>
        /// <param name="generatorDescriptor">The generator descriptor.</param>
        /// <param name="resources">The resources dictionary.</param>
        /// <returns>The <see cref="IAdaptiveFeatureGenerator"/> feature generator object.</returns>
        internal static IAdaptiveFeatureGenerator CreateFeatureGenerator(byte[] generatorDescriptor,
            Dictionary<string, object> resources) {
            if (generatorDescriptor == null)
                return null;

            return GeneratorFactory.Create(new MemoryStream(generatorDescriptor), identifier => {
                try {
                    if (resources != null && resources.ContainsKey(identifier))
                        return resources[identifier];

                    return null;
                } catch (Exception ex) {
                    throw new FeatureGeneratorException("A exception with the feature generator has occured.", ex);
                }
            });
        }

        #endregion

        #region . DropOverlappingSpans .

#if DEBUG
        [Obsolete("Method moved to Span class.")]
        public static Span[] DropOverlappingSpans(Span[] spans) {
            throw new NotSupportedException();
        }
#endif

        #endregion

        #region . ExtractNameType .

        /// <summary>
        /// Gets the name type from the outcome.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The name type, or <c>null</c> if not set.</returns>
        public static string ExtractNameType(string outcome) {
            if (string.IsNullOrEmpty(outcome))
                return null;

            var match = TypedOutcomePattern.Match(outcome);
            return match.Success ? match.Groups[1].Value : null;
        }

        #endregion

        #region + Find .

        /// <summary>
        /// Generates name tags for the given sequence, typically a sentence, returning token spans for any identified names.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence, typically a sentence.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string[] tokens) {
            return Find(tokens, Empty);
        }

        /// <summary>
        /// Generates name tags for the given sequence, typically a sentence, returning token spans for any identified names.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence, typically a sentence.</param>
        /// <param name="additionalContext">Features which are based on context outside of the sentence but which should also be used.</param>
        /// <returns>An array of spans for each of the names identified.</returns>
        public Span[] Find(string[] tokens, string[][] additionalContext) {
            additionalContextFeatureGenerator.SetCurrentContext(additionalContext);

            bestSequence = model.BestSequence(tokens,
                Array.ConvertAll(additionalContext, input => (object) input),
                contextGenerator,
                sequenceValidator);

            var outcomes = bestSequence.Outcomes.ToArray();

            contextGenerator.UpdateAdaptiveData(tokens, outcomes);

            var spans = sequenceCodec.Decode(outcomes);

            var probs = Probs(spans);
            for (var i = 0; i < probs.Length; i++) {
                spans[i].Probability = probs[i];
            }

            return spans;
        }

        #endregion

        #region + Train .
        /// <summary>
        /// Trains a name finder model.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="samples">The training samples.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="factory">The name finder factory.</param>
        /// <returns>the newly <see cref="TokenNameFinderModel"/> trained model.</returns>
        public static TokenNameFinderModel Train(
            string languageCode, 
            IObjectStream<NameSample> samples, 
            TrainingParameters parameters,
            TokenNameFinderFactory factory) {
            return Train(languageCode, DefaultType, samples, parameters, factory);
        }
        /// <summary>
        /// Trains a name finder model.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="samples">The training samples.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="factory">The name finder factory.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.</param>
        /// <returns>the newly <see cref="TokenNameFinderModel"/> trained model.</returns>
        public static TokenNameFinderModel Train(string languageCode, IObjectStream<NameSample> samples, TrainingParameters parameters, TokenNameFinderFactory factory, Monitor monitor) {
            return Train(languageCode, DefaultType, samples, parameters, factory, monitor);
        }

        /// <summary>
        /// Trains a name finder model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="type">Overrides the type parameter in the provided samples. This value can be null.</param>
        /// <param name="samples">The training samples.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="factory">The name finder factory.</param>
        /// <returns>the newly <see cref="TokenNameFinderModel"/> trained model.</returns>
        public static TokenNameFinderModel Train(
            string languageCode,
            string type,
            IObjectStream<NameSample> samples,
            TrainingParameters parameters,
            TokenNameFinderFactory factory) {

            return Train(languageCode, type, samples, parameters, factory, null);
        }

        /// <summary>
        /// Trains a name finder model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="type">Overrides the type parameter in the provided samples. This value can be null.</param>
        /// <param name="samples">The training samples.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="factory">The name finder factory.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.</param>
        /// <returns>the newly <see cref="TokenNameFinderModel"/> trained model.</returns>
        public static TokenNameFinderModel Train(string languageCode, string type, IObjectStream<NameSample> samples, TrainingParameters parameters, TokenNameFinderFactory factory, Monitor monitor) {
            var beamSize = parameters.Get(Parameters.BeamSize, DefaultBeamSize);
            var manifestInfoEntries = new Dictionary<string, string>();
            var trainerType = TrainerFactory.GetTrainerType(parameters);

            IMaxentModel meModel = null;
            ML.Model.ISequenceClassificationModel<string> scModel = null;

            switch (trainerType) {
                case TrainerType.EventModelTrainer:
                    var eventStream = new NameFinderEventStream(samples, type, factory.CreateContextGenerator(),
                        factory.CreateSequenceCodec());
                    var nfTrainer = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries, monitor);

                    meModel = nfTrainer.Train(eventStream);
                    break;
                case TrainerType.EventModelSequenceTrainer:
                    var sampleStream = new NameSampleSequenceStream(samples, factory.CreateContextGenerator());
                    var nsTrainer = TrainerFactory.GetEventModelSequenceTrainer(parameters, manifestInfoEntries, monitor);

                    meModel = nsTrainer.Train(sampleStream);
                    break;
                case TrainerType.SequenceTrainer:
                    var sequenceStream = new NameSampleSequenceStream(samples, factory.CreateContextGenerator());
                    var sqTrainer = TrainerFactory.GetSequenceModelTrainer(parameters, manifestInfoEntries, monitor);


                    scModel = sqTrainer.Train(sequenceStream);
                    break;
                default:
                    throw new InvalidOperationException("Unexpected trainer type!");
            }

            if (scModel != null) {
                return new TokenNameFinderModel(
                    languageCode,
                    scModel,
                    factory.FeatureGenerator,
                    factory.Resources,
                    manifestInfoEntries,
                    factory.SequenceCodec);
            }

            return new TokenNameFinderModel(
                languageCode,
                meModel,
                beamSize,
                factory.FeatureGenerator,
                factory.Resources,
                manifestInfoEntries,
                factory.SequenceCodec);
        }

        #endregion

        #region . Probs .
        /// <summary>
        /// Returns an array of probabilities for each of the specified spans which is
        /// the arithmetic mean of the probabilities for each of the outcomes which
        /// make up the span.
        /// </summary>
        /// <param name="spans">The spans of the names for which probabilities are desired.</param>
        /// <returns>An array of probabilities for each of the specified spans.</returns>
        public double[] Probs(Span[] spans) {
            var sprobs = new double[spans.Length];

            for (var si = 0; si < spans.Length; si++) {
                double p = 0;

                for (var oi = spans[si].Start; oi < spans[si].End; oi++) {
                    p += bestSequence.Probabilities[oi];
                }

                p /= spans[si].Length;

                sprobs[si] = p;
            }

            return sprobs;
        }
        #endregion

    }
}