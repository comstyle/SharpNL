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
using System.Text.RegularExpressions;
using NUnit.Framework;
using SharpNL.NameFind;

namespace SharpNL.Tests.NameFind {
    /// <summary>
    /// Tests for <see cref="RegexNameFinder"/> class.
    /// </summary>
    [TestFixture]
    internal class RegexNameFinderTest {

        [Test]
        public void TestFindSingleTokenPattern() {
            var testPattern = new Regex("test", RegexOptions.Compiled);
            var sentence = new[] {"a", "test", "b", "c"};

            var patterns = new[] {testPattern};
            var regexMap = new Dictionary<string, Regex[]>();
            const string type = "testtype";

            regexMap.Add(type, patterns);

            var finder = new RegexNameFinder(regexMap);

            var result = finder.Find(sentence);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Start);
            Assert.AreEqual(2, result[0].End);
            Assert.AreEqual(type, result[0].Type);
        }

        [Test]
        public void TestFindTokenizedPattern() {
            var finder = new RegexNameFinder(new Dictionary<string, Regex[]> {
                {"match", new[] {new Regex("[0-9]+ year")}}
            });

            var sentence = new[] { "a", "80", "year", "b", "c" };

            var result = finder.Find(sentence);

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(1, result[0].Start);
            Assert.AreEqual(3, result[0].End);
            Assert.AreEqual("match", result[0].Type);
        }

        [Test]
        public void TestFindMatchingPatternWithoutMatchingTokenBounds() {
            var finder = new RegexNameFinder(new Dictionary<string, Regex[]> {
                {"match", new[] {new Regex("[0-8] year")}}
            });

            var sentence = new[] { "a", "80", "year", "c" };

            var result = finder.Find(sentence);

            Assert.AreEqual(0, result.Length);
        }

    }
}