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

namespace SharpNL.Formats.OntoNotes {
    /// <summary>
    /// Reads a plain text file and return each line as a <see cref="string"/> object.
    /// </summary>
    public class DocumentToLineStream : SegmentedObjectStream<string, string> {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentedObjectStream{S, T}"/> class.
        /// </summary>
        /// <param name="samples">The samples to be segmented.</param>
        public DocumentToLineStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Reads the segments of the specified sample.
        /// </summary>
        /// <param name="sample">The sample.</param>
        /// <returns>The segments of the <paramref name="sample"/> object.</returns>
        protected override List<string> Read(string sample) {
            if (sample == null)
                return null;

            return new List<string>(sample.Split(new []{ '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)) {
                string.Empty // documents must be empty line terminated
            };           
        }
    }
}