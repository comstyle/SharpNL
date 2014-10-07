using SharpNL.Utility;

using NUnit.Framework;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    public class ListHeapTest {

        [Test]
        public void TestSimple() {

            int size = 5;

            var heap = new ListHeap<int>(size);

            for (int ai = 0; ai < 10; ai++) {

                if (ai < size)
                    Assert.AreEqual(ai, heap.Size());
                else
                    Assert.AreEqual(size, heap.Size());

                heap.Add(ai);
            }

            Assert.AreEqual(0, heap.Extract());
            Assert.AreEqual(4, heap.Size());

            Assert.AreEqual(1, heap.Extract());
            Assert.AreEqual(3, heap.Size());

            Assert.AreEqual(2, heap.Extract());
            Assert.AreEqual(2, heap.Size());

            Assert.AreEqual(3, heap.Extract());
            Assert.AreEqual(1, heap.Size());

            Assert.AreEqual(4, heap.Extract());
            Assert.AreEqual(0, heap.Size());


        }


    }
}
