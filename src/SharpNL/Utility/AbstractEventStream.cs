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

namespace SharpNL.Utility {
    public abstract class AbstractEventStream<T> : IObjectStream<Event> {

        private readonly IObjectStream<T> samples;
        private IEnumerator<Event> events = new List<Event>().GetEnumerator();

        protected AbstractEventStream(IObjectStream<T> samples) {
            if (samples == null) {
                throw new ArgumentNullException("samples");
            }

            this.samples = samples;
        }

        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected abstract IEnumerator<Event> CreateEvents(T sample);

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            events = null;
            samples.Dispose();
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next <see cref="T:Event"/>. Calling this method repeatedly until it returns,
        /// null will return each event from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public Event Read() {

            if (events.MoveNext()) {
                return events.Current;
            }

            T sample;

            while ((sample = samples.Read()) != null) {
                events = CreateEvents(sample);

                if (events != null && events.MoveNext()) {
                    return events.Current;
                }
            }
            
            return null;
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            events = new List<Event>().GetEnumerator();
            samples.Reset();
        }
        #endregion

    }
}