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
using SharpNL.SentenceDetector;
using SharpNL.Tests.Sentence;
using SharpNL.Utility;

using JavaModel = opennlp.tools.sentdetect.SentenceModel;
using SharpModel = SharpNL.SentenceDetector.SentenceModel;

using JavaSDME = opennlp.tools.sentdetect.SentenceDetectorME;
using SharpSDME = SharpNL.SentenceDetector.SentenceDetectorME;

namespace SharpNL.Tests.Compatibility {
    [TestFixture(Category = "Compatibility")]
    internal class SentenceDetectorTest {
        private static JavaModel OpenJavaModel() {
            return new JavaModel(OpenNLP.OpenInputStream("opennlp/models/en-sent.bin"));
        }

        private static SharpModel OpenSharpModel() {
            return new SharpModel(Tests.OpenFile("opennlp/models/en-sent.bin"));
        }


        private static void TestSentences(JavaSDME javaMe, SharpSDME sharpME, string data) {
            var jSentences = javaMe.sentPosDetect(data);
            var sSentences = sharpME.SentPosDetect(data);

            var jProb = javaMe.getSentenceProbabilities();
            var sProb = sharpME.GetSentenceProbabilities();

            Assert.AreEqual(jSentences.Length, sSentences.Length);
            Assert.AreEqual(jProb.Length, sProb.Length);

            for (int i = 0; i < jSentences.Length; i++) {
                var a = jSentences[i].getCoveredText(data).toString(); // CharSequence -> string
                var b = sSentences[i].GetCoveredText(data);

                Assert.AreEqual(jSentences[i].getStart(), sSentences[i].Start);
                Assert.AreEqual(jSentences[i].getEnd(), sSentences[i].End);

                Assert.AreEqual(a, b);
                Assert.AreEqual(jProb[i], sProb[i]);
            }
        }

        [Test]
        public void TestModel() {
            var jModel = OpenJavaModel();
            var sModel = OpenSharpModel();

            Assert.Null(jModel.getAbbreviations());
            Assert.Null(sModel.Abbreviations);

            Assert.Null(jModel.getEosCharacters());
            Assert.Null(sModel.EosCharacters);

            Assert.AreEqual(jModel.useTokenEnd(), sModel.UseTokenEnd);

            var jFactory = jModel.getFactory();
            var sFactory = sModel.Factory;
            
            Assert.AreEqual(jFactory.isUseTokenEnd(), sFactory.UseTokenEnd);
            Assert.AreEqual(jFactory.getLanguageCode(), sFactory.LanguageCode);
            
        }

        [Test]
        public void TestSentenceDetect() {

            const string data = 
                "The Apache OpenNLP library is a machine learning based toolkit for the " +
                "processing of natural language text. It supports the most common NLP tasks, "+
                "such as tokenization, sentence segmentation, part-of-speech tagging, named entity "+
                "extraction, chunking, parsing, and coreference resolution. These tasks are usually "+
                "required to build more advanced text processing services.";
            
            var jME = new JavaSDME(OpenJavaModel());
            var sME = new SharpSDME(OpenSharpModel());

            TestSentences(jME, sME, data);

            // nice :D
            // Knuppe: I swear I did not expect to hit the same (double precision) the probability.
        }

        [Test]
        public void TestCrossCompatibility() {
            var jModel = OpenJavaModel();
            var sModel = OpenSharpModel();

            var jFile = Path.GetTempFileName();
            var sFile = Path.GetTempFileName();

            var jFileStream = OpenNLP.CreateOutputStream(jFile);
            jModel.serialize(jFileStream);
            jFileStream.close();

            sModel.Serialize(new FileStream(sFile, FileMode.Create));

            // now java opens the csharp model and vice versa :)

            var jModel2 = new JavaModel(OpenNLP.CreateInputStream(sFile));
            var sModel2 = new SharpModel(jFile);

            Assert.Null(jModel2.getAbbreviations());
            Assert.Null(sModel2.Abbreviations);

            Assert.Null(jModel2.getEosCharacters());
            Assert.Null(sModel2.EosCharacters);

            Assert.AreEqual(jModel2.useTokenEnd(), sModel2.UseTokenEnd);
                                                         
            var jFactory2 = jModel2.getFactory();
            var sFactory2 = sModel2.Factory;

            Assert.AreEqual(jFactory2.isUseTokenEnd(), sFactory2.UseTokenEnd);
            Assert.AreEqual(jFactory2.getLanguageCode(), sFactory2.LanguageCode);

            Assert.True(true);
        }

