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
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// Parser for the training files of the BioNLP/NLPBA 2004 shared task.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The data contains five named entity types: DNA, RNA, protein, cell_type and cell_line.
    /// </para>
    /// 
    /// Data can be found on this web site:<br />
    /// <see href="http://www-tsujii.is.s.u-tokyo.ac.jp/GENIA/ERtask/report.html" /> 
    /// </remarks>
    public class BioNLP2004NameSampleStream : IObjectStream<NameSample> {

        internal const int GENERATE_DNA_ENTITIES = 0x01;
        internal const int GENERATE_PROTEIN_ENTITIES = 0x01 << 1;
        internal const int GENERATE_CELLTYPE_ENTITIES = 0x01 << 2;
        internal const int GENERATE_CELLLINE_ENTITIES = 0x01 << 3;
        internal const int GENERATE_RNA_ENTITIES = 0x01 << 4;

        private readonly int types;

        private readonly IObjectStream<string> lineStream;

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="BioNLP2004NameSampleStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="types">The types.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.ArgumentException">The input stream was not readable.</exception>
        public BioNLP2004NameSampleStream(Stream inputStream, int types) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException(@"The input stream was not readable.", "inputStream");

            lineStream = new PlainTextByLineStream(inputStream, Encoding.UTF8);
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
                if (line.StartsWith("###MEDLINE:")) {
                    isClearAdaptiveData = true;
                    lineStream.Read();
                    continue;
                }

                if (line.Contains("ABSTRACT TRUNCATED"))
                    continue;

                var fields = line.Split('\t');

                if (fields.Length == 2) {
                    sentence.Add(fields[0]);
                    tags.Add(fields[1]);
                } else {
                    throw new InvalidFormatException("Expected two fields per line in training data, got " +
                                                     fields.Length + " for line '" + line + "'!");
                }
            }

            if (sentence.Count > 0) {
                // convert name tags into spans
                var names = new List<Span>();

                var beginIndex = -1;
                var endIndex = -1;
                for (var i = 0; i < tags.Count; i++) {
                    var tag = tags[i];

                    if (tag.EndsWith("DNA") && (types & GENERATE_DNA_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("protein") && (types & GENERATE_PROTEIN_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("cell_type") && (types & GENERATE_CELLTYPE_ENTITIES) == 0)
                        tag = "O";

                    if (tag.EndsWith("cell_line") && (types & GENERATE_CELLTYPE_ENTITIES) == 0)
                        tag = "O";
                    if (tag.EndsWith("RNA") && (types & GENERATE_RNA_ENTITIES) == 0)
                        tag = "O";

                    if (tag.StartsWith("B-")) {
                        if (beginIndex != -1) {
                            names.Add(new Span(beginIndex, endIndex, tags[beginIndex].Substring(2)));
                            //beginIndex = -1;
                            //endIndex = -1;
                        }

                        beginIndex = i;
                        endIndex = i + 1;
                    } else if (tag.StartsWith("I-")) {
                        endIndex++;
                    } else if (tag.Equals("O")) {
                        if (beginIndex != -1) {
                            names.Add(new Span(beginIndex, endIndex, tags[beginIndex].Substring(2)));
                            beginIndex = -1;
                            endIndex = -1;
                        }
                    } else {
                        throw new IOException("Invalid tag: " + tag);
                    }
                }

                // if one span remains, create it here
                if (beginIndex != -1)
                    names.Add(new Span(beginIndex, endIndex, tags[beginIndex].Substring(2)));

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