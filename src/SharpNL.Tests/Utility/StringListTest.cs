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

using System;
using NUnit.Framework;
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    internal class StringListTest {
        [TestCase(1, 1)]
        [TestCase(100, 100)]
        public void TestHashCode(int size, int tests) {
            for (var n = 0; n < tests; n++) {
                var rnd = new Random();
                var items = new string[size];
                for (var i = 0; i < size; i++) {
                    items[i] = rnd.Next(1, 3) == 1 ? "axxx" : "baa223";
                }

                Assert.AreEqual(new StringList(items), new StringList(items));
            }
        }

        [Test]
        public void TestBasics() {
            var list = new StringList("a", "b");

            Assert.AreEqual(list.Count, 2);

            Assert.AreEqual(list[0], "a");
            Assert.AreEqual(list[1], "b");
        }

        [Test]
        public void TestCompareToIgnoreCase() {
            Assert.True(new StringList("a", "B").CompareToIgnoreCase(new StringList("A", "b")));
        }

        [Test]
        public void TestEnumerator() {
            var list = new StringList("a", "b", "c");

            var e = list.GetEnumerator();

            Assert.IsNull(e.Current);
            Assert.True(e.MoveNext());
            Assert.AreEqual(e.Current, "a");
            Assert.True(e.MoveNext());
            Assert.True(e.MoveNext());
            Assert.AreEqual(e.Current, "c");
            Assert.False(e.MoveNext());
        }

        [Test]
        public void TestEquals() {
            Assert.AreEqual(new StringList("a", "b"), new StringList("a", "b"));

            Assert.AreNotEqual(new StringList("A", "b"), new StringList("a", "B"));
        }


        [Test]
        public void TestToString() {
            Assert.AreEqual("[a,b]", new StringList("a", "b").ToString());
        }
    }
}