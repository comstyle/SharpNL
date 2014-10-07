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
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// This stream formats a <see cref="TokenSample"/>s into whitespace separated token strings.
    /// </summary>
    public class WhitespaceTokenStream : FilterObjectStream<TokenSample, string> {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhitespaceTokenStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public WhitespaceTokenStream(IObjectStream<TokenSample> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override string Read() {
            var tokenSample = Samples.Read();

            if (tokenSample != null) {
                var sb = new StringBuilder();

                foreach (var token in tokenSample.TokenSpans) {
                    sb.Append(token.GetCoveredText(tokenSample.Text));
                    sb.Append(' ');
                }

                // Shorten string by one to get rid of last space
                if (sb.Length > 0) {
                    sb.Remove(sb.Length - 1, 1);
                }

                return sb.ToString();
            }

            return null;
        }
    }
}