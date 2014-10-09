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
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {
    /// <summary>
    /// Test class for the <see cref="TokenNameFinderEvaluator"/>.
    /// </summary>
    [TestFixture]
    internal class TokenNameFinderEvaluatorTest {
        private static readonly string[] Sentence = {
            "U", ".", "S", ".", "President", "Barack", "Obama", "is",
            "considering", "sending", "additional", "American", "forces",
            "to", "Afghanistan", "."
        };

        private static NameSample CreateSimpleNameSampleA() {
            var names = new[] {
                new Span(0, 4, "Location"),
                new Span(5, 7, "Person"),
                new Span(14, 15, "Location")
            };

            return new NameSample(Sentence, names, false);
        }

        private static NameSample CreateSimpleNameSampleB() {
            Span[] names = {new Span(0, 4, "Location"), new Span(14, 15, "Location")};
            return new NameSample(Sentence, names, false);
        }

        [Test]
        public void TestPositive() {

            // TODO: Add the listener

            var pred = CreateSimpleNameSampleA().Names;
            var eval = new TokenNameFinderEvaluator(new DummyNameFinder(pred));

            eval.EvaluateSample(CreateSimpleNameSampleA());

            Assert.AreEqual(1d, eval.FMeasure.Value);
        }
        [Test]
        public void TestNegative() {

            // TODO: Add the listener

            var pred = CreateSimpleNameSampleB().Names;
            var eval = new TokenNameFinderEvaluator(new DummyNameFinder(pred));

            eval.EvaluateSample(CreateSimpleNameSampleA());

            Assert.AreEqual(0.8d, eval.FMeasure.Value);
        }

        private class DummyNameFinder : ITokenNameFinder {
            private readonly Span[] ret;

            public DummyNameFinder(Span[] ret) {
                this.ret = ret;
            }

            public Span[] Find(string[] tokens) {
                return ret;
            }

            public void ClearAdaptiveData() {}
        }
    }
}