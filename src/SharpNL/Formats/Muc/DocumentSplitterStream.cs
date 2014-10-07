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
using SharpNL.Utility;

namespace SharpNL.Formats.Muc {
    public class DocumentSplitterStream : FilterObjectStream<string, string> {
        private const string DOC_START_ELEMENT = "<DOC>";
        private const string DOC_END_ELEMENT = "</DOC>";

        private readonly List<string> docs = new List<string>();


        public DocumentSplitterStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override string Read() {
            if (docs.Count == 0) {
                var newDocs = Samples.Read();

                if (newDocs != null) {
                    var docStartOffset = 0;

                    while (true) {
                        var startDocElement = newDocs.IndexOf(DOC_START_ELEMENT, docStartOffset,
                            StringComparison.InvariantCulture);
                        var endDocElement = newDocs.IndexOf(DOC_END_ELEMENT, docStartOffset,
                            StringComparison.InvariantCulture);

                        if (startDocElement != -1 && endDocElement != -1) {
                            if (startDocElement < endDocElement) {
                                docs.Add(newDocs.Substring(startDocElement, (endDocElement + DOC_END_ELEMENT.Length) - startDocElement));
                                docStartOffset = endDocElement + DOC_END_ELEMENT.Length;
                            } else {
                                throw new InvalidFormatException("<DOC> element is not closed!");
                            }
                        } else if (startDocElement != endDocElement) {
                            throw new InvalidFormatException("Missing <DOC> or </DOC> element!");
                        } else {
                            break;
                        }
                    }
                }
            }

            if (docs.Count > 0) {
                var doc = docs[0];
                docs.RemoveAt(0);
                return doc;
            }
            return null;
        }
    }
}