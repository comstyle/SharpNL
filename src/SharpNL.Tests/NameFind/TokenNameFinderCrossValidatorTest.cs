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
using SharpNL.NameFind;
using SharpNL.Tests.NameFind.Support;
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {
    [TestFixture]
    public class TokenNameFinderCrossValidatorTest {

        private const string Type = null;

        [Test]
        public void TestWithNullResources() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/AnnotatedSentences.txt")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file, "ISO-8859-1"));

                var parameters = new TrainingParameters();
                parameters.Set(Parameters.Iterations, "70");
                parameters.Set(Parameters.Cutoff, "1");
                parameters.Set(Parameters.Algorithm, Parameters.Algorithms.MaxEnt);

                var cv = new TokenNameFinderCrossValidator("en", Type, parameters);

                cv.Evaluate(sampleStream, 2);

                Assert.NotNull(cv.FMeasure);
            }

        }

        [Test]
        public void TestWithNameEvaluationErrorListener() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/AnnotatedSentences.txt")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file, "ISO-8859-1"));

                var parameters = new TrainingParameters();
                parameters.Set(Parameters.Iterations, "70");
                parameters.Set(Parameters.Cutoff, "1");
                parameters.Set(Parameters.Algorithm, Parameters.Algorithms.MaxEnt);

                var cv = new TokenNameFinderCrossValidator("en", Type, parameters, new NameEvaluationErrorListener());

                cv.Evaluate(sampleStream, 2);

                Assert.NotNull(cv.FMeasure);
            }

        }


    }
}