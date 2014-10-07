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
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Formats {
    public class Conll02NameSampleStream : IObjectStream<NameSample> {
        public enum Language {
            ES,
            NL
        }

        internal const int GENERATE_PERSON_ENTITIES = 0x01;
        internal const int GENERATE_ORGANIZATION_ENTITIES = 0x01 << 1;
        internal const int GENERATE_LOCATION_ENTITIES = 0x01 << 2;
        internal const int GENERATE_MISC_ENTITIES = 0x01 << 3;

        internal const String DOCSTART = "-DOCSTART-";

        internal readonly Language lang;
        internal readonly IObjectStream<String> lineStream;

        internal readonly int types;

        #region + Constructors .

        public Conll02NameSampleStream(Language lang, IObjectStream<string> lineStream, int types) {
            if (!Enum.IsDefined(typeof (Language), lang))
                throw new ArgumentOutOfRangeException("lang");

            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            this.lang = lang;
            this.lineStream = lineStream;
            this.types = types;
        }

        public Conll02NameSampleStream(Language lang, IInputStreamFactory streamFactory, int types) {
            if (!Enum.IsDefined(typeof (Language), lang))
                throw new ArgumentOutOfRangeException("lang");

            if (streamFactory == null)
                throw new ArgumentNullException("streamFactory");

            lineStream = new PlainTextByLineStream(streamFactory);
            this.types = types;
        }

        #endregion

        #region . Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            lineStream.Dispose();
        }

        #endregion

        #region . Extract .

        public static Span Extract(int begin, int end, string beginTag) {
            var type = beginTag.Substring(2);

            switch (type) {
                case "PER":
                    type = "person";
                    break;
                case "LOC":
                    type = "location";
                    break;
                case "MISC":
                    type = "misc";
                    break;
                case "ORG":
                    type = "organization";
                    break;
                default:
                    throw new InvalidFormatException("Unknown type: " + type);
            }
            return new Span(begin, end, type);
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public NameSample Read() {
            var sentence = new List<string>();
            var tags = new List<string>();

            var isClearAdaptiveData = false;

            // Empty line indicates end of sentence

            string line;
            while ((line = lineStream.Read()) != null && !string.IsNullOrWhiteSpace(line)) {
                if (lang == Language.NL && line.StartsWith(DOCSTART)) {
                    isClearAdaptiveData = true;
                    continue;
                }

                var fields = line.Split(' ');

                if (fields.Length == 3) {
                    sentence.Add(fields[0]);
                    tags.Add(fields[2]);
                } else {
                    throw new InvalidFormatException("Expected three fields per line in training data, got " +
                                                     fields.Length + " for line '" + line + "'!");
                }
            }

            // Always clear adaptive data for spanish
            if (lang == Language.ES)
                isClearAdaptiveData = true;

            if (sentence.Count > 0) {
                // convert name tags into spans
                var names = new List<Span>();

                var beginIndex = -1;
                var endIndex = -1;
                for (var i = 0; i < tags.Count; i++) {
                    var tag = tags[i];

                    if (tag.EndsWith("PER") && (types & GENERATE_PERSON_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("ORG") && (types & GENERATE_ORGANIZATION_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("LOC") && (types & GENERATE_LOCATION_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("MISC") && (types & GENERATE_MISC_ENTITIES) == 0)
                        tag = "O";

                    if (tag.StartsWith("B-")) {
                        if (beginIndex != -1) {
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                            //beginIndex = -1;
                            //endIndex = -1;
                        }

                        beginIndex = i;
                        endIndex = i + 1;
                    } else if (tag.StartsWith("I-")) {
                        endIndex++;
                    } else if (tag.Equals("O")) {
                        if (beginIndex != -1) {
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                            beginIndex = -1;
                            endIndex = -1;
                        }
                    } else {
                        throw new InvalidFormatException("Invalid tag: " + tag);
                    }
                }

                // if one span remains, create it here
                if (beginIndex != -1)
                    names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));

                return new NameSample(sentence.ToArray(), names.ToArray(), isClearAdaptiveData);
            }
            if (line != null) {
                // Just filter out empty events, if two lines in a row are empty
                return Read();
            }
            // source stream is not returning anymore lines
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
            lineStream.Reset();
        }

        #endregion

    }
}