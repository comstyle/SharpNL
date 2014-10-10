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
    public class Conll03NameSampleStreamTest {

        private const string ENGLISH_SAMPLE = "conll2003-en.sample";
        private const string GERMAN_SAMPLE = "conll2003-de.sample";

        private static IObjectStream<NameSample> OpenData(CoNLL03NameSampleStream.Language language, string name) {
            return new CoNLL03NameSampleStream(
                language,
                Tests.OpenSample("opennlp/tools/formats/" + name),
                CoNLL.Types.PersonEntities);
        }

        [Test]
        public void TestParsingEnglishSample() {
            var sampleStream = OpenData(CoNLL.Language.En, ENGLISH_SAMPLE);

            var personName = sampleStream.Read();
            Assert.NotNull(personName);

            Assert.AreEqual(9, personName.Sentence.Length);
            Assert.AreEqual(0, personName.Names.Length);
            Assert.AreEqual(true, personName.ClearAdaptiveData);

            personName = sampleStream.Read();

            Assert.NotNull(personName);

            Assert.AreEqual(2, personName.Sentence.Length);
            Assert.AreEqual(1, personName.Names.Length);
            Assert.AreEqual(false, personName.ClearAdaptiveData);

            Span nameSpan = personName.Names[0];
            Assert.AreEqual(0, nameSpan.Start);
            Assert.AreEqual(2, nameSpan.End);

            Assert.Null(sampleStream.Read());
        }

        [Test, ExpectedException(typeof(InvalidFormatException))]
        public void TestParsingEnglishSampleWithGermanAsLanguage() {
            var sampleStream = OpenData(CoNLL03NameSampleStream.Language.De, ENGLISH_SAMPLE);

            sampleStream.Read();
        }

        [Test, ExpectedException(typeof(InvalidFormatException))]
        public void TestParsingGermanSampleWithEnglishAsLanguage() {
            var sampleStream = OpenData(CoNLL03NameSampleStream.Language.En, GERMAN_SAMPLE);

            sampleStream.Read();
        }

        [Test]
        public void TestParsingGermanSample() {
            var sampleStream = OpenData(CoNLL03NameSampleStream.Language.De, GERMAN_SAMPLE);

            var personName = sampleStream.Read();
            Assert.NotNull(personName);

            Assert.AreEqual(5, personName.Sentence.Length);
            Assert.AreEqual(0, personName.Names.Length);
            Assert.AreEqual(true, personName.ClearAdaptiveData);
        }

        [Test]
        public void TestReset() {

            var sampleStream = OpenData(CoNLL03NameSampleStream.Language.De, GERMAN_SAMPLE);

            var sample = sampleStream.Read();

            sampleStream.Reset();

            Assert.AreEqual(sample, sampleStream.Read());
        }
    }
}