using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpNL.Tokenize;

using Operation = SharpNL.Tokenize.DetokenizationDictionary.Operation;

namespace SharpNL.Tests.Tokenize {
    [TestFixture]
    public class DetokenizationDictionaryTest {
        private DetokenizationDictionary dict;

        [TestFixtureSetUp]
        public void Setup() {
            dict = new DetokenizationDictionary {
                {"\"", Operation.RightLeftMatching},
                {"(", Operation.MoveRight},
                {")", Operation.MoveLeft},
                {"-", Operation.MoveBoth}
            };
        }

        private static void TestEntries(DetokenizationDictionary dict) {
            Assert.AreEqual(Operation.RightLeftMatching, dict["\""]);
            Assert.AreEqual(Operation.MoveRight, dict["("]);
            Assert.AreEqual(Operation.MoveLeft, dict[")"]);
            Assert.AreEqual(Operation.MoveBoth, dict["-"]);
        }

        [Test]
        public void TestSimpleDict() {
            TestEntries(dict);
        }

        [Test]
        public void TestSerialization() {

            using (var data = new MemoryStream()) {
                dict.Serialize(data);
                data.Seek(0, SeekOrigin.Begin);

                var parsedDict = new DetokenizationDictionary(data);

                TestEntries(parsedDict);

            }

        }

    }
}
