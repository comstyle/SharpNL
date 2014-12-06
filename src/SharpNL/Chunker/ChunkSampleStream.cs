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

namespace SharpNL.Chunker {
    /// <summary>
    /// Parses the conll 2000 shared task shallow parser training data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Data format is specified on the conll page: <br />
    /// <see href="http://www.cnts.ua.ac.be/conll2000/chunking/" />
    /// </para>
    /// </remarks>
    public class ChunkSampleStream : FilterObjectStream<string, ChunkSample> {
        private int lineNum;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkSampleStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public ChunkSampleStream(IObjectStream<string> samples) : base(samples) {
            lineNum = 0;
        }

        #region . Read .
        /// <summary>
        /// Returns the next <see cref="ChunkSample"/>. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override ChunkSample Read() {

            var toks = new List<string>();
            var tags = new List<string>();
            var preds = new List<string>();

            for (string line = Samples.Read(); !string.IsNullOrWhiteSpace(line); line = Samples.Read(), lineNum++) {
                var parts = line.Split(' ');
                if (parts.Length != 3) {
                    Console.Error.WriteLine("Skipping corrupt line #{0}: {1}", lineNum, line);
                    continue;
                }

                toks.Add(parts[0]);
                tags.Add(parts[1]);
                preds.Add(parts[2]);
            }

            if (toks.Count > 0)
                return new ChunkSample(toks.ToArray(), tags.ToArray(), preds.ToArray());

            return null;
        }
        #endregion
        
    }
}