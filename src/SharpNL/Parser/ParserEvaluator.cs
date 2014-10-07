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
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Parser {
    /// <summary>
    /// Parser Evaluator.
    /// </summary>
    public class ParserEvaluator : Evaluator<Parse, Span> {
        private readonly Chunking.Parser parser;

        public ParserEvaluator(Chunking.Parser parser, params IEvaluationMonitor<Parse>[] listeners)
            : base(listeners) {
            this.parser = parser;
            FMeasure = new FMeasure<Span>();
        }

        #region . GetConstituencySpans .

        /// <summary>
        /// Obtain <see cref="Span"/>s for every parse in the sentence.
        /// </summary>
        /// <param name="parse">The parse from which to obtain the spans</param>
        /// <returns>An array containing every span for the parse.</returns>
        internal static Span[] GetConstituencySpans(Parse parse) {
            var stack = new Stack<Parse>();
            if (parse.ChildCount > 0) {
                foreach (var child in parse.Children) {
                    stack.Push(child);
                }
            }
            var list = new List<Span>();
            while (stack.Count > 0) {
                var pop = stack.Pop();

                if (!pop.IsPosTag) {
                    var span = pop.Span;
                    list.Add(new Span(span.Start, span.End, pop.Type));

                    foreach (var child in pop.Children) {
                        stack.Push(child);
                    }
                }
            }

            return list.ToArray();
        }

        #endregion

        #region . ProcessSample .

        /// <summary>
        /// Evaluates the given reference sample object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override Parse ProcessSample(Parse reference) {
            var sentenceText = reference.Text;

            var predictions = ParserTool.ParseLine(sentenceText, parser, 1);

            Parse prediction = null;
            if (predictions.Length > 0) {
                prediction = predictions[0];
            }

            FMeasure.UpdateScores(GetConstituencySpans(reference), GetConstituencySpans(prediction));

            return prediction;
        }

        #endregion

    }
}