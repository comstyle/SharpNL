using System.IO;
using System.Security.Cryptography;
using NUnit.Framework;
using SharpNL.Parser;
using SharpNL.Utility;

namespace SharpNL.Tests.Parser.Chunking {

    [TestFixture]
    public class ParserTest {

        [Test]
        public void TestChunkingParserTraining() {

            var parseSamples = ParserTestUtil.CreateParseTestStream();
            //var testSamples = ParserTestUtil.CreateParseTestStream();
            var headRules = ParserTestUtil.CreateTestHeadRules();

            var model = SharpNL.Parser.Chunking.Parser.Train("en", parseSamples, headRules, 100, 0);

            Assert.NotNull(model);

            var parser = ParserFactory.Create(model);

            Assert.NotNull(parser);

            ParserModel deserialized;
            using (var stream = new MemoryStream()) {

                model.Serialize(new UnclosableStream(stream));

                stream.Seek(0, SeekOrigin.Begin);

                deserialized = new ParserModel(stream);
            }

            Assert.NotNull(deserialized);

            // TODO: compare both models
        }
    }
}
