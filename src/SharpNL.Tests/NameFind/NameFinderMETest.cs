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
using System.Linq;
using NUnit.Framework;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {

    /// <summary>
    /// This is the test class for <see cref="NameFinderME"/>.
    /// <para>
    /// A proper testing and evaluation of the name finder is only possible  with a large 
    /// corpus which contains a huge amount of test sentences.</para>
    /// <para>
    /// The scope of this test is to make sure that the name finder
    /// code can be executed. This test can not detect
    /// mistakes which lead to incorrect feature generation
    /// or other mistakes which decrease the tagging
    /// performance of the name finder.</para>
    /// <para>
    /// In this test the <see cref="NameFinderME"/> is trained with
    /// a small amount of training sentences and then the
    /// computed model is used to predict sentences from the
    /// training sentences.</para>
    /// </summary>
    [TestFixture]
    internal class NameFinderMETest {
        private const string Type = "default";

        private static bool HasOtherAsOutcome(TokenNameFinderModel nameFinderModel) {
            var outcomes = nameFinderModel.NameFinderSequenceModel.GetOutcomes();
            return outcomes.Any(s => s.Equals(NameFinderME.Other));
        }


        [Test]
        public void TestNameFinder() {

            using (var file = Tests.OpenFile("opennlp/tools/namefind/AnnotatedSentences.txt")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file, "ISO-8859-1"));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences
                var sentence = new[] {
                    "Alisa",
                    "appreciated",
                    "the",
                    "hint",
                    "and",
                    "enjoyed",
                    "a",
                    "delicious",
                    "traditional",
                    "meal."
                };

                var names = nameFinder.Find(sentence);

                Assert.AreEqual(1, names.Length);
                Assert.AreEqual(new Span(0, 1, Type), names[0]);

                sentence = new[] {
                    "Hi",
                    "Mike",
                    ",",
                    "it's",
                    "Stefanie",
                    "Schmidt",
                    "."
                };

                names = nameFinder.Find(sentence);

                Assert.AreEqual(2, names.Length);
                Assert.AreEqual(new Span(1, 2, Type), names[0]);
                Assert.AreEqual(new Span(4, 6, Type), names[1]);

            }
        }

        #region . TestNameFinderWithTypes .

        /// <summary>
        /// Train <see cref="NameFinderME"/> using AnnotatedSentencesWithTypes.txt with "person"
        /// nameType and try the model in a sample text.
        /// </summary>
        [Test]
        public void TestNameFinderWithTypes() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/AnnotatedSentencesWithTypes.txt")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file, "ISO-8859-1"));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences

                var sentence = new[] { "Alisa", "appreciated", "the", "hint", "and", "enjoyed", "a", "delicious", "traditional", "meal." };
                var names = nameFinder.Find(sentence);

                Assert.AreEqual(1, names.Length);
                Assert.AreEqual(new Span(0, 1, "person"), names[0]);
                Assert.True(HasOtherAsOutcome(model));

                sentence = new[] { "Hi", "Mike", ",", "it's", "Stefanie", "Schmidt", "." };
                names = nameFinder.Find(sentence);

                Assert.AreEqual(2, names.Length);
                Assert.AreEqual(new Span(1, 2, "person"), names[0]);
                Assert.AreEqual(new Span(4, 6, "person"), names[1]);
                Assert.AreEqual("person", names[0].Type);
                Assert.AreEqual("person", names[1].Type);

            }
        }
        #endregion

        #region . TestOnlyWithNames .

        /// <summary>
        /// Train <see cref="NameFinderME"/> using OnlyWithNames.train. The goal is to check if the model validator accepts it.
        /// This is related to the issue OPENNLP-9
        /// </summary>
        [Test]
        public void TestOnlyWithNames() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/OnlyWithNames.train")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences
                var sentence = WhitespaceTokenizer.Instance.Tokenize(
                    "Neil Abercrombie Anibal Acevedo-Vila Gary Ackerman Robert Aderholt " +
                    "Daniel Akaka Todd Akin Lamar Alexander Rodney Alexander");
                
                var names = nameFinder.Find(sentence);

                Assert.AreEqual(new Span(0, 2, Type), names[0]);
                Assert.AreEqual(new Span(2, 4, Type), names[1]);
                Assert.AreEqual(new Span(4, 6, Type), names[2]);
                Assert.True(!HasOtherAsOutcome(model));
            }
        }

        #endregion

        [Test]
        public void TestOnlyWithNamesWithTypes() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/OnlyWithNamesWithTypes.train")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences
                var sentence = WhitespaceTokenizer.Instance.Tokenize(
                    "Neil Abercrombie Anibal Acevedo-Vila Gary Ackerman Robert Aderholt " +
                    "Daniel Akaka Todd Akin Lamar Alexander Rodney Alexander");

                var names = nameFinder.Find(sentence);

                Assert.AreEqual(new Span(0, 2, "person"), names[0]);
                Assert.AreEqual(new Span(2, 4, "person"), names[1]);
                Assert.AreEqual(new Span(4, 6, "person"), names[2]);
                Assert.True(!HasOtherAsOutcome(model));
            }
        }


        /// <summary>
        /// Train <see cref="NameFinderME"/> using OnlyWithNames.train.
        /// The goal is to check if the model validator accepts it.
        /// This is related to the issue OPENNLP-9
        /// </summary>
        [Test]
        public void TestOnlyWithEntitiesWithTypes() {

            using (var file = Tests.OpenFile("opennlp/tools/namefind/OnlyWithEntitiesWithTypes.train")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences
                var sentence = WhitespaceTokenizer.Instance.Tokenize("NATO United States Barack Obama");

                var names = nameFinder.Find(sentence);

                Assert.AreEqual(new Span(0, 1, "organization"), names[0]);
                Assert.AreEqual(new Span(1, 3, "location"), names[1]);
                Assert.AreEqual(new Span(3, 5, "person"), names[2]);
                Assert.False(HasOtherAsOutcome(model));
            }
        }

        /// <summary>
        /// Train <see cref="NameFinderME"/> using voa1.train with several nameTypes and try the model in a sample text.
        /// </summary>
        [Test]
        public void TestNameFinderWithMultipleTypes() {
            using (var file = Tests.OpenFile("opennlp/tools/namefind/voa1.train")) {
                var sampleStream = new NameSampleStream(new PlainTextByLineStream(file));

                var param = new TrainingParameters();
                param.Set(Parameters.Iterations, "70");
                param.Set(Parameters.Cutoff, "1");

                var model = NameFinderME.Train(
                    "en",
                    sampleStream,
                    param,
                    new TokenNameFinderFactory(null, new Dictionary<string, object>()));

                var nameFinder = new NameFinderME(model);

                // now test if it can detect the sample sentences
                var sentence = new [] { "U", ".", "S", ".", "President", "Barack", "Obama", "has", 
                    "arrived", "in", "South", "Korea", ",", "where", "he", "is", "expected", "to", 
                    "show", "solidarity", "with", "the", "country", "'", "s", "president", "in",
                    "demanding", "North", "Korea", "move", "toward", "ending", "its", "nuclear", 
                    "weapons", "programs", "." };

                var names = nameFinder.Find(sentence);

                Assert.AreEqual(4, names.Length);
                Assert.AreEqual(new Span(0, 4, "location"), names[0]);
                Assert.AreEqual(new Span(5, 7, "person"), names[1]);
                Assert.AreEqual(new Span(10, 12, "location"), names[2]);
                Assert.AreEqual(new Span(28, 30, "location"), names[3]);

                /*
                These asserts are not needed because the equality comparer handles the Type 
                assertEquals("location", names1[0].getType());
                assertEquals("person", names1[1].getType());
                assertEquals("location", names1[2].getType());
                assertEquals("location", names1[3].getType());
                 */

                sentence = new[] {
                    "Scott", "Snyder", "is", "the", "director", "of", "the", 
                    "Center", "for", "U", ".", "S", ".", "Korea", "Policy", "."
                };

                names = nameFinder.Find(sentence);

                Assert.AreEqual(2, names.Length);
                Assert.AreEqual(new Span(0, 2, "person"), names[0]);
                Assert.AreEqual(new Span(7, 15, "organization"), names[1]);

                /* 
                 
                assertEquals("person", names2[0].getType());
                assertEquals("organization", names2[1].getType());
                 
                */
            }
        }           

    }
}