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

using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpNL.Formats.Ad;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    internal class AdNameSampleStreamTest {

        private List<NameSample> samples;

        [TestFixtureSetUp]
        public void TestNameSampleStream() {
            using (var file = Tests.OpenFile("opennlp/tools/formats/ad.sample")) {

                samples = new List<NameSample>();

                var sampleStream = new AdNameSampleStream(file, Encoding.UTF8, true, true);

                NameSample sample;
                while ((sample = sampleStream.Read()) != null) {
                    samples.Add(sample);
                }

            }
        }


        public void testSimpleCount() {
            Assert.AreEqual(AdParagraphStreamTest.NumSentences, samples.Count);
        }

        [Test]
        public void testCheckMergedContractions() {

            Assert.AreEqual("no", samples[0].Sentence[1]);
            Assert.AreEqual("no", samples[0].Sentence[11]);
            Assert.AreEqual("Com", samples[1].Sentence[0]);
            Assert.AreEqual("relação", samples[1].Sentence[1]);
            Assert.AreEqual("à", samples[1].Sentence[2]);
            Assert.AreEqual("mais", samples[2].Sentence[4]);
            Assert.AreEqual("de", samples[2].Sentence[5]);
            Assert.AreEqual("da", samples[2].Sentence[8]);
            Assert.AreEqual("num", samples[3].Sentence[26]);

        }

        [Test]
        public void testSize() {
            Assert.AreEqual(25, samples[0].Sentence.Length);
            Assert.AreEqual(12, samples[1].Sentence.Length);
            Assert.AreEqual(59, samples[2].Sentence.Length);
            Assert.AreEqual(33, samples[3].Sentence.Length);
        }

        [Test]
        public void testNames() {

            Assert.AreEqual(new Span(4, 7, "time"), samples[0].Names[0]);
            Assert.AreEqual(new Span(8, 10, "place"), samples[0].Names[1]);
            Assert.AreEqual(new Span(12, 14, "place"), samples[0].Names[2]);
            Assert.AreEqual(new Span(15, 17, "person"), samples[0].Names[3]);
            Assert.AreEqual(new Span(18, 19, "numeric"), samples[0].Names[4]);
            Assert.AreEqual(new Span(20, 22, "place"), samples[0].Names[5]);
            Assert.AreEqual(new Span(23, 24, "place"), samples[0].Names[6]);

            Assert.AreEqual(new Span(22, 24, "person"), samples[2].Names[0]); //    22..24
            Assert.AreEqual(new Span(25, 27, "person"), samples[2].Names[1]); //    25..27
            Assert.AreEqual(new Span(28, 30, "person"), samples[2].Names[2]); //    28..30
            Assert.AreEqual(new Span(31, 34, "person"), samples[2].Names[3]); //    31..34
            Assert.AreEqual(new Span(35, 37, "person"), samples[2].Names[4]); //    35..37
            Assert.AreEqual(new Span(38, 40, "person"), samples[2].Names[5]); //    38..40
            Assert.AreEqual(new Span(41, 43, "person"), samples[2].Names[6]); //    41..43
            Assert.AreEqual(new Span(44, 46, "person"), samples[2].Names[7]); //    44..46
            Assert.AreEqual(new Span(47, 49, "person"), samples[2].Names[8]); //    47..49
            Assert.AreEqual(new Span(50, 52, "person"), samples[2].Names[9]); //    50..52
            Assert.AreEqual(new Span(53, 55, "person"), samples[2].Names[10]); //    53..55

            Assert.AreEqual(new Span(0, 1, "place"), samples[3].Names[0]); //    0..1
            Assert.AreEqual(new Span(6, 7, "event"), samples[3].Names[1]); //    6..7
            Assert.AreEqual(new Span(15, 16, "organization"), samples[3].Names[2]); //    15..16
            Assert.AreEqual(new Span(18, 19, "event"), samples[3].Names[3]); //    18..19
            Assert.AreEqual(new Span(27, 28, "event"), samples[3].Names[4]); //    27..28
            Assert.AreEqual(new Span(29, 30, "event"), samples[3].Names[5]); //    29..30

            Assert.AreEqual(new Span(1, 6, "time"), samples[4].Names[0]); //    0..1

            Assert.AreEqual(new Span(0, 3, "person"), samples[5].Names[0]); //    0..1
        }

        [Test]
        public void testSmallSentence() {
            Assert.AreEqual(2, samples[6].Sentence.Length);
        }

        [Test]
        public void testMissingRightContraction() {
            Assert.AreEqual(new Span(0, 1, "person"), samples[7].Names[0]);
            Assert.AreEqual(new Span(3, 4, "person"), samples[7].Names[1]);
            Assert.AreEqual(new Span(5, 6, "person"), samples[7].Names[2]);
        }
    }
}