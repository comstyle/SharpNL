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
using System.Text.RegularExpressions;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// A Tokenizer for converting raw text into separated tokens.
    /// </summary>
    /// <remarks>
    /// The features are loosely based off of Jeff Reynar's UPenn thesis: 
    /// <see href="http://repository.upenn.edu/ircs_reports/66/">Topic Segmentation: Algorithms and Applications.</see>
    /// This tokenizer needs a statistical model to tokenize a text which reproduces the tokenization observed in the training data used to create the model.
    /// The <see cref="TokenizerModel"/> class encapsulates the model and provides methods to create it from the binary representation.
    /// </remarks>
    /// <seealso cref="ITokenizer"/>
    /// <seealso cref="TokenizerModel"/>
    /// <seealso cref="TokenSample"/>
    public class TokenizerME : AbstractTokenizer {
        /// <summary>
        /// Constant indicates a token split.
        /// </summary>
        public const string Split = "T";

        /// <summary>
        /// Constant indicates no token split.
        /// </summary>
        public const string NoSplit = "F";

        private readonly Regex alphanumeric;
        private readonly ITokenContextGenerator cg;
        private readonly IMaxentModel model;

        private readonly List<Span> newTokens;
        private readonly List<Double> tokProbs;
        private readonly bool useAlphaNumericOptimization;

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenizerME"/> class.
        /// </summary>
        /// <param name="model">The tokenizer model.</param>
        public TokenizerME(TokenizerModel model) {
            this.model = model.MaxentModel;

            var factory = model.Factory;

            alphanumeric = new Regex(factory.AlphaNumericPattern, RegexOptions.Compiled);
            cg = factory.ContextGenerator;
            useAlphaNumericOptimization = model.UseAlphaNumericOptimization;

            newTokens = new List<Span>();
            tokProbs = new List<double>(50);
        }

        #endregion

        #region + Properties .

        #region . TokenProbabilities .

        /// <summary>
        /// Gets the probabilities associated with the most recent calls to 
        /// <see cref="TokenizerME.Tokenize"/> or <see cref="TokenizerME.TokenizePos"/>.
        /// </summary>
        /// <value>
        /// The probability for each token returned for the most recent call to tokenize. 
        /// If not applicable an empty array is returned.
        /// </value>
        public double[] TokenProbabilities {
            get { return tokProbs.ToArray(); }
        }

        #endregion

        #endregion

        #region . TokenizePos .

        /// <summary>
        /// Finds the boundaries of atomic parts in a string.
        /// </summary>
        /// <param name="value">The string to be tokenized.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into s) for each token as the individuals array elements.</returns>
        public override Span[] TokenizePos(string value) {
            var tokens = WhitespaceTokenizer.Instance.TokenizePos(value);
            newTokens.Clear();
            tokProbs.Clear();
            for (int i = 0, il = tokens.Length; i < il; i++) {
                var s = tokens[i];
                //string tok = value.Substring(s.getStart(), s.getEnd());
                var tok = tokens[i].GetCoveredText(value);
                // Can't tokenize single characters
                if (tok.Length < 2) {
                    newTokens.Add(s);
                    tokProbs.Add(1d);
                } else if (useAlphaNumericOptimization && alphanumeric.IsMatch(tok)) {
                    newTokens.Add(s);
                    tokProbs.Add(1d);
                } else {
                    var start = s.Start;
                    var end = s.End;
                    var origStart = s.Start;
                    var tokenProb = 1.0;
                    for (var j = origStart + 1; j < end; j++) {
                        var probs = model.Eval(cg.GetContext(tok, j - origStart));
                        var best = model.GetBestOutcome(probs);
                        tokenProb *= probs[model.GetIndex(best)];
                        if (best == Split) {
                            newTokens.Add(new Span(start, j));
                            tokProbs.Add(tokenProb);
                            start = j;
                            tokenProb = 1.0;
                        }
                    }
                    newTokens.Add(new Span(start, end));
                    tokProbs.Add(tokenProb);
                }
            }
            return newTokens.ToArray();
        }

        #endregion

        #region + Train .

        /// <summary>
        /// Trains a model for the <see cref="TokenizerME"/>.
        /// </summary>
        /// <param name="samples">The samples used for the training.</param>
        /// <param name="factory">A <see cref="TokenizerFactory"/> to get resources from.</param>
        /// <param name="mlParams">The machine learning train parameters.</param>
        /// <returns>The trained <see cref="TokenizerModel"/>.</returns>
        public static TokenizerModel Train(IObjectStream<TokenSample> samples, TokenizerFactory factory,
            TrainingParameters mlParams) {
            var manifestInfoEntries = new Dictionary<string, string>();

            var eventStream = new TokSpanEventStream(samples, factory.UseAlphaNumericOptimization,
                factory.AlphaNumericPattern, factory.ContextGenerator);

            var trainer = TrainerFactory.GetEventTrainer(mlParams, manifestInfoEntries);
            var model = trainer.Train(eventStream);

            return new TokenizerModel(model, manifestInfoEntries, factory);
        }

        #endregion
    }
}