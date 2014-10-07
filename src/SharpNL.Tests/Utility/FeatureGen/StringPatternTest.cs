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
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Tests.Utility.FeatureGen {
    [TestFixture]
    public class StringPatternTest {
        [Test]
        public void testContainsComma() {
            Assert.True(StringPattern.Recognize("test,").ContainsComma);
            Assert.True(StringPattern.Recognize("23,5").ContainsComma);
            Assert.False(StringPattern.Recognize("test./-1").ContainsComma);
        }

        [Test]
        public void testContainsDigit() {
            Assert.True(StringPattern.Recognize("test1").ContainsDigit);
            Assert.True(StringPattern.Recognize("23,5").ContainsDigit);
            Assert.False(StringPattern.Recognize("test./-,").ContainsDigit);
        }

        [Test]
        public void testContainsHyphen() {
            Assert.True(StringPattern.Recognize("test--").ContainsHyphen);
            Assert.True(StringPattern.Recognize("23-5").ContainsHyphen);
            Assert.False(StringPattern.Recognize("test.1/,").ContainsHyphen);
        }

        [Test]
        public void testContainsLetters() {
            Assert.True(StringPattern.Recognize("test--").ContainsLetters);
            Assert.True(StringPattern.Recognize("23h5ßm").ContainsLetters);
            Assert.False(StringPattern.Recognize("---.1/,").ContainsLetters);
        }

        [Test]
        public void testContainsPeriod() {
            Assert.True(StringPattern.Recognize("test.").ContainsPeriod);
            Assert.True(StringPattern.Recognize("23.5").ContainsPeriod);
            Assert.False(StringPattern.Recognize("test,/-1").ContainsPeriod);
        }

        [Test]
        public void testContainsSlash() {
            Assert.True(StringPattern.Recognize("test/").ContainsSlash);
            Assert.True(StringPattern.Recognize("23/5").ContainsSlash);
            Assert.False(StringPattern.Recognize("test.1-,").ContainsSlash);
        }

        [Test]
        public void testDigits() {
            Assert.AreEqual(6, StringPattern.Recognize("123456").Digits);
            Assert.AreEqual(3, StringPattern.Recognize("123fff").Digits);
            Assert.AreEqual(0, StringPattern.Recognize("test").Digits);
        }

        [Test]
        public void testIsAllCapitalLetter() {
            Assert.True(StringPattern.Recognize("TEST").AllCapitalLetter);
            Assert.True(StringPattern.Recognize("ÄÄÄÜÜÜÖÖÖÖ").AllCapitalLetter);
            Assert.False(StringPattern.Recognize("ÄÄÄÜÜÜÖÖä").AllCapitalLetter);
            Assert.False(StringPattern.Recognize("ÄÄÄÜÜdÜÖÖ").AllCapitalLetter);
        }

        [Test]
        public void testIsAllDigit() {
            Assert.True(StringPattern.Recognize("123456").AllDigit);
            Assert.False(StringPattern.Recognize("123,56").AllDigit);
            Assert.False(StringPattern.Recognize("12356f").AllDigit);
        }

        [Test]
        public void testIsAllLetters() {
            Assert.True(StringPattern.Recognize("test").AllLetter);
            Assert.True(StringPattern.Recognize("TEST").AllLetter);
            Assert.True(StringPattern.Recognize("TesT").AllLetter);
            Assert.True(StringPattern.Recognize("grün").AllLetter);
            Assert.True(StringPattern.Recognize("üäöæß").AllLetter);
        }

        [Test]
        public void testIsAllLowerCaseLetter() {
            Assert.True(StringPattern.Recognize("test").AllLowerCaseLetter);
            Assert.True(StringPattern.Recognize("öäü").AllLowerCaseLetter);
            Assert.True(StringPattern.Recognize("öäüßßß").AllLowerCaseLetter);
            Assert.False(StringPattern.Recognize("Test").AllLowerCaseLetter);
            Assert.False(StringPattern.Recognize("TEST").AllLowerCaseLetter);
            Assert.False(StringPattern.Recognize("testT").AllLowerCaseLetter);
            Assert.False(StringPattern.Recognize("tesÖt").AllLowerCaseLetter);
        }

        [Test]
        public void testIsInitialCapitalLetter() {
            Assert.True(StringPattern.Recognize("Test").InitialCapitalLetter);
            Assert.False(StringPattern.Recognize("tEST").InitialCapitalLetter);
            Assert.True(StringPattern.Recognize("TesT").InitialCapitalLetter);
            Assert.True(StringPattern.Recognize("Üäöæß").InitialCapitalLetter);
        }
    }
}