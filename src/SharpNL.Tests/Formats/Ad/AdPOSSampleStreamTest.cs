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
using SharpNL.Formats.Ad;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    internal class AdPosSampleStreamTest {

        [Test]
        public void TestExpandME() {
            // add one sentence with expandME = true
            var stream = new AdPosSampleStream(
                new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/formats/ad.sample")), true, false, false);

            stream.Read();

            var sample = stream.Read();

            Assert.AreEqual(13, sample.Sentence.Length);

            Assert.AreEqual("Com", sample.Sentence[0]);
            Assert.AreEqual("B-prp", sample.Tags[0]);

            Assert.AreEqual("relação", sample.Sentence[1]);
            Assert.AreEqual("I-prp", sample.Tags[1]);

            Assert.AreEqual("a", sample.Sentence[2]);
            Assert.AreEqual("I-prp", sample.Tags[2]);

            Assert.AreEqual("a", sample.Sentence[3]);
            Assert.AreEqual("artf", sample.Tags[3]);

            Assert.AreEqual("que", sample.Sentence[11]);
            Assert.AreEqual("conj-s", sample.Tags[11]);
        }

        [Test]
        public void TestSimple() {
            var stream = new AdPosSampleStream(
                new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/formats/ad.sample")), false, false, false);

            var sample = stream.Read();

            Assert.AreEqual(23, sample.Sentence.Length);

            Assert.AreEqual("Inicia", sample.Sentence[0]);
            Assert.AreEqual("v-fin", sample.Tags[0]);

            Assert.AreEqual("em", sample.Sentence[1]);
            Assert.AreEqual("prp", sample.Tags[1]);

            Assert.AreEqual("o", sample.Sentence[2]);
            Assert.AreEqual("artm", sample.Tags[2]);

            Assert.AreEqual("Porto_Poesia", sample.Sentence[9]);
            Assert.AreEqual("prop", sample.Tags[9]);
        }

        [Test]
        public void TestIncludeFeats() {
            // add one sentence with includeFeats = true
            var stream = new AdPosSampleStream(
                new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/formats/ad.sample")), false, true, false);

            var sample = stream.Read();

            Assert.AreEqual(23, sample.Sentence.Length);

            Assert.AreEqual("Inicia", sample.Sentence[0]);
            Assert.AreEqual("v-fin=PR=3S=IND=VFIN", sample.Tags[0]);

            Assert.AreEqual("em", sample.Sentence[1]);
            Assert.AreEqual("prp", sample.Tags[1]);

            Assert.AreEqual("o", sample.Sentence[2]);
            Assert.AreEqual("art=DET=M=S", sample.Tags[2]);

            Assert.AreEqual("Porto_Poesia", sample.Sentence[9]);
            Assert.AreEqual("prop=M=S", sample.Tags[9]);
        }
    }
}