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
using System.IO;
using NUnit.Framework;
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tests.ML.MaxEnt {
    [TestFixture]
    internal class ScaleDoestMatterTest {
        /// <summary>
        /// This test sets out to prove that the scale you use on real valued
        /// predicates doesn't matter when it comes the probability assigned to each
        /// outcome. Strangely, if we use (1,2) and (10,20) there's no difference. If
        /// we use (0.1,0.2) and (10,20) there is a difference.
        /// </summary>
        [Test]
        public void TestScaleResults() {
            const string smallValues = "predA=0.1 predB=0.2 A\n" + "predB=0.3 predA=0.1 B\n";
            const string largeValues = "predA=10 predB=20 A\n" + "predB=30 predA=10 B\n";

            const string smallTest = "predA=0.2 predB=0.2";
            const string largeTest = "predA=20 predB=20";

            var smallReader = new StringReader(smallValues);
            var smallEventStream = new RealBasicEventStream(new PlainTextByLineStream(smallReader));

            var smallModel = GIS.TrainModel(100, new OnePassRealValueDataIndexer(smallEventStream, 0), false);

            var contexts = smallTest.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var values = RealValueFileEventStream.ParseContexts(contexts);
            var smallResults = smallModel.Eval(contexts, values);

            var smallResultString = smallModel.GetAllOutcomes(smallResults);

            Console.Out.WriteLine("smallResults: " + smallResultString);

            var largeReader = new StringReader(largeValues);
            var largeEventStream = new RealBasicEventStream(new PlainTextByLineStream(largeReader));

            var largeModel = GIS.TrainModel(100, new OnePassRealValueDataIndexer(largeEventStream, 0), false);
            contexts = largeTest.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            values = RealValueFileEventStream.ParseContexts(contexts);
            var largeResults = largeModel.Eval(contexts, values);

            var largeResultString = smallModel.GetAllOutcomes(largeResults);
            Console.Out.WriteLine("largeResults: " + largeResultString);

            Assert.AreEqual(smallResults.Length, largeResults.Length);
            for (var i = 0; i < smallResults.Length; i++) {
                /*
      System.out.println(string.format(
          "classify with smallModel: %1$s = %2$f", smallModel.getOutcome(i),
          smallResults[i]));
      System.out.println(string.format(
          "classify with largeModel: %1$s = %2$f", largeModel.getOutcome(i),
          largeResults[i])); */

                Assert.AreEqual(smallResults[i], largeResults[i], 0.01f);
            }
        }
    }
}