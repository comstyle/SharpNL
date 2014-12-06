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
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {
    /// <summary>
    /// Represents a Penn Treebank <see cref="TokenSample"/> stream. This class cannot be inherited.
    /// </summary>
    public sealed class PtbTokenSampleStream : PtbSampleStream<TokenSample> {

        private readonly IDetokenizer detokenizer;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSampleStream{T}" /> using a specified Penn Treebank stream.
        /// </summary>
        /// <param name="stream">The Penn Treebank stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="detokenizer"/>
        /// or
        /// <paramref name="stream"/>
        /// </exception>
        public PtbTokenSampleStream(PtbStreamReader stream, IDetokenizer detokenizer) : base(stream) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            this.detokenizer = detokenizer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbTokenSampleStream"/> using a <see cref="T:IObjectStream{string}"/>.
        /// The function tags will be ignored using this constructor.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        public PtbTokenSampleStream(string language, IObjectStream<string> lineStream, IDetokenizer detokenizer)
            : this(language, lineStream, detokenizer, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbTokenSampleStream"/> using a <see cref="T:IObjectStream{string}"/> and a evaluation monitor.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="monitor">The monitor.</param>
        public PtbTokenSampleStream(string language, IObjectStream<string> lineStream, IDetokenizer detokenizer, Monitor monitor)
            : base(new PtbStreamReader(language, lineStream, false, monitor)) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            this.detokenizer = detokenizer;
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next <see cref="TokenSample"/> object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="TokenSample"/> object or <c>null</c> to signal that the stream is exhausted.
        /// </returns>
        public override TokenSample Read() {
            var node = Stream.Read();
            return node != null
                ? new TokenSample(detokenizer, node.Tokens)
                : null;
        }
        #endregion

    }
}