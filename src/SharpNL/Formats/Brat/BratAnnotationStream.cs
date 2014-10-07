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
using System.Text;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Brat {
    public class BratAnnotationStream : IObjectStream<BratAnnotation> {

        private readonly AnnotationConfiguration config;
        private readonly StreamReader reader;
        private readonly Stream input;
        private readonly long start;
        private readonly string id;
        

        public BratAnnotationStream(AnnotationConfiguration config, string id, Stream inputStream) {

            this.config = config;
            this.id = id;

            if (inputStream.CanSeek) {
                input = inputStream;
                start = inputStream.Position;
            } else {
                start = -1;
            }

            reader = new StreamReader(inputStream, Encoding.UTF8);
        }

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            reader.Dispose();
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next annotation. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next annotation or null to signal that the stream is exhausted.
        /// </returns>
        public BratAnnotation Read() {
            string line = reader.ReadLine();

            if (line != null) {
                var tokens = WhitespaceTokenizer.Instance.TokenizePos(line);

                if (tokens.Length > 2) {
                    string type = config[tokens[BratParser.TYPE_OFFSET].GetCoveredText(line)];

                    BratParse parse;

                    switch (type) {
                        case AnnotationConfiguration.SPAN_TYPE:
                        case AnnotationConfiguration.ENTITY_TYPE:
                            parse = BratParser.ParseSpanAnnotation;
                            break;
                        case AnnotationConfiguration.RELATION_TYPE:
                            parse = BratParser.ParseRelationAnnotation;
                            break;
                        default:
                            throw new InvalidDataException(
                                "Failed to parse ann document with id " + id +
                                " type class, no parser registered: " + tokens[BratParser.TYPE_OFFSET]);
                    }
                    return parse(tokens, line);
                }
            }
            return null;
        }

        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            if (input == null) 
                throw new NotSupportedException("The input stream was not seekable.");

            input.Seek(start, SeekOrigin.Begin);
        }
        #endregion

    }
}