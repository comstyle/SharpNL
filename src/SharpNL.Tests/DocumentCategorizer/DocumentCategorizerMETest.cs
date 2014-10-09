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
using SharpNL.DocumentCategorizer;
using SharpNL.Utility;

namespace SharpNL.Tests.DocumentCategorizer {

    [TestFixture]
    internal class DocumentCategorizerMETest {

        [Test]
        public void testSimpleTraining() {

            IObjectStream<DocumentSample> samples = new GenericObjectStream<DocumentSample>(new[] {
                new DocumentSample("1", new[] {"a", "b", "c"}),
                new DocumentSample("1", new[] {"a", "b", "c", "1", "2"}),
                new DocumentSample("1", new[] {"a", "b", "c", "3", "4"}),
                new DocumentSample("0", new[] {"x", "y", "z"}),
                new DocumentSample("0", new[] {"x", "y", "z", "5", "6"}),
                new DocumentSample("0", new[] {"x", "y", "z", "7", "8"})
            });

            var param = new TrainingParameters();
            param.Set(Parameters.Iterations, "100");
            param.Set(Parameters.Cutoff, "0");

            var model = DocumentCategorizerME.Train("x-unspecified", samples, param, new DocumentCategorizerFactory());

            var doccat = new DocumentCategorizerME(model);

            var aProbs = doccat.Categorize("a");

            Assert.AreEqual("1", doccat.GetBestCategory(aProbs));

            var bProbs = doccat.Categorize("x");
            Assert.AreEqual("0", doccat.GetBestCategory(bProbs));

            //test to make sure sorted map's last key is cat 1 because it has the highest score.
            var sortedScoreMap = doccat.SortedScoreMap("a");

            foreach (var pair in sortedScoreMap) {
                Assert.AreEqual("1", pair.Value[0]);
                break;   
            }
        }
    }
}