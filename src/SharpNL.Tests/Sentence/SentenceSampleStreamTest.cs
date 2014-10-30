
using NUnit.Framework;
using SharpNL.SentenceDetector;
using SharpNL.Utility;

namespace SharpNL.Tests.Sentence {

    [TestFixture]
    class SentenceSampleStreamTest {

        [Test]
        public void TestStream() {
            using (var file = Tests.OpenFile("/opennlp/tools/sentdetect/Sentences.txt")) {
                var stream = new SentenceSampleStream(new PlainTextByLineStream(file));

                var sample = stream.Read();
                
                Assert.NotNull(sample);
                Assert.AreEqual(sample.Sentences.Length, 5);

                var a = sample.Sentences[0].GetCoveredText(sample.Document);
                var b = sample.Sentences[1].GetCoveredText(sample.Document);
                var c = sample.Sentences[2].GetCoveredText(sample.Document);

                Assert.AreEqual("Last September, I tried to find out the address of an old school friend whom I hadnt't seen for 15 years.", a);
                Assert.AreEqual("I just knew his name, Alan McKennedy, and I'd heard the rumour that he'd moved to Scotland, the country of his ancestors.", b);
                Assert.AreEqual("So I called Julie, a friend who's still in contact with him.", c);

            }
        }

    }
}
