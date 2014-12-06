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
using System.Text;

namespace SharpNL.Parser.Chunking {
    /// <summary>
    /// Class for generating predictive context for deciding when a constituent is complete.
    /// </summary>
    public class CheckContextGenerator : AbstractContextGenerator {
        /// <summary>
        /// Gets the predictive context for deciding whether the specified constituents between the specified start and end index
        /// can be combined to form a new constituent of the specified type.
        /// </summary>
        /// <param name="constituents">The constituents which have yet to be combined into new constituents.</param>
        /// <param name="type">The type of the new constituent proposed.</param>
        /// <param name="start">The first constituent of the proposed constituent.</param>
        /// <param name="end">The last constituent of the proposed constituent.</param>
        /// <returns>The predictive context for deciding whether a new constituent should be created.</returns>
        public string[] GetContext(Parse[] constituents, string type, int start, int end) {
            var ps = constituents.Length;

            var features = new List<string> {
                "default", 
                "fl=" + constituents[0].Label // first constituent label
            };
            
            var pstart = constituents[start];
            var pend = constituents[end];
            CheckCons(pstart, "begin", type, features);
            CheckCons(pend, "last", type, features);
            var production = new StringBuilder();
            var punctProduction = new StringBuilder();
            production.Append("p=").Append(type).Append("->");
            punctProduction.Append("pp=").Append(type).Append("->");
            for (var pi = start; pi < end; pi++) {
                var p = constituents[pi];
                CheckCons(p, pend, type, features);
                production.Append(p.Type).Append(",");
                punctProduction.Append(p.Type).Append(",");
                var nextPunct = p.NextPunctuationSet;
                if (nextPunct != null) {
                    foreach (var punct in nextPunct) {
                        punctProduction.Append(punct.Type).Append(",");
                    }
                }
            }
            production.Append(pend.Type);
            punctProduction.Append(pend.Type);
            features.Add(production.ToString());
            features.Add(punctProduction.ToString());
            Parse p_2 = null;
            Parse p_1 = null;
            Parse p1 = null;
            Parse p2 = null;
            var p1s = constituents[end].NextPunctuationSet;
            SortedSet<Parse> p2s = null;
            var p_1s = constituents[start].PreviousPunctuationSet;
            SortedSet<Parse> p_2s = null;
            if (start - 2 >= 0) {
                p_2 = constituents[start - 2];
            }
            if (start - 1 >= 0) {
                p_1 = constituents[start - 1];
                p_2s = p_1.PreviousPunctuationSet;
            }
            if (end + 1 < ps) {
                p1 = constituents[end + 1];
                p2s = p1.NextPunctuationSet;
            }
            if (end + 2 < ps) {
                p2 = constituents[end + 2];
            }
            Surround(p_1, -1, type, p_1s, features);
            Surround(p_2, -2, type, p_2s, features);
            Surround(p1, 1, type, p1s, features);
            Surround(p2, 2, type, p2s, features);

            return features.ToArray();
        }
    }
}