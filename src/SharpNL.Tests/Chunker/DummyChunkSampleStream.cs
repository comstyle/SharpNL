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

namespace SharpNL.Tests.Chunker {
    /// <summary>
    /// This dummy chunk sample stream reads a file formatted as described at 
    /// <see href="http://www.cnts.ua.ac.be/conll2000/chunking/output.html/"/>
    /// and can be used together with DummyChunker simulate a chunker.
    /// </summary>
    internal class DummyChunkSampleStream : FilterObjectStream<string, ChunkSample> {
        private readonly bool isPredicted;
        private int count;

        public DummyChunkSampleStream(IObjectStream<string> samples, bool isPredicted)
            : base(samples) {
            this.isPredicted = isPredicted;
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override ChunkSample Read() {
            var toks = new List<string>();
            var posTags = new List<string>();
            var chunkTags = new List<string>();
            var predictedChunkTags = new List<string>();


            for (var line = Samples.Read(); !string.IsNullOrEmpty(line); line = Samples.Read()) {
                var parts = line.Split(' ');
                if (parts.Length != 4) {
                    Console.Error.WriteLine("Skipping corrupt line " + count + ": " + line);
                } else {
                    toks.Add(parts[0]);
                    posTags.Add(parts[1]);
                    chunkTags.Add(parts[2]);
                    predictedChunkTags.Add(parts[3]);
                }
                count++;
            }

            if (toks.Count > 0) {
                if (isPredicted) {
                    return new ChunkSample(
                        toks.ToArray(),
                        posTags.ToArray(),
                        predictedChunkTags.ToArray());
                }
                return new ChunkSample(
                    toks.ToArray(),
                    posTags.ToArray(),
                    chunkTags.ToArray());
            }
            return null;
        }
    }
}