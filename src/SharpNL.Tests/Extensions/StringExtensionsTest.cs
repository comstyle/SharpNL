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
using SharpNL.Extensions;

namespace SharpNL.Tests.Extensions {
    [TestFixture]
    internal class StringExtensionsTest {

        [Test]
        public void LeftTest() {

            const string text = "forgiveness";

            Assert.AreEqual("forgive", text.Left(7));

            Assert.AreEqual("forgive", text.Left(-4));
        }

        [Test]
        public void RightTest() {
            const string text = "freely";

            Assert.AreEqual("ly", text.Right(2));

            Assert.AreEqual("eely", text.Right(-2)); 


        }

        #region . TrimEnd .

        [Test]
        public void TrimEndTest() {

            const string text = "abcdeefg";

            Assert.AreEqual("abcde", text.TrimEnd("efg"));
            Assert.AreEqual("abcdeefg", text.TrimEnd("xxx"));
        }

        #endregion
       
    }
}