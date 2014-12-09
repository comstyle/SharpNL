
using System;
using NUnit.Framework;
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    
    [TestFixture]
    class SequenceTest {

        [Test]
        public void testAddMethod() {
            var sequence = new Sequence();
            sequence.Add("a", 10d);

            Assert.AreEqual("a", sequence.Outcomes[0]);
            Assert.AreEqual(10d, sequence.Probabilities[0]);
        }

        [Test]
        public void testCompareTo() {
            var lowScore = new Sequence();
            lowScore.Add("A", 1d);
            lowScore.Add("B", 2d);
            lowScore.Add("C", 3d);

            var highScore = new Sequence();
            lowScore.Add("A", 7d);
            lowScore.Add("B", 8d);
            lowScore.Add("C", 9d);


            Assert.AreEqual(-1, lowScore.CompareTo(highScore));
            Assert.AreEqual(1, highScore.CompareTo(lowScore));
            Assert.AreEqual(0, highScore.CompareTo(highScore));
        }

        [Test]
        public void testClone() {

            var sequence = new Sequence();
            sequence.Add("a", 10);
            sequence.Add("b", 20);

            var copy = sequence.Clone() as Sequence;

            Assert.NotNull(copy);

            Assert.True(sequence.Outcomes.SequenceEqual(copy.Outcomes));
            Assert.True(sequence.Probabilities.SequenceEqual(copy.Probabilities));
            Assert.True(sequence.CompareTo(copy) == 0);
        }

    }
}
