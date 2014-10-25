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
    public class AdContractionNameSampleStreamTest {
        private readonly List<NameSample> samples = new List<NameSample>();

        private static Span CreateSpan(int start, int end) {
            return new Span(start, end, "default");
        }

        [TestFixtureSetUp]
        public void Setup() {
            using (var file = Tests.OpenFile("opennlp/tools/formats/ad.sample")) {
                using (var stream = new AdContractionNameSampleStream(file, Encoding.UTF8, false)) {
                    NameSample sample;
                    while ((sample = stream.Read()) != null) {
                        samples.Add(sample);
                    }
                }
            }
        }

        [Test]
        public void TestSize() {
            Assert.AreEqual(8, samples.Count);

            Assert.AreEqual(21, samples[0].Sentence.Length);
            Assert.AreEqual(12, samples[1].Sentence.Length);
            Assert.AreEqual(46, samples[2].Sentence.Length);
            Assert.AreEqual(32, samples[3].Sentence.Length);
        }

        [Test]
        public void TestCheckMergedContractions() {

            Assert.AreEqual("no", samples[0].Sentence[1]);
            Assert.AreEqual("no", samples[0].Sentence[10]);
            Assert.AreEqual("Com", samples[1].Sentence[0]);

            Assert.AreEqual("mais_de", samples[2].Sentence[4]);
            Assert.AreEqual("da", samples[2].Sentence[7]);
            Assert.AreEqual("num", samples[3].Sentence[25]);
        }

        [Test]
        public void testAll() {

            Assert.AreEqual(2, samples[0].Names.Length);
            Assert.AreEqual(CreateSpan(1, 2), samples[0].Names[0]);
            Assert.AreEqual(CreateSpan(10, 11), samples[0].Names[1]);

            Assert.AreEqual(1, samples[1].Names.Length);
            Assert.AreEqual(CreateSpan(2, 3), samples[1].Names[0]);

            Assert.AreEqual(2, samples[2].Names.Length);
            Assert.AreEqual(CreateSpan(7, 8), samples[2].Names[0]);
            Assert.AreEqual(CreateSpan(9, 10), samples[2].Names[1]);

            Assert.AreEqual(2, samples[3].Names.Length);
            Assert.AreEqual(CreateSpan(25, 26), samples[3].Names[0]);
            Assert.AreEqual(CreateSpan(29, 30), samples[3].Names[1]);

        }
    }
}