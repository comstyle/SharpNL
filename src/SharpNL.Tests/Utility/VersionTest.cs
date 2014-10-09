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
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    public class VersionTest {

        [Test]
        public void TestParse() {
            var version = Library.OpenNLPVersion;

            Assert.AreEqual(version, Version.Parse(version.ToString()));
            Assert.AreEqual(new Version(1, 5, 2, false), Version.Parse("1.5.2-incubating"));
            Assert.AreEqual(new Version(1, 5, 2, false), Version.Parse("1.5.2"));
        }

        [Test]
        public void TestParseSnapshot() {
            Assert.AreEqual(new Version(1, 5, 2, true), Version.Parse("1.5.2-incubating-SNAPSHOT"));
            Assert.AreEqual(new Version(1, 5, 2, true), Version.Parse("1.5.2-SNAPSHOT"));
        }

        [TestCase("1.5"), TestCase("1.5."), TestCase("a.b.c")]
        [ExpectedException(typeof(InvalidFormatException))]
        public void TestInvalidVersion(string text) {
            var nop = Version.Parse(text);
            Assert.True(false);
        }


    }
}