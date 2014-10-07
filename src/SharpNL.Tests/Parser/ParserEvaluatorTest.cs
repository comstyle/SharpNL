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
using SharpNL.Parser;
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Tests.Parser {
    [TestFixture]
    internal class ParserEvaluatorTest {

        [Test]
        public static void TestEvaluator() {
            const string goldParseString =
                "(TOP (S (NP (NNS Sales) (NNS executives)) (VP (VBD were) (VP (VBG examing) (NP (DT the) (NNS figures)) (PP (IN with) (NP (JJ great) (NN care))) ))  (NP (NN yesterday)) (. .) ))";
            const string testParseString =
                "(TOP (S (NP (NNS Sales) (NNS executives)) (VP (VBD were) (VP (VBG examing) (NP (DT the) (NNS figures)) (PP (IN with) (NP (JJ great) (NN care) (NN yesterday))) ))  (. .) ))";

            var gold = ParserEvaluator.GetConstituencySpans(Parse.ParseParse(goldParseString));
            var test = ParserEvaluator.GetConstituencySpans(Parse.ParseParse(testParseString));

            var measure = new FMeasure<Span>();

            measure.UpdateScores(gold, test);

            // Java expected output:
            // Precision: 0.42857142857142855
            // Recall: 0.375
            // F-Measure: 0.39999999999999997

            // c# expected output - close enough :)
            // Precision: 0,428571428571429
            // Recall: 0,375
            // F-Measure: 0,4

            Assert.AreEqual(measure.RecallScore, 0.375d);
            Assert.AreEqual(measure.Value, 0,4d);
        }

    }
}