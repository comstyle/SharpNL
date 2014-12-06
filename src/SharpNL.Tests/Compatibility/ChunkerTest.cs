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
using SharpNL.Tests.Chunker;
using SharpNL.Utility;

namespace SharpNL.Tests.Compatibility {
    [TestFixture]
    public class ChunkerTest {
        private static opennlp.tools.chunker.ChunkSampleStream JavaSampleStream() {
            return new opennlp.tools.chunker.ChunkSampleStream(
                new opennlp.tools.util.PlainTextByLineStream(
                    OpenNLP.OpenInputStream("opennlp/tools/chunker/test.txt"), "utf-8"));
        }

        private IChunker sChunker;
        private opennlp.tools.chunker.Chunker jChunker;

        [TestFixtureSetUp]
        public void Setup() {
            var sParams = new TrainingParameters();
            sParams.Set(Parameters.Iterations, "70");
            sParams.Set(Parameters.Cutoff, "1");

            var jParams = new opennlp.tools.util.TrainingParameters();
            jParams.put("Iterations", "70");
            jParams.put("Cutoff", "1");

            var sModel = ChunkerME.Train("en", ChunkerMETest.CreateSampleStream(), sParams, new ChunkerFactory());

            var jModel = opennlp.tools.chunker.ChunkerME.train("en", JavaSampleStream(), jParams,
                new opennlp.tools.chunker.ChunkerFactory());

            Assert.NotNull(sModel);
            Assert.NotNull(jModel);

            sChunker = new ChunkerME(sModel);
            jChunker = new opennlp.tools.chunker.ChunkerME(jModel);
        }



        [Test]
        public void TestChunkAsArray() {
            var sPreds = sChunker.Chunk(ChunkerMETest.toks1, ChunkerMETest.tags1);
            var jPreds = jChunker.chunk(ChunkerMETest.toks1, ChunkerMETest.tags1);
            
            Assert.AreEqual(sPreds.Length, jPreds.Length);

            for (var i = 0; i < jPreds.Length; i++) {
                Assert.AreEqual(jPreds[i], sPreds[i]);
            }
        }

        [Test]
        public void TestTokenProbArray() {
            var sTop = sChunker.TopKSequences(ChunkerMETest.toks1, ChunkerMETest.tags1);
            var jTop = jChunker.topKSequences(ChunkerMETest.toks1, ChunkerMETest.tags1);

            Assert.AreEqual(jTop.Length, sTop.Length);

            for (var i = 0; i < jTop.Length; i++) {

                var jOut = jTop[i].getOutcomes();
                var jProbs = jTop[i].getProbs();

                Assert.AreEqual(jOut.size(), sTop[i].Outcomes.Count);

                for (var j = 0; j < jOut.size(); j++) {
                    Assert.AreEqual(jOut.get(j), sTop[i].Outcomes[j]);
                    Assert.AreEqual(jProbs[j], sTop[i].Probabilities[j], 0.0000000001d);
                }

                Assert.AreEqual(jTop[i].getScore(), sTop[i].Score, 0.0000000001d);
            }
        }

        [Test]
        public void TestTokenProbMinScore() {
            var sTop = sChunker.TopKSequences(ChunkerMETest.toks1, ChunkerMETest.tags1, -5.55);
            var jTop = jChunker.topKSequences(ChunkerMETest.toks1, ChunkerMETest.tags1, -5.55);

            Assert.AreEqual(jTop.Length, sTop.Length);

            for (var i = 0; i < jTop.Length; i++) {

                var jOut = jTop[i].getOutcomes();
                var jProbs = jTop[i].getProbs();

                Assert.AreEqual(jOut.size(), sTop[i].Outcomes.Count);

                for (var j = 0; j < jOut.size(); j++) {
                    Assert.AreEqual(jOut.get(j), sTop[i].Outcomes[j]);
                    Assert.AreEqual(jProbs[j], sTop[i].Probabilities[j], 0.0000000001d);
                }

                Assert.AreEqual(jTop[i].getScore(), sTop[i].Score, 0.0000000001d);
            }
        }
    }
}