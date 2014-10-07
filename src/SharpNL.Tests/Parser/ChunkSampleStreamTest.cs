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
using SharpNL.Chunker;
using SharpNL.Parser;
using SharpNL.Utility;
using ChunkSampleStream = SharpNL.Parser.ChunkSampleStream;

namespace SharpNL.Tests.Parser {
    [TestFixture]
    public class ChunkSampleStreamTest {

        [Test]
        public void testConvertParseToPosSample() {
            IObjectStream<ChunkSample> chunkSampleStream =
                new ChunkSampleStream(new ParseSampleStream(new GenericObjectStream<string>(ParseTest.ParseString)));

            var sample = chunkSampleStream.Read();

            Assert.AreEqual("She", sample.Sentence[0]);
            Assert.AreEqual("PRP", sample.Tags[0]);
            Assert.AreEqual("S-NP", sample.Preds[0]);
            Assert.AreEqual("was", sample.Sentence[1]);
            Assert.AreEqual("VBD", sample.Tags[1]);
            Assert.AreEqual("O", sample.Preds[1]);
            Assert.AreEqual("just", sample.Sentence[2]);
            Assert.AreEqual("RB", sample.Tags[2]);
            Assert.AreEqual("S-ADVP", sample.Preds[2]);
            Assert.AreEqual("another", sample.Sentence[3]);
            Assert.AreEqual("DT", sample.Tags[3]);
            Assert.AreEqual("S-NP", sample.Preds[3]);
            Assert.AreEqual("freighter", sample.Sentence[4]);
            Assert.AreEqual("NN", sample.Tags[4]);
            Assert.AreEqual("C-NP", sample.Preds[4]);
            Assert.AreEqual("from", sample.Sentence[5]);
            Assert.AreEqual("IN", sample.Tags[5]);
            Assert.AreEqual("O", sample.Preds[5]);
            Assert.AreEqual("the", sample.Sentence[6]);
            Assert.AreEqual("DT", sample.Tags[6]);
            Assert.AreEqual("S-NP", sample.Preds[6]);
            Assert.AreEqual("States", sample.Sentence[7]);
            Assert.AreEqual("NNPS", sample.Tags[7]);
            Assert.AreEqual("C-NP", sample.Preds[7]);
            Assert.AreEqual(",", sample.Sentence[8]);
            Assert.AreEqual(",", sample.Tags[8]);
            Assert.AreEqual("O", sample.Preds[8]);
            Assert.AreEqual("and", sample.Sentence[9]);
            Assert.AreEqual("CC", sample.Tags[9]);
            Assert.AreEqual("O", sample.Preds[9]);
            Assert.AreEqual("she", sample.Sentence[10]);
            Assert.AreEqual("PRP", sample.Tags[10]);
            Assert.AreEqual("S-NP", sample.Preds[10]);
            Assert.AreEqual("seemed", sample.Sentence[11]);
            Assert.AreEqual("VBD", sample.Tags[11]);
            Assert.AreEqual("O", sample.Preds[11]);
            Assert.AreEqual("as", sample.Sentence[12]);
            Assert.AreEqual("RB", sample.Tags[12]);
            Assert.AreEqual("S-ADJP", sample.Preds[12]);
            Assert.AreEqual("commonplace", sample.Sentence[13]);
            Assert.AreEqual("JJ", sample.Tags[13]);
            Assert.AreEqual("C-ADJP", sample.Preds[13]);
            Assert.AreEqual("as", sample.Sentence[14]);
            Assert.AreEqual("IN", sample.Tags[14]);
            Assert.AreEqual("O", sample.Preds[14]);
            Assert.AreEqual("her", sample.Sentence[15]);
            Assert.AreEqual("PRP$", sample.Tags[15]);
            Assert.AreEqual("S-NP", sample.Preds[15]);
            Assert.AreEqual("name", sample.Sentence[16]);
            Assert.AreEqual("NN", sample.Tags[16]);
            Assert.AreEqual("C-NP", sample.Preds[16]);
            Assert.AreEqual(".", sample.Sentence[17]);
            Assert.AreEqual(".", sample.Tags[17]);
            Assert.AreEqual("O", sample.Preds[17]);

            Assert.Null(chunkSampleStream.Read());
        }
    }
}