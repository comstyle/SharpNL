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
using SharpNL.Formats.Convert;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    internal class AdTokenSampleStreamTest {

        private const string sampleFile = "opennlp/tools/formats/ad.sample";

        private List<TokenSample> samplesFromConvert;
        private List<TokenSample> samplesFromStream;

        [TestFixtureSetUp]
        public void Setup() {
            var dict = new DetokenizationDictionary(Tests.OpenFile("opennlp/tools/tokenize/latin-detokenizer.xml"));
            var stream = new NameToTokenSampleStream(
                new DictionaryDetokenizer(dict),
                new AdNameSampleStream(Tests.OpenFile(sampleFile), Encoding.UTF8, true, false));

            samplesFromConvert = new List<TokenSample>();

            TokenSample sample;

            while ((sample = stream.Read()) != null) {
                samplesFromConvert.Add(sample);               
            }

            samplesFromStream = new List<TokenSample>();

            var sampleStream = new AdTokenSampleStream(
                new PlainTextByLineStream(Tests.OpenFile(sampleFile)), 
                new DictionaryDetokenizer(dict),
                true, false);


            while ((sample = sampleStream.Read()) != null) {
                samplesFromStream.Add(sample);
            }
        }

        [Test]
        public void TestSimpleCount() {           
            Assert.AreEqual(AdParagraphStreamTest.NumSentences, samplesFromConvert.Count);

            Assert.AreEqual(samplesFromConvert.Count, samplesFromStream.Count);
        }

        [Test]
        public void TestSentences() {
            Assert.True(samplesFromConvert[5].Text.Contains("ofereceu-me"));
        }

        [Test]
        public void TestSamplesFromStream() {
            Assert.AreEqual(samplesFromConvert.Count, samplesFromStream.Count);

            for (var i = 0; i < samplesFromConvert.Count; i++) {
                Assert.AreEqual(samplesFromConvert[i], samplesFromStream[i]);
            }          
        }


    }
}