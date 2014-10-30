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

using NUnit.Framework;
using SharpNL.SentenceDetector;
using SharpNL.Utility;

namespace SharpNL.Tests.Sentence {
    [TestFixture]
    internal class SentenceSampleTest {


        [Test]
        public void testRetrievingContent() {
            var sample = new SentenceSample("1. 2.", new Span(0, 2), new Span(3, 5));

            Assert.AreEqual("1. 2.", sample.Document);
            Assert.AreEqual(new Span(0, 2), sample.Sentences[0]);
            Assert.AreEqual(new Span(3, 5), sample.Sentences[1]);

        }

        [Test]
        public void testEquals() {
            Assert.False(CreateGoldSample() == CreateGoldSample());
            Assert.True(CreateGoldSample().Equals(CreateGoldSample()));
            Assert.False(CreatePredSample().Equals(CreateGoldSample()));
            Assert.False(CreatePredSample().Equals(new object()));
        }


        internal static SentenceSample CreateGoldSample() {
            return new SentenceSample("1. 2.", new Span(0, 2), new Span(3, 5));
        }

        internal static SentenceSample CreatePredSample() {
            return new SentenceSample("1. 2.", new Span(0, 1), new Span(4, 5));
        }

    }
}