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

using System.IO;
using SharpNL.Utility;

namespace SharpNL.POSTag {
    /// <summary>
    /// A stream filter which reads a sentence per line which contains
    /// words and tags in word_tag format and outputs a <see cref="POSSample"/> objects.
    /// </summary>
    /// <remarks>
    /// The native POS Tagger training material looks like this:
    /// <para>
    /// <c>About_IN 10_CD Euro_NNP ,_, I_PRP reckon_VBP ._. That_DT sounds_VBZ good_JJ ._.</c>
    /// </para>
    /// </remarks>
    public class WordTagSampleStream : FilterObjectStream<string, POSSample> {

        private readonly Monitor monitor;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using a <see cref="PlainTextByLineStream"/> to read the input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <seealso cref="PlainTextByLineStream"/>
        public WordTagSampleStream(Stream inputStream) : this(new PlainTextByLineStream(inputStream)) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using 
        /// a <see cref="PlainTextByLineStream"/> to read the input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        /// <seealso cref="PlainTextByLineStream"/>
        public WordTagSampleStream(Stream inputStream, Monitor monitor) : this(new PlainTextByLineStream(inputStream)) {
            this.monitor = monitor;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using the
        /// <see cref="T:IObjectStream{string}"/> as sample reader.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public WordTagSampleStream(IObjectStream<string> samples) : base(samples) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using the
        /// <see cref="T:IObjectStream{string}"/> as sample reader.
        /// </summary>
        /// <param name="samples">The samples.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        public WordTagSampleStream(IObjectStream<string> samples, Monitor monitor) : base(samples) {
            this.monitor = monitor;
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
        public override POSSample Read() {
        retry:

            var sentence = Samples.Read();
            if (sentence == null)
                return null;

            POSSample sample;
            try {
                sample = POSSample.Parse(sentence);
            } catch {
                if (monitor != null)
                    monitor.OnWarning("Error during parsing, ignoring sentence: " + sentence);

                goto retry;
            }
            return sample;
        }
        #endregion

    }
}