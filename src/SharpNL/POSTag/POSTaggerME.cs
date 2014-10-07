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
using System.Threading;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.NGram;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using Dic = SharpNL.Dictionary.Dictionary;
using Sequence = SharpNL.Utility.Sequence;

namespace SharpNL.POSTag {
    /// <summary>
    /// A part-of-speech tagger that uses maximum entropy.  Tries to predict whether
    /// words are nouns, verbs, or any of 70 other POS tags depending on their
    /// surrounding context.
    /// </summary>
    public class POSTaggerME : IPOSTagger {

        /// <summary>
        /// The default beam size for the <see cref="POSTaggerME"/>.
        /// </summary>
        public const int DefaultBeamSize = 3;

        private readonly POSModel modelPackage;

        protected Dic ngramDictionary;

        /// <summary>
        /// Says whether a filter should be used to check whether a tag assignment
        /// is to a word outside of a closed class.
        /// </summary>
        protected bool useClosedClassTagsFilter = false;

        /// <summary>
        /// The size of the beam to be used in determining the best sequence of pos tags.
        /// </summary>
        protected int size;


        private Sequence bestSequence;

        private readonly ML.Model.ISequenceClassificationModel<string> model;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="POSTaggerME"/> with the provided model
        /// and the default beam size of 3.
        /// </summary>
        /// <param name="model">The model.</param>
        public POSTaggerME(POSModel model) : this(model, DefaultBeamSize, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="POSTaggerME" /> with the provided
        /// model and provided beam size.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="beamSize">Size of the beam.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="model"/></exception>
        /// <exception cref="System.InvalidOperationException">Unable to retrieve the model.</exception>
        public POSTaggerME(POSModel model, int beamSize, int cacheSize) {
            if (model == null)
                throw new ArgumentNullException("model");


            size = beamSize;
            modelPackage = model;

            TagDictionary = modelPackage.Factory.TagDictionary;
            ContextGenerator = model.Factory.GetPOSContextGenerator(cacheSize);
            SequenceValidator = modelPackage.Factory.GetSequenceValidator();

            this.model = model.GetPosSequenceModel();

            if (this.model == null) {
                throw new InvalidOperationException("Unable to retrieve the model.");
            }


        }

        #endregion
        
        #region + Propreties .

        #region . ContextGenerator .
        /// <summary>
        /// Gets the feature context generator.
        /// </summary>
        /// <value>The feature context generator.</value>
        protected IPOSContextGenerator ContextGenerator { get; private set; }
        #endregion

        #region . TagDictionary .
        /// <summary>
        /// Gets tag dictionary used for restricting words to a fixed set of tags.
        /// </summary>
        /// <value>The tag dictionary used for restricting words to a fixed set of tags.</value>
        protected ITagDictionary TagDictionary { get; private set; }
        #endregion

        #region . SequenceValidator .

        /// <summary>
        /// Gets the sequence validator.
        /// </summary>
        /// <value>The sequence validator.</value>
        protected ISequenceValidator<string> SequenceValidator { get; private set; }

        #endregion

        #region . Probabilities .
        /// <summary>
        /// Gets an array with the probabilities for each tag of the last tagged sentence.
        /// </summary>
        /// <value>An array with the probabilities for each tag of the last tagged sentence.</value>
        public double[] Probabilities {
            get { return bestSequence.Probabilities.ToArray(); }
        }
        #endregion

        #endregion

        #region . BuildNGramDictionary .

        /// <summary>
        /// Builds the NGram dictionary with the given samples.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="cutoff">The cutoff.</param>
        /// <returns>The NGram dictionary.</returns>
        public static Dic BuildNGramDictionary(IObjectStream<POSSample> samples, int cutoff) {

            var model = new NGramModel();
            POSSample sample;

            while ((sample = samples.Read()) != null) {

                if (sample.Sentence.Length > 0) {
                    model.Add(new StringList(sample.Sentence), 1, 1);
                }

            }
            model.CutOff(cutoff, int.MaxValue);

            return model.ToDictionary();
        }

        #endregion

        #region . GetAllPosTags .
        /// <summary>
        /// Gets an array of all possible part-of-speech tags from the tagger.
        /// </summary>
        /// <returns>An array of all possible part-of-speech tags from the tagger.</returns>
        public string[] GetAllPosTags() {
            return model.GetOutcomes();
        }
        #endregion

        #region + GetOrderedTags .
        /// <summary>
        /// Gets the ordered tags by probability.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="index">The index.</param>
        /// <returns>String[].</returns>
        /// <exception cref="System.NotSupportedException">This method can only be called if the classification model is an event model!</exception>
        public String[] GetOrderedTags(string[] words, string[] tags, int index) {
            return GetOrderedTags(words, tags, index, null);
        }

        /// <summary>
        /// Gets the ordered tags by probability.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="index">The index.</param>
        /// <param name="tagProbs">The tag probs.</param>
        /// <returns>String[].</returns>
        /// <exception cref="System.NotSupportedException">This method can only be called if the classification model is an event model!</exception>
        public string[] GetOrderedTags(string[] words, string[] tags, int index, double[] tagProbs) {

            var posModel = modelPackage.MaxentModel;
            if (posModel != null) {
                var probs = posModel.Eval(ContextGenerator.GetContext(index, words, tags, null));
                var orderedTags = new string[probs.Length];
                for (int i = 0; i < probs.Length; i++) {
                    int max = 0;
                    for (int ti = 0; ti < probs.Length; ti++) {
                        if (probs[ti] > probs[max])
                            max = ti;
                    }
                    orderedTags[i] = posModel.GetOutcome(max);

                    if (tagProbs != null) {
                        tagProbs[i] = probs[max];
                    }
                    probs[max] = 0;
                }
                return orderedTags;
            }

            throw new NotSupportedException("This method can only be called if the classification model is an event model!");
        }

        #endregion

        #region . PopulatePOSDictionary .

        public static void PopulatePOSDictionary(IObjectStream<POSSample> samples, IMutableTagDictionary dictionary, bool caseSensitive, int cutoff) {

            var newEntries = new Dictionary<string, Dictionary<string, int>>();
            POSSample sample;
            while ((sample = samples.Read()) != null) {

                for (int i = 0; i < sample.Sentence.Length; i++) {
                    if (!StringPattern.Recognize(sample.Sentence[i]).ContainsDigit) {
                        string word = caseSensitive ? sample.Sentence[i] : sample.Sentence[i].ToLowerInvariant();

                        if (!newEntries.ContainsKey(word)) {
                            newEntries.Add(word, new Dictionary<string, int>());
                        }

                        var dicTags = dictionary.GetTags(word);
                        if (dicTags != null) {
                            foreach (var tag in dicTags) {
                                if (!newEntries[word].ContainsKey(tag)) {
                                    newEntries[word].Add(tag, cutoff);
                                }
                            }
                        }

                        if (!newEntries[word].ContainsKey(sample.Tags[i])) {
                            newEntries[word].Add(sample.Tags[i], 1);
                        } else {
                            newEntries[word][sample.Tags[i]]++;
                        }
                    }
                }
            }

            foreach (var wordEntry in newEntries) {
                var tagsForWord = new List<string>();
                foreach (var entry in wordEntry.Value) {
                    if (entry.Value >= cutoff) {
                        tagsForWord.Add(entry.Key);
                    }
                }
                if (tagsForWord.Count > 0) {
                    dictionary.Put(wordEntry.Key, tagsForWord.ToArray());
                }
            }
        }
        #endregion

        #region + Tag .

        /// <summary>
        /// Assigns the sentence of tokens pos tags.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be tagged.</param>
        /// <returns>an array of pos tags for each token provided in sentence.</returns>
        public string[] Tag(string[] sentence) {
            return Tag(sentence, null);
        }

        /// <summary>
        /// Assigns the sentence of tokens pos tags.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be tagged.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>an array of pos tags for each token provided in sentence.</returns>
        public string[] Tag(string[] sentence, object[] additionalContext) {
            bestSequence = model.BestSequence(sentence, additionalContext, ContextGenerator, SequenceValidator);
            return bestSequence.Outcomes.ToArray();
        }

        /// <summary>
        /// Returns at most the specified number of taggings for the specified sentence.
        /// </summary>
        /// <param name="numTaggings">The number of tagging to be returned.</param>
        /// <param name="sentence">An array of tokens which make up a sentence.</param>
        /// <returns>At most the specified number of taggings for the specified sentence.</returns>
        public string[][] Tag(int numTaggings, string[] sentence) {
            var bestSequences = model.BestSequences(numTaggings, sentence, null, ContextGenerator, SequenceValidator);
            var tags = new string[bestSequences.Length][];
            for (int i = 0; i < tags.Length; i++) {
                tags[i] = bestSequences[i].Outcomes.ToArray();
            }
            return tags;
        }

        #endregion

        #region . TopKSequences .

        /// <summary>
        /// Returns the top k sequences for the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be evaluated.</param>
        /// <returns>The top k sequences for the specified sentence.</returns>
        public Sequence[] TopKSequences(string[] sentence) {
            return TopKSequences(sentence, null);
        }

        /// <summary>
        /// Returns the top k sequences for the specified sentence.
        /// </summary>
        /// <param name="sentence">The sentence of tokens to be evaluated.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The top k sequences for the specified sentence.</returns>
        public Sequence[] TopKSequences(string[] sentence, object[] additionalContext) {
            return model.BestSequences(size, sentence, additionalContext, ContextGenerator, SequenceValidator);
        }

        #endregion

        #region + Train .

        public static POSModel Train(
            string languageCode, 
            IObjectStream<POSSample> samples, 
            TrainingParameters trainParams,
            POSTaggerFactory posFactory) {

            //int beamSize = trainParams.Get(Parameters.BeamSize, NameFinderME.DefaultBeamSize);

            var contextGenerator = posFactory.GetPOSContextGenerator();
            var manifestInfoEntries = new Dictionary<string, string>();

            var trainerType = TrainerFactory.GetTrainerType(trainParams);

            IMaxentModel posModel = null;
            ML.Model.ISequenceClassificationModel<string> seqPosModel = null;
            switch (trainerType) {
                case TrainerType.EventModelTrainer:
                    var es = new POSSampleEventStream(samples, contextGenerator);
                    var trainer = TrainerFactory.GetEventTrainer(trainParams, manifestInfoEntries);

                    posModel = trainer.Train(es);
                    break;
                case TrainerType.EventModelSequenceTrainer:
                    var ss = new POSSampleSequenceStream(samples, contextGenerator);
                    var trainer2 = TrainerFactory.GetEventModelSequenceTrainer(trainParams, manifestInfoEntries);

                    posModel = trainer2.Train(ss);
                    break;
                case TrainerType.SequenceTrainer:
                    var trainer3 = TrainerFactory.GetSequenceModelTrainer(trainParams, manifestInfoEntries);

                    // TODO: This will probably cause issue, since the feature generator uses the outcomes array

                    var ss2 = new POSSampleSequenceStream(samples, contextGenerator);
                    seqPosModel = trainer3.Train(ss2);
                    break;
                default:
                    throw new NotSupportedException("Trainer type is not supported.");
            }

            if (posModel != null) {
                return new POSModel(languageCode, posModel, manifestInfoEntries, posFactory);
            } 

            return new POSModel(languageCode, seqPosModel, manifestInfoEntries, posFactory);

        }
        #endregion
    }
}