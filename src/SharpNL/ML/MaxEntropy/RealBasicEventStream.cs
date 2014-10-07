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
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.ML.MaxEntropy {

    public class RealBasicEventStream : IObjectStream<Event> {

        private readonly IObjectStream<string> objectStream;

        public RealBasicEventStream(IObjectStream<string> objectStream) {
            
            if (objectStream == null)
                throw new ArgumentNullException("objectStream");

            this.objectStream = objectStream;
        }

        #region . CreateEvent .

        private static Event CreateEvent(string value) {
            var lastSpace = value.LastIndexOf(' ');
            if (lastSpace == -1)
                return null;

            var contexts = value.Substring(0, lastSpace).RegExSplit(Expressions.Expression.Space);
            var values = RealValueFileEventStream.ParseContexts(contexts);
            return new Event(value.Substring(lastSpace + 1), contexts, values);

        }
        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            objectStream.Dispose();
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public Event Read() {
            var eventString = objectStream.Read();
            return eventString != null ? CreateEvent(eventString) : null;
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            objectStream.Reset();
        }
        #endregion

    }
}