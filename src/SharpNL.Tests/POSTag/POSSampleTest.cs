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
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Tests.POSTag {
    [TestFixture]
    public class POSSampleTest {
        internal static POSSample CreateGoldSample() {
            const string sentence = "the_DT stories_NNS about_IN well-heeled_JJ " +
                                    "communities_NNS and_CC developers_NNS";
            return POSSample.Parse(sentence);
        }

        internal static POSSample CreatePredSample() {
            const string sentence = "the_DT stories_NNS about_NNS well-heeled_JJ " +
                                    "communities_NNS and_CC developers_CC";
            return POSSample.Parse(sentence);
        }

        [Test]
        public void TestEquals() {
            // ReSharper disable once EqualExpressionComparison
            // ReSharper disable once PossibleUnintendedReferenceComparison
            Assert.False(CreateGoldSample() == CreateGoldSample());
            Assert.True(CreateGoldSample().Equals(CreateGoldSample()));
            Assert.False(CreateGoldSample().Equals(CreatePredSample()));
            Assert.False(CreateGoldSample().Equals(new object()));
        }

        [Test]
        public void TestParse() {
            const string sentence = "the_DT stories_NNS about_IN well-heeled_JJ " +
                                    "communities_NNS and_CC developers_NNS";

            var sample = POSSample.Parse(sentence);

            Assert.AreEqual(sentence, sample.ToString());
        }

        [Test]
        public void TestParseEmptyString() {
            var sample = POSSample.Parse(string.Empty);

            Assert.AreEqual(0, sample.Sentence.Length);
            Assert.AreEqual(0, sample.Tags.Length);

            var value = sample.ToString();

            Assert.IsEmpty(value);
        }

        [Test]
        public void TestParseEmptyTag() {
            var sample = POSSample.Parse("the_DT stories_");

            Assert.AreEqual(string.Empty, sample.Tags[1]);
        }

        [Test]
        public void TestParseEmptyToken() {
            var sample = POSSample.Parse("the_DT _NNS");

            Assert.AreEqual(string.Empty, sample.Sentence[1]);
        }

        [Test, ExpectedException(typeof (InvalidFormatException))]
        public void TestParseWithError() {
            POSSample.Parse("the_DT stories");
        }
    }
}