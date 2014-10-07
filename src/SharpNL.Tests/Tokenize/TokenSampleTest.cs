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

using System;
using NUnit.Framework;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.Tokenize {
    [TestFixture]
    internal class TokenSampleTest {
        public static TokenSample createGoldSample() {
            return new TokenSample("A test.", new[] {
                new Span(0, 1),
                new Span(2, 6)
            });
        }

        public static TokenSample createPredSample() {
            return new TokenSample("A test.", new[] {
                new Span(0, 3),
                new Span(2, 6)
            });
        }

        [Test]
        public void testCreationWithDetokenizer() {
            var detokenizer = DictionaryDetokenizerTest.CreateLatinDetokenizer();

            var tokens = new[] {
                "start",
                "(", // move right
                ")", // move left
                "end",
                ".", // move left
                "hyphen",
                "-", // move both
                "string",
                "."
            };

            var a = new TokenSample(detokenizer, tokens);

            Assert.AreEqual("start () end. hyphen-string.", a.Text);
            //  0123456789012345678901234567
            Assert.AreEqual("start (" + TokenSample.DefaultSeparator + ") end" + TokenSample.DefaultSeparator + "."
                            + " hyphen" + TokenSample.DefaultSeparator + "-" + TokenSample.DefaultSeparator + "string" +
                            TokenSample.DefaultSeparator + ".", a.ToString());

            Assert.AreEqual(9, a.TokenSpans.Length);

            Assert.AreEqual(new Span(0, 5), a.TokenSpans[0]);
            Assert.AreEqual(new Span(6, 7), a.TokenSpans[1]);
            Assert.AreEqual(new Span(7, 8), a.TokenSpans[2]);
            Assert.AreEqual(new Span(9, 12), a.TokenSpans[3]);
            Assert.AreEqual(new Span(12, 13), a.TokenSpans[4]);

            Assert.AreEqual(new Span(14, 20), a.TokenSpans[5]);
            Assert.AreEqual(new Span(20, 21), a.TokenSpans[6]);
            Assert.AreEqual(new Span(21, 27), a.TokenSpans[7]);
            Assert.AreEqual(new Span(27, 28), a.TokenSpans[8]);
        }

        [Test]
        public void testEquals() {
            // ReSharper disable once EqualExpressionComparison
            // ReSharper disable once PossibleUnintendedReferenceComparison
            Assert.False(createGoldSample() == createGoldSample());
            Assert.AreEqual(createGoldSample(), createGoldSample());
            Assert.AreNotEqual(createPredSample(), createGoldSample());
            Assert.AreNotEqual(createPredSample(), new Object());
        }

        [Test]
        public void testRetrievingContent() {
            const string sentence = "A test";

            var sample = new TokenSample(sentence, new[] {new Span(0, 1), new Span(2, 6)});

            Assert.AreEqual("A test", sample.Text);

            Assert.AreEqual(new Span(0, 1), sample.TokenSpans[0]);
            Assert.AreEqual(new Span(2, 6), sample.TokenSpans[1]);
        }
    }
}