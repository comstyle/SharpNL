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
using SharpNL.Chunker;
using SharpNL.Formats.Ad;

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    public class AdChunkSampleStreamTest {

        private readonly List<ChunkSample> samples = new List<ChunkSample>();

        [TestFixtureSetUp]
        public void Setup() {
            using (var file = Tests.OpenFile("opennlp/tools/formats/ad.sample")) {
                using (var stream = new AdChunkSampleStream(file, Encoding.UTF8, false)) {
                    ChunkSample sample;
                    while ((sample = stream.Read()) != null) {
                        samples.Add(sample);
                    }
                }
            }
        }

        [Test]
        public void TestChunks() {

            Assert.AreEqual("Inicia", samples[0].Sentence[0]);
            Assert.AreEqual("v-fin", samples[0].Tags[0]);
            Assert.AreEqual("B-VP", samples[0].Preds[0]);

            Assert.AreEqual("em", samples[0].Sentence[1]);
            Assert.AreEqual("prp", samples[0].Tags[1]);
            Assert.AreEqual("B-PP", samples[0].Preds[1]);

            Assert.AreEqual("o", samples[0].Sentence[2]);
            Assert.AreEqual("art", samples[0].Tags[2]);
            Assert.AreEqual("B-NP", samples[0].Preds[2]);

            Assert.AreEqual("próximo", samples[0].Sentence[3]);
            Assert.AreEqual("adj", samples[0].Tags[3]);
            Assert.AreEqual("I-NP", samples[0].Preds[3]);

            Assert.AreEqual("Casas", samples[3].Sentence[0]);
            Assert.AreEqual("n", samples[3].Tags[0]);
            Assert.AreEqual("B-NP", samples[3].Preds[0]);

        }

    }
}