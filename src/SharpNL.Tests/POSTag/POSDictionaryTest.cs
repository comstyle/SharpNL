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
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Tests.POSTag {
    [TestFixture]
    internal class POSDictionaryTest {

        private static POSDictionary LoadDictionary(string fileName) {
            using (var file = Tests.OpenFile(fileName)) {
                return new POSDictionary(file);             
            }
        }


        private static POSDictionary SerializeDeserializeDict(POSDictionary dict) {
            using (var data = new MemoryStream()) {
                dict.Serialize(new UnclosableStream(data));

                data.Seek(0, SeekOrigin.Begin);

                return new POSDictionary(data); 
            }
        }

        [Test]
        public void TestSerialization() {

            var dict = new POSDictionary();

            dict.Put("a", "1", "2", "3");
            dict.Put("b", "4", "5", "6");
            dict.Put("c", "7", "8", "9");
            dict.Put("Always", "RB", "NNP");

            Assert.AreEqual(dict, SerializeDeserializeDict(dict));
        }

        [Test]
        public void TestLoadingDictionaryWithoutCaseAttribute() {
            var dict = LoadDictionary("opennlp/tools/postag/TagDictionaryWithoutCaseAttribute.xml");

            Assert.AreEqual(new [] { "NNP" }, dict.GetTags("McKinsey"));
            Assert.Null(dict.GetTags("Mckinsey"));
        }

        [Test]
        public void TestCaseSensitiveDictionary() {
            var dict = LoadDictionary("opennlp/tools/postag/TagDictionaryCaseSensitive.xml");

            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("McKinsey"));
            Assert.Null(dict.GetTags("Mckinsey"));

            dict = SerializeDeserializeDict(dict);

            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("McKinsey"));
            Assert.Null(dict.GetTags("Mckinsey"));

        }

        [Test]
        public void TestCaseInsensitiveDictionary() {
            var dict = LoadDictionary("opennlp/tools/postag/TagDictionaryCaseInsensitive.xml");

            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("McKinsey"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("Mckinsey"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("MCKINSEY"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("mckinsey"));

            dict = SerializeDeserializeDict(dict);

            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("McKinsey"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("Mckinsey"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("MCKINSEY"));
            Assert.AreEqual(new[] { "NNP" }, dict.GetTags("mckinsey"));

        }

        [Test]
        public void TestToString() {
            var dict = LoadDictionary("opennlp/tools/postag/TagDictionaryCaseInsensitive.xml");

            Assert.AreEqual("POSDictionary{size=1, caseSensitive=False}", dict.ToString());

            dict = LoadDictionary("opennlp/tools/postag/TagDictionaryCaseSensitive.xml");

            Assert.AreEqual("POSDictionary{size=1, caseSensitive=True}", dict.ToString());
        }

  
    }
}