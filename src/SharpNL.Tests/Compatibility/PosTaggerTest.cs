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
using JavaModel = opennlp.tools.postag.POSModel;
using SharpModel = SharpNL.POSTag.POSModel;

using JavaTagger = opennlp.tools.postag.POSTaggerME;
using SharpTagger = SharpNL.POSTag.POSTaggerME;

namespace SharpNL.Tests.Compatibility {
    [TestFixture]
    internal class PosTaggerTest {

        private const string ModeFile = "opennlp/models/en-pos-maxent.bin";

        private static JavaModel OpenJavaModel() {
            return new JavaModel(OpenNLP.OpenInputStream(ModeFile));
        }

        private static SharpModel OpenSharpModel() {
            return new SharpModel(Tests.OpenFile(ModeFile));
        }

        [Test]
        public void TestEverything() {
            var sentences = new[] {
                "What you get by achieving your goals is not as important as what you become by achieving your goals .",
                "The price of anything is the amount of life you exchange for it .",
                "The light which puts out our eyes is darkness to us .",
                "Only that day dawns to which we are awake .",
                "There is more day to dawn. The sun is but a morning star ."
            };

            var sTagger = new SharpTagger(OpenSharpModel());
            var jTagger = new JavaTagger(OpenJavaModel());

            foreach (var sentence in sentences) {
                var tokens = WhitespaceTokenizer.Instance.Tokenize(sentence);

                var sTags = sTagger.Tag(tokens);
                var jTags = jTagger.tag(tokens);

                Assert.AreEqual(jTags.Length, sTags.Length);
                for (var i = 0; i < sTags.Length; i++) {                   
                    Assert.AreEqual(jTags[i], sTags[i]);
                }

                var sTop = sTagger.TopKSequences(tokens);
                var jTop = jTagger.topKSequences(tokens);

                Assert.AreEqual(jTop.Length, sTop.Length);

                for (var i = 0; i < sTop.Length; i++) {

                    var jOut = jTop[i].getOutcomes();
                    var jProbs = jTop[i].getProbs();

                    Assert.AreEqual(jOut.size(), sTop[i].Outcomes.Count);

                    for (var j = 0; j < jOut.size(); j++) {
                        Assert.AreEqual(jOut.get(j), sTop[i].Outcomes[j]);
                        Assert.AreEqual(jProbs[j], sTop[i].Probabilities[j], 0.0000000001d);
                    }

                    Assert.AreEqual(jTop[i].getScore(), sTop[i].Score, 0.0000000001d);
                }
            }
        }
    }
}