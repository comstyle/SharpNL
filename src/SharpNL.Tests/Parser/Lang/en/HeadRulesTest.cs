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
using NUnit.Framework;
using SharpNL.Parser.Lang.en;

namespace SharpNL.Tests.Parser.Lang.en {
    [TestFixture]
    internal class HeadRulesTest {
        [Test]
        public void TestSerialization() {
            var headRulesOriginal =
                new HeadRules(new StreamReader(Tests.OpenFile("/opennlp/tools/parser/en_head_rules")));

            using (var data = new MemoryStream()) {
                headRulesOriginal.Serialize(new StreamWriter(data));

                data.Seek(0, SeekOrigin.Begin);

                var headRulesRecreated = new HeadRules(new StreamReader(data));

                Assert.AreEqual(headRulesOriginal, headRulesRecreated);
            }

        }
    }
}