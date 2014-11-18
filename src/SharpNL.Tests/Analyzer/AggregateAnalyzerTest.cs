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
using SharpNL.Analyzer;
using SharpNL.Text;

namespace SharpNL.Tests.Analyzer {
    [TestFixture]
    internal class AggregateAnalyzerTest {

        private AggregateAnalyzer analyzer;

        [TestFixtureSetUp]
        public void Setup() {           
            // initialize the analyzer

            analyzer = new AggregateAnalyzer {
                Tests.GetFullPath("/opennlp/models/en-sent.bin"),
                Tests.GetFullPath("/opennlp/models/en-token.bin"),
                Tests.GetFullPath("/opennlp/models/en-ner-money.bin"), // en-ner-person don't detect Bart as a person :(
                Tests.GetFullPath("/opennlp/models/en-pos-maxent.bin"),
                Tests.GetFullPath("/opennlp/models/en-chunker.bin"),
                Tests.GetFullPath("/opennlp/models/en-parser-chunking.bin")
            };
        }

        [Test]
        public void TestEverything() {
            var doc = new Document("en", 
                "Bart, with $10,000, we'd be millionaires! We could buy all kinds of useful things like... love!");

            analyzer.Analyze(doc);

            Assert.NotNull(doc);
            Assert.AreEqual(2, doc.Sentences.Count);
            Assert.AreEqual(true, doc.IsTokenized);
            Assert.AreEqual(true, doc.IsTagged);
            Assert.AreEqual(true, doc.IsChunked);
            Assert.AreEqual(true, doc.IsParsed);
            Assert.AreEqual(1, doc.Sentences[0].Entities.Count);
        }
    }
}