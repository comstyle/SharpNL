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

 /*
using System.IO;
using NUnit.Framework;
using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Tests.Chunker {
   
    [TestFixture]
    public class ChunkerDetailedFMeasureListenerTest {

        [Test]
        public void TestEvaluator() {
            DummyChunkSampleStream predictedSample;
            DummyChunkSampleStream expectedSample;

            using (var data = Tests.OpenSample("opennlp/tools/chunker/output.txt")) {
            
                predictedSample = new DummyChunkSampleStream(new PlainTextByLineStream(new UnclosableStream(data)), false);

                data.Seek(0, SeekOrigin.Begin);

                expectedSample = new DummyChunkSampleStream(new PlainTextByLineStream(data), true);

            }

            var dummyChunker = new DummyChunker(predictedSample);
            //var listener = new ChunkerDetailedFMeasureListener();

            var evaluator = new ChunkerEvaluator(dummyChunker, new );




        }

    }
    
}
*/