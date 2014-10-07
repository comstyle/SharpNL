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

using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Convert {

    internal class NameToSentenceSampleStream : AbstractToSentenceSampleStream<NameSample> {
        public NameToSentenceSampleStream(IDetokenizer detokenizer, IObjectStream<NameSample> samples, int chunkSize)
            : base(detokenizer, samples, chunkSize) {
            
        }
        protected override string[] ToSentence(NameSample sample) {
            return sample.Sentence;
        }
    }
}