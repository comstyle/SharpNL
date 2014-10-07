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
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    internal class PlainTextByLineStreamTest {
        [Test]
        public void TestLineSegmentation() {
            var testString = new StringBuilder();
            testString.Append("line1");
            testString.Append('\n');
            testString.Append("line2");
            testString.Append('\n');
            testString.Append("line3");
            testString.Append("\r\n");
            testString.Append("line4");
            testString.Append('\n');

            var stream = new PlainTextByLineStream(new MemoryStream(Encoding.UTF8.GetBytes(testString.ToString())));

            Assert.AreEqual("line1", stream.Read());
            Assert.AreEqual("line2", stream.Read());
            Assert.AreEqual("line3", stream.Read());
            Assert.AreEqual("line4", stream.Read());
        }
    }
}