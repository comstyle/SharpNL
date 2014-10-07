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
using System.IO;
using SharpNL.Utility;

namespace SharpNL.POSTag {
    /// <summary>
    /// A stream filter which reads a sentence per line which contains
    /// words and tags in word_tag format and outputs a <see cref="POSSample"/> objects.
    /// </summary>
    public class WordTagSampleStream : FilterObjectStream<string, POSSample> {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using 
        /// a <see cref="PlainTextByLineStream"/> to read the input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <seealso cref="PlainTextByLineStream"/>
        public WordTagSampleStream(Stream inputStream) : this(new PlainTextByLineStream(inputStream)) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WordTagSampleStream"/> using the
        /// <see cref="T:IObjectStream{string}"/> as sample reader.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public WordTagSampleStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override POSSample Read() {
            retry:
            string sentence = Samples.Read();

            if (sentence != null) {
                POSSample sample;
                try {
                    sample = POSSample.Parse(sentence);
                } catch (Exception) {
                    // TODO: implement a better logger
                    Console.Error.WriteLine("Error during parsing, ignoring sentence: " + sentence);
                    goto retry;
                }
                return sample;
            }

            return null;
        }
    }
}