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
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Ptb {
    [TestFixture]
    internal class PtbPosSampleStreamTest {
        /// <summary>
        /// Test the <see cref="PtbParseSampleStream"/> with the OpenNLP parser.
        /// </summary>
        [Test]
        public void TestSamples() {

            using (
                var reader = new PtbStreamReader("en",
                    new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/parser/test.parse")), false, null)) {

                var stream = new PtbPosSampleStream(reader);

                var p1 = stream.Read();
                var p2 = stream.Read();

                Assert.AreEqual("At_IN your_PRP$ age_NN ,_, Jackie_NNP ,_, you_PRP ought_MD to_TO know_VB that_IN you_PRP ca_MD n't_RB make_VB soup_NN without_IN turning_VBG up_RP the_DT flame_NN ._.", p1.ToString());
                Assert.AreEqual("A_DT few_JJ hours_NNS later_RB ,_, the_DT stock_NN market_NN dropped_VBD 190_CD points_NNS ._.", p2.ToString());
            }
        }
    }
}