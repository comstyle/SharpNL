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
using SharpNL.NameFind;
using SharpNL.Tokenize;

namespace SharpNL.Tests.NameFind {
    [TestFixture]
    internal class DictionaryNameFinderTest {

        private SharpNL.Dictionary.Dictionary dictionary;
        private ITokenNameFinder nameFinder;

        private static SharpNL.Dictionary.Dictionary CreateDictionary() {
            return new SharpNL.Dictionary.Dictionary {
                new []{ "Vanessa" },
                new []{ "Vanessa", "Williams" },
                new []{ "Max" },
                new []{ "Michael", "Jordan" }           
            };
        }

        [TestFixtureSetUp]
        public void Setup() {
            dictionary = CreateDictionary();
            nameFinder = new DictionaryNameFinder(dictionary);
        }

        [Test]
        public void TestSingleTokeNameAtSentenceStart() {

            const string sentence = "Max a b c d";

            var tokens = SimpleTokenizer.Instance.Tokenize(sentence);

            var names = nameFinder.Find(tokens);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(0, names[0].Start);
            Assert.AreEqual(1, names[0].End);
        }

        [Test]
        public void TestSingleTokeNameAtSentenceEnd() {

            const string sentence = "a b c Max";

            var tokens = SimpleTokenizer.Instance.Tokenize(sentence);

            var names = nameFinder.Find(tokens);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(3, names[0].Start);
            Assert.AreEqual(4, names[0].End);
        }

        [Test]
        public void TestLastMatchingTokenNameIsChosen() {

            var sentence = new[] {"a", "b", "c", "Vanessa"};
            var names = nameFinder.Find(sentence);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(3, names[0].Start);
            Assert.AreEqual(4, names[0].End);
        }

        [Test]
        public void TestLongerTokenNameIsPreferred() {

            var sentence = new[] { "a", "b", "c", "Vanessa", "Williams" };
            var names = nameFinder.Find(sentence);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(3, names[0].Start);
            Assert.AreEqual(5, names[0].End);
        }

        [Test]
        public void TestCaseSensitivity() {

            var sentence = new[] { "a", "b", "c", "vanessa", "williams" };
            var names = nameFinder.Find(sentence);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(3, names[0].Start);
            Assert.AreEqual(5, names[0].End);
        }


        [Test]
        public void TestCaseLongerEntry() {

            var sentence = new[] { "a", "b", "michael", "jordan" };
            var names = nameFinder.Find(sentence);

            Assert.AreEqual(1, names.Length);
            Assert.AreEqual(2, names[0].Length);
        }


    }
}