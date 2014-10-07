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
using System.Collections.ObjectModel;
using SharpNL.Chunker;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Represents the chunker analyzer.
    /// </summary>
    public class Chunker : IAnalyzer {

        private readonly IChunker chunker;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunker"/> with the specified <see cref="IChunker"/>
        /// </summary>
        /// <param name="chunker">The chunker.</param>
        /// <exception cref="System.ArgumentNullException">chunker</exception>
        public Chunker(IChunker chunker) {
            if (chunker == null)
                throw new ArgumentNullException("chunker");

            this.chunker = chunker;
        }

        /// <summary>
        /// Analyzes the chunks of the specified document.
        /// </summary>
        /// <param name="document">The document to be analyzed.</param>
        public void Analyze(Document document) {
           
            foreach (var sentence in document.Sentences) {
                var tokens = sentence.Tokens;
                var tokStr = TextUtils.TokensToString(tokens);
                var tags = new string[tokens.Count];
                for (int i = 0; i < tokens.Count; i++) {
                    tags[i] = tokens[i].POSTag;
                }

                string[] chunkTags;
                lock (chunker) {
                    chunkTags = chunker.Chunk(tokStr, tags);
                }

                for (int i = 0; i < chunkTags.Length; i++) {
                    tokens[i].ChunkTag = chunkTags[i];
                }
                
                var chunkSpans = ChunkSample.PhrasesAsSpanList(tokStr, tags, chunkTags);
                var chunks = new List<Chunk>(chunkSpans.Length);
                foreach (var span in chunkSpans) {
                    chunks.Add(new Chunk(span.Type, span.Start, span.End, sentence));
                }
                sentence.Chunks = new ReadOnlyCollection<Chunk>(chunks);
            }


        }
    }
}