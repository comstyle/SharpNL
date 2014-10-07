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
    /// <summary>
    /// Creates predictive context for the pre-chunking phases of parsing.
    /// </summary>
    public class ChunkContextGenerator : IChunkerContextGenerator {
        private const string EOS = "eos";
        private readonly Cache contextsCache;
        private object wordsKey;

        public ChunkContextGenerator() : this(0) {}

        public ChunkContextGenerator(int cacheSize) {
            if (cacheSize > 0)
                contextsCache = new Cache(cacheSize);
        }

        /// <summary>Gets the context for the specified position in the specified sequence (list).</summary>
        /// <param name="index">The index of the sequence.</param>
        /// <param name="sequence">The sequence of items over which the beam search is performed.</param>
        /// <param name="priorDecisions">The sequence of decisions made prior to the context for which this decision is being made.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for the specified position in the specified sequence.</returns>
        public string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the contexts for chunking of the specified index.
        /// </summary>
        /// <param name="index">The index of the token in the specified tokens array for which the context should be constructed.</param>
        /// <param name="tokens">The tokens of the sentence. The <code>ToString</code> methods of these objects should return the token text.</param>
        /// <param name="tags">The POS tags for the the specified tokens.</param>
        /// <param name="prevDecisions">The previous decisions made in the tagging of this sequence.</param>
        /// <returns>An array of predictive contexts on which a model basis its decisions.</returns>
        public string[] GetContext(int index, string[] tokens, string[] tags, string[] prevDecisions) {
            var features = new List<string>(19);
            var x0 = index;
            var x_2 = x0 - 2;
            var x_1 = x0 - 1;
            var x2 = x0 + 2;
            var x1 = x0 + 1;

            string w_2, w_1, w1, w2;
            string t_2, t_1, t1, t2;
            string p_2, p_1;

            // chunkandpostag(-2)
            if (x_2 >= 0) {
                t_2 = tags[x_2];
                p_2 = prevDecisions[x_2];
                w_2 = tokens[x_2];
            } else {
                t_2 = EOS;
                p_2 = EOS;
                w_2 = EOS;
            }

            // chunkandpostag(-1)
            if (x_1 >= 0) {
                t_1 = tags[x_1];
                p_1 = prevDecisions[x_1];
                w_1 = tokens[x_1];
            } else {
                t_1 = EOS;
                p_1 = EOS;
                w_1 = EOS;
            }

            // chunkandpostag(0)
            string t0 = tags[x0];
            string w0 = tokens[x0];

            // chunkandpostag(1)
            if (x1 < tags.Length) {
                t1 = tags[x1];
                w1 = tokens[x1];
            } else {
                t1 = EOS;
                w1 = EOS;
            }

            // chunkandpostag(2)
            if (x2 < tags.Length) {
                t2 = tags[x2];
                w2 = tokens[x2];
            } else {
                t2 = EOS;
                w2 = EOS;
            }

            var cacheKey = x0 + t_2 + t1 + t0 + t1 + t2 + p_2 + p_1;
            if (contextsCache != null) {
                if (wordsKey == tokens) {
                    var contexts = (String[]) contextsCache.Get(cacheKey);
                    if (contexts != null) {
                        return contexts;
                    }
                } else {
                    contextsCache.Clear();
                    wordsKey = tokens;
                }
            }

            var ct_2 = ChunkAndPosTag(-2, w_2, t_2, p_2);
            var ctBo_2 = ChunkAndPosTagBo(-2, t_2, p_2);
            var ct_1 = ChunkAndPosTag(-1, w_1, t_1, p_1);
            var ctBo_1 = ChunkAndPosTagBo(-1, t_1, p_1);
            var ct0 = ChunkAndPosTag(0, w0, t0, null);
            var ctbo0 = ChunkAndPosTagBo(0, t0, null);
            var ct1 = ChunkAndPosTag(1, w1, t1, null);
            var ctbo1 = ChunkAndPosTagBo(1, t1, null);
            var ct2 = ChunkAndPosTag(2, w2, t2, null);
            var ctbo2 = ChunkAndPosTagBo(2, t2, null);

            features.Add("default");
            features.Add(ct_2);
            features.Add(ctBo_2);
            features.Add(ct_1);
            features.Add(ctBo_1);
            features.Add(ct0);
            features.Add(ctbo0);
            features.Add(ct1);
            features.Add(ctbo1);
            features.Add(ct2);
            features.Add(ctbo2);

            //chunkandpostag(-1,0)
            features.Add(ct_1 + "," + ct0);
            features.Add(ctBo_1 + "," + ct0);
            features.Add(ct_1 + "," + ctbo0);
            features.Add(ctBo_1 + "," + ctbo0);

            //chunkandpostag(0,1)
            features.Add(ct0 + "," + ct1);
            features.Add(ctbo0 + "," + ct1);
            features.Add(ct0 + "," + ctbo1);
            features.Add(ctbo0 + "," + ctbo1);

            if (contextsCache != null) {
                var contexts = features.ToArray();
                contextsCache.Put(cacheKey, contexts);
                return contexts;
            }
            return features.ToArray();
        }

        private string ChunkAndPosTag(int i, string tok, string tag, string chunk) {
            return string.Format("{0}={1}|{2}{3}", i, tok, tag, i < 0 ? "|" + chunk : string.Empty);
        }

        private string ChunkAndPosTagBo(int i, String tag, String chunk) {
            return string.Format("{0}*={1}{2}", i, tag, i < 0 ? "|" + chunk : string.Empty);
        }
    }
}