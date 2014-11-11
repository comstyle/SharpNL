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

using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// The <see cref="NameSampleStream"/> class converts tagged strings
    /// provided by a sample stream to <see cref="NameSample"/> objects.
    /// It uses text that is is one-sentence per line and tokenized
    /// with names identified by <i>&lt;START&gt;</i> and <i>&lt;END&gt;</i> tags.
    /// </summary>
    public class NameSampleStream : FilterObjectStream<string, NameSample> {
        public const string START_TAG_PREFIX = "<START:";
        public const string START_TAG = "<START>";
        public const string END_TAG = "<END>";

        public NameSampleStream(IObjectStream<string> samples) : base(samples) {}

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override NameSample Read() {
            var token = Samples.Read();

            var isClearAdaptiveData = false;

            // An empty line indicates the begin of a new article
            // for which the adaptive data in the feature generators
            // must be cleared
            while (token != null && token.Trim().Length == 0) {
                isClearAdaptiveData = true;
                token = Samples.Read();
            }

            if (token != null) {
                return NameSample.Parse(token, isClearAdaptiveData);
            }

            return null;
        }
    }
}