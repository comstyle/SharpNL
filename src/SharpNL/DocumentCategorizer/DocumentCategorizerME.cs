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
using SharpNL.Utility;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Maximum entropy implementation of <see cref="IDocumentCategorizer"/>.
    /// </summary>
    /// <example>
    /// This example shows how to use the <see cref="DocumentCategorizerME"/>.
    /// <code>
    /// var tokens = new[] { "This", "is", "a", "sample", "text", "." };
    /// var model = new DocumentCategorizerModel(new FileStream("modelfile.bin", FileMode.Open));
    /// var categorizer = new DocumentCategorizerME(model);
    /// 
    /// double[] outcomes = categorizer.Categorize(tokens);
    /// string category = categorizer.GetBestCategory(outcomes);
    /// 
    /// </code>
    /// </example>
    public class DocumentCategorizerME : IDocumentCategorizer {

        private readonly DocumentCategorizerModel model;
        private readonly DocumentCategorizerContextGenerator cg;

        
        #region + Constructors .

        /// <summary>
        /// Initializes static members of the <see cref="DocumentCategorizerME"/> class.
        /// </summary>
        static DocumentCategorizerME() {
            DefaultFeatureGenerator = new BagOfWordsFeatureGenerator();

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCategorizerME"/> with a document categorizer model.
        /// The default feature generation will be used.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="System.ArgumentNullException">model</exception>
        public DocumentCategorizerME(DocumentCategorizerModel model) {
            if (model == null)
                throw new ArgumentNullException("model");

            cg = new DocumentCategorizerContextGenerator(model.Factory.FeatureGenerators);

            this.model = model;
        }
        #endregion

        #region + Properties .

        #region . DefaultFeatureGenerator .
        /// <summary>
        /// Gets or sets the default feature generator.
        /// </summary>
        public static IFeatureGenerator DefaultFeatureGenerator { get; set; }
        #endregion

        #region . NumberOfCategories .
        /// <summary>
        /// Gets the number of categories.
        /// </summary>
        /// <value>The number of categories.</value>
        public int NumberOfCategories {
            get { return model.MaxentModel.GetNumOutcomes(); }
        }
        #endregion

        #endregion

        #region + Categorize .
        /// <summary>
        /// Categorizes the specified text.
        /// </summary>
        /// <param name="tokens">The text.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public double[] Categorize(string[] tokens) {
            return Categorize(tokens, null);
        }

        /// <summary>
        /// Categorizes the specified document.
        /// </summary>
        /// <param name="document">The document string.</param>
        /// <returns>A double array containing the outcomes.</returns>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public double[] Categorize(string document) {
            return Categorize(model.Factory.Tokenizer.Tokenize(document), null);
        }

        /// <summary>
        /// Categorizes the specified text with extra informations.
        /// </summary>
        /// <param name="tokens">The sentence tokens.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        public double[] Categorize(string[] tokens, Dictionary<string, object> extraInformation) {
            return model.MaxentModel.Eval(cg.GetContext(tokens, extraInformation));
        }

        /// <summary>
        /// Categorizes the specified document with extra information.
        /// </summary>
        /// <param name="document">The document string.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        /// <remarks>The document string will be tokenized by the tokenizer specified in the factory.</remarks>
        public double[] Categorize(string document, Dictionary<string, object> extraInformation) {
            return Categorize(model.Factory.Tokenizer.Tokenize(document), extraInformation);
        }
        #endregion

        #region . GetBestCategory .
        /// <summary>
        /// Returns the best category for the given outcome.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The best category.</returns>
        public string GetBestCategory(double[] outcome) {
            return model.MaxentModel.GetBestOutcome(outcome);
        }
        #endregion

        #region . GetCategory .
        /// <summary>
        /// Gets the category with the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The category.</returns>
        public string GetCategory(int index) {
            return model.MaxentModel.GetOutcome(index);
        }
        #endregion

        #region . GetIndex .
        /// <summary>
        /// Gets the category index.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>Category index.</returns>
        public int GetIndex(string category) {
            return model.MaxentModel.GetIndex(category);
        }
        #endregion

        #region . Train .
        /// <summary>
        /// Trains document categorizer model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The document categorizer factory.</param>
        /// <returns>The trained <see cref="DocumentCategorizerModel"/> model.</returns>
        public static DocumentCategorizerModel Train(
            string languageCode,
            IObjectStream<DocumentSample> samples,
            TrainingParameters parameters,
            DocumentCategorizerFactory factory) {

            return Train(languageCode, samples, parameters, factory, null);
        }

        /// <summary>
        /// Trains document categorizer model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The document categorizer factory.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The trained <see cref="DocumentCategorizerModel"/> model.</returns>
        public static DocumentCategorizerModel Train(string languageCode, IObjectStream<DocumentSample> samples, TrainingParameters parameters, DocumentCategorizerFactory factory, Monitor monitor) {

            var manifestInfoEntries = new Dictionary<string, string>();

            var eventStream = new DocumentCategorizerEventStream(samples, factory.FeatureGenerators);
            var trainer = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries, monitor);
            var model = trainer.Train(eventStream);

            return new DocumentCategorizerModel(languageCode, model, manifestInfoEntries, factory);
        }
        #endregion

        #region + ScoreMap .
        /// <summary>
        /// Returns a map in which the key is the category name and the value is the score.
        /// </summary>
        /// <param name="text">text the input text to classify.</param>
        /// <returns>The dictionary with the categories with the scores.</returns>
        public Dictionary<string, double> ScoreMap(string text) {
            var outcomes = Categorize(text);
            var list = new Dictionary<string, double>(outcomes.Length);
            for (var i = 0; i < list.Count; i++) {
                var cat = GetCategory(i);
                list.Add(cat, outcomes[GetIndex(cat)]);
            }

            return list;
        }

        /// <summary>
        /// Returns a map in which the key is the category name and the value is the score.
        /// </summary>
        /// <param name="tokens">The sentence tokens to classify.</param>
        /// <returns>The dictionary with the categories with the scores.</returns>
        public Dictionary<string, double> ScoreMap(string[] tokens) {
            var outcomes = Categorize(tokens);
            var list = new Dictionary<string, double>(outcomes.Length);
            for (var i = 0; i < list.Count; i++) {
                var cat = GetCategory(i);
                list.Add(cat, outcomes[GetIndex(cat)]);
            }

            return list;
        }

        #endregion

        #region . SortedScoreMap .
        /// <summary>
        /// Returns a map with the score as a key in ascending order.
        /// </summary>
        /// <param name="text">Text the input text to classify.</param>
        /// <returns>A dictionary of categories with the score.</returns>
        public SortedDictionary<double, List<string>> SortedScoreMap(string text) {
            var descendingMap = new SortedDictionary<double, List<string>>(
                Comparer<double>.Create((x, y) => y.CompareTo(x))
            );

            var categorize = Categorize(text);
            var catSize = NumberOfCategories;
            for (var i = 0; i < catSize; i++) {
                var category = GetCategory(i);
                var score = categorize[GetIndex(category)];
                if (descendingMap.ContainsKey(score)) {
                    descendingMap[score].Add(category);
                } else {
                    descendingMap.Add(score, new List<string> {
                        category
                    });
                }
            }
            return descendingMap;
        }
        #endregion

    }
}