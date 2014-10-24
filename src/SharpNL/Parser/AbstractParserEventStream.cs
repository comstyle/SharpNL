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

using System.Linq;
using System.Collections.Generic;

using SharpNL.Chunker;
using SharpNL.ML.Model;
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Parser {
    /// <summary>
    /// Abstract class extended by parser event streams which perform tagging and chunking.
    /// </summary>
    public abstract class AbstractParserEventStream : AbstractEventStream<Parse> {

        private readonly IChunkerContextGenerator chunkerContextGenerator;
        private readonly IPOSContextGenerator posContextGenerator;

        protected AbstractParserEventStream(
            IObjectStream<Parse> samples,
            IHeadRules headRules,
            ParserEventTypeEnum type)
            : this(samples, headRules, type, null) {
            
        }

        protected AbstractParserEventStream(
            IObjectStream<Parse> samples,
            IHeadRules headRules, 
            ParserEventTypeEnum type,
            Dictionary.Dictionary dictionary) : base(samples) {
            
            Rules = headRules;
            Punctuation = headRules.PunctuationTags;
            Dictionary = dictionary;
            FixPossesives = false;

            Type = type;

            switch (type) {
                case ParserEventTypeEnum.Chunk:
                    chunkerContextGenerator = new ChunkContextGenerator();
                    break;
                case ParserEventTypeEnum.Tag:
                    posContextGenerator = new DefaultPOSContextGenerator(null);
                    break;
            }
        }

        #region + Properties .

        #region . Dictionary .
        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        /// <value>The dictionary.</value>
        protected Dictionary.Dictionary Dictionary { get; private set; }
        #endregion

        #region . FixPossesives .

        protected bool FixPossesives { get; set; }

        #endregion

        #region . Punctuation .
        /// <summary>
        /// Gets or sets the punctuation list.
        /// </summary>
        /// <value>The punctuation list.</value>
        protected List<string> Punctuation { get; set; }
        #endregion

        #region . Rules .

        /// <summary>
        /// Gets the head rules.
        /// </summary>
        /// <value>The head rules.</value>
        protected IHeadRules Rules { get; private set; }

        #endregion

        #region . Type .
        /// <summary>
        /// Gets the parser event type.
        /// </summary>
        /// <value>The parser event type.</value>
        protected ParserEventTypeEnum Type { get; private set; }
        #endregion

        #endregion

        #region . AddChunkEvents .

        private void AddChunkEvents(List<Event> chunkEvents, Parse[] chunks) {
            var toks = new List<string>();
            var tags = new List<string>();
            var preds = new List<string>();
            for (int ci = 0, cl = chunks.Length; ci < cl; ci++) {
                var c = chunks[ci];
                if (c.IsPosTag) {
                    toks.Add(c.CoveredText);
                    tags.Add(c.Type);
                    preds.Add(AbstractBottomUpParser.OTHER);
                } else {
                    var start = true;
                    var kids = c.Children;
                    for (int ti = 0, tl = kids.Length; ti < tl; ti++) {
                        var tok = kids[ti];
                        toks.Add(tok.CoveredText);
                        tags.Add(tok.Type);
                        if (start) {
                            preds.Add(AbstractBottomUpParser.START + c.Type);
                            start = false;
                        } else {
                            preds.Add(AbstractBottomUpParser.CONT + c.Type);
                        }
                    }
                }
            }
            for (int ti = 0, tl = toks.Count; ti < tl; ti++) {
                chunkEvents.Add(
                    new Event(
                        preds[ti],
                        chunkerContextGenerator.GetContext(ti, toks.ToArray(), tags.ToArray(), preds.ToArray())));
            }
        }

        #endregion

        #region . AddParseEvents .
        /// <summary>
        /// Produces all events for the specified sentence chunks and adds them to the specified list.
        /// </summary>
        /// <param name="parseEvents">A list of events to be added to.</param>
        /// <param name="chunks">Pre-chunked constituents of a sentence.</param>
        protected abstract void AddParseEvents(List<Event> parseEvents, Parse[] chunks);
        #endregion

        #region . AddTagEvents .
        private void AddTagEvents(List<Event> tagEvents, Parse[] chunks) {
            var toks = new List<string>();
            var preds = new List<string>();
            for (int ci = 0, cl = chunks.Length; ci < cl; ci++) {
                var c = chunks[ci];
                if (c.IsPosTag) {
                    toks.Add(c.CoveredText);
                    preds.Add(c.Type);
                } else {
                    var kids = c.Children;
                    for (int ti = 0, tl = kids.Length; ti < tl; ti++) {
                        var tok = kids[ti];
                        toks.Add(tok.CoveredText);
                        preds.Add(tok.Type);
                    }
                }
            }
            for (int ti = 0, tl = toks.Count; ti < tl; ti++) {
                tagEvents.Add(
                    new Event(
                        preds[ti],
                        posContextGenerator.GetContext(ti, toks.ToArray(), preds.ToArray(), null)));
            }
        }
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

            if (FixPossesives)
                Parse.FixPossessives(sample);

            sample.UpdateHeads(Rules);

            var chunks = GetInitialChunks(sample);

            switch (Type) {
                case ParserEventTypeEnum.Chunk:
                    AddChunkEvents(newEvents, chunks);
                    break;
                case ParserEventTypeEnum.Tag:
                    AddTagEvents(newEvents, chunks);
                    break;
                default:
                    AddParseEvents(newEvents, AbstractBottomUpParser.CollapsePunctuation(chunks, Punctuation));
                    break;
            }

            return newEvents.GetEnumerator();
        }
        #endregion

        #region . LastChild .
        /// <summary>
        /// Returns true if the specified child is the last child of the specified parent.
        /// </summary>
        /// <param name="child">The child.</param>
        /// <param name="parent">The parent.</param>
        /// <returns><c>true</c> if the specified child is the last child of the specified parent, <c>false</c> otherwise.</returns>
        protected bool LastChild(Parse child, Parse parent) {
            var kids = AbstractBottomUpParser.CollapsePunctuation(parent.Children, Punctuation);
            return (Equals(kids[kids.Length - 1], child));
        }
        #endregion

        #region . GetInitialChunks .

        public static Parse[] GetInitialChunks(Parse parse) {
            var list = new List<Parse>();

            GetInitialChunks(parse, list);

            return list.ToArray();
        }

        private static void GetInitialChunks(Parse parse, ICollection<Parse> chunks) {
            if (parse.IsPosTag) {
                chunks.Add(parse);
                return;
            }

            var allKidsAreTags = parse.Children.All(child => child.IsPosTag);
            if (allKidsAreTags) {
                chunks.Add(parse);
            } else {
                foreach (var child in parse.Children) {
                    GetInitialChunks(child, chunks);
                }
            }          
        }

        #endregion

    }
}