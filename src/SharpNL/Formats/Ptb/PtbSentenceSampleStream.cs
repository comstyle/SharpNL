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
using System.Text;
using SharpNL.SentenceDetector;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {
    /// <summary>
    /// This stream provides <see cref="SentenceSample"/> samples using a Penn Treebank stream as input.
    /// The samples are created using a specific number of sentences defined in the constructor.
    /// </summary>
    public class PtbSentenceSampleStream : PtbSampleStream<SentenceSample> {

        private readonly int sampleSize;
        private readonly IDetokenizer detokenizer;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSentenceSampleStream" /> using a specified Penn Treebank stream.
        /// </summary>
        /// <param name="stream">The Penn Treebank stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="sampleSize">The number of sentences in each sample.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="detokenizer" />
        /// or
        /// <paramref name="stream" /></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">sampleSize</exception>
        public PtbSentenceSampleStream(PtbStreamReader stream, IDetokenizer detokenizer, int sampleSize) : base(stream) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            if (sampleSize < 1)
                throw new ArgumentOutOfRangeException("sampleSize");

            this.detokenizer = detokenizer;
            this.sampleSize = sampleSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSentenceSampleStream" /> using a <see cref="T:IObjectStream{string}" />.
        /// The function tags will be ignored using this constructor.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="sampleSize">The number of sentences in each sample.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="detokenizer" /></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">sampleSize</exception>
        public PtbSentenceSampleStream(string language, IObjectStream<string> lineStream, IDetokenizer detokenizer, int sampleSize)
            : this(language, lineStream, detokenizer, sampleSize, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSentenceSampleStream" /> using a <see cref="T:IObjectStream{string}" /> and a evaluation monitor.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer.</param>
        /// <param name="sampleSize">The number of sentences in each sample.</param>
        /// <param name="monitor">The monitor.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="detokenizer" /></exception>
        /// <exception cref="System.ArgumentOutOfRangeException">sampleSize</exception>
        public PtbSentenceSampleStream(string language, IObjectStream<string> lineStream, IDetokenizer detokenizer, int sampleSize, Monitor monitor)
            : base(new PtbStreamReader(language, lineStream, false, monitor)) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            if (sampleSize < 1)
                throw new ArgumentOutOfRangeException("sampleSize");

            this.detokenizer = detokenizer;
            this.sampleSize = sampleSize;
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next <see cref="SentenceSample"/> object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="SentenceSample"/> object or <c>null</c> to signal that the stream is exhausted.
        /// </returns>
        public override SentenceSample Read() {
            var sb = new StringBuilder(512);           
            var spans = new List<Span>(sampleSize);
            var count = 0;

            PtbNode node;
            while (count <= sampleSize && (node = Stream.Read()) != null) {
                var start = sb.Length;
                var text = detokenizer.Detokenize(node.Tokens, null);
                sb.Append(text);

                spans.Add(new Span(start, sb.Length));

                sb.Append(" ");

                count++;
            }

            return count > 0
                ? new SentenceSample(sb.ToString(), spans.ToArray())
                : null;

        }
        #endregion
    }
}