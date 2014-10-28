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

using System.Text;
using NUnit.Framework;
using SharpNL.Parser;

namespace SharpNL.Tests.Parser {
    [TestFixture]
    public class ParseTest {
        internal const string ParseString =
            "(TOP  (S (S (NP-SBJ (PRP She)  )(VP (VBD was)  (ADVP (RB just)  )(NP-PRD (NP (DT another)  (NN freighter)  )(PP (IN from)  (NP (DT the)  (NNPS States)  )))))(, ,)  (CC and) (S (NP-SBJ (PRP she)  )(VP (VBD seemed)  (ADJP-PRD (ADJP (RB as)  (JJ commonplace)  )(PP (IN as)  (NP (PRP$ her)  (NN name)  )))))(. .)  ))";

        [Test]
        public void TestParse() {
            var p = Parse.ParseParse(ParseString);

            Assert.NotNull(p);
        }

        [Test]
        public void TestToHashCode() {
            var p = Parse.ParseParse(ParseString);

            Assert.NotNull(p.GetHashCode());
        }

        [Test]
        public void testEquals() {
            var p1 = Parse.ParseParse(ParseString);
            // ReSharper disable once EqualExpressionComparison
            Assert.True(p1.Equals(p1));
        }

        [Test]
        public void testGetTagNodes() {
            var p = Parse.ParseParse(ParseString);

            var tags = p.GetTagNodes();

            foreach (var node in tags) {
                Assert.True(node.IsPosTag);
            }

            Assert.AreEqual("PRP", tags[0].Type);
            Assert.AreEqual("VBD", tags[1].Type);
            Assert.AreEqual("RB", tags[2].Type);
            Assert.AreEqual("DT", tags[3].Type);
            Assert.AreEqual("NN", tags[4].Type);
            Assert.AreEqual("IN", tags[5].Type);
            Assert.AreEqual("DT", tags[6].Type);
            Assert.AreEqual("NNPS", tags[7].Type);
            Assert.AreEqual(",", tags[8].Type);
            Assert.AreEqual("CC", tags[9].Type);
            Assert.AreEqual("PRP", tags[10].Type);
            Assert.AreEqual("VBD", tags[11].Type);
            Assert.AreEqual("RB", tags[12].Type);
            Assert.AreEqual("JJ", tags[13].Type);
            Assert.AreEqual("IN", tags[14].Type);
            Assert.AreEqual("PRP$", tags[15].Type);
            Assert.AreEqual("NN", tags[16].Type);
            Assert.AreEqual(".", tags[17].Type);
        }

        [Test]
        public void testGetText() {
            var p = Parse.ParseParse(ParseString);

            // TODO: Why does parse attaches a space to the end of the text ???
            const string expectedText =
                "She was just another freighter from the States , and she seemed as commonplace as her name . ";

            Assert.AreEqual(expectedText, p.Text);
        }

        [Test]
        public void testParseClone() {
            var p1 = Parse.ParseParse(ParseString);
            var p2 = (Parse) p1.Clone();
            Assert.True(p1.Equals(p2));
            Assert.True(p2.Equals(p1));
        }

        [Test]
        public void testShow() {

            // Show method was removed, now the result of the show is returned by the ToString method.

            var p1 = Parse.ParseParse(ParseString);

            var p2 = Parse.ParseParse(p1.ToString());

            Assert.AreEqual(p1, p2);
        }

        [Test]
        public void testToString() {
            var p1 = Parse.ParseParse(ParseString);
            Assert.IsNotEmpty(p1.ToString());
        }

        [Test]
        public void testTokenReplacement() {
            var p1 = Parse.ParseParse("(TOP  (S-CLF (NP-SBJ (PRP It)  )(VP (VBD was) " +
                                      " (NP-PRD (NP (DT the)  (NN trial)  )(PP (IN of) " +
                                      " (NP (NP (NN oleomargarine)  (NN heir)  )(NP (NNP Minot) " +
                                      " (PRN (-LRB- -LRB-) (NNP Mickey) " +
                                      " (-RRB- -RRB-) )(NNP Jelke)  )))(PP (IN for) " +
                                      " (NP (JJ compulsory)  (NN prostitution) " +
                                      " ))(PP-LOC (IN in)  (NP (NNP New)  (NNP York) " +
                                      " )))(SBAR (WHNP-1 (WDT that)  )(S (VP (VBD put) " +
                                      " (NP (DT the)  (NN spotlight)  )(PP (IN on)  (NP (DT the) " +
                                      " (JJ international)  (NN play-girl)  ))))))(. .)  ))");

            var p2 = Parse.ParseParse(p1.ToString());

            Assert.AreEqual(p1, p2);
        }
    }
}