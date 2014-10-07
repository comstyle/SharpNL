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

namespace SharpNL.Tests.Utility {
    using SharpNL.Utility;

    [TestFixture]
    public class PropertiesSeriealizerTest {

        private static Properties prop;

        [TestFixtureSetUp]
        public void Setup() {
            prop = new Properties();
            prop["one"] = "1";
            prop["two"] = "2";
        }

        [Test]
        public void TestSerializer() {

            using (var stream = new MemoryStream()) {
                
                prop.Save(new UnclosableStream(stream));

                stream.Seek(0, SeekOrigin.Begin);

                var other = new Properties();
                other.Load(stream);

                Assert.AreEqual(prop.Count, other.Count);
                Assert.AreEqual(prop, other);
            }
            
           


        }

    }
}