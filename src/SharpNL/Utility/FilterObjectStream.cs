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

namespace SharpNL.Utility {
    /// <summary>
    /// Abstract base class for filtering <see cref="T:IObjectStream{T}"/>.
    /// Filtering streams take an existing stream and convert its output to something else.
    /// </summary>
    /// <typeparam name="S">The type of the source/input stream.</typeparam>
    /// <typeparam name="T">The type of this stream.</typeparam>
    public abstract class FilterObjectStream<S, T> : IObjectStream<T> {
        protected FilterObjectStream(IObjectStream<S> samples) {
            if (samples == null) {
                throw new ArgumentNullException("samples", @"The items must not be null.");
            }
            Samples = samples;
        }

        /// <summary>
        /// Gets the items from this filtered stream.
        /// </summary>
        /// <value>The items from this filtered stream.</value>
        protected IObjectStream<S> Samples { get; private set; }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose() {
            Samples.Dispose();
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public abstract T Read();

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public virtual void Reset() {
            Samples.Reset();
        }
    }
}