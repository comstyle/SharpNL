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
using SharpNL.ML.Model;
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Tests.POSTag {
    [TestFixture]
    public class POSTaggerMETest {

        internal static IObjectStream<POSSample> CreateSampleStream() {
            return new WordTagSampleStream(Tests.OpenFile("opennlp/tools/postag/AnnotatedSentences.txt"));
        }

        internal static POSModel TrainPOSModel(ModelType type = ModelType.Maxent) {

            var p = new TrainingParameters();
            switch (type) {
                case ModelType.Maxent:
                    p.Set(Parameters.Algorithm, "MAXENT");
                    break;
                case ModelType.Perceptron:
                    p.Set(Parameters.Algorithm, "PERCEPTRON");
                    break;
                default:
                    throw new NotSupportedException();
            }

            p.Set(Parameters.Iterations, "100");
            p.Set(Parameters.Cutoff, "5");

            return POSTaggerME.Train("en", CreateSampleStream(), p, new POSTaggerFactory());
        }

        [Test]
        public void TestPOSTagger() {
            var posModel = TrainPOSModel();

            var tagger = new POSTaggerME(posModel);

            var tags = tagger.Tag(new[] { "The", "driver", "got", "badly", "injured", "." });

            Assert.AreEqual(6, tags.Length);

            Assert.AreEqual("DT", tags[0]);
            Assert.AreEqual("NN", tags[1]);
            Assert.AreEqual("VBD", tags[2]);
            Assert.AreEqual("RB", tags[3]);
            Assert.AreEqual("VBN", tags[4]);
            Assert.AreEqual(".", tags[5]);
        }

        [Test]
        public void TestBuildNGramDictionary() {
            var samples = CreateSampleStream();


            var d = POSTaggerME.BuildNGramDictionary(samples, 0);

            Assert.NotNull(d);

        }


    }
}