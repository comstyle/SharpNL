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

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Stream to to clean up empty lines for empty line separated document streams.
    /// <code>
    ///  - Skips empty line at training data start.
    ///  - Transforms multiple empty lines in a row into one.
    ///  - Replaces white space lines with empty lines.
    /// </code>
    /// This stream should be used by the components that mark empty lines to mark document boundaries.
    /// </summary>
    internal class EmptyLinePreprocessorStream : FilterObjectStream<string, string> {
        private bool lastLineWasEmpty = true;

        public EmptyLinePreprocessorStream(IObjectStream<string> samples) : base(samples) {}

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override string Read() {
            var line = Samples.Read();

            if (lastLineWasEmpty) {
                lastLineWasEmpty = false;

                while (line != null && line.Trim().Length == 0) {
                    line = Samples.Read();
                }
            }

            if (line != null && line.Trim().Length == 0) {
                lastLineWasEmpty = true;
                line = string.Empty;
            }

            return line;
        }

        #endregion
    }
}