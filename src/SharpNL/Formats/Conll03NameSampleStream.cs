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
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// An import stream which can parse the CoNLL03 data.
    /// </summary>
    public class CoNLL03NameSampleStream : CoNLL, IObjectStream<NameSample> {

        private readonly Language language;
        private readonly IObjectStream<string> lineStream;
        private readonly Types types;

        private const string DocStart = "-DOCSTART-";

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL03NameSampleStream"/> class.
        /// </summary>
        /// <param name="language">The language of the data. The valid languages are: En, De</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="types">The types to be readed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">language</exception>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        /// <exception cref="System.ArgumentException">The specified language is not supported.</exception>
        public CoNLL03NameSampleStream(Language language, IObjectStream<string> lineStream, Types types) {
            if (!Enum.IsDefined(typeof(Language), language))
                throw new ArgumentOutOfRangeException("language");

            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            if (!language.In(Language.En, Language.De))
                throw new ArgumentException("The specified language is not supported.");


            this.language = language;
            this.lineStream = lineStream;
            this.types = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL03NameSampleStream" /> class.
        /// </summary>
        /// <param name="language">The language of the data. The valid languages are: En, De</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="types">The types to be readed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">language</exception>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        /// <exception cref="ArgumentException">The stream is not readable.</exception>
        /// <exception cref="System.ArgumentException">The specified language is not supported.</exception>
        public CoNLL03NameSampleStream(Language language, Stream inputStream, Types types) {
            if (!Enum.IsDefined(typeof(Language), language))
                throw new ArgumentOutOfRangeException("language");

            if (!Enum.IsDefined(typeof(Types), types))
                throw new ArgumentOutOfRangeException("types");

            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (inputStream.CanRead)
                throw new ArgumentException("The stream is not readable.");

            if (!language.In(Language.En, Language.De))
                throw new ArgumentException("The specified language is not supported.");

            this.language = language;
            lineStream = new PlainTextByLineStream(inputStream);
            this.types = types;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoNLL03NameSampleStream" /> class.
        /// </summary>
        /// <param name="language">The language of the data. The valid languages are: En, De</param>
        /// <param name="streamFactory">The stream factory.</param>
        /// <param name="types">The types to be readed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">language</exception>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        /// <exception cref="System.ArgumentException">The specified language is not supported.</exception>
        public CoNLL03NameSampleStream(Language language, IInputStreamFactory streamFactory, Types types)
            : this(language, new PlainTextByLineStream(streamFactory), types) {
            
        }
        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            lineStream.Dispose();
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
                if (line.StartsWith(DocStart)) {
                    isClearAdaptiveData = true;

                    line = lineStream.Read();
                    if (!string.IsNullOrEmpty(line))
                        throw new InvalidFormatException("Empty line after -DOCSTART- not empty: '" + line + "'!");

                    continue;
                }

                var fields = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                
                if (language == Language.En && fields.Length == 4) {
                    // For English: WORD  POS-TAG SC-TAG NE-TAG
                    sentence.Add(fields[0]);
                    tags.Add(fields[3]);
                } else if (language == Language.De && fields.Length == 5) {
                    // For German: WORD  LEMA-TAG POS-TAG SC-TAG NE-TAG
                    sentence.Add(fields[0]);
                    tags.Add(fields[4]);
                } else {
                    throw new InvalidFormatException(
                        string.Format("Incorrect number of fields per line for language: '{0}'!", line));
                }
            }

            if (sentence.Count > 0) {
                // convert name tags into spans
                var names = new List<Span>();

                var beginIndex = -1;
                var endIndex = -1;
                for (var i = 0; i < tags.Count; i++) {
                    var tag = tags[i];

                    if (tag.EndsWith("PER") && (types & Types.PersonEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("ORG") && (types & Types.OrganizationEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("LOC") && (types & Types.LocationEntities) == 0)
                        tag = "O";

                    if (tag.EndsWith("MISC") && (types & Types.MiscEntities) == 0)
                        tag = "O";

                    if (tag == "O") {
                        if (beginIndex == -1) 
                            continue;

                        names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                        beginIndex = -1;
                        endIndex = -1;
                    } else if (tag.StartsWith("B-")) {
                        // B- prefix means we have two same entities next to each other
                        if (beginIndex != -1) {
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                        }
                        beginIndex = i;
                        endIndex = i + 1;

                    } else if (tag.StartsWith("I-")) {
                        // I- starts or continues a current name entity
                        if (beginIndex == -1) {
                            beginIndex = i;
                            endIndex = i + 1;
                        } else if (!tag.EndsWith(tags[beginIndex].Substring(1))) {
                            // we have a new tag type following a tagged word series
                            // also may not have the same I- starting the previous!
                            names.Add(Extract(beginIndex, endIndex, tags[beginIndex]));
                            beginIndex = i;
                            endIndex = i + 1;
                        } else {
                            endIndex++;
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