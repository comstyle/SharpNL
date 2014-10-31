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
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.Utility;
using BeamSearch = SharpNL.ML.BeamSearch<string>;
using Sequence = SharpNL.Utility.Sequence;

namespace SharpNL.Chunker {
    /// <summary>
    /// The class represents a maximum-entropy-based chunker. Such a chunker can be used to 
    /// find flat structures based on sequence inputs such as noun phrases or named entities.
    /// </summary>
    public class ChunkerME : IChunker {
        public const int DEFAULT_BEAM_SIZE = 10;

        private readonly IChunkerContextGenerator contextGenerator;
        private readonly ISequenceValidator<string> sequenceValidator;
        private Sequence bestSequence;

        /// <summary>
        /// The model used to assign chunk tags to a sequence of tokens.
        /// </summary>
        protected ML.Model.ISequenceClassificationModel<string> model;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerME"/> with the specified <see cref="ChunkerModel"/>.
        /// </summary>
        /// <param name="model">The chunker model.</param>
        public ChunkerME(ChunkerModel model) {
            contextGenerator = model.Factory.GetContextGenerator();
            sequenceValidator = model.Factory.GetSequenceValidator();

            this.model = model.ChunkerSequenceModel ?? new BeamSearch(model.BeamSize, model.MaxentModel);
        }

        /// <summary>
        /// Initializes the current instance with the specified model and the specified beam size.
        /// </summary>
        /// <param name="model">The model for this chunker</param>
        /// <param name="beamSize">The size of the beam that should be used when decoding sequences.</param>
        /// <param name="sequenceValidator">The <see cref="ISequenceValidator{String}"/> to determines whether the outcome is valid for the preceding sequence. This can be used to implement constraints on what sequences are valid..</param>
        /// <param name="contextGenerator">The context generator.</param>
        internal ChunkerME(ChunkerModel model, int beamSize, ISequenceValidator<string> sequenceValidator, IChunkerContextGenerator contextGenerator) {
            // This method is marked as deprecated in the OpenNLP, but it is required in the Parser,
            // I could change the cg in the factory, but its not ideal in this situation (i think) :P

            this.sequenceValidator = sequenceValidator;
            this.contextGenerator = contextGenerator;
            this.model = model.ChunkerSequenceModel ?? new BeamSearch(beamSize, model.MaxentModel);
        }

        #region + Properties .

        #region . Probabilities .

        /// <summary>
        /// Returns an array with the probabilities of the last decoded sequence.
        /// The sequence was determined based on the previous call to <see cref="M:ChunkerME.Chunk"/>.
        /// </summary>
        /// <value>An array with the same number of probabilities as tokens were sent to <see cref="M:ChunkerME.Chunk"/> when it was last called.</value>
        public double[] Probabilities {
            get { return bestSequence == null ? null : bestSequence.Probabilities.ToArray(); }
        }

        #endregion

        #endregion

        #region . Chunk .

        /// <summary>
        /// Generates chunk tags for the given sequence returning the result in an array.
        /// </summary>
        /// <param name="tokens">an array of the tokens or words of the sequence.</param>
        /// <param name="tags">an array of the pos tags of the sequence.</param>
        /// <returns>an array of chunk tags for each token in the sequence.</returns>
        public string[] Chunk(string[] tokens, string[] tags) {
            bestSequence = model.BestSequence(tokens, new object[] {tags}, contextGenerator, sequenceValidator);

            return bestSequence.Outcomes.ToArray();
        }

        #endregion

        #region . ChunkAsSpans .

        /// <summary>
        /// Generates tagged chunk spans for the given sequence returning the result in a span array.
        /// </summary>
        /// <param name="tokens">An array of the tokens or words of the sequence.</param>
        /// <param name="tags">An array of the pos tags of the sequence.</param>
        /// <returns>An array of spans with chunk tags for each chunk in the sequence.</returns>
        public Span[] ChunkAsSpans(string[] tokens, string[] tags) {
            var preds = Chunk(tokens, tags);
            return ChunkSample.PhrasesAsSpanList(tokens, tags, preds);
        }

        #endregion

        #region + TopKSequences .

        /// <summary>
        /// Returns the top k chunk sequences for the specified sentence with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence.</param>
        /// <param name="tags">The pos-tags for the specified sentence.</param>
        /// <returns>The top k chunk sequences for the specified sentence.</returns>
        public Sequence[] TopKSequences(string[] tokens, string[] tags) {
            return model.BestSequences(
                DEFAULT_BEAM_SIZE,
                tokens,
                new object[] {tags},
                contextGenerator,
                sequenceValidator);
        }

        /// <summary>
        /// Returns the top k chunk sequences for the specified sentence with the specified pos-tags.
        /// </summary>
        /// <param name="tokens">The tokens of the sentence.</param>
        /// <param name="tags">The pos-tags for the specified sentence.</param>
        /// <param name="minScore">A lower bound on the score of a returned sequence.</param>
        /// <returns>The top k chunk sequences for the specified sentence.</returns>
        public Sequence[] TopKSequences(string[] tokens, string[] tags, double minScore) {
            return model.BestSequences(
                DEFAULT_BEAM_SIZE,
                tokens,
                new object[] {tags},
                minScore,
                contextGenerator,
                sequenceValidator);
        }

        #endregion

        #region + Train .

        /// <summary>
        /// Trains a chunker model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <returns>The trained <see cref="ChunkerModel"/> object.</returns>
        /// <exception cref="System.InvalidOperationException">The trainer was not specified.</exception>
        /// <exception cref="System.NotSupportedException">Trainer type is not supported.</exception>
        public static ChunkerModel Train(
            string languageCode,
            IObjectStream<ChunkSample> samples,
            TrainingParameters parameters,
            ChunkerFactory factory) {

            return Train(languageCode, samples, parameters, factory, null);
        }

        /// <summary>
        /// Trains a chunker model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.</param>
        /// <returns>The trained <see cref="ChunkerModel"/> object.</returns>
        /// <exception cref="System.InvalidOperationException">The trainer was not specified.</exception>
        /// <exception cref="System.NotSupportedException">Trainer type is not supported.</exception>
        public static ChunkerModel Train(string languageCode, IObjectStream<ChunkSample> samples, TrainingParameters parameters, ChunkerFactory factory, Monitor monitor) {

            var trainerType = TrainerFactory.GetTrainerType(parameters);
            if (!trainerType.HasValue) {
                throw new InvalidOperationException("The trainer was not specified.");
            }

            var manifestInfoEntries = new Dictionary<string, string>();

            IMaxentModel chunkerModel = null;
            ML.Model.ISequenceClassificationModel<string> seqChunkerModel = null;

            switch (trainerType) {
                case TrainerType.SequenceTrainer:
                    var st = TrainerFactory.GetSequenceModelTrainer(parameters, manifestInfoEntries, monitor);

                    // TODO: This will probably cause issue, since the feature generator uses the outcomes array

                    var ss = new ChunkSampleSequenceStream(samples, factory.GetContextGenerator());

                    seqChunkerModel = st.Train(ss);
                    break;
                case TrainerType.EventModelTrainer:
                    var es = new ChunkerEventStream(samples, factory.GetContextGenerator());
                    var et = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries, monitor);

                    chunkerModel = et.Train(es);
                    break;
                default:
                    throw new NotSupportedException("Trainer type is not supported.");
            }

            return chunkerModel != null
                ? new ChunkerModel(languageCode, chunkerModel, manifestInfoEntries, factory) 
                : new ChunkerModel(languageCode, seqChunkerModel, manifestInfoEntries, factory);
        }

        #endregion
    }
}