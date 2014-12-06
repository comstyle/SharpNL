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
using System.Globalization;
using System.IO;
using System.Text;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a real value file event stream.
    /// </summary>
    public class RealValueFileEventStream : FileEventStream {

        public RealValueFileEventStream(Stream input) : base(input) {}

        public RealValueFileEventStream(Stream input, Monitor monitor) : base(input) {
            Monitor = monitor;
        }

        public RealValueFileEventStream(string fileName) : base(fileName) {}

        public RealValueFileEventStream(string fileName, Monitor monitor) : base(fileName) {
            Monitor = monitor;
        }

        public RealValueFileEventStream(string fileName, Encoding encoding) : base(fileName, encoding) {}

        public RealValueFileEventStream(string fileName, Encoding encoding, Monitor monitor) : base(fileName, encoding) {
            Monitor = monitor;
        }

        #region . Monitor .
        /// <summary>
        /// Gets the evaluation monitor.
        /// </summary>
        /// <value>The evaluation monitor.</value>
        protected Monitor Monitor { get; private set; }
        #endregion

        #region + ParseContexts .
        /// <summary>
        /// Parses the specified contexts and re-populates context array with features and returns the 
        /// values for these features. 
        /// If all values are unspecified, then null is returned.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        /// <returns>The event values.</returns>
        /// <exception cref="System.InvalidOperationException">Negative values are not allowed: context</exception>
        public static float[] ParseContexts(string[] contexts) {
            return ParseContexts(contexts, null);
        }

        /// <summary>
        /// Parses the specified contexts and re-populates context array with features and returns 
        /// the values for these features. 
        /// If all values are unspecified, then null is returned.
        /// </summary>
        /// <param name="contexts">The contexts.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        /// <returns>The event values.</returns>
        /// <exception cref="System.InvalidOperationException">Negative values are not allowed: context</exception>
        public static float[] ParseContexts(string[] contexts, Monitor monitor) {
            var hasRealValue = false;
            var values = new float[contexts.Length];
            for (var ci = 0; ci < contexts.Length; ci++) {
                var ei = contexts[ci].LastIndexOf("=", StringComparison.InvariantCulture);
                if (ei > 0 && ei + 1 < contexts[ci].Length) {
                    var gotReal = true;
                    try {
                        values[ci] = float.Parse(contexts[ci].Substring(ei + 1), CultureInfo.InvariantCulture);
                    } catch (Exception) {
                        gotReal = false;

                        if (monitor != null)
                            monitor.OnError("Unable to determine value in context:" + contexts[ci]);

                        values[ci] = 1;
                    }
                    if (gotReal) {
                        if (values[ci] < 0) {
                            throw new InvalidOperationException("Negative values are not allowed: " + contexts[ci]);
                        }
                        contexts[ci] = contexts[ci].Substring(0, ei);
                        hasRealValue = true;
                    }
                } else {
                    values[ci] = 1;
                }
            }
            if (!hasRealValue) {
                values = null;
            }
            return values;
        }

        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override Event Read() {
            string line;
            if ((line = Reader.ReadLine()) != null) {
                var si = line.IndexOf(' ');
                var outcome = line.Substring(0, si);
                var contexts = line.Substring(si + 1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var values = ParseContexts(contexts, Monitor);
                return (new Event(outcome, contexts, values));
            }

            return null;
        }

        #endregion
    }
}