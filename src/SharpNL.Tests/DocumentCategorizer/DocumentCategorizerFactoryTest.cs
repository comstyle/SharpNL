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
using SharpNL.DocumentCategorizer;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.DocumentCategorizer {
    [TestFixture]
    internal class DocumentCategorizerFactoryTest {

        private static IObjectStream<DocumentSample> CreateSampleStream() {
            return
                new DocumentSampleStream(
                    new PlainTextByLineStream(Tests.OpenSample("opennlp/tools/doccat/DoccatSample.txt")));
        }

        private static DocumentCategorizerModel Train(DocumentCategorizerFactory factory = null) {
            return DocumentCategorizerME.Train(
                "x-unspecified",
                CreateSampleStream(),
                TrainingParameters.DefaultParameters(),
                factory ?? new DocumentCategorizerFactory());
        }


        [Test]
        public void TestDefault() {
            var model = Train();

            Assert.NotNull(model);


            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var deserialized = new DocumentCategorizerModel(data);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Factory);

                Assert.AreEqual(1, deserialized.Factory.FeatureGenerators.Length);
                Assert.AreEqual(typeof (BagOfWordsFeatureGenerator), deserialized.Factory.FeatureGenerators[0].GetType());

                Assert.AreEqual(typeof (WhitespaceTokenizer), deserialized.Factory.Tokenizer.GetType());
            }
        }

        [Test]
        public void TestCustom() {
            var featureGenerators = new IFeatureGenerator[] {
                new BagOfWordsFeatureGenerator(),
                new NGramFeatureGenerator()
            };
            var factory = new DocumentCategorizerFactory(SimpleTokenizer.Instance, featureGenerators);
            var model = Train(factory);

            Assert.NotNull(model);

            using (var data = new MemoryStream()) {
                model.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                var deserialized = new DocumentCategorizerModel(data);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Factory);

                Assert.AreEqual(2, deserialized.Factory.FeatureGenerators.Length);
                Assert.AreEqual(typeof (BagOfWordsFeatureGenerator), deserialized.Factory.FeatureGenerators[0].GetType());
                Assert.AreEqual(typeof (NGramFeatureGenerator), deserialized.Factory.FeatureGenerators[1].GetType());

                Assert.AreEqual(typeof (SimpleTokenizer), deserialized.Factory.Tokenizer.GetType());
            }
        }
    }
}