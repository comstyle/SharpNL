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

namespace SharpNL.Utility.Evaluation {
    /// <summary>
    /// The FMeasure is an utility class for evaluators which measure precision,
    /// recall and the resulting f-measure (weighted harmonic mean).
    /// </summary>
    /// <typeparam name="T">The type of the objects to be evaluated.</typeparam>
    /// <remarks>Evaluation results are the arithmetic mean of the precision scores calculated
    /// for each reference sample and the arithmetic mean of the recall scores
    /// calculated for each reference sample.</remarks>
    public sealed class FMeasure<T> where T : class {
        private long selected;
        private long target;
        private long truePositive;

        #region + Properties .

        #region . PrecisionScore .

        /// <summary>
        /// Retrieves the arithmetic mean of the precision scores calculated for each evaluated sample. 
        /// </summary>
        /// <value>The arithmetic mean of all precision scores</value>
        /// <remarks>In other words the precision score means the percent of selected items that are correct.</remarks>
        public double PrecisionScore {
            get { return selected > 0 ? truePositive/(double) selected : 0; }
        }

        #endregion

        #region . RecallScore .

        /// <summary>
        /// Retrieves the arithmetic mean of the recall score calculated for each evaluated sample.
        /// </summary>
        /// <value>The arithmetic mean of all recall scores</value>
        /// <remarks>
        /// In other words is the opposite measure of <see cref="PrecisionScore"/>, means the percent
        /// of correct items that are selected. 
        /// </remarks>
        public double RecallScore {
            get { return target > 0 ? truePositive/(double) target : 0; }
        }

        #endregion

        #region . Value .

        /// <summary>
        /// Retrieves the f-measure score.
        /// <para>
        ///   f-measure = 2 * precision * recall / (precision + recall)
        /// </para>
        /// </summary>
        /// <value>The value.</value>
        public double Value {
            get {
                var p = PrecisionScore;
                var r = RecallScore;
                if (p + r > 0) {
                    return 2*(p*r)/(p + r);
                }
                return -1; // cannot divide by zero, return error code
            }
        }

        #endregion

        #endregion

        #region . CountTruePositives .

        /// <summary>
        /// This method counts the number of objects which are equal and occur in the references and predictions arrays.
        /// Matched items are removed from the prediction list.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <param name="predictions">The predictions.</param>
        /// <returns>The number of true positives.</returns>
        public static int CountTruePositives(T[] references, T[] predictions) {

            var predListSpans = new List<T>(predictions);
            int truePositives = 0;
            T matchedItem = null;

            for (int referenceIndex = 0; referenceIndex < references.Length; referenceIndex++) {
                object referenceName = references[referenceIndex];

                for (int predIndex = 0; predIndex < predListSpans.Count; predIndex++) {

                    if (referenceName.Equals(predListSpans[predIndex])) {
                        matchedItem = predListSpans[predIndex];
                        truePositives++;
                    }
                }
                if (matchedItem != null) {
                    predListSpans.Remove(matchedItem);
                }
            }
            return truePositives;
        }

        #endregion

        #region . UpdateScores .
        /// <summary>
        /// Updates the score based on the number of true positives and the number of predictions and references.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <param name="predictions">The predictions.</param>
        public void UpdateScores(T[] references, T[] predictions) {
            truePositive += CountTruePositives(references, predictions);
            selected += predictions.Length;
            target += references.Length;
        }

        #endregion

        #region . MergeInto .
        /// <summary>
        /// Merge results into FMeasure metric.
        /// </summary>
        /// <param name="measure">The measure.</param>
        public void MergeInto(FMeasure<T> measure) {
            selected += measure.selected;
            target += measure.target;
            truePositive += measure.truePositive;
        }
        #endregion

        #region . Precision .

        /// <summary>
        /// Calculates the precision score for the given reference and predicted spans.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <param name="predictions">The predictions.</param>
        /// <returns>The precision score or NaN if there are no predicted spans.</returns>
        public static double Precision(T[] references, T[] predictions) {
            if (predictions.Length > 0) {
                return CountTruePositives(references, predictions)/(double) predictions.Length;
            }
            return double.NaN;
        }

        #endregion

        #region . Recall .
        /// <summary>
        /// Calculates the recall score for the given reference and predicted spans.
        /// </summary>
        /// <param name="references">The references.</param>
        /// <param name="predictions">The predictions.</param>
        /// <returns>The recall score or NaN if there are no reference spans.</returns>
        public static double Recall(T[] references, T[] predictions) {
            if (references.Length > 0) {
                return CountTruePositives(references, predictions) / (double)references.Length;
            }
            return double.NaN;
        }

        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("Precision: {0}\nRecall: {1}\nF-Measure: {2}", PrecisionScore, RecallScore, Value);
        }
        #endregion

    }
}