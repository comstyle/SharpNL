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
using SharpNL.Tokenize;

namespace SharpNL.Tests.Tokenize {
    [TestFixture]
    internal class DictionaryDetokenizerTest {
        internal static IDetokenizer CreateLatinDetokenizer() {
            using (var dictIn = Tests.OpenFile("/opennlp/tools/tokenize/latin-detokenizer.xml")) {
                var dict = new DetokenizationDictionary(dictIn);
                return new DictionaryDetokenizer(dict);
            }
        }

        [Test]
        public void TestDetokenizer() {
            var dict = new DetokenizationDictionary {
                {".", DetokenizationDictionary.Operation.MoveLeft},
                {"!", DetokenizationDictionary.Operation.MoveLeft},
                {"(", DetokenizationDictionary.Operation.MoveRight},
                {")", DetokenizationDictionary.Operation.MoveLeft},
                {"\"", DetokenizationDictionary.Operation.RightLeftMatching},
                {"-", DetokenizationDictionary.Operation.MoveBoth}
            };

            var detokenizer = new DictionaryDetokenizer(dict);

            var detokenizeOperations = detokenizer.Detokenize(new[] {"Simple", "test", ".", "co", "-", "worker"});

            Assert.AreEqual(DetokenizationOperation.NoOperation, detokenizeOperations[0]);
            Assert.AreEqual(DetokenizationOperation.NoOperation, detokenizeOperations[1]);
            Assert.AreEqual(DetokenizationOperation.MergeToLeft, detokenizeOperations[2]);
            Assert.AreEqual(DetokenizationOperation.NoOperation, detokenizeOperations[3]);
            Assert.AreEqual(DetokenizationOperation.MergeBoth, detokenizeOperations[4]);
            Assert.AreEqual(DetokenizationOperation.NoOperation, detokenizeOperations[5]);
        }

        [Test]
        public void testDetokenizeToString() {
            var detokenizer = CreateLatinDetokenizer();

            var tokens = new[] {"A", "test", ",", "(", "string", ")", "."};

            var sentence = detokenizer.Detokenize(tokens, null);

            Assert.AreEqual("A test, (string).", sentence);
        }

        [Test]
        public void testDetokenizeToString2() {
            var detokenizer = CreateLatinDetokenizer();

            var tokens = new[] {"A", "co", "-", "worker", "helped", "."};

            var sentence = detokenizer.Detokenize(tokens, null);

            Assert.AreEqual("A co-worker helped.", sentence);
        }
    }
}