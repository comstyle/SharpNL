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

using SharpNL.Extensions;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.DocumentCategorizer {
    /// <summary>
    /// This class reads in string encoded training samples, parses them and outputs <see cref="DocumentSample"/> objects.
    /// </summary>
    /// <remarks>
    /// Format:<br />
    /// Each line contains one sample document.<br />
    /// The category is the first string in the line followed by a tab and whitespace separated document tokens.<br />
    /// Sample line: category-string tab-char whitespace-separated-tokens line-break-char(s)
    /// </remarks>
    public class DocumentSampleStream : FilterObjectStream<string, DocumentSample> {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentSampleStream"/> with the given samples.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public DocumentSampleStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override DocumentSample Read() {
            var sampleString = Samples.Read();

            if (sampleString == null)
                return null;

            var tokens = WhitespaceTokenizer.Instance.Tokenize(sampleString);

            if (tokens.Length > 1) {
                return new DocumentSample(tokens[0], tokens.SubArray(1, tokens.Length - 1));
            }

            throw new InvalidFormatException("Empty lines, or lines with only a category string are not allowed!");
        }
    }
}