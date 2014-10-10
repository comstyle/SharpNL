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

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a abstract segmented object stream that creates sub segments/samples of a single sample. 
    /// </summary>
    /// <typeparam name="S">The sample type.</typeparam>
    /// <typeparam name="T">The segment type.</typeparam>
    public abstract class SegmentedObjectStream<S, T> : FilterObjectStream<S, T> 
        where T : class 
        where S : class {

        private IEnumerator<T> sampleEnumerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedObjectStream{S, T}"/> class.
        /// </summary>
        /// <param name="samples">The samples to be segmented.</param>
        protected SegmentedObjectStream(IObjectStream<S> samples) : base(samples) { }

        /// <summary>
        /// Reads the segments of the specified sample.
        /// </summary>
        /// <param name="sample">The sample.</param>
        /// <returns>The segments of the <paramref name="sample"/> object.</returns>
        protected abstract List<T> Read(S sample);

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override T Read() {
            if (sampleEnumerator != null && sampleEnumerator.MoveNext()) {
                return sampleEnumerator.Current;
            }

            var inSample = Samples.Read();
            if (inSample != null) {
                var outSamples = Read(inSample);
                if (outSamples != null) {
                    sampleEnumerator = outSamples.GetEnumerator();
                    return Read();
                }
            }
            return null;
        }
    }
}