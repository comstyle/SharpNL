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
    internal class CachedFeatureGeneratorTest {

        private IAdaptiveFeatureGenerator[] identityGenerator = {
            new IdentityFeatureGenerator()
        };

        private string[] testSentence1;

        private string[] testSentence2;

        private List<string> features;

        [TestFixtureSetUp]
        public void Setup() {
            testSentence1 = new[] { "a1", "b1", "c1", "d1" };

            testSentence2 = new[] { "a2", "b2", "c2", "d2" };

            features = new List<string>();
        }

        [Test]
        public void TestCachingOfSentence() {

            var generator = new CachedFeatureGenerator(identityGenerator);

            const int testIndex = 0;
            const int testIndex2 = testIndex + 1;

            // after this call features are cached for testIndex
            generator.CreateFeatures(features, testSentence1, testIndex, null);

            Assert.AreEqual(1, generator.NumberOfCacheMisses);
            Assert.AreEqual(0, generator.NumberOfCacheHits);

            Assert.True(features.Contains(testSentence1[testIndex]));

            features.Clear();

            // check if features are really cached

            var expectedToken = testSentence1[testIndex];

            testSentence1[testIndex] = null;

            generator.CreateFeatures(features, testSentence1, testIndex, null);

            Assert.AreEqual(1, generator.NumberOfCacheMisses);
            Assert.AreEqual(1, generator.NumberOfCacheHits);

            Assert.True(features.Contains(expectedToken));

            Assert.AreEqual(1, features.Count);

            features.Clear();

            // try caching with an other index

            generator.CreateFeatures(features, testSentence1, testIndex2, null);

            Assert.AreEqual(2, generator.NumberOfCacheMisses);
            Assert.AreEqual(1, generator.NumberOfCacheHits);

            Assert.True(features.Contains(testSentence1[testIndex2]));

            features.Clear();

            // now check if cache still contains feature for testIndex

            generator.CreateFeatures(features, testSentence1, testIndex, null);

            Assert.True(features.Contains(expectedToken));

        }

        [Test]
        public void TestCacheClearAfterSentenceChange() {

            var generator = new CachedFeatureGenerator(identityGenerator);

            const int testIndex = 0;

            // use generator with sentence 1
            generator.CreateFeatures(features, testSentence1, testIndex, null);

            features.Clear();

            // use another sentence but same index
            generator.CreateFeatures(features, testSentence2, testIndex, null);

            Assert.AreEqual(2, generator.NumberOfCacheMisses);
            Assert.AreEqual(0, generator.NumberOfCacheHits);

            Assert.True(features.Contains(testSentence2[testIndex]));

            Assert.AreEqual(1, features.Count);

            features.Clear();

            // check if features are really cached
            var expectedToken = testSentence2[testIndex];

            testSentence2[testIndex] = null;

            generator.CreateFeatures(features, testSentence2, testIndex, null);

            Assert.True(features.Contains(expectedToken));

            Assert.AreEqual(1, features.Count);

        }

    }
}