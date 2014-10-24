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
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Parser.Chunking {
    public class ParserEventStream : AbstractParserEventStream {
        protected BuildContextGenerator bcg;
        protected CheckContextGenerator kcg;

        #region + Constructors .

        /// <summary>
        /// Create an event stream based on the specified data stream of the specified type using the specified head rules.
        /// </summary>
        /// <param name="samples">A 1-parse-per-line Penn Treebank Style parse.</param>
        /// <param name="rules">The head rules.</param>
        /// <param name="eType">The type of events desired (tag, chunk, build, or check).</param>
        public ParserEventStream(IObjectStream<Parse> samples, AbstractHeadRules rules, ParserEventTypeEnum eType)
            : this(samples, rules, eType, null) {}

        /// <summary>
        /// Create an event stream based on the specified data stream of the specified type using the specified head rules.
        /// </summary>
        /// <param name="samples">A 1-parse-per-line Penn Treebank Style parse.</param>
        /// <param name="rules">The head rules.</param>
        /// <param name="eType">The type of events desired (tag, chunk, build, or check).</param>
        /// <param name="dictionary">A tri-gram dictionary to reduce feature generation.</param>
        public ParserEventStream(IObjectStream<Parse> samples, AbstractHeadRules rules, ParserEventTypeEnum eType,
            Dictionary.Dictionary dictionary)
            : base(samples, rules, eType, dictionary) {

            switch (eType) {
                case ParserEventTypeEnum.Build:
                    bcg = new BuildContextGenerator(dictionary);
                    break;
                case ParserEventTypeEnum.Check:
                    kcg = new CheckContextGenerator();
                    break;
            }

        }

        #endregion

        /// <summary>
        /// Adds events for parsing (post tagging and chunking to the specified list of events for the specified parse chunks.
        /// </summary>
        /// <param name="parseEvents">The events for the specified chunks.</param>
        /// <param name="chunks">The incomplete parses to be parsed.</param>
        protected override void AddParseEvents(List<Event> parseEvents, Parse[] chunks) {
            var ci = 0;
            while (ci < chunks.Length) {
                //System.err.println("parserEventStream.addParseEvents: chunks="+Arrays.asList(chunks));
                var c = chunks[ci];
                var parent = c.Parent;
                if (parent != null) {
                    var type = parent.Type;
                    String outcome;
                    if (FirstChild(c, parent)) {
                        outcome = AbstractBottomUpParser.START + type;
                    } else {
                        outcome = AbstractBottomUpParser.CONT + type;
                    }
                    //System.err.println("parserEventStream.addParseEvents: chunks["+ci+"]="+c+" label="+outcome+" bcg="+bcg);
                    c.Label = outcome;
                    if (Type == ParserEventTypeEnum.Build) {
                        parseEvents.Add(new Event(outcome, bcg.GetContext(chunks, ci)));
                    }
                    var start = ci - 1;
                    while (start >= 0 && chunks[start].Parent.Equals(parent)) {
                        start--;
                    }
                    if (LastChild(c, parent)) {
                        if (Type == ParserEventTypeEnum.Check) {
                            parseEvents.Add(new Event(AbstractBottomUpParser.COMPLETE,
                                kcg.GetContext(chunks, type, start + 1, ci)));
                        }
                        //perform reduce
                        var reduceStart = ci;
                        while (reduceStart >= 0 && chunks[reduceStart].Equals(parent)) {
                            reduceStart--;
                        }
                        reduceStart++;
                        chunks = ReduceChunks(chunks, ref ci, parent);
                        ci = reduceStart - 1; //ci will be incremented at end of loop
                    } else {
                        if (Type == ParserEventTypeEnum.Check) {
                            parseEvents.Add(new Event(AbstractBottomUpParser.INCOMPLETE,
                                kcg.GetContext(chunks, type, start + 1, ci)));
                        }
                    }
                }
                ci++;
            }
        }

        #region . FirstChild .

        /// <summary>
        /// Checks if the specified child is the first child of the specified parent.
        /// </summary>
        /// <param name="child">The child parse.</param>
        /// <param name="parent">The parent parse.</param>
        /// <returns><c>true</c> if the specified child is the first child of the specified parent, <c>false</c> otherwise.</returns>
        protected bool FirstChild(Parse child, Parse parent) {
            return AbstractBottomUpParser.CollapsePunctuation(parent.Children, Punctuation)[0].Equals(child);
        }

        #endregion

        #region . ReduceChunks .

        public static Parse[] ReduceChunks(Parse[] chunks, ref int ci, Parse parent) {
            var type = parent.Type;
            //  perform reduce
            var reduceStart = ci;
            var reduceEnd = ci;
            while (reduceStart >= 0 && chunks[reduceStart].Parent.Equals(parent)) {
                reduceStart--;
            }
            reduceStart++;
            Parse[] reducedChunks;
            if (!type.Equals(AbstractBottomUpParser.TOP_NODE)) {
                reducedChunks = new Parse[chunks.Length - (reduceEnd - reduceStart + 1) + 1];
                    //total - num_removed + 1 (for new node)
                //insert nodes before reduction
                for (int i = 0, rn = reduceStart; i < rn; i++) {
                    reducedChunks[i] = chunks[i];
                }
                //insert reduced node
                reducedChunks[reduceStart] = parent;
                //propagate punctuation sets
                parent.PreviousPunctuationSet = chunks[reduceStart].PreviousPunctuationSet;
                parent.NextPunctuationSet = chunks[reduceEnd].NextPunctuationSet;

                //insert nodes after reduction
                var ri = reduceStart + 1;
                for (var rci = reduceEnd + 1; rci < chunks.Length; rci++) {
                    reducedChunks[ri] = chunks[rci];
                    ri++;
                }
                ci = reduceStart - 1; //ci will be incremented at end of loop
            } else {
                reducedChunks = new Parse[0];
            }
            return reducedChunks;
        }

        #endregion
    }
}