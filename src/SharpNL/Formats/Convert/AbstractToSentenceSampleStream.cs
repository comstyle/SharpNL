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
using SharpNL.Sentence;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Convert {
    public abstract class AbstractToSentenceSampleStream<T> : FilterObjectStream<T, SentenceSample> where T : class {
        private readonly int chunkSize;
        private readonly IDetokenizer detokenizer;

        protected AbstractToSentenceSampleStream(IDetokenizer detokenizer, IObjectStream<T> samples, int chunkSize)
            : base(samples) {
            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            if (chunkSize < 0)
                throw new ArgumentOutOfRangeException("chunkSize", chunkSize, @"chunkSize must be zero or larger");

            this.detokenizer = detokenizer;
            this.chunkSize = chunkSize > 0 ? chunkSize : int.MaxValue;
        }

        protected abstract string[] ToSentence(T sample);

        public override SentenceSample Read() {
            var sentences = new List<string[]>();


            T posSample;
            var chunks = 0;
            while ((posSample = Samples.Read()) != null && chunks < chunkSize) {
                sentences.Add(ToSentence(posSample));
                chunks++;
            }

            if (sentences.Count > 0)
                return new SentenceSample(detokenizer, sentences.ToArray());
            if (posSample != null)
                return Read(); // filter out empty line

            return null; // last sample was read
        }
    }
}