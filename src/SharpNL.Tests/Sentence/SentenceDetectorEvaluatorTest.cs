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
    public class SentenceDetectorEvaluatorTest {
        private class DummySD : ISentenceDetector {
            private readonly SentenceSample sample;

            public DummySD(SentenceSample sample) {
                this.sample = sample;
            }

            public string[] SentDetect(string text) {
                return null;
            }

            public Span[] SentPosDetect(string text) {
                return sample.Sentences;
            }
        }

        [Test]
        public void TestPositive() {
            var eval = new SentenceDetectorEvaluator(new DummySD(SentenceSampleTest.CreateGoldSample()));
            var stream = new CollectionObjectStream<SentenceSample>(
                new[] {SentenceSampleTest.CreateGoldSample()});

            eval.Evaluate(stream);

            Assert.AreEqual(1, eval.FMeasure.Value);
        }

        [Test]
        public void TestNegative() {
            var eval = new SentenceDetectorEvaluator(new DummySD(SentenceSampleTest.CreateGoldSample()));
            var stream = new CollectionObjectStream<SentenceSample>(
                new[] { SentenceSampleTest.CreatePredSample() });

            eval.Evaluate(stream);

            Assert.AreEqual(-1, eval.FMeasure.Value);
        }

    }
}