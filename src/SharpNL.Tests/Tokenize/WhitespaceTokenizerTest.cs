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
using SharpNL.Tokenize;

namespace SharpNL.Tests.Tokenize {
    [TestFixture]
    internal class WhitespaceTokenizerTest {
        [Test]
        public void TestOneToken() {
            Assert.AreEqual("one", WhitespaceTokenizer.Instance.Tokenize("one")[0]);
            Assert.AreEqual("one", WhitespaceTokenizer.Instance.Tokenize(" one")[0]);
            Assert.AreEqual("one", WhitespaceTokenizer.Instance.Tokenize("one ")[0]);
        }

        [Test]
        public void TestTokenizationOfStringWithoutTokens() {
            Assert.AreEqual(0, WhitespaceTokenizer.Instance.Tokenize("").Length); // empty
            Assert.AreEqual(0, WhitespaceTokenizer.Instance.Tokenize(" ").Length); // space
            Assert.AreEqual(0, WhitespaceTokenizer.Instance.Tokenize("\t").Length); // tab
            Assert.AreEqual(0, WhitespaceTokenizer.Instance.Tokenize("     ").Length);
        }

        /// <summary>
        /// Tests if it can tokenize whitespace separated tokens.
        /// </summary>
        [Test]
        public void TestWhitespaceTokenization() {
            const string text = "a b c  d     e                f    ";

            var tokenizedText = WhitespaceTokenizer.Instance.Tokenize(text);

            Assert.AreEqual("a", tokenizedText[0]);
            Assert.AreEqual("b", tokenizedText[1]);
            Assert.AreEqual("c", tokenizedText[2]);
            Assert.AreEqual("d", tokenizedText[3]);
            Assert.AreEqual("e", tokenizedText[4]);
            Assert.AreEqual("f", tokenizedText[5]);

            Assert.True(tokenizedText.Length == 6);
        }
    }
}