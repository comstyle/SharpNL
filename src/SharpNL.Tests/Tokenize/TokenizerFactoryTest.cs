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
using SharpNL.Tokenize;
using SharpNL.Tokenize.Language;
using SharpNL.Utility;
using Dict = SharpNL.Dictionary.Dictionary;

namespace SharpNL.Tests.Tokenize {
    [TestFixture]
    internal class TokenizerFactoryTest {
        private static IObjectStream<TokenSample> CreateSampleStream() {
            return
                new TokenSampleStream(new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/tokenize/token.train")));
        }

        private static TokenizerModel Train(TokenizerFactory factory) {
            return TokenizerME.Train(CreateSampleStream(), factory, TrainingParameters.DefaultParameters());
        }

        private static Dict LoadAbbDictionary() {
            using (var data = Tests.OpenFile("opennlp/tools/sentdetect/abb.xml")) return new Dict(data);
        }

        [Test]
        public void TestDefault() {
            var dict = LoadAbbDictionary();
            const string lang = "es";

            var model = Train(new TokenizerFactory(lang, dict, false, null));

            var factory = model.Factory;

            Assert.IsInstanceOf(typeof (Dict), factory.AbbreviationDictionary);
            Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

            Assert.AreEqual(Factory.DefaultAlphanumeric, factory.AlphaNumericPattern);
            Assert.AreEqual(lang, factory.LanguageCode);
            Assert.AreEqual(lang, model.Language);

            Assert.AreEqual(false, factory.UseAlphaNumericOptimization);

            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new TokenizerModel(data);

                factory = fromSerialized.Factory;

                Assert.IsInstanceOf(typeof (Dict), factory.AbbreviationDictionary);
                Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

                Assert.AreEqual(Factory.DefaultAlphanumeric, factory.AlphaNumericPattern);
                Assert.AreEqual(lang, factory.LanguageCode);
                Assert.AreEqual(lang, fromSerialized.Language);

                Assert.AreEqual(false, factory.UseAlphaNumericOptimization);
            }
        }

        [Test]
        public void TestNullDict() {
            const string lang = "es";

            var model = Train(new TokenizerFactory(lang, null, false, null));

            var factory = model.Factory;

            Assert.IsNull(factory.AbbreviationDictionary);
            Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

            Assert.AreEqual(Factory.DefaultAlphanumeric, factory.AlphaNumericPattern);
            Assert.AreEqual(lang, factory.LanguageCode);
            Assert.AreEqual(lang, model.Language);

            Assert.AreEqual(false, factory.UseAlphaNumericOptimization);

            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new TokenizerModel(data);

                factory = fromSerialized.Factory;

                Assert.IsNull(factory.AbbreviationDictionary);
                Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

                Assert.AreEqual(Factory.DefaultAlphanumeric, factory.AlphaNumericPattern);
                Assert.AreEqual(lang, factory.LanguageCode);
                Assert.AreEqual(lang, fromSerialized.Language);

                Assert.AreEqual(false, factory.UseAlphaNumericOptimization);
            }
        }

        [Test]
        public void TestCustomPatternAndAlphaOpt() {
            const string lang = "es";

            const string pattern = "^[0-9A-Za-z]+$";

            var model = Train(new TokenizerFactory(lang, null, true, pattern));

            var factory = model.Factory;

            Assert.IsNull(factory.AbbreviationDictionary);
            Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

            Assert.AreEqual(pattern, factory.AlphaNumericPattern);
            Assert.AreEqual(lang, factory.LanguageCode);
            Assert.AreEqual(lang, model.Language);

            Assert.AreEqual(true, factory.UseAlphaNumericOptimization);

            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new TokenizerModel(data);

                factory = fromSerialized.Factory;

                Assert.IsNull(factory.AbbreviationDictionary);
                Assert.IsInstanceOf(typeof (DefaultTokenContextGenerator), factory.ContextGenerator);

                Assert.AreEqual(pattern, factory.AlphaNumericPattern);
                Assert.AreEqual(lang, factory.LanguageCode);
                Assert.AreEqual(lang, fromSerialized.Language);

                Assert.AreEqual(true, factory.UseAlphaNumericOptimization);
            }
        }

        [Test]
        public void TestDummyFactory() {

            const string lang = "es";
            const string pattern = "^[0-9A-Za-z]+$";

            var dic = LoadAbbDictionary();

            var model = Train(new DummyTokenizerFactory(lang, dic, true, pattern));

            Assert.IsInstanceOf(typeof(DummyTokenizerFactory), model.Factory);

            var factory = model.Factory;

            Assert.IsInstanceOf(typeof(DummyTokenizerFactory.DummyDictionary), factory.AbbreviationDictionary);
            Assert.IsInstanceOf(typeof(DummyTokenizerFactory.DummyContextGenerator), factory.ContextGenerator);

            Assert.AreEqual(pattern, factory.AlphaNumericPattern);
            Assert.AreEqual(lang, factory.LanguageCode);
            Assert.AreEqual(lang, model.Language);
            Assert.AreEqual(true, factory.UseAlphaNumericOptimization);

            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new TokenizerModel(data);

                Assert.IsInstanceOf(typeof(DummyTokenizerFactory), fromSerialized.Factory);

                factory = fromSerialized.Factory;

                Assert.IsInstanceOf(typeof(DummyTokenizerFactory.DummyDictionary), factory.AbbreviationDictionary);
                Assert.IsInstanceOf(typeof(DummyTokenizerFactory.DummyContextGenerator), factory.ContextGenerator);

                Assert.AreEqual(pattern, factory.AlphaNumericPattern);
                Assert.AreEqual(lang, factory.LanguageCode);
                Assert.AreEqual(lang, fromSerialized.Language);
                Assert.AreEqual(true, factory.UseAlphaNumericOptimization);
            }
        }

    }
}