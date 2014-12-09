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
using System.IO;
using NUnit.Framework;
using SharpNL.Chunker;
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
    [TestFixture]
    internal class ChunkerSampleTests {
        private static ChunkSample createGoldSample() {
            return new ChunkSample(createSentence(), createTags(), createChunks());
        }

        private static ChunkSample createPredSample() {
            var chunks = createChunks();
            chunks[5] = "B-NP";
            return new ChunkSample(createSentence(), createTags(), chunks);
        }


        private static string[] createSentence() {
            return new[] {
                "Forecasts",
                "for",
                "the",
                "trade",
                "figures",
                "range",
                "widely",
                ",",
                "Forecasts",
                "for",
                "the",
                "trade",
                "figures",
                "range",
                "widely",
                "."
            };
        }

        private static string[] createTags() {
            return new[] {
                "NNS",
                "IN",
                "DT",
                "NN",
                "NNS",
                "VBP",
                "RB",
                ",",
                "NNS",
                "IN",
                "DT",
                "NN",
                "NNS",
                "VBP",
                "RB",
                "."
            };
        }

        private static string[] createChunks() {
            return new[] {
                "B-NP",
                "B-PP",
                "B-NP",
                "I-NP",
                "I-NP",
                "B-VP",
                "B-ADVP",
                "O",
                "B-NP",
                "B-PP",
                "B-NP",
                "I-NP",
                "I-NP",
                "B-VP",
                "B-ADVP",
                "O"
            };
        }

        [Test]
        public void testAsSpan() {
            var sample = new ChunkSample(createSentence(), createTags(), createChunks());
            var spans = sample.GetPhrasesAsSpanList();

            Assert.AreEqual(10, spans.Length);
            Assert.AreEqual(new Span(0, 1, "NP"), spans[0]);
            Assert.AreEqual(new Span(1, 2, "PP"), spans[1]);
            Assert.AreEqual(new Span(2, 5, "NP"), spans[2]);
            Assert.AreEqual(new Span(5, 6, "VP"), spans[3]);
            Assert.AreEqual(new Span(6, 7, "ADVP"), spans[4]);
            Assert.AreEqual(new Span(8, 9, "NP"), spans[5]);
            Assert.AreEqual(new Span(9, 10, "PP"), spans[6]);
            Assert.AreEqual(new Span(10, 13, "NP"), spans[7]);
            Assert.AreEqual(new Span(13, 14, "VP"), spans[8]);
            Assert.AreEqual(new Span(14, 15, "ADVP"), spans[9]);
        }

        [Test]
        public void testEquals() {
            Assert.False(createGoldSample() == createGoldSample());
            Assert.True(createGoldSample().Equals(createGoldSample()));
            Assert.False(createPredSample().Equals(createGoldSample()));
            Assert.False(createPredSample().Equals(new Object()));
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void testInvalidPhraseAsSpan1() {
            ChunkSample.PhrasesAsSpanList(new string[2], new string[1], new string[1]);
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void testInvalidPhraseAsSpan2() {
            ChunkSample.PhrasesAsSpanList(new string[1], new string[2], new string[1]);
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void testInvalidPhraseAsSpan3() {
            ChunkSample.PhrasesAsSpanList(new string[1], new string[1], new string[2]);
        }

        [Test]
        public void testNicePrint() {
            var sample = new ChunkSample(createSentence(), createTags(), createChunks());

            Assert.AreEqual(" [NP Forecasts_NNS ] [PP for_IN ] [NP the_DT trade_NN figures_NNS ] " +
                            "[VP range_VBP ] [ADVP widely_RB ] ,_, [NP Forecasts_NNS ] [PP for_IN ] [NP the_DT trade_NN figures_NNS ] " +
                            "[VP range_VBP ] [ADVP widely_RB ] ._.", sample.NicePrint);
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void testParameterValidation() {
            var dummy = new ChunkSample(new[] {string.Empty}, new[] {string.Empty}, new[] {string.Empty, "Nop!"});

            Assert.Null(dummy);
        }

        [Test]
        public void testPhraseAsSpan() {
            var spans = ChunkSample.PhrasesAsSpanList(createSentence(), createTags(), createChunks());

            Assert.AreEqual(10, spans.Length);
            Assert.AreEqual(new Span(0, 1, "NP"), spans[0]);
            Assert.AreEqual(new Span(1, 2, "PP"), spans[1]);
            Assert.AreEqual(new Span(2, 5, "NP"), spans[2]);
            Assert.AreEqual(new Span(5, 6, "VP"), spans[3]);
            Assert.AreEqual(new Span(6, 7, "ADVP"), spans[4]);
            Assert.AreEqual(new Span(8, 9, "NP"), spans[5]);
            Assert.AreEqual(new Span(9, 10, "PP"), spans[6]);
            Assert.AreEqual(new Span(10, 13, "NP"), spans[7]);
            Assert.AreEqual(new Span(13, 14, "VP"), spans[8]);
            Assert.AreEqual(new Span(14, 15, "ADVP"), spans[9]);
        }

        [Test]
        public void testRetrievingContent() {
            var s = createSentence();
            var t = createTags();
            var c = createChunks();

            var sample = new ChunkSample(s, t, c);

            Assert.True(s.SequenceEqual(sample.Sentence));
            Assert.True(c.SequenceEqual(sample.Preds));
            Assert.True(t.SequenceEqual(sample.Tags));
        }

        [Test]
        public void testToString() {
            var s = createSentence();
            var t = createTags();
            var c = createChunks();

            var sample = new ChunkSample(s, t, c);

            var reader = new StringReader(sample.ToString());

            for (var i = 0; i < s.Length; i++) {
                var line = reader.ReadLine();
                var parts = line.Split(' ');

                Assert.AreEqual(3, parts.Length);
                Assert.AreEqual(s[i], parts[0]);
                Assert.AreEqual(t[i], parts[1]);
                Assert.AreEqual(c[i], parts[2]);
            }
        }
    }
}