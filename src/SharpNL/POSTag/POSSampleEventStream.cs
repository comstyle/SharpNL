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
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.POSTag {
    /// <summary>
    /// This class reads the <see cref="POSSample"/>s from the given <see cref="T:IObjectStream{POSSample}"/>
    /// and converts the <see cref="POSSample"/>s into <see cref="Event"/>s which can be used 
    /// by the maxent library for training.
    /// </summary>
    public class POSSampleEventStream : AbstractEventStream<POSSample> {
        private readonly IPOSContextGenerator cg;

        /// <summary>
        /// Initializes the current instance with the given samples and a <see cref="DefaultPOSContextGenerator"/>.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public POSSampleEventStream(IObjectStream<POSSample> samples)
            : this(samples, new DefaultPOSContextGenerator(null)) {}

        /// <summary>
        /// Initializes the current instance with the given samples and the given <see cref="IPOSContextGenerator"/>.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="cg">The context generator.</param>
        public POSSampleEventStream(IObjectStream<POSSample> samples, IPOSContextGenerator cg) : base(samples) {
            this.cg = cg;
        }

        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(POSSample sample) {
            var events = GenerateEvents(sample.Sentence, sample.Tags, sample.GetAdditionalContext(), cg);
            return events.GetEnumerator();
        }

        internal static List<Event> GenerateEvents(string[] sentence, string[] tags, object[] additionalContext,
            IPOSContextGenerator cg) {
            var events = new List<Event>();
            for (var i = 0; i < sentence.Length; i++) {
                // it is safe to pass the tags as previous tags because
                // the context generator does not look for non predicted tags
                var context = cg.GetContext(i, sentence, tags, additionalContext);

                events.Add(new Event(tags[i], context));
            }
            return events;
        }

        internal static List<Event> GenerateEvents(string[] sentence, string[] tags, IPOSContextGenerator cg) {
            return GenerateEvents(sentence, tags, null, cg);
        }

    }
}