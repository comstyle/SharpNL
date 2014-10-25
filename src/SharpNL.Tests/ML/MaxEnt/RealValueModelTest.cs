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
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.Model;

namespace SharpNL.Tests.ML.MaxEnt {
    [TestFixture]
    internal class RealValueModelTest {

        [Test]
        public void TestRealValuedWeightsVsRepeatWeighting() {
            var rvfes1 = new RealValueFileEventStream(Tests.OpenFile("opennlp/data/maxent/real-valued-weights-training-data.txt"));
            var rvfes2 = new FileEventStream(Tests.OpenFile("opennlp/data/maxent/repeat-weighting-training-data.txt"));

            var realModel = GIS.TrainModel(100, new OnePassRealValueDataIndexer(rvfes1, 1));
            var repeatModel = GIS.TrainModel(100, new OnePassRealValueDataIndexer(rvfes2, 1));

            rvfes1.Dispose();
            rvfes2.Dispose();

            Assert.NotNull(realModel);
            Assert.NotNull(repeatModel);

            var features2Classify = new[] {"feature2", "feature5"};
            var realResults = realModel.Eval(features2Classify);
            var repeatResults = repeatModel.Eval(features2Classify);

            Assert.AreEqual(realResults.Length, repeatResults.Length);
            
            for (var i = 0; i < realResults.Length; i++) {
                Assert.AreEqual(realResults[i], repeatResults[i], 0.01f);
            }

            features2Classify = new[] {"feature1", "feature2", "feature3", "feature4", "feature5"};
            realResults = realModel.Eval(features2Classify, new[] {5.5f, 6.1f, 9.1f, 4.0f, 1.8f});
            repeatResults = repeatModel.Eval(features2Classify, new[] {5.5f, 6.1f, 9.1f, 4.0f, 1.8f});

            Assert.AreEqual(realResults.Length, repeatResults.Length);
            for (var i = 0; i < realResults.Length; i++) {
                Assert.AreEqual(realResults[i], repeatResults[i], 0.01f);
            }
        }
    }
}