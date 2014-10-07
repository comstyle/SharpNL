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
using SharpNL.ML.Model;
using SharpNL.Utility;
using Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.Parser {
    /// <summary>
    /// Abstract class extended by parser event streams which perform tagging and chunking.
    /// </summary>
    public abstract class AbstractParserEventStream : AbstractEventStream<Parse> {
        protected Dic dictionary;
        protected ParserEventTypeEnum eType;
        protected bool fixPossesives;
        protected List<string> punctSet;
        protected AbstractHeadRules rules;

        protected AbstractParserEventStream(IObjectStream<Parse> samples, AbstractHeadRules rules, ParserEventTypeEnum eType) :
            this(samples, rules, eType, null) {}

        protected AbstractParserEventStream(IObjectStream<Parse> samples, AbstractHeadRules rules, ParserEventTypeEnum eType,
            Dic dictionary) : base(samples) {
            this.dictionary = dictionary;
            this.rules = rules;
            punctSet = rules.PunctuationTags;
            this.eType = eType;
            fixPossesives = false;
        }

        #region . AddParseEvents .

        /// <summary>
        /// Produces all events for the specified sentence chunks and adds them to the specified list.
        /// </summary>
        /// <param name="newEvents">A list of events to be added to.</param>
        /// <param name="chunks">Pre-chunked constituents of a sentence.</param>
        protected abstract void AddParseEvents(List<Event> newEvents, Parse[] chunks);

        #endregion

        #region . CreateEvents .

        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(Parse sample) {
            var newEvents = new List<Event>();

            Parse.PurneParse(sample);
            if (fixPossesives)
                Parse.FixPossessives(sample);

            sample.UpdateHeads(rules);

            var chunks = GetInitialChunks(sample);

            AddParseEvents(newEvents, AbstractBottomUpParser.CollapsePunctuation(chunks, punctSet));

            return newEvents.GetEnumerator();
        }

        #endregion

        #region . GetInitialChunks .

        public static Parse[] GetInitialChunks(Parse p) {
            var chunks = new List<Parse>();
            GetInitialChunks(p, chunks);
            return chunks.ToArray();
        }

        private static void GetInitialChunks(Parse p, List<Parse> iChunks) {
            if (p.IsPosTag) {
                iChunks.Add(p);
            } else {
                var kids = p.Children;
                var allKidsAreTags = true;
                for (int ci = 0, cl = kids.Length; ci < cl; ci++) {
                    if (!kids[ci].IsPosTag) {
                        allKidsAreTags = false;
                        break;
                    }
                }

                if (allKidsAreTags) {
                    iChunks.Add(p);
                } else {
                    for (int ci = 0, cl = kids.Length; ci < cl; ci++) {
                        GetInitialChunks(kids[ci], iChunks);
                    }
                }
            }
        }

        #endregion

        #region . IsLastChild .

        /// <summary>
        /// Determines whether the specified child is the last child of the specified parent.
        /// </summary>
        /// <param name="child">The child parse.</param>
        /// <param name="parent">The parent parse.</param>
        /// <returns><c>true</c> if the specified child is the last child of the specified parent; otherwise, <c>false</c>.</returns>
        protected bool IsLastChild(Parse child, Parse parent) {
            var kids = AbstractBottomUpParser.CollapsePunctuation(parent.Children, punctSet);
            return kids[kids.Length - 1].Equals(child);
        }

        #endregion

    }
}