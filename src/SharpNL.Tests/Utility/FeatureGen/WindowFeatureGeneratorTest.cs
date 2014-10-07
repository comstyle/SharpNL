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
    internal class WindowFeatureGeneratorTest {
        private readonly string[] testSentence = {
            "a", "b", "c", "d",
            "e", "f", "g", "h"
        };

        /// <summary>
        /// Tests if the <see cref="WindowFeatureGenerator"/> works as specified, with a previous
        /// and next window size of zero.
        /// </summary>
        [Test]
        public void TestWithoutWindow() {
            var windowFeatureGenerator = new WindowFeatureGenerator(
                new IdentityFeatureGenerator(), 0, 0);

            const int testTokenIndex = 2;

            var features = new List<string>();

            windowFeatureGenerator.CreateFeatures(features, testSentence, testTokenIndex, null);

            Assert.AreEqual(1, features.Count);

            Assert.AreEqual(features[0], testSentence[testTokenIndex]);
        }

        [Test]
        public void TestWindowSizeOne() {
            var windowFeatureGenerator = new WindowFeatureGenerator(
                new IdentityFeatureGenerator(), 1, 1);

            const int testTokenIndex = 2;

            var features = new List<string>();

            windowFeatureGenerator.CreateFeatures(features, testSentence, testTokenIndex, null);

            Assert.AreEqual(3, features.Count);
        }

        [Test]
        public void TestWindowAtBeginOfSentence() {
            var windowFeatureGenerator = new WindowFeatureGenerator(new IdentityFeatureGenerator(), 1, 0);

            const int testTokenIndex = 0;

            var features = new List<string>();

            windowFeatureGenerator.CreateFeatures(features, testSentence, testTokenIndex, null);

            Assert.AreEqual(1, features.Count);

            Assert.AreEqual(features[0], testSentence[testTokenIndex]);
        }

        [Test]
        public void TestWindowAtEndOfSentence() {
            var windowFeatureGenerator = new WindowFeatureGenerator(
                new IdentityFeatureGenerator(), 0, 1);

            var testTokenIndex = testSentence.Length - 1;

            var features = new List<string>();

            windowFeatureGenerator.CreateFeatures(features, testSentence, testTokenIndex, null);

            Assert.AreEqual(1, features.Count);

            Assert.AreEqual(features[0], testSentence[testTokenIndex]);
        }

        /// <summary>
        /// Tests for a window size of previous and next 2 if the features are correct.
        /// </summary>
        [Test]
        public void TestForCorrectFeatures() {
            var windowFeatureGenerator = new WindowFeatureGenerator(
                new IdentityFeatureGenerator(), 2, 2);

            const int testTokenIndex = 3;

            var features = new List<string>();

            windowFeatureGenerator.CreateFeatures(features, testSentence, testTokenIndex, null);

            Assert.AreEqual(5, features.Count);

            Assert.True(features.Contains(WindowFeatureGenerator.PREV_PREFIX + "2" +
                                          testSentence[testTokenIndex - 2]));
            Assert.True(features.Contains(WindowFeatureGenerator.PREV_PREFIX + "1" +
                                          testSentence[testTokenIndex - 1]));

            Assert.True(features.Contains(testSentence[testTokenIndex]));

            Assert.True(features.Contains(WindowFeatureGenerator.NEXT_PREFIX + "1" +
                                          testSentence[testTokenIndex + 1]));
            Assert.True(features.Contains(WindowFeatureGenerator.NEXT_PREFIX + "2" +
                                          testSentence[testTokenIndex + 2]));
        }
    }
}