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

using System.IO;
using NUnit.Framework;
using SharpNL.Formats.Ptb;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ptb {

    [TestFixture]
    internal class PtbStreamReaderTest {

        private static bool CheckFormat(PtbNode node) {
            if (node == null)
                return false;

            var str = node.ToString();
            var open = 0;
            var close = 0;
            foreach (var chr in str) {
                switch (chr) {
                    case '(': open++; continue;
                    case ')': close++; continue;
                }
            }
            return (open - close) == 0;
        }

        [Test]
        public void ParseTest() {

            var reader = new PtbStreamReader("en",
                new PlainTextByLineStream(
                    new StringReader(Parser.ParseTest.ParseString)), true, null);

            var node = reader.Read();

            Assert.NotNull(node);

            Assert.AreEqual(1, node.Children.Count);

            Assert.AreEqual(true, node.Children[0].HasChildren);

            Assert.AreEqual(5, node.Children[0].Children.Count);
        }


        [Test]
        public void ParseSampleFile() {
            var reader = new PtbStreamReader("x-unspecified", 
                new PlainTextByLineStream(Tests.OpenFile("sharpnl/formats/ptb/sample.ptb")), true, null);

            var p1 = reader.Read();
            var p2 = reader.Read();
            var p3 = reader.Read();
            var p4 = reader.Read();
            var p5 = reader.Read();
            var p6 = reader.Read();
            var p7 = reader.Read();

            Assert.True(CheckFormat(p1));
            Assert.True(CheckFormat(p2));
            Assert.True(CheckFormat(p3));
            Assert.True(CheckFormat(p4));
            Assert.True(CheckFormat(p5));
            Assert.True(CheckFormat(p6));           
            Assert.IsNull(p7);

            Assert.AreEqual("(STA+fcl (SUBJ+np (>N+art O)(>N+adj próximo)(H+n adversário))(P+v-fin será)(SC+np (>N+art o)(«)(H+n artista)(»)(N<+prop Hicham_Arazi)(N<+fcl (SUBJ+pron-indp que)(P+v-fin garantiu)(ACC+np (>N+adj nova)(H+n presença)(N</ADVL[-1]+pp (H+prp em)(P<+np (>N+art os)(H+n quartos-de-final)(N<+pp (H+prp de)(P<+np (>N+art o)(H+prop Grand_Slam)(N<+adj francês))))))))(.))", p1.ToString());
            Assert.AreEqual("(TOP (S (S (NP-SBJ (PRP She))(VP (VBD was)(ADVP (RB just))(NP-PRD (NP (DT another)(NN freighter))(PP (IN from)(NP (DT the)(NNPS States))))))(, ,)(CC and)(S (NP-SBJ (PRP she))(VP (VBD seemed)(ADJP-PRD (ADJP (RB as)(JJ commonplace))(PP (IN as)(NP (PRP$ her)(NN name))))))(. .)))", p2.ToString());
        }

        [Test]
        public void ParseSemiInvalidFile() {
            var reader = new PtbStreamReader("en",
                new PlainTextByLineStream(Tests.OpenFile("sharpnl/formats/ptb/invalid.ptb")), true, null);

            var p1 = reader.Read();
            var p2 = reader.Read();
            var p3 = reader.Read();

            Assert.True(CheckFormat(p1));
            Assert.True(CheckFormat(p2));
            Assert.IsNull(p3);

            Assert.AreEqual("one", p1.Type);
            Assert.AreEqual("three", p2.Type);
        }

        [Test]
        public void NodeSpanTest() {
            var reader = new PtbStreamReader("pt", new PlainTextByLineStream(Tests.OpenFile("sharpnl/formats/ptb/sample.ptb")), true, null);

            PtbNode node;
            while ((node = reader.Read()) != null) {
                var text = string.Join(" ", node.Tokens);

                CheckSpan(text, node);
            }

        }

        private static void CheckSpan(string text, PtbNode node) {
            var check = string.Join(" ", node.Tokens);
            
            Assert.AreEqual(text.Substring(node.Span.Start, node.Span.End - node.Span.Start), check);

            if (!node.HasChildren)
                return;

            foreach (var child in node.Children) {
                CheckSpan(text, child);
            }
        }


    }
}