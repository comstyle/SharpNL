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
    public class ChunkerME : IChunker {
        public const int DEFAULT_BEAM_SIZE = 10;

        private readonly IChunkerContextGenerator contextGenerator;
        private readonly ISequenceValidator<string> sequenceValidator;
        private Sequence bestSequence;

        /// <summary>
        /// The model used to assign chunk tags to a sequence of tokens.
        /// </summary>
        protected ML.Model.ISequenceClassificationModel<string> model;


        public ChunkerME(ChunkerModel model) {
            contextGenerator = model.Factory.GetContextGenerator();
            sequenceValidator = model.Factory.GetSequenceValidator();

            var seqModel = model.ChunkerSequenceModel;
            this.model = seqModel ?? new BeamSearch(model.BeamSize, model.MaxentModel);
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

        public static ChunkerModel Train(string languageCode, IObjectStream<ChunkSample> samples,
            TrainingParameters parameters, ChunkerFactory factory) {
            var trainerType = TrainerFactory.GetTrainerType(parameters);
            if (!trainerType.HasValue) {
                throw new InvalidOperationException("The trainer was not specified.");
            }

            var manifestInfoEntries = new Dictionary<string, string>();

            IMaxentModel chunkerModel = null;
            ML.Model.ISequenceClassificationModel<string> seqChunkerModel = null;

            switch (trainerType) {
                case TrainerType.SequenceTrainer:
                    var st = TrainerFactory.GetSequenceModelTrainer(parameters, manifestInfoEntries);

                    // TODO: This will probably cause issue, since the feature generator uses the outcomes array

                    var ss = new ChunkSampleSequenceStream(samples, factory.GetContextGenerator());

                    seqChunkerModel = st.Train(ss);
                    break;
                case TrainerType.EventModelTrainer:
                    var es = new ChunkerEventStream(samples, factory.GetContextGenerator());
                    var et = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries);

                    chunkerModel = et.Train(es);
                    break;
                default:
                    throw new NotSupportedException("Trainer type is not supported.");
            }

            if (chunkerModel != null) {
                return new ChunkerModel(languageCode, chunkerModel, manifestInfoEntries, factory);
            }
            return new ChunkerModel(languageCode, seqChunkerModel, manifestInfoEntries, factory);
        }
    }
}