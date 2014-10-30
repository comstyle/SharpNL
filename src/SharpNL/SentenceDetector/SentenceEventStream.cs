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

namespace SharpNL.SentenceDetector {

    using Java;
    using Utility;
    using ML.Model;

    public class SentenceEventStream : AbstractEventStream<SentenceSample> {

        private readonly ISentenceContextGenerator cg;
        private readonly IEndOfSentenceScanner scanner;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="SentenceEventStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="cg">The sentence context generator.</param>
        /// <param name="scanner">The end of sentence scanner.</param>
        public SentenceEventStream(IObjectStream<SentenceSample> samples, ISentenceContextGenerator cg, IEndOfSentenceScanner scanner) : base(samples) {

            this.cg = cg;
            this.scanner = scanner;

        }
        #endregion

        #region . CreateEvents .
        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(SentenceSample sample) {
            var events = new List<Event>();
            foreach (var sentenceSpan in sample.Sentences) {
                var sentenceString = sentenceSpan.GetCoveredText(sample.Document);

                for (var it = new IteratorAdapter<int>(scanner.GetPositions(sentenceString)); it.HasNext();) {
                    int candidate = it.Next();
                    string type = SentenceDetectorME.NO_SPLIT;
                    if (!it.HasNext()) {
                        type = SentenceDetectorME.SPLIT;
                    }
                    events.Add(new Event(type, cg.GetContext(sample.Document, sentenceSpan.Start + candidate)));
                }
            }

            return events.GetEnumerator();
        }
        #endregion

    }
}