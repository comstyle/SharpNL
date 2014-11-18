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

using System.Collections.Generic;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// Interface for classes which categorize documents.
    /// </summary>
    public interface IDocumentCategorizer {

        /// <summary>
        /// Categorizes the specified text.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Categorize(string[] tokens);

        /// <summary>
        /// Categorizes the specified document.
        /// </summary>
        /// <param name="document">The document string.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Categorize(string document);

        /// <summary>
        /// Categorizes the specified text with extra informations.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Categorize(string[] tokens, Dictionary<string, object> extraInformation);


        /// <summary>
        /// Categorizes the specified document with extra information.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="extraInformation">The extra information.</param>
        /// <returns>An array of the probabilities for each of the different outcomes, all of which sum to 1.</returns>
        double[] Categorize(string document, Dictionary<string, object> extraInformation);

        /// <summary>
        /// Returns the best category for the given outcome.
        /// </summary>
        /// <param name="outcome">The outcome.</param>
        /// <returns>The best category.</returns>
        string GetBestCategory(double[] outcome);

        /// <summary>
        /// Returns the category index.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>Category index.</returns>
        int GetIndex(string category);

        /// <summary>
        /// Gets the number of categories.
        /// </summary>
        /// <value>The number of categories.</value>
        int NumberOfCategories { get; }

        /// <summary>
        /// Returns a map in which the key is the category name and the value is the score.
        /// </summary>
        /// <param name="text">text the input text to classify.</param>
        /// <returns>The dictionary with the categories with the scores.</returns>
        Dictionary<string, double> ScoreMap(string text);

        /// <summary>
        /// Returns a map in which the key is the category name and the value is the score.
        /// </summary>
        /// <param name="tokens">The sentence tokens to classify.</param>
        /// <returns>The dictionary with the categories with the scores.</returns>
        Dictionary<string, double> ScoreMap(string[] tokens);
            
        /// <summary>
        /// Returns a map with the score as a key in ascending order.
        /// </summary>
        /// <param name="text">Text the input text to classify.</param>
        /// <returns>A dictionary of categories with the score.</returns>
        /// <returns>
        /// Many categories can have the same score, hence the set as value
        /// </returns>
        SortedDictionary<double, List<string>> SortedScoreMap(string text);


    }
}