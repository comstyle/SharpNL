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
using System.IO;
using System.Text;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Muc {
    public class MucNameSampleStream : FilterObjectStream<string, NameSample> {

        private readonly ITokenizer tokenizer;
        private List<NameSample> storedSamples = new List<NameSample>();

        public MucNameSampleStream(ITokenizer tokenizer, IObjectStream<string> samples) : base(samples) {
            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            this.tokenizer = tokenizer;
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override NameSample Read() {

            if (storedSamples.Count == 0) {

                var document = Samples.Read();

                // Note: This is a hack to fix invalid formating in some MUC files ...
                if (document != null) {
                    document = document.Replace(">>", ">");

                    new SgmlParser().Parse(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(document))), new MucNameContentHandler(tokenizer, storedSamples));
                }

            }

            if (storedSamples.Count > 0) {
                var sample = storedSamples[0];
                storedSamples.RemoveAt(0);
                return sample;
            }

            return null;
        }
    }
}