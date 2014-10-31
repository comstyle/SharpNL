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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SharpNL.SentenceDetector;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    public class AdSentenceSampleStream : IObjectStream<SentenceSample> {
        public static readonly char[] eosCharacters;
        private static readonly Regex metaTag1;

        private readonly Monitor monitor;
        private readonly IObjectStream<AdSentence> adSentenceStream;
        private readonly bool isIncludeTitles;

        private bool isSamePara;
        private bool isSameText;
        private bool isTitle;
        private int para = -1;
        private AdSentence sentence;
        private int text = -1;

        static AdSentenceSampleStream() {
            metaTag1 = new Regex("^(?:[a-zA-Z\\-]*(\\d+)).*?p=(\\d+).*", RegexOptions.Compiled);

            eosCharacters = new[] {'.', '?', '!', ';', ':', '(', ')', '«', '»', '\'', '"'};
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AdSentenceSampleStream"/> stream from a line stream.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="includeTitles">if set to <c>true</c> it will output the sentences marked as news headlines.</param>
        /// <param name="safeParse">if set to <c>true</c> it will ignore the invalid Ad elements.</param>
        public AdSentenceSampleStream(IObjectStream<string> lineStream, bool includeTitles, bool safeParse) {
            adSentenceStream = new AdSentenceStream(lineStream, safeParse);
            isIncludeTitles = includeTitles;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdSentenceSampleStream"/> stream from a line stream.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="includeTitles">if set to <c>true</c> it will output the sentences marked as news headlines.</param>
        /// <param name="safeParse">if set to <c>true</c> it will ignore the invalid Ad elements.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        public AdSentenceSampleStream(IObjectStream<string> lineStream, bool includeTitles, bool safeParse, Monitor monitor) {
            adSentenceStream = new AdSentenceStream(lineStream, safeParse, monitor);
            isIncludeTitles = includeTitles;
            this.monitor = monitor;
        }

        #region . Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            adSentenceStream.Dispose();
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next <see cref="T:SentenceSample"/>. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public SentenceSample Read() {
            if (sentence == null) {
                sentence = adSentenceStream.Read();
                UpdateMeta();
                if (sentence == null) {
                    return null;
                }
            }


            var document = new StringBuilder();
            var sentences = new List<Span>();

            do {
                do {
                    if (!isTitle || (isTitle && isIncludeTitles)) {
                        if (HasPunctuation(sentence.Text)) {
                            var start = document.Length;
                            document.Append(sentence.Text);
                            sentences.Add(new Span(start, document.Length));
                            document.Append(' ');
                        }
                    }
                    sentence = adSentenceStream.Read();
                    UpdateMeta();
                } while (isSamePara);
            } while (isSameText);

            return new SentenceSample(
                document.Length > 0 ? document.ToString(0, document.Length - 1) : document.ToString(),
                sentences.ToArray()
                );
        }

        #endregion

        private static bool HasPunctuation(string text) {
            return !string.IsNullOrEmpty(text) && text.IndexOfAny(eosCharacters) != -1;
        }

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            adSentenceStream.Reset();
        }

        #endregion

        #region . UpdateMeta .

        private void UpdateMeta() {
            if (sentence != null) {
                var m = metaTag1.Match(sentence.Metadata);
                int currentText;
                int currentPara;

                if (m.Success) {
                    currentText = int.Parse(m.Groups[1].Value);
                    currentPara = int.Parse(m.Groups[2].Value);
                } else {
                    var ex = new InvalidDataException("Invalid metadata: " + sentence.Metadata);

                    if (monitor == null) 
                        throw ex;

                    monitor.OnException(ex);
                    return;
                }

                isTitle = sentence.Metadata.Contains("title");
                isSameText = currentText == text;
                isSamePara = isSameText && currentPara == para;

                text = currentText;
                para = currentPara;
            } else {
                isSamePara = isSameText = false;
            }
        }

        #endregion
    }
}