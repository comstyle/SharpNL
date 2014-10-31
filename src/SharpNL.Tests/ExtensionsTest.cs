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

using System.Collections.Generic;
using System.Text;
using com.sun.jdi;
using NUnit.Framework;

namespace SharpNL.Tests {
    [TestFixture]
    internal class ExtensionsTest {
        [Test]
        public void TestStackToArray() {
            var stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);

            var array = stack.ToArray(2);

            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(3, array[0]);
            Assert.AreEqual(2, array[1]);
            
        }

        [Test]
        public void TestArrayAdd() {

            var array = new[] {1, 2, 3};

            array = array.Add(4);

            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);           
        }

        [Test]
        public void TestAllIndexOf() {
            var values = new[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

            var found = values.AllIndexOf(1, 5, 9);

            Assert.NotNull(found);
            Assert.AreEqual(3, found.Length);

            Assert.AreEqual(1, found[0]);
            Assert.AreEqual(5, found[1]);
            Assert.AreEqual(9, found[2]);

            found = values.AllIndexOf(85, 99);

            Assert.NotNull(found);
            Assert.AreEqual(0, found.Length);
        }

        [Test]
        public void StringBuilderIndexOf() {
            // name of the song that I was listening when I create the function :)
            var sb = new StringBuilder("Blue Pilots Project - Million Clouds");

            Assert.AreEqual(5, sb.IndexOf("Pilots"));
            Assert.AreEqual(20, sb.IndexOf('-'));
            Assert.AreEqual(22, sb.IndexOf("m", 0, true));
        }

    }
}