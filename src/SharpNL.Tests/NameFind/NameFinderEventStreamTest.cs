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
    /// This is the test class for <see cref="NameFinderEventStream"/>
    /// </summary>
    [TestFixture]
    internal class NameFinderEventStreamTest {

        /// <summary>
        /// Tests the correctly generated outcomes for a test sentence.
        /// </summary>
        [Test]
        public void TestOutcomesForSingleTypeSentence() {

            var sentence = new[] {
                "Elise",
                "Wendel",
                "appreciated",
                "the",
                "hint",
                "and",
                "enjoyed",
                "a",
                "delicious",
                "traditional",
                "meal",
                "."
            };

            var nameSample = new NameSample(sentence, new[] {new Span(0, 2, "person")}, false);
            var eventStream = new NameFinderEventStream(new CollectionObjectStream<NameSample>(nameSample));

            Assert.AreEqual("person-" + NameFinderME.START, eventStream.Read().Outcome);
            Assert.AreEqual("person-" + NameFinderME.Continue, eventStream.Read().Outcome);

            for (int i = 0; i < 10; i++) {
                Assert.AreEqual(NameFinderME.Other, eventStream.Read().Outcome);
            }

            Assert.Null(eventStream.Read());


        }



    }
}