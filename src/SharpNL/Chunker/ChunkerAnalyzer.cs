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
using SharpNL.Analyzer;
using SharpNL.Text;

namespace SharpNL.Chunker {
    /// <summary>
    /// Represents a chunker analyzer which allows the easy abstraction of the chunking. This class is thread-safe.
    /// </summary>
    public class ChunkerAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Gets the chunker.
        /// </summary>
        /// <value>The chunker.</value>
        protected IChunker Chunker { get; private set; }

        #region + Constructor .
                /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerAnalyzer"/> class.
        /// </summary>
        /// <param name="chunker">The chunker.</param>
        public ChunkerAnalyzer(IChunker chunker) : this(chunker, 5f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractAnalyzer" /> with the specified weight.
        /// </summary>
        /// <param name="chunker">The chunker used in this analyzer.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">chunker</exception>
        public ChunkerAnalyzer(IChunker chunker, float weight)
            : base(weight) {
            
            if (chunker == null)
                throw new ArgumentNullException("chunker");

            Chunker = chunker;

        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected override void Evaluate(ITextFactory factory, IDocument document) {
            if (document.Sentences == null)
                throw new AnalyzerException(this, "The document does not have the sentences detected.");

            foreach (var sentence in document.Sentences) {
                var toks = sentence.GetTokens();
                var tags = sentence.GetTags();

                if (toks == null)
                    throw new AnalyzerException(this, "The document have a sentence without the tokenization.");

                if (tags == null)
                    throw new AnalyzerException(this, "The document have a sentence without the part-of-speech tags.");

                string[] chunks;
                lock (Chunker) {
                    chunks = Chunker.Chunk(toks, tags);
                }

                for (var i = 0; i < chunks.Length; i++) {
                    sentence.Tokens[i].ChunkTag = chunks[i];
                }

                var spans = ChunkSample.PhrasesAsSpanList(toks, tags, chunks);
                var list = new List<IChunk>(spans.Length);

                foreach (var span in spans) {
                    var chunk = factory.CreateChunk(sentence, span);
                    if (chunk != null)
                        list.Add(chunk);

                }

                sentence.Chunks = new ReadOnlyCollection<IChunk>(list);
            }
        }
        #endregion

    }
}