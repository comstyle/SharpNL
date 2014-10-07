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

namespace SharpNL.ML.Model {
    /// <summary>
    /// Class which models a sequence.
    /// </summary>
    public class Sequence {

        private readonly object source;
        /// <summary>
        /// Creates a new sequence made up of the specified events and derived from the specified source.
        /// </summary>
        /// <param name="events">The events of the sequence.</param>
        /// <param name="source">The source object for this sequence.</param>
        public Sequence(Event[] events, object source) {
            Events = events;
            this.source = source;
        }

        /// <summary>
        /// Gets the events which make up this sequence.
        /// </summary>
        /// <value>The events which make up this sequence.</value>
        public Event[] Events { get; private set; }

        /// <summary>
        /// Gets an object from which this sequence can be derived. This object is
        /// used when the events for this sequence need to be re-derived such as in a
        /// call to <see cref="ISequenceStream.UpdateContext"/>.
        /// </summary>
        /// <value>An object from which this sequence can be derived.</value>
        public T GetSource<T>() {
            return (T) source;
        }
    }
}