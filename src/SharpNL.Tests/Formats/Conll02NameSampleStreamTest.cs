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
using SharpNL.Formats;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats {
    [TestFixture]
    public class Conll02NameSampleStreamTest {
        private static IObjectStream<NameSample> OpenData(string name) {
            return OpenData(CoNLL.Language.De, name);
        }

        private static IObjectStream<NameSample> OpenData(CoNLL.Language language, string name) {
            return new CoNLL02NameSampleStream(
                language,
                Tests.OpenFile("opennlp/tools/formats/" + name),
                CoNLL.Types.PersonEntities);
        }

        [Test]
        public void TestParsingSpanishSample() {
            var sampleStream = OpenData(CoNLL.Language.Es, "conll2002-es.sample");

            var personName = sampleStream.Read();

            Assert.NotNull(personName);

            Assert.AreEqual(5, personName.Sentence.Length);
            Assert.AreEqual(1, personName.Names.Length);
            Assert.AreEqual(true, personName.ClearAdaptiveData);

            var nameSpan = personName.Names[0];
            Assert.AreEqual(0, nameSpan.Start);
            Assert.AreEqual(4, nameSpan.End);
            Assert.AreEqual(true, personName.ClearAdaptiveData);

            Assert.AreEqual(0, sampleStream.Read().Names.Length);

            Assert.Null(sampleStream.Read());
        }

        [Test]
        public void TestParsingDutchSample() {
            var sampleStream = OpenData(CoNLL.Language.Nl, "conll2002-nl.sample");

            var personName = sampleStream.Read();

            Assert.AreEqual(0, personName.Names.Length);
            Assert.True(personName.ClearAdaptiveData);

            personName = sampleStream.Read();

            Assert.False(personName.ClearAdaptiveData);
            Assert.Null(sampleStream.Read());
        }

        [Test]
        public void TestReset() {
            var sampleStream = OpenData(CoNLL.Language.Nl, "conll2002-nl.sample");

            var sample = sampleStream.Read();

            sampleStream.Reset();

            Assert.AreEqual(sample, sampleStream.Read());
        }
    }
}