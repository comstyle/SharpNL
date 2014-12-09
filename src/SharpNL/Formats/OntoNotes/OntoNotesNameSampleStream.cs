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
using SharpNL.Extensions;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.OntoNotes {
    /// <summary>
    /// Name Sample Stream parser for the OntoNotes 4.0 corpus.
    /// </summary>
    public class OntoNotesNameSampleStream : FilterObjectStream<string, NameSample> {
        private const string typeBegin = "TYPE=\"";

        private static readonly Dictionary<string, string> tokenConversionMap;

        private readonly List<NameSample> nameSamples;

        static OntoNotesNameSampleStream() {
            tokenConversionMap = new Dictionary<string, string> {
                {"-LRB-", "("},
                {"-RRB-", ")"},
                {"-LSB-", "["},
                {"-RSB-", "]"},
                {"-LCB-", "{"},
                {"-RCB-", "}"},
                {"-AMP-", "&"}
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OntoNotesNameSampleStream"/> with the given samples.
        /// </summary>
        /// <param name="samples">The samples.</param>
        public OntoNotesNameSampleStream(IObjectStream<string> samples) : base(samples) {
            nameSamples = new List<NameSample>();
        }

        #region . ConvertToken .

        private static string ConvertToken(string token) {
            var sb = new StringBuilder(token);

            var startTagEndIndex = sb.IndexOf('>');

            if (token.Contains("=\"") && startTagEndIndex != -1)
                sb.Remove(0, startTagEndIndex + 1);

            var endTagBeginIndex = sb.IndexOf('<');
            var endTagEndIndex = sb.IndexOf('>');

            if (endTagBeginIndex != -1 && endTagEndIndex != -1)
                sb.Remove(endTagBeginIndex, endTagEndIndex + 1);

            var cleanedToken = sb.ToString();

            if (tokenConversionMap.ContainsKey(cleanedToken)) {
                cleanedToken = tokenConversionMap[cleanedToken];
            }

            return cleanedToken;
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override NameSample Read() {
            if (nameSamples.Count == 0) {
                var doc = Samples.Read();

                if (doc != null) {
                    var docIn = new StringReader(doc);

                    var clearAdaptiveData = true;

                    string line;
                    while ((line = docIn.ReadLine()) != null) {
                        if (line.StartsWith("<DOC")) {
                            continue;
                        }

                        if (line.Equals("</DOC>")) {
                            break;
                        }

                        var tokens = WhitespaceTokenizer.Instance.Tokenize(line);

                        var entities = new List<Span>();
                        var cleanedTokens = new List<string>(tokens.Length);


                        var tokenIndex = 0;
                        var entityBeginIndex = -1;
                        var insideStartEnmaxTag = false;

                        string entityType = null;
                        foreach (var token in tokens) {
                            // Split here, next part of tag is in new token
                            if (token.StartsWith("<ENAMEX")) {
                                insideStartEnmaxTag = true;
                                continue;
                            }

                            if (insideStartEnmaxTag) {
                                if (token.StartsWith(typeBegin)) {
                                    var typeEnd = token.IndexOf("\"", typeBegin.Length, StringComparison.Ordinal);

                                    entityType =
                                        token.Substring(typeBegin.Length, typeEnd - typeBegin.Length).ToLowerInvariant();
                                    //entityType = StringUtil.toLowerCase(token.substring(typeBegin.length(), typeEnd));
                                }

                                if (token.Contains(">")) {
                                    entityBeginIndex = tokenIndex;
                                    insideStartEnmaxTag = false;
                                } else {
                                    continue;
                                }
                            }

                            if (token.EndsWith("</ENAMEX>")) {
                                entities.Add(new Span(entityBeginIndex, tokenIndex + 1, entityType));
                                entityBeginIndex = -1;
                            }

                            cleanedTokens.Add(ConvertToken(token));
                            tokenIndex++;
                        }

                        nameSamples.Add(new NameSample(cleanedTokens.ToArray(), entities.ToArray(), clearAdaptiveData));

                        clearAdaptiveData = false;
                    }
                }
            }

            return nameSamples.Count > 0 ? nameSamples.Pop() : null;
        }

        #endregion
    }
}