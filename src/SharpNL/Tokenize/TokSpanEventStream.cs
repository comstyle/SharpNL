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

using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SharpNL.ML.Model;
using SharpNL.Tokenize.Language;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    public class TokSpanEventStream : AbstractEventStream<TokenSample> {
        private readonly Regex alphaNumeric;
        private readonly ITokenContextGenerator cg;

        private readonly bool skipAlphaNumerics;

        public TokSpanEventStream(IObjectStream<TokenSample> tokenSamples, bool skipAlphaNumerics)
            : this(tokenSamples, skipAlphaNumerics, new DefaultTokenContextGenerator()) {}

        public TokSpanEventStream(IObjectStream<TokenSample> samples, bool skipAlphaNumerics, ITokenContextGenerator cg)
            : base(samples) {
            alphaNumeric = new Regex(Factory.GetAlphanumeric(null), RegexOptions.Compiled);
            this.skipAlphaNumerics = skipAlphaNumerics;
            this.cg = cg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokSpanEventStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="skipAlphaNumerics">if set to <c>true</c> [skip alpha numerics].</param>
        /// <param name="alphaNumericPattern">The alpha numeric pattern.</param>
        /// <param name="cg">The token context generator.</param>
        public TokSpanEventStream(IObjectStream<TokenSample> samples, bool skipAlphaNumerics, string alphaNumericPattern,
            ITokenContextGenerator cg)
            : base(samples) {
            alphaNumeric = new Regex(alphaNumericPattern, RegexOptions.Compiled);
            this.skipAlphaNumerics = skipAlphaNumerics;
            this.cg = cg;
        }


        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(TokenSample sample) {
            var events = new List<Event>(50);

            var tokens = sample.TokenSpans;
            var text = sample.Text;

            if (tokens.Length > 0) {
                var start = tokens[0].Start;
                var end = tokens[tokens.Length - 1].End;

                var sent = text.Substring(start, end - start);

                var candTokens = WhitespaceTokenizer.Instance.TokenizePos(sent);

                var firstTrainingToken = -1;
                var lastTrainingToken = -1;
                foreach (var candToken in candTokens) {
                    var cSpan = candToken;
                    var ctok = cSpan.GetCoveredText(sent);
                    //adjust cSpan to text offsets
                    cSpan = new Span(cSpan.Start + start, cSpan.End + start);
                    //should we skip this token
                    if (ctok.Length > 1
                        && (!skipAlphaNumerics || !alphaNumeric.IsMatch(ctok))) {
                        //find offsets of annotated tokens inside of candidate tokens
                        var foundTrainingTokens = false;
                        for (var ti = lastTrainingToken + 1; ti < tokens.Length; ti++) {
                            if (cSpan.Contains(tokens[ti])) {
                                if (!foundTrainingTokens) {
                                    firstTrainingToken = ti;
                                    foundTrainingTokens = true;
                                }
                                lastTrainingToken = ti;
                            } else if (cSpan.End < tokens[ti].End) {
                                break;
                            } else if (tokens[ti].End < cSpan.Start) {
                                //keep looking
                            } else {
                                // TODO: Add a logging mechanic
                                // warning
                                Debug.Print("Bad training token: " + tokens[ti] + " cand: " + cSpan + " token=" +
                                            tokens[ti].GetCoveredText(text));
                            }
                        }

                        // create training data
                        if (foundTrainingTokens) {
                            for (var ti = firstTrainingToken; ti <= lastTrainingToken; ti++) {
                                var tSpan = tokens[ti];
                                var cStart = cSpan.Start;
                                for (var i = tSpan.Start + 1; i < tSpan.End; i++) {
                                    var context = cg.GetContext(ctok, i - cStart);
                                    events.Add(new Event(TokenizerME.NoSplit, context));
                                }

                                if (tSpan.End != cSpan.End) {
                                    var context = cg.GetContext(ctok, tSpan.End - cStart);
                                    events.Add(new Event(TokenizerME.Split, context));
                                }
                            }
                        }
                    }
                }
            }

            return events.GetEnumerator();
        }
    }
}