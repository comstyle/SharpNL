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
using SharpNL.Tests.Tokenize;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.Compatibility {
    [TestFixture(Category = "Compatibility")]
    internal class TokenizerTest {

        internal const string modelFile = "opennlp/models/en-token.bin";

        //
        // DO NOT USE THIS TESTS AS SAMPLES TO BUILD YOUR STUFF !
        //  
        //  I use some things here, that are not needed in a "real" implementation 
        //
        private static opennlp.tools.tokenize.TokenizerModel OpenJavaModel(string fileName) {
            java.io.FileInputStream inputStream = null;
            try {
                inputStream = OpenNLP.OpenInputStream(fileName);
                return new opennlp.tools.tokenize.TokenizerModel(inputStream);
            } finally {
                if (inputStream != null)
                    inputStream.close();
            }
        }

        private static opennlp.tools.tokenize.TokenizerME CreateJavaTokenizer(string fileName) {
            return new opennlp.tools.tokenize.TokenizerME(OpenJavaModel(fileName));
        }

        private static TokenizerModel OpenSharpModel(string fileName) {
            FileStream fileStream = null;
            try {
                fileStream = Tests.OpenFile(fileName);
                return new TokenizerModel(fileStream);
            } finally {
                if (fileStream != null)
                    fileStream.Close();
            }
        }
        private static TokenizerME CreateSharpTokenizer(string fileName) {
            return new TokenizerME(OpenSharpModel(fileName));
        }

        internal static void TestJavaTokenizer(opennlp.tools.tokenize.Tokenizer tokenizer) {
            var tokens = tokenizer.tokenize("Sounds like it's not properly thought through!");

            Assert.AreEqual(9, tokens.Length);
            Assert.AreEqual("Sounds", tokens[0]);
            Assert.AreEqual("like", tokens[1]);
            Assert.AreEqual("it", tokens[2]);
            Assert.AreEqual("'s", tokens[3]);
            Assert.AreEqual("not", tokens[4]);
            Assert.AreEqual("properly", tokens[5]);
            Assert.AreEqual("thought", tokens[6]);
            Assert.AreEqual("through", tokens[7]);
            Assert.AreEqual("!", tokens[8]);
        }

        [Test]
        public void TestModels() {
            var jModel = OpenJavaModel(modelFile);
            var sModel = OpenSharpModel(modelFile);

            Assert.AreEqual(jModel.getLanguage(), sModel.Language);
            Assert.AreEqual(jModel.useAlphaNumericOptimization(), sModel.UseAlphaNumericOptimization);

            Assert.Null(jModel.getAbbreviations());
            Assert.Null(sModel.Abbreviations);           
        }

        [Test]
        public void TestTokenizer() {
            var jME = CreateJavaTokenizer(modelFile);
            var sME = CreateSharpTokenizer(modelFile);

            TestJavaTokenizer(jME);
            TokenizerMETest.TestTokenizer(sME);

            var jProbs = jME.getTokenProbabilities();
            var sProbs = sME.TokenProbabilities;

            Assert.AreEqual(jProbs.Length, sProbs.Length);

            for (int i = 0; i < jProbs.Length; i++) {               
                Assert.AreEqual(jProbs[i], sProbs[i]);
            }

            // first try again ? woot! 
            //
            // ** the gods of programming... they must love me! #lol :P
        }

        [Test]
        public void TestCrossCompatibility() {
            using (var data = Tests.OpenFile("/opennlp/tools/tokenize/token.train")) {
                var samples = new TokenSampleStream(new PlainTextByLineStream(data));
                var mlParams = new TrainingParameters();
                mlParams.Set(Parameters.Iterations, "100");
                mlParams.Set(Parameters.Cutoff, "0");
                var model = TokenizerME.Train(samples, new TokenizerFactory("en", null, true), mlParams);

                var sMe = new TokenizerME(model);

                TokenizerMETest.TestTokenizer(sMe);

                var sProbs = sMe.TokenProbabilities;

                // --- java \/

                var sFile = Path.GetTempFileName();

                model.Serialize(new FileStream(sFile, FileMode.Create));

                var jModel = new opennlp.tools.tokenize.TokenizerModel(
                    OpenNLP.CreateInputStream(sFile) 
                );

                var jMe = new opennlp.tools.tokenize.TokenizerME(jModel);

                TestJavaTokenizer(jMe);

                var jProbs = jMe.getTokenProbabilities();

                Assert.AreEqual(jProbs.Length, sProbs.Length);

                for (int i = 0; i < jProbs.Length; i++) {

                    // one difference :(
                    // -0.00000000000000011102230246251565
                    //
                    // but still "insignificant" :)
                    Assert.AreEqual(jProbs[i], sProbs[i], 0.0000000001d);
                }
            }
        }



        
    }
}