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
using SharpNL.ML.Model;
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Tests.POSTag {
    [TestFixture]
    public class POSModelTest {
        [Test]
        public void TestPOSModelSerializationMaxent() {
            var posModel = POSTaggerMETest.TrainPOSModel();

            using (var stream = new MemoryStream()) {
                posModel.Serialize(new UnclosableStream(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var recreated = new POSModel(stream);

                Assert.AreEqual(posModel.Language, recreated.Language);
                Assert.AreEqual(posModel.Manifest, recreated.Manifest);
                Assert.AreEqual(posModel.PosSequenceModel.GetType(), recreated.PosSequenceModel.GetType());
                Assert.AreEqual(posModel.Factory.GetType(), recreated.Factory.GetType());
            }
        }

        [Test]
        public void TestPOSModelSerializationPerceptron() {
            var posModel = POSTaggerMETest.TrainPOSModel(ModelType.Perceptron);

            using (var stream = new MemoryStream()) {
                posModel.Serialize(new UnclosableStream(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var recreated = new POSModel(stream);

                Assert.AreEqual(posModel.Language, recreated.Language);
                Assert.AreEqual(posModel.Manifest, recreated.Manifest);
                Assert.AreEqual(posModel.PosSequenceModel.GetType(), recreated.PosSequenceModel.GetType());
                Assert.AreEqual(posModel.Factory.GetType(), recreated.Factory.GetType());

            }
        }
    }
}