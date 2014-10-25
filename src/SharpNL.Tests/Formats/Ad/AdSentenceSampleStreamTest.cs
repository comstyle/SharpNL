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
using NUnit.Framework;
using SharpNL.Formats.Ad;
using SharpNL.Sentence;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    internal class AdSentenceSampleStreamTest {
        private List<SentenceSample> samples;

        [TestFixtureSetUp]
        public void Setup() {
            var file = Tests.OpenFile("/opennlp/tools/formats/ad.sample");

            try {
                samples = new List<SentenceSample>();

                var stream = new AdSentenceSampleStream(new PlainTextByLineStream(file), true, false);

                SentenceSample sample;
                while ((sample = stream.Read()) != null) {
                    samples.Add(sample);
                }
            } finally {
                if (file != null) {
                    file.Close();
                }
            }
        }

        [Test]
        public void testSentences() {
            Assert.NotNull(samples[0].Document);
            Assert.AreEqual(3, samples[0].Sentences.Length);
            Assert.AreEqual(new Span(0, 119), samples[0].Sentences[0]);
            Assert.AreEqual(new Span(120, 180), samples[0].Sentences[1]);
        }

        [Test]
        public void testSimpleCount() {
            Assert.AreEqual(5, samples.Count);
        }


        /*
        [Test, Ignore]
        public void testWithFile() {

            const string fileName = @"";

            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName)) {
                using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {

                    var stream = new AdSentenceSampleStream(new PlainTextByLineStream(file), true, false);
                    var list = new List<SentenceSample>();

                    SentenceSample sample;
                    while ((sample = stream.Read()) != null) {
                        list.Add(sample);
                    }

                    samples.Clear();
                }
            }
        }
        */
    }
}