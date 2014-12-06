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

using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Chunker {
    /// <summary>
    /// The ChunkerEvaluator measures the performance of the given <see cref="IChunker"/> with
    /// the provided reference <see cref="ChunkSample"/>s.
    /// </summary>
    public class ChunkerEvaluator : Evaluator<ChunkSample, Span> {

        private readonly IChunker chunker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerEvaluator"/> class.
        /// </summary>
        /// <param name="chunker">The chunker.</param>
        /// <param name="listeners">The evaluation listeners.</param>
        public ChunkerEvaluator(IChunker chunker, params IEvaluationMonitor<ChunkSample>[] listeners) : base(listeners) {

            this.chunker = chunker;

            FMeasure = new FMeasure<Span>();
        }

        /// <summary>
        /// Evaluates the given reference <see cref="ChunkSample"/> object.
        /// The implementation has to update the score after every invocation.
        /// </summary>
        /// <param name="reference">The reference sample.</param>
        /// <returns>The predicted sample</returns>
        protected override ChunkSample ProcessSample(ChunkSample reference) {
            var preds = chunker.Chunk(reference.Sentence.ToArray(), reference.Tags.ToArray());
            var result = new ChunkSample(reference.Sentence.ToArray(), reference.Tags.ToArray(), preds);

            FMeasure.UpdateScores(reference.GetPhrasesAsSpanList(), result.GetPhrasesAsSpanList());

            return result;
        }
    }
}