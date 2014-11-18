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
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Parser {
    public class ChunkSampleStream : FilterObjectStream<Parse, ChunkSample> {
        public ChunkSampleStream(IObjectStream<Parse> samples) : base(samples) {}

        #region . GetInitialChunks .

        public static Parse[] GetInitialChunks(Parse parse) {
            var chunks = new List<Parse>();
            GetInitialChunks(parse, chunks);
            return chunks.ToArray();
        }

        private static void GetInitialChunks(Parse p, List<Parse> chunks) {
            if (p.IsPosTag) {
                chunks.Add(p);
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
                    chunks.Add(p);
                } else {
                    for (int ci = 0, cl = kids.Length; ci < cl; ci++) {
                        GetInitialChunks(kids[ci], chunks);
                    }
                }
            }
        }

        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next <see cref="ChunkSample"/>. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="ChunkSample"/> or null to signal that the stream is exhausted.
        /// </returns>
        public override ChunkSample Read() {
            var parse = Samples.Read();

            if (parse != null) {
                var chunks = GetInitialChunks(parse);
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
                        var type = c.Type;
                        var kids = c.Children;
                        for (int ti = 0, tl = kids.Length; ti < tl; ti++) {
                            var tok = kids[ti];
                            toks.Add(tok.CoveredText);
                            tags.Add(tok.Type);
                            if (start) {
                                preds.Add(AbstractBottomUpParser.START + type);
                                start = false;
                            } else {
                                preds.Add(AbstractBottomUpParser.CONT + type);
                            }
                        }
                    }
                }

                return new ChunkSample(toks.ToArray(), tags.ToArray(), preds.ToArray());
            }
            return null;
        }
        #endregion

    }
}