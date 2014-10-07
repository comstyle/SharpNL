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

using System;
using System.IO;
using NUnit.Framework;
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
    [TestFixture]
    public class ChunkerFactoryTest {

        private static ChunkerModel TrainModel(ChunkerFactory factory) {
            return ChunkerME.Train("en", ChunkerMETest.CreateSampleStream(), TrainingParameters.DefaultParameters(), factory);
        }

        [Test]
        public void TestDefaultFactory() {

            var model = TrainModel(new ChunkerFactory());

            Assert.IsInstanceOf<DefaultChunkerContextGenerator>(model.Factory.GetContextGenerator());
            Assert.IsInstanceOf<DefaultChunkerSequenceValidator>(model.Factory.GetSequenceValidator());

            using (var stream = new MemoryStream()) {
                
                model.Serialize(new UnclosableStream(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new ChunkerModel(stream);

                Assert.IsInstanceOf<DefaultChunkerContextGenerator>(fromSerialized.Factory.GetContextGenerator());
                Assert.IsInstanceOf<DefaultChunkerSequenceValidator>(fromSerialized.Factory.GetSequenceValidator());

            }
        }

        [Test]
        public void TestDummyFactory() {

            var model = TrainModel(new DummyChunkerFactory());

            Assert.IsInstanceOf<DummyChunkerFactory>(model.Factory);
            Assert.IsInstanceOf<DummyChunkerFactory.DummyContextGenerator>(model.Factory.GetContextGenerator());
            Assert.IsInstanceOf<DummyChunkerFactory.DummySequenceValidator>(model.Factory.GetSequenceValidator());


            using (var stream = new MemoryStream()) {
                model.Serialize(new UnclosableStream(stream));
                stream.Seek(0, SeekOrigin.Begin);

                var fromSerialized = new ChunkerModel(stream);
                Assert.IsInstanceOf<DummyChunkerFactory>(model.Factory);
                Assert.IsInstanceOf<DummyChunkerFactory.DummyContextGenerator>(
                    fromSerialized.Factory.GetContextGenerator());
                Assert.IsInstanceOf<DummyChunkerFactory.DummySequenceValidator>(
                    fromSerialized.Factory.GetSequenceValidator());
            }

            var chunker = new ChunkerME(model);

            String[] toks1 = {
                "Rockwell", "said", "the", "agreement", "calls", "for",
                "it", "to", "supply", "200", "additional", "so-called", "shipsets",
                "for", "the", "planes", "."
            };

            String[] tags1 = {
                "NNP", "VBD", "DT", "NN", "VBZ", "IN", "PRP", "TO", "VB",
                "CD", "JJ", "JJ", "NNS", "IN", "DT", "NNS", "."
            };

            chunker.Chunk(toks1, tags1);
        }
    }
}