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
using SharpNL.Utility.Evaluation;

namespace SharpNL.Tests.Utility.Evaluation {
    [TestFixture]
    public class MeanTest {
        [Test]
        public void TestMean() {
            var a = new Mean();
            a.Add(1);

            Assert.AreEqual(1, a.Count);
            Assert.AreEqual(1d, a.Value, 0.00001d);

            a.Add(1);
            Assert.AreEqual(2, a.Count);
            Assert.AreEqual(1d, a.Value, 0.00001d);
            var a1 = a.ToString();

            var b = new Mean();
            b.Add(0.5);
            Assert.AreEqual(1, b.Count);
            Assert.AreEqual(0.5d, b.Value, 0.00001d);

            b.Add(2);
            Assert.AreEqual(2, b.Count);
            Assert.AreEqual(1.25d, b.Value, 0.00001d);
            var b1 = b.ToString();

            var c = new Mean();
            Assert.AreEqual(0, c.Count);
            Assert.AreEqual(0d, c.Value, 0.00001d);
            var c1 = c.ToString();

        }
    }
}