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

using System.Text;
using NUnit.Framework;
using SharpNL.Formats.Muc;
using SharpNL.Utility;

namespace SharpNL.Tests.Formats.Muc {
    [TestFixture]
    internal class DocumentSplitterStreamTest {


        [Test]
        public void TestSplitTwoDocuments() {

            var docsString = new StringBuilder();

            for (int i = 0; i < 2; i++) {
                docsString.Append("<DOC>\n");
                docsString.Append("test document #" + i + "\n");
                docsString.Append("</DOC>\n");
            }

            var docs = new DocumentSplitterStream(new GenericObjectStream<string>(docsString.ToString()));

            var doc1 = docs.Read();
            Assert.AreEqual(docsString.Length / 2, doc1.Length + 1);
            Assert.True(doc1.Contains("#0"));

            var doc2 = docs.Read();
            Assert.AreEqual(docsString.Length / 2, doc2.Length + 1);
            Assert.True(doc2.Contains("#1"));

            Assert.Null(docs.Read());
            Assert.Null(docs.Read());

        }
        

    }
}