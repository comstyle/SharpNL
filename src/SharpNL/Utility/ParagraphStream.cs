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


using System.Text;

namespace SharpNL.Utility {
    /// <summary>
    /// Stream filter which merges text lines into paragraphs. The boundary of paragraph is defined
    /// by an empty text line. If the last paragraph in the stream is not terminated by an empty line
    /// the left over is assumed to be a paragraph.
    /// </summary>
    public class ParagraphStream : FilterObjectStream<string, string> {
        public ParagraphStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override string Read() {
            var sb = new StringBuilder();

            while (true) {
                var line = Samples.Read();

                // The last paragraph in the input might not
                // be terminated well with a new line at the end.

                if (string.IsNullOrEmpty(line)) {
                    if (sb.Length > 0) {
                        return sb.ToString();
                    }
                } else {
                    sb.AppendLine(line);
                }

                if (line == null)
                    return null;
            }

        }
    }
}