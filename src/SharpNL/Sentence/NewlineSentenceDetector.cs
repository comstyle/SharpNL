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
using SharpNL.Utility;

namespace SharpNL.Sentence {
    /// <summary>
    /// The Newline Sentence Detector assumes that sentences are line delimited and 
    /// recognizes one sentence per non-empty line.
    /// </summary>
    public class NewlineSentenceDetector : ISentenceDetector {
        /// <summary>
        /// Detects the sentences in the specified string.
        /// </summary>
        /// <param name="text">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:string[]"/> with the individual sentences as the array elements.</returns>
        public string[] SentDetect(string text) {
            return Span.SpansToStrings(SentPosDetect(text), text);
        }

        /// <summary>
        /// Detects the position of the sentences in the specified string.
        /// </summary>
        /// <param name="text">The string to be sentence detected.</param>
        /// <returns>The <see cref="T:Span[]"/> with the spans (offsets into <paramref name="text"/>) for each detected sentence as the individuals array elements.</returns>
        public Span[] SentPosDetect(string text) {
            var sentences = new List<Span>();
            var start = 0;

            for (var i = 0; i < text.Length; i++) {
                if (text[i] == '\n' || text[i] == '\r') {
                    if (start - i > 0) {
                        var span = new Span(start, i).Trim(text);
                        if (span.Length > 0) {
                            sentences.Add(span);
                        }
                        start = i + 1;
                    }
                }
            }

            if (text.Length - start > 0) {
                var span = new Span(start, text.Length).Trim(text);
                if (span.Length > 0) {
                    sentences.Add(span);
                }
            }

            return sentences.ToArray();
        }
    }
}