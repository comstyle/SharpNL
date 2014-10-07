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
using System.Text;
using NUnit.Framework;
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
    [TestFixture]
    internal class ChunkSampleStreamTest {

        [Test]
        public void TestReadingEvents() {
            var sample = new StringBuilder();

            // First sample sentence
            sample.Append("word11 tag11 pred11");
            sample.Append('\n');
            sample.Append("word12 tag12 pred12");
            sample.Append('\n');
            sample.Append("word13 tag13 pred13");
            sample.Append('\n');

            // Start next sample sentence
            sample.Append('\n');

            // Second sample sentence
            sample.Append("word21 tag21 pred21");
            sample.Append('\n');
            sample.Append("word22 tag22 pred22");
            sample.Append('\n');
            sample.Append("word23 tag23 pred23");
            sample.Append('\n');

            var stringStream = new PlainTextByLineStream(new MemoryStream(Encoding.UTF8.GetBytes(sample.ToString())));

            var chunkStream = new ChunkSampleStream(stringStream);

            // read first sample
            var firstSample = chunkStream.Read();
            Assert.AreEqual("word11", firstSample.Sentence[0]);
            Assert.AreEqual("tag11", firstSample.Tags[0]);
            Assert.AreEqual("pred11", firstSample.Preds[0]);
            Assert.AreEqual("word12", firstSample.Sentence[1]);
            Assert.AreEqual("tag12", firstSample.Tags[1]);
            Assert.AreEqual("pred12", firstSample.Preds[1]);
            Assert.AreEqual("word13", firstSample.Sentence[2]);
            Assert.AreEqual("tag13", firstSample.Tags[2]);
            Assert.AreEqual("pred13", firstSample.Preds[2]);


            // read second sample
            ChunkSample secondSample = chunkStream.Read();
            Assert.AreEqual("word21", secondSample.Sentence[0]);
            Assert.AreEqual("tag21", secondSample.Tags[0]);
            Assert.AreEqual("pred21", secondSample.Preds[0]);
            Assert.AreEqual("word22", secondSample.Sentence[1]);
            Assert.AreEqual("tag22", secondSample.Tags[1]);
            Assert.AreEqual("pred22", secondSample.Preds[1]);
            Assert.AreEqual("word23", secondSample.Sentence[2]);
            Assert.AreEqual("tag23", secondSample.Tags[2]);
            Assert.AreEqual("pred23", secondSample.Preds[2]);

            Assert.Null(chunkStream.Read());
        }

    }
}