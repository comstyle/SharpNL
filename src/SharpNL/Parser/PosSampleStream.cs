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

using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Parser {
    public class PosSampleStream : FilterObjectStream<Parse, POSSample> {
        public PosSampleStream(IObjectStream<Parse> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override POSSample Read() {
            var parse = Samples.Read();

            if (parse != null) {
                var nodes = parse.GetTagNodes();

                var toks = new string[nodes.Length];
                var preds = new string[nodes.Length];

                for (var ti = 0; ti < nodes.Length; ti++) {
                    toks[ti] = nodes[ti].CoveredText;
                    preds[ti] = nodes[ti].Type;
                }

                return new POSSample(toks, preds);
            }

            return null;
        }
    }
}