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
using SharpNL.Utility;

namespace SharpNL.Tests.Dictionary {
    [TestFixture]
    internal class DictionaryTest {

        [Test]
        public void TestOpenNLPDic() {
            var dic = new SharpNL.Dictionary.Dictionary(Tests.OpenFile("opennlp/tools/dictionary/tags.tagdict"));

            Assert.NotNull(dic);
            Assert.AreEqual(7, dic.Count);
            Assert.AreEqual("brave", dic[0].Tokens[0]);
            Assert.AreEqual("JJ VB", dic[0].Attributes["tags"]);

            Assert.AreEqual("computer-driven", dic[6].Tokens[0]);
            Assert.AreEqual("JJ", dic[6].Attributes["tags"]);
        }

        [Test]
        public void TestDifferentCaseLookup() {
            var entry1 = new StringList(new[] {"1a", "1b"});
            var entry2 = new StringList(new[] {"1A", "1B"});

            var dic = new SharpNL.Dictionary.Dictionary(false) {entry1};

            Assert.True(dic.Contains(entry2));
        }


        [Test]
        public void TestDifferentCaseLookupCaseSensitive() {
            var entry1 = new StringList(new[] {"1a", "1b"});
            var entry2 = new StringList(new[] {"1A", "1B"});

            var dic = new SharpNL.Dictionary.Dictionary(true) {entry1};

            Assert.False(dic.Contains(entry2));
        }

        [Test]
        public void TestEquals() {
            var entry1 = new StringList(new[] {"1a", "1b"});
            var entry2 = new StringList(new[] {"2a", "2b"});

            var a = new SharpNL.Dictionary.Dictionary(false) {entry1, entry2};
            var b = new SharpNL.Dictionary.Dictionary(false) {entry1, entry2};
            var c = new SharpNL.Dictionary.Dictionary(true) {entry1, entry2};

            Assert.True(a.Equals(b));
            Assert.True(c.Equals(a));
            Assert.True(b.Equals(c));
        }

        [Test]
        public void TestHashCode() {
            var entry1 = new StringList(new[] {"1a", "1b"});
            var entry2 = new StringList(new[] {"1A", "1B"});

            var a = new SharpNL.Dictionary.Dictionary(false) {entry1};
            var b = new SharpNL.Dictionary.Dictionary(false) {entry2};
            var c = new SharpNL.Dictionary.Dictionary(true) {entry1};
            var d = new SharpNL.Dictionary.Dictionary(true) {entry2};

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreEqual(b.GetHashCode(), c.GetHashCode());
            Assert.AreEqual(c.GetHashCode(), d.GetHashCode());
        }

        [Test]
        public void TestLookup() {
            var a = new StringList("1a", "1b");
            var b = new StringList("1A", "1B");
            var c = new StringList("1A", "1C");

            var dic = new SharpNL.Dictionary.Dictionary(false) {a};


            Assert.True(dic.Contains(a));
            Assert.True(dic.Contains(b));
            Assert.False(dic.Contains(c));
        }

        [Test]
        public void TestLookupCaseSensitive() {
            var a = new StringList("1a", "1b");
            var b = new StringList("1A", "1B");
            var c = new StringList("1A", "1C");

            var dic = new SharpNL.Dictionary.Dictionary(true) {a};

            Assert.True(dic.Contains(a));
            Assert.False(dic.Contains(b));
            Assert.False(dic.Contains(c));
        }


        [Test]
        public void TestParseOneEntryPerLine() {
            const string testDictionary = "1a 1b 1c 1d \n 2a 2b 2c \n 3a \n 4a    4b   ";

            var dic = SharpNL.Dictionary.Dictionary.ParseOneEntryPerLine(new StringReader(testDictionary));

            Assert.True(dic.Count == 4);

            Assert.True(dic.Contains(new StringList(new[] {"1a", "1b", "1c", "1d"})));

            Assert.True(dic.Contains(new StringList(new[] {"2a", "2b", "2c"})));

            Assert.True(dic.Contains(new StringList(new[] {"3a"})));

            Assert.True(dic.Contains(new StringList(new[] {"4a", "4b"})));
        }

        [Test]
        public void TestSerialization() {
            var dic = new SharpNL.Dictionary.Dictionary(false) {new StringList("a1", "a2", "a3", "a4")};
            var data = new MemoryStream();

            dic.Serialize(data);

            data.Seek(0, SeekOrigin.Begin);

            var dic2 = new SharpNL.Dictionary.Dictionary(data);

            Assert.True(dic.Equals(dic2));
        }

        [Test]
        public void TestSerializationWithAttributes() {
            var dic = new SharpNL.Dictionary.Dictionary(false);
            var data = new MemoryStream();

            var entry = dic.Add(new StringList("a1", "a2", "a3", "a4"));
            entry.Attributes["one"] = "1";

            dic.Serialize(data);

            data.Seek(0, SeekOrigin.Begin);

            var dic2 = new SharpNL.Dictionary.Dictionary(data);

            Assert.True(dic.Equals(dic2));
            Assert.AreEqual(false, dic2.IsCaseSensitive);
            Assert.AreEqual("1", dic2[0].Attributes["one"]);
        }

        [Test]
        public void TestToString() {
            var a = new SharpNL.Dictionary.Dictionary(false) {new StringList(new[] {"1a", "1b"})};
            Assert.IsNotEmpty(a.ToString());
        }
    }
}