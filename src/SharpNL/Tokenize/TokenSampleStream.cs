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
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// This class is a stream filter which reads in string encoded samples and creates
    /// <see cref="TokenSample"/>s out of them. The input string sample is tokenized if a
    /// whitespace or the special separator chars occur.
    /// </summary>
    public class TokenSampleStream : FilterObjectStream<string, TokenSample> {
        private readonly string separatorChars;


        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSampleStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public TokenSampleStream(IObjectStream<string> samples) : this(samples, TokenSample.DefaultSeparator) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenSampleStream"/> class.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="separatorChars">The separator chars.</param>
        /// <exception cref="ArgumentNullException">separatorChars</exception>
        public TokenSampleStream(IObjectStream<string> samples, string separatorChars) : base(samples) {
            if (separatorChars == null)
                throw new ArgumentNullException("separatorChars");

            this.separatorChars = separatorChars;
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override TokenSample Read() {
            var sampleString = Samples.Read();

            if (sampleString != null) {
                return TokenSample.Parse(sampleString, separatorChars);
            }

            return null;
        }
    }
}