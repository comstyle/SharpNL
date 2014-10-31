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

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// A maximum entropy model is used to evaluate end-of-sentence characters in a
    /// string to determine if they signify the end of a sentence.
    /// </summary>
    public class SentenceDetectorME : ISentenceDetector {
        /// <summary>
        /// Constant indicates a sentence split.
        /// </summary>
        internal const string SPLIT = "s";

        /// <summary>
        /// Constant indicates no sentence split.
        /// </summary>
        internal const string NO_SPLIT = "n";

        private readonly ISentenceContextGenerator cgen;
        private readonly IMaxentModel model;


        private readonly IEndOfSentenceScanner scanner;

        private readonly List<double> sentProbs = new List<double>();

        private readonly bool useTokenEnd;

        public SentenceDetectorME(SentenceModel sentenceModel) {
            model = sentenceModel.MaxentModel;
            cgen = sentenceModel.Factory.GetContextGenerator();
            scanner = sentenceModel.Factory.GetEndOfSentenceScanner();
            useTokenEnd = sentenceModel.UseTokenEnd;
        }

        #region . SentDetect .

        /// <summary>
        /// Detects the sentences in the specified string.
        /// </summary>
        /// <param name="value">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:string[]"/> with the individual sentences as the array elements.</returns>
        public string[] SentDetect(string value) {
            var spans = SentPosDetect(value);

            if (spans.Length > 0) {
                var sentences = new String[spans.Length];

                for (var i = 0; i < spans.Length; i++) {
                    sentences[i] = spans[i].GetCoveredText(value);
                }

                return sentences;
            }
            return new string[] {};
        }

        #endregion

        #region . SentPosDetect .

        /// <summary>
        /// Detects the position of the sentences in the specified string.
        /// </summary>
        /// <param name="text">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into <paramref name="text"/>) for each detected sentence as the individuals array elements.</returns>
        public Span[] SentPosDetect(string text) {
            sentProbs.Clear();

            var enders = scanner.GetPositions(text);
            var positions = new List<int>(enders.Count);

            for (int i = 0, end = enders.Count, index = 0; i < end; i++) {
                var cint = enders[i]; // candidate position

                // skip over the leading parts of non-token final delimiters
                var fws = GetFirstWS(text, cint + 1);

                if (i + 1 < end && enders[i + 1] < fws) {
                    continue;
                }
                if (positions.Count > 0 && cint < positions[positions.Count - 1]) continue;

                var probs = model.Eval(cgen.GetContext(text, cint));
                var bestOutcome = model.GetBestOutcome(probs);

                if (bestOutcome.Equals(SPLIT) && IsAcceptableBreak(text, index, cint)) {
                    if (index != cint) {

                        positions.Add(useTokenEnd
                            ? GetFirstNonWS(text, GetFirstWS(text, cint + 1))
                            : GetFirstNonWS(text, cint + 1));

                        sentProbs.Add(probs[model.GetIndex(bestOutcome)]);
                    }
                    index = cint + 1;
                }
            }

            var starts = new int[positions.Count];
            for (var i = 0; i < starts.Length; i++) {
                starts[i] = positions[i];
            }

            // string does not contain sentence end positions
            if (starts.Length == 0) {
                // remove leading and trailing whitespace
                var start = 0;
                var end = text.Length;

                while (start < text.Length && char.IsWhiteSpace(text[start]))
                    start++;

                while (end > 0 && char.IsWhiteSpace(text[end - 1]))
                    end--;

                if ((end - start) > 0) {
                    sentProbs.Add(1d);
                    return new[] {new Span(start, end)};
                }

                return new Span[0];
            }

            // Convert the sentence end indexes to spans

            var leftover = starts[starts.Length - 1] != text.Length;
            var spans = new Span[leftover ? starts.Length + 1 : starts.Length];

            for (var si = 0; si < starts.Length; si++) {

                int start = si == 0 ? 0 : starts[si - 1];

                // A span might contain only white spaces, in this case the length of
                // the span will be zero after trimming and should be ignored.
                var span = new Span(start, starts[si]).Trim(text);
                if (span.Length > 0) {
                    spans[si] = span;
                } else {
                    sentProbs.Remove(si);
                }
            }

            if (leftover) {
                var span = new Span(starts[starts.Length - 1], text.Length).Trim(text);
                if (span.Length > 0) {
                    spans[spans.Length - 1] = span;
                    sentProbs.Add(1d);
                }
            }
            /**
             * set the prob for each span
             */
            for (var i = 0; i < spans.Length; i++) {
                var prob = sentProbs[i];
                spans[i] = new Span(spans[i], prob);
            }

            return spans;
        }

        #endregion

        #region . IsAcceptableBreak .
        /// <summary>
        /// Allows subclasses to check an overzealous (read: poorly trained) model from flagging 
        /// obvious non-breaks as breaks based on some boolean determination of a breaks acceptability. 
        /// </summary>
        /// <param name="s">The string in which the break occurred.</param>
        /// <param name="fromIndex">The start of the segment currently being evaluated.</param>
        /// <param name="candidateIndex">The index of the candidate sentence ending.</param>
        /// <returns><c>true</c> if if the break is acceptable; otherwise, <c>false</c>.</returns>
        protected bool IsAcceptableBreak(String s, int fromIndex, int candidateIndex) {
            return true;
        }
        #endregion

        #region . GetFirstWS .
        private static int GetFirstWS(string s, int pos) {
            while (pos < s.Length && !char.IsWhiteSpace(s[pos])) {
                pos++;
            }
            return pos;
        }
        #endregion

        #region . GetFirstNonWS .
        private static int GetFirstNonWS(string s, int pos) {
            while (pos < s.Length && char.IsWhiteSpace(s[pos])) {
                pos++;
            }
            return pos;
        }
        #endregion

        #region . GetSentenceProbabilities .
        /// <summary>
        /// Gets the probabilities associated with the most recent calls to <see cref="M:SentDetect"/>.
        /// </summary>
        /// <returns>
        /// Probability for each sentence returned for the most recent call to sentDetect.
        /// If not applicable an empty array is returned.
        /// </returns>
        public double[] GetSentenceProbabilities() {
            return sentProbs.ToArray();
        }
        #endregion

        #region . Train .

        /// <summary>
        /// Trains sentence detection model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <returns>The trained <see cref="SentenceModel"/> object.</returns>
        public static SentenceModel Train(
            string languageCode,
            IObjectStream<SentenceSample> samples,
            SentenceDetectorFactory factory,
            TrainingParameters parameters) {

            return Train(languageCode, samples, factory, parameters, null);
        }

        /// <summary>
        /// Trains sentence detection model with the given parameters.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="samples">The data samples.</param>
        /// <param name="factory">The sentence detector factory.</param>
        /// <param name="parameters">The machine learnable parameters.</param>
        /// <param name="monitor">
        /// A evaluation monitor that can be used to listen the messages during the training or it can cancel the training operation.
        /// This argument can be a <c>null</c> value.
        /// </param>
        /// <returns>The trained <see cref="SentenceModel"/> object.</returns>
        public static SentenceModel Train(string languageCode, IObjectStream<SentenceSample> samples, SentenceDetectorFactory factory, TrainingParameters parameters, Monitor monitor) {

            var manifestInfoEntries = new Dictionary<string, string>();

            // TODO: Fix the EventStream to throw exceptions when training goes wrong
            var eventStream = new SentenceEventStream(
                samples, 
                factory.GetContextGenerator(),
                factory.GetEndOfSentenceScanner());

            var trainer = TrainerFactory.GetEventTrainer(parameters, manifestInfoEntries, monitor);
            var model = trainer.Train(eventStream);

            return new SentenceModel(languageCode, model, manifestInfoEntries, factory);
        }
        #endregion

    }
}