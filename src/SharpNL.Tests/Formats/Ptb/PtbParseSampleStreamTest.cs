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

using SharpNL.Formats.Ptb;
using SharpNL.Parser;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ptb {
    [TestFixture]
    internal class PtbParseSampleStreamTest {
        /// <summary>
        /// Test the <see cref="PtbParseSampleStream"/> with the OpenNLP parser.
        /// </summary>
        [Test]
        public void TestSamples() {

            using (
                var reader1 = new PtbStreamReader("en",
                    new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/parser/test.parse")), false, null)) {
 
                using (
                    var reader2 = new PtbStreamReader("en",
                        new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/parser/test.parse")), false, null)) {

                    var stream1 = new PtbParseSampleStream(reader1);

                    Parse p;
                    while ((p = stream1.Read()) != null) {

                        var t2 = reader2.Read();
                        var op = Parse.ParseParse(t2.ToString());


                        Assert.AreEqual(p.ToString(), op.ToString());
                    }
                }
            }

        }

    }
}