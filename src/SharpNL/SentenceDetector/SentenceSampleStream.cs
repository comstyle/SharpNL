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

using System.Collections.Generic;
using System.Text;
using SharpNL.Utility;

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// This class is a stream filter which reads a sentence by line samples from
    /// a reader and converts them into <see cref="T:SentenceSample"/> objects.
    /// An empty line indicates the begin of a new document.
    /// </summary>
    public class SentenceSampleStream : FilterObjectStream<string, SentenceSample> {
        public SentenceSampleStream(IObjectStream<string> items) :
            base(new EmptyLinePreprocessorStream(items)) {}

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override SentenceSample Read() {
            var sentenceSpans = new List<Span>();
            var sentencesString = new StringBuilder();

            string sentence;
            while ((sentence = Samples.Read()) != null && !sentence.Equals(string.Empty)) {
                var begin = sentencesString.Length;
                sentence = sentence.Trim();
                sentence = ReplaceNewLineEscapeTags(sentence);
                sentencesString.Append(sentence);
                var end = sentencesString.Length;
                sentenceSpans.Add(new Span(begin, end));
                sentencesString.Append(' ');
            }

            if (sentenceSpans.Count > 0) {
                return new SentenceSample(sentencesString.ToString(), sentenceSpans.ToArray());
            }
            return null;
        }

        #endregion

        #region . ReplaceNewLineEscapeTags .
        internal static string ReplaceNewLineEscapeTags(string value) {
            return value.Replace("<LF>", "\n").Replace("<CR>", "\r");
        }
        #endregion

    }
}