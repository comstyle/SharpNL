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

using System.IO;
using NUnit.Framework;
using SharpNL.Sentence;
using SharpNL.Sentence.Language;
using SharpNL.Utility;
using Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.Tests.Sentence {

    [TestFixture]
    public class SentenceDetectorFactoryTest {

        private static IObjectStream<SentenceSample> CreateSampleStream() {
            return new SentenceSampleStream(new PlainTextByLineStream(Tests.OpenSample("opennlp/tools/sentdetect/Sentences.txt")));
        }

        private static SentenceModel Train(SentenceDetectorFactory factory) {
            return SentenceDetectorME.Train("en", CreateSampleStream(), factory, TrainingParameters.DefaultParameters());
        }

        private static Dic LoadAbbDictionary() {
            return new Dic(Tests.OpenSample("opennlp/tools/sentdetect/abb.xml"));
        }

        [Test]
        public void TestDefault() {

            var dic = LoadAbbDictionary();

            Assert.NotNull(dic);

            char[] eos = {'.', '?'};
            var sdModel = Train(new SentenceDetectorFactory("en", true, dic, eos));

            Assert.NotNull(sdModel);

            SentenceDetectorFactory factory = sdModel.Factory;

            Assert.NotNull(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(eos.SequenceEqual(factory.EOSCharacters));

            var o = new MemoryStream();

            sdModel.Serialize(new UnclosableStream(o));

            o.Seek(0, SeekOrigin.Begin);

            var fromSerialized = new SentenceModel(o);

            factory = fromSerialized.Factory;
            Assert.NotNull(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(eos.SequenceEqual(factory.EOSCharacters));
        }

        [Test]
        public void TestNullDict() {
            char[] eos = { '.', '?' };
            var sdModel = Train(new SentenceDetectorFactory("en", true, null, eos));

            Assert.NotNull(sdModel);

            SentenceDetectorFactory factory = sdModel.Factory;

            Assert.Null(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(eos.SequenceEqual(factory.EOSCharacters));

            var o = new MemoryStream();

            sdModel.Serialize(new UnclosableStream(o));

            o.Seek(0, SeekOrigin.Begin);

            var fromSerialized = new SentenceModel(o);

            factory = fromSerialized.Factory;
            Assert.Null(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(eos.SequenceEqual(factory.EOSCharacters));
        }

        [Test]
        public void TestDefaultEOS() {
            var sdModel = Train(new SentenceDetectorFactory("en", true, null, null));

            Assert.NotNull(sdModel);

            SentenceDetectorFactory factory = sdModel.Factory;

            Assert.Null(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(factory.EOSCharacters.SequenceEqual(Factory.defaultEosCharacters));

            var o = new MemoryStream();

            sdModel.Serialize(new UnclosableStream(o));

            o.Seek(0, SeekOrigin.Begin);

            var fromSerialized = new SentenceModel(o);

            factory = fromSerialized.Factory;
            Assert.Null(factory.AbbreviationDictionary);
            Assert.True(factory.GetContextGenerator() is DefaultSentenceContextGenerator);
            Assert.True(factory.GetEndOfSentenceScanner() is DefaultEndOfSentenceScanner);
            Assert.True(factory.EOSCharacters.SequenceEqual(Factory.defaultEosCharacters));
        }

    }
}