        [Test]
        public void TestEverything() {
            using (var file = Tests.OpenFile("/opennlp/tools/sentdetect/Sentences.txt")) {

                var mlParams = new TrainingParameters();

                mlParams.Set(Parameters.Iterations, "100");
                mlParams.Set(Parameters.Cutoff, "0");

                var sdFactory = new SentenceDetectorFactory("en", true, null, null);
                var stream = new SentenceSampleStream(new PlainTextByLineStream(file));

                var model = SentenceDetectorME.Train("en", stream, sdFactory, mlParams);

                Assert.AreEqual("en", model.Language);
                Assert.AreEqual(model.UseTokenEnd, true);

                var sMe = new SentenceDetectorME(model);
                
                // test the SharpNL sentences
                SentenceDetectorMETest.EvalSentences(sMe);

                var sFile = Path.GetTempFileName();

                model.Serialize(new FileStream(sFile, FileMode.Create));

                var jModel2 = new JavaModel(OpenNLP.CreateInputStream(sFile));

                var jMe = new JavaSDME(jModel2);

                // test the Java OpenNLP sentences.
                JavaEvalSentences(jMe);

                // first try?! Yes! ;-)

            }
        }


        internal static void JavaEvalSentences(JavaSDME sentDetect) {
            const string sampleSentences1 = "This is a test. There are many tests, this is the second.";
            var sents = sentDetect.sentDetect(sampleSentences1);
            Assert.AreEqual(sents.Length, 2);
            Assert.AreEqual(sents[0], "This is a test.");
            Assert.AreEqual(sents[1], "There are many tests, this is the second.");
            var probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(probs.Length, 2);

            const string sampleSentences2 = "This is a test. There are many tests, this is the second";
            sents = sentDetect.sentDetect(sampleSentences2);
            Assert.AreEqual(sents.Length, 2);
            probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(probs.Length, 2);
            Assert.AreEqual(sents[0], "This is a test.");
            Assert.AreEqual(sents[1], "There are many tests, this is the second");

            const string sampleSentences3 = "This is a \"test\". He said \"There are many tests, this is the second.\"";
            sents = sentDetect.sentDetect(sampleSentences3);
            Assert.AreEqual(sents.Length, 2);
            probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(probs.Length, 2);
            Assert.AreEqual(sents[0], "This is a \"test\".");
            Assert.AreEqual(sents[1], "He said \"There are many tests, this is the second.\"");

            const string sampleSentences4 = "This is a \"test\". I said \"This is a test.\"  Any questions?";
            sents = sentDetect.sentDetect(sampleSentences4);
            Assert.AreEqual(sents.Length, 3);
            probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(probs.Length, 3);
            Assert.AreEqual(sents[0], "This is a \"test\".");
            Assert.AreEqual(sents[1], "I said \"This is a test.\"");
            Assert.AreEqual(sents[2], "Any questions?");

            const string sampleSentences5 = "This is a one sentence test space at the end.    ";
            sents = sentDetect.sentDetect(sampleSentences5);
            Assert.AreEqual(1, sentDetect.getSentenceProbabilities().Length);
            Assert.AreEqual(sents[0], "This is a one sentence test space at the end.");

            const string sampleSentences6 = "This is a one sentences test with tab at the end.            ";
            sents = sentDetect.sentDetect(sampleSentences6);
            Assert.AreEqual(sents[0], "This is a one sentences test with tab at the end.");

            const string sampleSentences7 = "This is a test.    With spaces between the two sentences.";
            sents = sentDetect.sentDetect(sampleSentences7);
            Assert.AreEqual(sents[0], "This is a test.");
            Assert.AreEqual(sents[1], "With spaces between the two sentences.");

            const string sampleSentences9 = "";
            sents = sentDetect.sentDetect(sampleSentences9);
            Assert.AreEqual(0, sents.Length);

            const string sampleSentences10 = "               "; // whitespaces and tabs
            sents = sentDetect.sentDetect(sampleSentences10);
            Assert.AreEqual(0, sents.Length);

            const string sampleSentences11 = "This is test sentence without a dot at the end and spaces          ";
            sents = sentDetect.sentDetect(sampleSentences11);
            Assert.AreEqual(sents[0], "This is test sentence without a dot at the end and spaces");
            probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(1, probs.Length);

            const string sampleSentence12 = "    This is a test.";
            sents = sentDetect.sentDetect(sampleSentence12);
            Assert.AreEqual(sents[0], "This is a test.");

            const string sampleSentence13 = " This is a test";
            sents = sentDetect.sentDetect(sampleSentence13);
            Assert.AreEqual(sents[0], "This is a test");

            // Test that sentPosDetect also works
            var pos = sentDetect.sentPosDetect(sampleSentences2);
            Assert.AreEqual(pos.Length, 2);
            probs = sentDetect.getSentenceProbabilities();
            Assert.AreEqual(probs.Length, 2);

            Assert.True(pos[0].AreEqual(new Span(0, 15)));
            Assert.True(pos[1].AreEqual(new Span(16, 56)));
        }

    }
}