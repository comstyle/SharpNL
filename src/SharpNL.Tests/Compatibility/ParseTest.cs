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
using SharpNL.Parser;

namespace SharpNL.Tests.Compatibility {

    //
    // DO NOT USE THIS TESTS AS SAMPLES TO BUILD YOUR STUFF !
    //  
    //  I use some things here, that are not needed in a "real" implementation 
    //

    [TestFixture]
    internal class ParseTest {

        internal const string modelFile = "opennlp/models/en-parser-chunking.bin";

        private static opennlp.tools.parser.ParserModel OpenJavaModel(string fileName) {
            java.io.FileInputStream inputStream = null;
            try {
                inputStream = OpenNLP.OpenInputStream(fileName);
                return new opennlp.tools.parser.ParserModel(inputStream);
            } finally {
                if (inputStream != null)
                    inputStream.close();
            }
        }

        private static opennlp.tools.parser.Parser CreateJavaParser(string fileName) {
            return opennlp.tools.parser.ParserFactory.create(OpenJavaModel(fileName));
        }

        private static ParserModel OpenSharpModel(string fileName) {
            FileStream fileStream = null;
            try {
                fileStream = Tests.OpenFile(fileName);
                return new ParserModel(fileStream);
            } finally {
                if (fileStream != null)
                    fileStream.Close();
            }
        }
        private static IParser CreateSharpParser(string fileName) {
            return ParserFactory.Create(OpenSharpModel(fileName));
        }

        [Test]
        public void TestParsers() {
            var jParser = CreateJavaParser(modelFile) as opennlp.tools.parser.chunking.Parser;
            var sParser = CreateSharpParser(modelFile) as SharpNL.Parser.Chunking.Parser;

            Assert.NotNull(jParser);
            Assert.NotNull(sParser);


        }



        [Test]
        public void TestQuestionsManually() {
            var sentences = new[] {
                "How are you?",
                "Do animals eat fruit ?",
                "What do animals eat ?",  
                "Why do animals eat fruit ?",
                "How much fruit do animals eat ?",
                "Where do animals eat ?",
                "When do animals eat ?"
            };

            var results = new[] {
                "(TOP (SBAR (WHADVP (WRB How)) (S (VP (VBP are))) (. you?)))",
                "(TOP (SQ (VBP Do) (NP (NNS animals)) (VP (VB eat) (NP (NN fruit))) (. ?)))",
                "(TOP (SBARQ (WHNP (WP What)) (SQ (VBP do) (NP (NNS animals)) (VP (VB eat))) (. ?)))", 
                "(TOP (SBARQ (WHADVP (WRB Why)) (SQ (VBP do) (NP (NNS animals)) (VP (VB eat) (NP (NN fruit)))) (. ?)))",
                "(TOP (SBAR (WHADJP (WRB How)) (S (NP (JJ much) (NN fruit)) (VP (VBP do) (S (NP (NNS animals)) (VP (VB eat))))) (. ?)))",
                "(TOP (SBARQ (WHADVP (WRB Where)) (SQ (VBP do) (NP (NNS animals)) (VP (VB eat))) (. ?)))",
                "(TOP (SBARQ (WHADVP (WRB When)) (SQ (VBP do) (NP (NNS animals)) (VP (VB eat))) (. ?)))"
            };

            var sParser = CreateSharpParser(modelFile);

            for (int i = 0; i < sentences.Length; i++) {
          
                var sParses = ParserTool.ParseLine(sentences[i], sParser, 1);

                Assert.AreEqual(1, sParses.Length);

                Assert.AreEqual(results[i], sParses[0].ToString());               
            }
        }


        //[Test]
        public void TestParse() {

            // This test can't be done becouse a bug in the OpenNLP IKVM version
            // https://issues.apache.org/jira/browse/OPENNLP-727

            //const string sentence = "The quick brown fox jumps over the lazy dog .";

            const string sentence = "How much fruit do animals eat ?";

            var jParser = CreateJavaParser(modelFile);
            var sParser = CreateSharpParser(modelFile);

            var jParses = opennlp.tools.cmdline.parser.ParserTool.parseLine(sentence, jParser, 1);
            var sParses = ParserTool.ParseLine(sentence, sParser, 1);

            var jsb = new java.lang.StringBuffer();

            jParses[0].show(jsb);

            Assert.AreEqual("(TOP (SBAR (WHADJP (WRB How)) (S (NP (JJ much) (NN fruit)) (VP (VBP do) (S (NP (NNS animals)) (VP (VB eat))))) (. ?)))",
                jsb.toString());

            Assert.AreEqual("(TOP (SBAR (WHADJP (WRB How)) (S (NP (JJ much) (NN fruit)) (VP (VBP do) (S (NP (NNS animals)) (VP (VB eat))))) (. ?)))",
                sParses[0].ToString());

            Assert.NotNull(sParses);

            //CheckParseChild(jParses, sParses);

        }

        //[Test]
        public void TestQuestions() {
            var sentences = new [] {
                "How are you?",
                "Do animals eat fruit ?",
                "What do animals eat ?",  
                "Why do animals eat fruit ?",
                "How much fruit do animals eat ?",
                "Where do animals eat ?",
                "When do animals eat ?"
            };

            // This test can't be done becouse a bug in the OpenNLP IKVM version
            // https://issues.apache.org/jira/browse/OPENNLP-727


            var jParser = CreateJavaParser(modelFile);
            var sParser = CreateSharpParser(modelFile);

            foreach (var sentence in sentences) {
                var jParses = opennlp.tools.cmdline.parser.ParserTool.parseLine(sentence, jParser, 1);
                var sParses = ParserTool.ParseLine(sentence, sParser, 1);

                CheckParseChild(jParses, sParses);
            }
        }

        private static void CheckParseChild(opennlp.tools.parser.Parse[] jParses, Parse[] sParses) {

            Assert.AreEqual(jParses.Length, sParses.Length);

            for (var i = 0; i < sParses.Length; i++) {
                var jParse = jParses[i];
                var sParse = sParses[i];

                Assert.AreEqual(jParse.getType(), sParse.Type);

                Assert.AreEqual(jParse.isFlat(), sParse.IsFlat);
                Assert.AreEqual(jParse.isPosTag(), sParse.IsPosTag);
                Assert.AreEqual(jParse.isChunk(), sParse.IsChunk);

                Assert.AreEqual(jParse.getChildCount(), sParse.ChildCount);

                if (sParse.ChildCount > 0)
                    CheckParseChild(jParse.getChildren(), sParse.Children);
            }
        }       
    }
}