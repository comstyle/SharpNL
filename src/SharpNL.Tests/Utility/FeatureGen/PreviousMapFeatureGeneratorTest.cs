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
using NUnit.Framework;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Tests.Utility.FeatureGen {
    [TestFixture]
    internal class PreviousMapFeatureGeneratorTest {
        [Test]
        public void TestFeatureGeneration() {
            var fg = new PreviousMapFeatureGenerator();

            var sentence = new[] {"a", "b", "c"};

            var features = new List<string>();

            // this should generate the pd=null feature
            fg.CreateFeatures(features, sentence, 0, null);
            Assert.AreEqual(1, features.Count);
            Assert.AreEqual("pd=null", features[0]);

            features.Clear();

            // this should generate the pd=1 feature
            fg.UpdateAdaptiveData(sentence, new[] {"1", "2", "3"});
            fg.CreateFeatures(features, sentence, 0, null);
            Assert.AreEqual(1, features.Count);
            Assert.AreEqual("pd=1", features[0]);

            features.Clear();

            // this should generate the pd=null feature again after
            // the adaptive data was cleared
            fg.ClearAdaptiveData();
            fg.CreateFeatures(features, sentence, 0, null);
            Assert.AreEqual(1, features.Count);
            Assert.AreEqual("pd=null", features[0]);
        }
    }
}