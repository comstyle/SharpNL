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

namespace SharpNL.Featurizer {
    /// <summary>
    /// Parses a featurizer training data.
    /// </summary>
    public class FeatureSampleStream : FilterObjectStream<string, FeatureSample> {
        public FeatureSampleStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override FeatureSample Read() {
            var toks = new List<String>();
            var lemmas = new List<String>();
            var tags = new List<String>();
            var preds = new List<String>();

            for (var line = Samples.Read(); line != null && !line.Equals(""); line = Samples.Read()) {
                var parts = line.Split(' ');
                if (parts.Length != 3) {
                    Console.Error.WriteLine("Skipping corrupt line: " + line);
                } else {
                    toks.Add(parts[0]);
                    lemmas.Add(parts[0]); // no lemma info for now
                    tags.Add(parts[1]);
                    preds.Add(parts[2]);
                }
            }

            return toks.Count > 0 ? 
                new FeatureSample(toks.ToArray(), lemmas.ToArray(), tags.ToArray(), preds.ToArray()) : 
                null;
        }
    }
}