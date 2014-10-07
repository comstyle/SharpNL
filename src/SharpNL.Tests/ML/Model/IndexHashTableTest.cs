using NUnit.Framework;
using SharpNL.ML.Model;

namespace SharpNL.Tests.ML.Model {
    [TestFixture]
    internal class IndexHashTableTest {

        [Test]
        public void testWithoutCollision() {

            var array = new[] {"4", "7", "5"};

            var arrayIndex = new IndexHashTable<string>(array, 1d);

            for (int i = 0; i < array.Length; i++) {
                Assert.AreEqual(i, arrayIndex[array[i]]);
            }

        }

        [Test]
        public void testWitCollision() {

            var array = new[] {"7", "21", "0"};

            var arrayIndex = new IndexHashTable<string>(array, 1d);

            for (int i = 0; i < array.Length; i++) {
                Assert.AreEqual(i, arrayIndex[array[i]]);
            }

            // has the same slot as as ""
            Assert.AreEqual(-1, arrayIndex["4"]);
        }
    }
}
