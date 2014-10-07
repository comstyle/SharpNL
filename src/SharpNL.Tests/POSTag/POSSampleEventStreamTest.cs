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
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Tests.POSTag {
    [TestFixture]
    public class POSSampleEventStreamTest {
        [Test]
        public void TestOutcomesForSingleSentence() {
            const string sentence = "That_DT sounds_VBZ good_JJ ._.";

            var sample = POSSample.Parse(sentence);

            var eventStream = new POSSampleEventStream(new GenericObjectStream<POSSample>(sample));

            Assert.AreEqual("DT", eventStream.Read().Outcome);
            Assert.AreEqual("VBZ", eventStream.Read().Outcome);
            Assert.AreEqual("JJ", eventStream.Read().Outcome);
            Assert.AreEqual(".", eventStream.Read().Outcome);

            Assert.Null(eventStream.Read());
        }
    }
}