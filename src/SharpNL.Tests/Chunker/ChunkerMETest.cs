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
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
    [TestFixture]
    public class ChunkerMETest {

        private IChunker chunker;

        internal static readonly string[] toks1 = {
            "Rockwell", "said", "the", "agreement", "calls", "for",
            "it", "to", "supply", "200", "additional", "so-called", "shipsets",
            "for", "the", "planes", "."
        };

        internal static readonly string[] tags1 = {
            "NNP", "VBD", "DT", "NN", "VBZ", "IN", "PRP", "TO", "VB",
            "CD", "JJ", "JJ", "NNS", "IN", "DT", "NNS", "."
        };

        internal static readonly string[] expect1 = {
            "B-NP", "B-VP", "B-NP", "I-NP", "B-VP", "B-SBAR",
            "B-NP", "B-VP", "I-VP", "B-NP", "I-NP", "I-NP", "I-NP", "B-PP", "B-NP",
            "I-NP", "O"
        };

        internal static ChunkSampleStream CreateSampleStream() {
            return new ChunkSampleStream(new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/chunker/test.txt")));
        }

        [TestFixtureSetUp]
        public void Setup() {
            var p = new TrainingParameters();
            p.Set(Parameters.Iterations, "70");
            p.Set(Parameters.Cutoff, "1");

            var chunkerModel = ChunkerME.Train("en", CreateSampleStream(), p, new ChunkerFactory());

            chunker = new ChunkerME(chunkerModel);
        }


        [Test]
        public void TestChunkAsArray() {
            var preds = chunker.Chunk(toks1, tags1);

            Assert.True(expect1.SequenceEqual(preds));
        }

        [Test]
        public void TestChunkAsSpan() {
            Span[] preds = chunker.ChunkAsSpans(toks1, tags1);

            Assert.AreEqual(10, preds.Length);
            Assert.AreEqual(new Span(0, 1, "NP"), preds[0]);
            Assert.AreEqual(new Span(1, 2, "VP"), preds[1]);
            Assert.AreEqual(new Span(2, 4, "NP"), preds[2]);
            Assert.AreEqual(new Span(4, 5, "VP"), preds[3]);
            Assert.AreEqual(new Span(5, 6, "SBAR"), preds[4]);
            Assert.AreEqual(new Span(6, 7, "NP"), preds[5]);
            Assert.AreEqual(new Span(7, 9, "VP"), preds[6]);
            Assert.AreEqual(new Span(9, 13, "NP"), preds[7]);
            Assert.AreEqual(new Span(13, 14, "PP"), preds[8]);
            Assert.AreEqual(new Span(14, 16, "NP"), preds[9]);
        }

        [Test]
        public void TestTokenProbArray() {
            var preds = chunker.TopKSequences(toks1, tags1);
            Assert.True(preds.Length > 0);
            Assert.AreEqual(expect1.Length, preds[0].Probabilities.Count);
            Assert.True(expect1.SequenceEqual(preds[0].Outcomes));
            Assert.False(expect1.SequenceEqual(preds[1].Outcomes));

        }

        [Test]
        public void TestTokenProbMinScore() {
            var preds = chunker.TopKSequences(toks1, tags1, -5.55);

            Assert.AreEqual(4, preds.Length);
            Assert.AreEqual(expect1.Length, preds[0].Probabilities.Count);
            Assert.True(expect1.SequenceEqual(preds[0].Outcomes));
            Assert.False(expect1.SequenceEqual(preds[1].Outcomes));
        }

        [Test]
        public void TestTokenProbMinScoreOpenNLP() {

            var model = new ChunkerModel(Tests.OpenFile("opennlp/models/en-chunker.bin"));

            Assert.NotNull(model);

            var ckr = new ChunkerME(model);

            Assert.NotNull(ckr);

            var preds = chunker.TopKSequences(toks1, tags1, -5.55);

            Assert.AreEqual(4, preds.Length);
            Assert.AreEqual(expect1.Length, preds[0].Probabilities.Count);
            Assert.True(expect1.SequenceEqual(preds[0].Outcomes));
            Assert.False(expect1.SequenceEqual(preds[1].Outcomes));
            


        }

    }
}