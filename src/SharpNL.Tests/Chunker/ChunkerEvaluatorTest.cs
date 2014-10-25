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
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
    [TestFixture]
    public class ChunkerEvaluatorTest {
        private const double DELTA = 1.0E-9d;

        [Test]
        public void TestEvaluator() {
            using (
                var predictedSample =
                    new DummyChunkSampleStream(
                        new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/chunker/output.txt")), true))
            using (
                var expectedSample =
                    new DummyChunkSampleStream(
                        new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/chunker/output.txt")), false)) {
                var dummyChunker = new DummyChunker(predictedSample);

                //var stream = new ByteArrayOutputStream();
                //ChunkerEvaluationMonitor listener = new ChunkEvaluationErrorListener(stream);
                var evaluator = new ChunkerEvaluator(dummyChunker);

                evaluator.Evaluate(expectedSample);

                Assert.AreEqual(0.8d, evaluator.FMeasure.PrecisionScore, DELTA);
                Assert.AreEqual(0.875d, evaluator.FMeasure.RecallScore, DELTA);

                //assertNotSame(stream.toString().length(), 0);
            }
        }

        [Test]
        public void TestEvaluatorNoError() {
            using (
                var predictedSample =
                    new DummyChunkSampleStream(
                        new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/chunker/output.txt")), true))
            using (
                var expectedSample =
                    new DummyChunkSampleStream(
                        new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/chunker/output.txt")), true)) {
                var dummyChunker = new DummyChunker(predictedSample);

                var evaluator = new ChunkerEvaluator(dummyChunker);

                evaluator.Evaluate(expectedSample);

                Assert.AreEqual(1d, evaluator.FMeasure.PrecisionScore, DELTA);
                Assert.AreEqual(1d, evaluator.FMeasure.RecallScore, DELTA);
            }
        }
    }
}