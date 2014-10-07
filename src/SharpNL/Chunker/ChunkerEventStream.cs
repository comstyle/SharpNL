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

using System.Collections.Generic;
using System.Linq;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Chunker {
    /// <summary>
    /// Represents an event stream out of data files for training a chunker.
    /// </summary>
    public class ChunkerEventStream : AbstractEventStream<ChunkSample> {

        private readonly IChunkerContextGenerator cg;

        /// <summary>
        /// Creates a new event stream based on the specified data stream using the specified context generator.
        /// </summary>
        /// <param name="samples">The samples for this event stream.</param>
        /// <param name="cg">The context generator which should be used in the creation of events for this event stream.</param>
        public ChunkerEventStream(IObjectStream<ChunkSample> samples, IChunkerContextGenerator cg) : base(samples) {
            this.cg = cg;
        }

        #region . CreateEvents .
        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(ChunkSample sample) {
            if (sample != null) {
                var events = new List<Event>();
                for (int i = 0, count = sample.Sentence.Count; i < count; i++) {
                    events.Add(new Event(sample.Preds[i], cg.GetContext(i, sample.Sentence.ToArray(), sample.Tags.ToArray(), sample.Preds.ToArray())));
                }
                return events.GetEnumerator();
            }
            return Enumerable.Empty<Event>().GetEnumerator();
        }
        #endregion

    }
}