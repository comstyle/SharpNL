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

namespace SharpNL.Tests.Formats.Ad {
    [TestFixture]
    internal class AdTokenSampleStreamTest {

        private List<TokenSample> samples;

        [TestFixtureSetUp]
        public void Setup() {
            var dict = new DetokenizationDictionary(Tests.OpenFile("opennlp/tools/tokenize/latin-detokenizer.xml"));
            var stream = new NameToTokenSampleStream(
                new DictionaryDetokenizer(dict),
                new AdNameSampleStream(Tests.OpenFile("opennlp/tools/formats/ad.sample"), Encoding.UTF8, true, false));

            samples = new List<TokenSample>();

            TokenSample sample;
            while ((sample = stream.Read()) != null) {
                samples.Add(sample);               
            }
        }

        [Test]
        public void TestSimpleCount() {           
            Assert.AreEqual(AdParagraphStreamTest.NumSentences, samples.Count);
        }

        [Test]
        public void TestSentences() {
            Assert.True(samples[5].Text.Contains("ofereceu-me"));
        }


    }
}