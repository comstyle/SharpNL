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
using System.Text;
using System.Collections.Generic;

using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// Parses the data from the CONLL 06 shared task into POS Samples.
    /// </summary>
    /// <remarks>
    /// More information about the data format can be found here:<br />
    /// <see href="http://www.cnts.ua.ac.be/conll2006/" />
    /// </remarks>
    public class ConllXPOSSampleStream : FilterObjectStream<string, POSSample> {
        public ConllXPOSSampleStream(IObjectStream<string> samples) : base(new ParagraphStream(samples)) {}

        public ConllXPOSSampleStream(IInputStreamFactory streamFactory, Encoding encoding)
            : base(new ParagraphStream(new PlainTextByLineStream(streamFactory, encoding))) {}


        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override POSSample Read() {
            // The CONLL-X data has a word per line and each line is tab separated
            // in the following format:
            // ID, FORM, LEMMA, CPOSTAG, POSTAG, ... (max 10 fields)

            // One paragraph contains a whole sentence and, the token
            // and tag will be read from the FORM and POSTAG field.

            var paragraph = Samples.Read();

            POSSample sample = null;

            if (paragraph != null) {
                // paragraph get lines
                var reader = new StringReader(paragraph);

                var tokens = new List<string>(100);
                var tags = new List<string>(100);

                string line;
                while ((line = reader.ReadLine()) != null) {
                    const int minNumberOfFields = 5;

                    var parts = line.Split('\t');

                    if (parts.Length >= minNumberOfFields) {
                        tokens.Add(parts[1]);
                        tags.Add(parts[4]);
                    } else {
                        throw new InvalidFormatException("Every non-empty line must have at least " +
                                                         minNumberOfFields + " fields: '" + line + "'!");
                    }
                }

                // just skip empty samples and read next sample
                if (tokens.Count == 0)
                    Read();

                sample = new POSSample(tokens.ToArray(), tags.ToArray());
            }

            return sample;
        }
    }
}