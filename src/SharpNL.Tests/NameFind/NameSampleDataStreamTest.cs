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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {
    [TestFixture]
    public class NameSampleDataStreamTest {
        private const string person = "person";
        private const string date = "date";
        private const string location = "location";
        private const string organization = "organization";

        private static Span createDefaultSpan(int start, int end) {
            return new Span(start, end, NameSample.DefaultType);
        }

        [Test]
        public void TestWithoutNameTypes() {
            using (var file = Tests.OpenSample("opennlp/tools/namefind/AnnotatedSentences.txt")) {
                var sampleStream = new NameSampleDataStream(new PlainTextByLineStream(file, "ISO-8859-1"));
                var expectedNames = new[] {
                    "Alan McKennedy", "Julie", "Marie Clara",
                    "Stefanie Schmidt", "Mike", "Stefanie Schmidt", "George", "Luise",
                    "George Bauer", "Alisa Fernandes", "Alisa", "Mike Sander",
                    "Stefan Miller", "Stefan Miller", "Stefan Miller", "Elenor Meier",
                    "Gina Schneider", "Bruno Schulz", "Michel Seile", "George Miller",
                    "Miller", "Peter Schubert", "Natalie"
                };

                var names = new List<string>();
                var spans = new List<Span>();
                NameSample ns;
                while ((ns = sampleStream.Read()) != null) {
                    foreach (var name in ns.Names) {                       
                        names.Add(name.GetCoveredText(ns.Sentence));
                        spans.Add(name);
                    }
                    
                }

                Assert.AreEqual(expectedNames.Length, names.Count);
                Assert.AreEqual(createDefaultSpan(6, 8), spans[0]);
                Assert.AreEqual(createDefaultSpan(3, 4), spans[1]);
                Assert.AreEqual(createDefaultSpan(1, 3), spans[2]);
                Assert.AreEqual(createDefaultSpan(4, 6), spans[3]);
                Assert.AreEqual(createDefaultSpan(1, 2), spans[4]);
                Assert.AreEqual(createDefaultSpan(4, 6), spans[5]);
                Assert.AreEqual(createDefaultSpan(2, 3), spans[6]);
                Assert.AreEqual(createDefaultSpan(16, 17), spans[7]);
                Assert.AreEqual(createDefaultSpan(18, 20), spans[8]);
                Assert.AreEqual(createDefaultSpan(0, 2), spans[9]);
                Assert.AreEqual(createDefaultSpan(0, 1), spans[10]);
                Assert.AreEqual(createDefaultSpan(3, 5), spans[11]);
                Assert.AreEqual(createDefaultSpan(3, 5), spans[12]);
                Assert.AreEqual(createDefaultSpan(10, 12), spans[13]);
                Assert.AreEqual(createDefaultSpan(1, 3), spans[14]);
                Assert.AreEqual(createDefaultSpan(6, 8), spans[15]);
                Assert.AreEqual(createDefaultSpan(6, 8), spans[16]);
                Assert.AreEqual(createDefaultSpan(8, 10), spans[17]);
                Assert.AreEqual(createDefaultSpan(12, 14), spans[18]);
                Assert.AreEqual(createDefaultSpan(1, 3), spans[19]);
                Assert.AreEqual(createDefaultSpan(0, 1), spans[20]);
                Assert.AreEqual(createDefaultSpan(2, 4), spans[21]);
                Assert.AreEqual(createDefaultSpan(5, 6), spans[22]);
            }
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWithoutNameTypeAndInvalidData1() {
            var sampleStream = new NameSampleDataStream(new GenericObjectStream<string>(
                "<START> <START> Name <END>"));

            sampleStream.Read();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWithoutNameTypeAndInvalidData2() {
            var sampleStream = new NameSampleDataStream(new GenericObjectStream<string>(
                "<START> Name <END> <END>"));

            sampleStream.Read();
        }


        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWithoutNameTypeAndInvalidData3() {
            var sampleStream = new NameSampleDataStream(new GenericObjectStream<string>(
                "<START> <START> Person <END> Street <END>"));

            sampleStream.Read();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWithNameTypeAndInvalidData1() {
            var sampleStream = new NameSampleDataStream(new GenericObjectStream<string>(
                "<START:> Name <END>"));

            sampleStream.Read();
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWithNameTypeAndInvalidData2() {
            var sampleStream = new NameSampleDataStream(new GenericObjectStream<string>(
                "<START:street> <START:person> Name <END> <END>"));

            sampleStream.Read();
        }

        [Test]
        public void TestWithNameTypes() {
            using (var file = Tests.OpenSample("opennlp/tools/namefind/voa1.train")) {
                var sampleStream = new NameSampleDataStream(new PlainTextByLineStream(file, "ISO-8859-1"));
                var names = new Dictionary<string, List<string>>();
                var spans = new Dictionary<string, List<Span>>();

                NameSample ns;
                while ((ns = sampleStream.Read()) != null) {
                    foreach (var nameSpan in ns.Names) {
                        if (!names.ContainsKey(nameSpan.Type)) {
                            names.Add(nameSpan.Type, new List<string>());
                            spans.Add(nameSpan.Type, new List<Span>());
                        }
                        names[nameSpan.Type].Add(nameSpan.GetCoveredText(ns.Sentence));
                        spans[nameSpan.Type].Add(nameSpan);
                    }
                }

                string[] expectedPerson = {
                    "Barack Obama", "Obama", "Obama",
                    "Lee Myung - bak", "Obama", "Obama", "Scott Snyder", "Snyder", "Obama",
                    "Obama", "Obama", "Tim Peters", "Obama", "Peters"
                };

                string[] expectedDate = {"Wednesday", "Thursday", "Wednesday"};

                string[] expectedLocation = {
                    "U . S .", "South Korea", "North Korea",
                    "China", "South Korea", "North Korea", "North Korea", "U . S .",
                    "South Korea", "United States", "Pyongyang", "North Korea",
                    "South Korea", "Afghanistan", "Seoul", "U . S .", "China"
                };

                string[] expectedOrganization = {"Center for U . S . Korea Policy"};

                Assert.AreEqual(expectedPerson.Length, names[person].Count);
                Assert.AreEqual(expectedDate.Length, names[date].Count);
                Assert.AreEqual(expectedLocation.Length, names[location].Count);
                Assert.AreEqual(expectedOrganization.Length, names[organization].Count);

                Assert.AreEqual(new Span(5, 7, person), spans[person][0]);
                Assert.AreEqual(expectedPerson[0], names[person][0]);
                Assert.AreEqual(new Span(10, 11, person), spans[person][1]);
                Assert.AreEqual(expectedPerson[1], names[person][1]);
                Assert.AreEqual(new Span(29, 30, person), spans[person][2]);
                Assert.AreEqual(expectedPerson[2], names[person][2]);
                Assert.AreEqual(new Span(23, 27, person), spans[person][3]);
                Assert.AreEqual(expectedPerson[3], names[person][3]);
                Assert.AreEqual(new Span(1, 2, person), spans[person][4]);
                Assert.AreEqual(expectedPerson[4], names[person][4]);
                Assert.AreEqual(new Span(8, 9, person), spans[person][5]);
                Assert.AreEqual(expectedPerson[5], names[person][5]);
                Assert.AreEqual(new Span(0, 2, person), spans[person][6]);
                Assert.AreEqual(expectedPerson[6], names[person][6]);
                Assert.AreEqual(new Span(25, 26, person), spans[person][7]);
                Assert.AreEqual(expectedPerson[7], names[person][7]);
                Assert.AreEqual(new Span(1, 2, person), spans[person][8]);
                Assert.AreEqual(expectedPerson[8], names[person][8]);
                Assert.AreEqual(new Span(6, 7, person), spans[person][9]);
                Assert.AreEqual(expectedPerson[9], names[person][9]);
                Assert.AreEqual(new Span(14, 15, person), spans[person][10]);
                Assert.AreEqual(expectedPerson[10], names[person][10]);
                Assert.AreEqual(new Span(0, 2, person), spans[person][11]);
                Assert.AreEqual(expectedPerson[11], names[person][11]);
                Assert.AreEqual(new Span(12, 13, person), spans[person][12]);
                Assert.AreEqual(expectedPerson[12], names[person][12]);
                Assert.AreEqual(new Span(12, 13, person), spans[person][13]);
                Assert.AreEqual(expectedPerson[13], names[person][13]);

                Assert.AreEqual(new Span(7, 8, date), spans[date][0]);
                Assert.AreEqual(expectedDate[0], names[date][0]);
                Assert.AreEqual(new Span(27, 28, date), spans[date][1]);
                Assert.AreEqual(expectedDate[1], names[date][1]);
                Assert.AreEqual(new Span(15, 16, date), spans[date][2]);
                Assert.AreEqual(expectedDate[2], names[date][2]);

                Assert.AreEqual(new Span(0, 4, location), spans[location][0]);
                Assert.AreEqual(expectedLocation[0], names[location][0]);
                Assert.AreEqual(new Span(10, 12, location), spans[location][1]);
                Assert.AreEqual(expectedLocation[1], names[location][1]);
                Assert.AreEqual(new Span(28, 30, location), spans[location][2]);
                Assert.AreEqual(expectedLocation[2], names[location][2]);
                Assert.AreEqual(new Span(3, 4, location), spans[location][3]);
                Assert.AreEqual(expectedLocation[3], names[location][3]);
                Assert.AreEqual(new Span(5, 7, location), spans[location][4]);
                Assert.AreEqual(expectedLocation[4], names[location][4]);
                Assert.AreEqual(new Span(16, 18, location), spans[location][5]);
                Assert.AreEqual(expectedLocation[5], names[location][5]);
                Assert.AreEqual(new Span(1, 3, location), spans[location][6]);
                Assert.AreEqual(expectedLocation[6], names[location][6]);
                Assert.AreEqual(new Span(5, 9, location), spans[location][7]);
                Assert.AreEqual(expectedLocation[7], names[location][7]);
                Assert.AreEqual(new Span(0, 2, location), spans[location][8]);
                Assert.AreEqual(expectedLocation[8], names[location][8]);
                Assert.AreEqual(new Span(4, 6, location), spans[location][9]);
                Assert.AreEqual(expectedLocation[9], names[location][9]);
                Assert.AreEqual(new Span(10, 11, location), spans[location][10]);
                Assert.AreEqual(expectedLocation[10], names[location][10]);
                Assert.AreEqual(new Span(6, 8, location), spans[location][11]);
                Assert.AreEqual(expectedLocation[11], names[location][11]);
                Assert.AreEqual(new Span(4, 6, location), spans[location][12]);
                Assert.AreEqual(expectedLocation[12], names[location][12]);
                Assert.AreEqual(new Span(10, 11, location), spans[location][13]);
                Assert.AreEqual(expectedLocation[13], names[location][13]);
                Assert.AreEqual(new Span(12, 13, location), spans[location][14]);
                Assert.AreEqual(expectedLocation[14], names[location][14]);
                Assert.AreEqual(new Span(5, 9, location), spans[location][15]);
                Assert.AreEqual(expectedLocation[15], names[location][15]);
                Assert.AreEqual(new Span(11, 12, location), spans[location][16]);
                Assert.AreEqual(expectedLocation[16], names[location][16]);

                Assert.AreEqual(new Span(7, 15, organization), spans[organization][0]);
                Assert.AreEqual(expectedOrganization[0], names[organization][0]);

            }
        }


        [Test]
        public void TestClearAdaptiveData() {
            var trainingData = new StringBuilder();
            trainingData.Append("a\n");
            trainingData.Append("b\n");
            trainingData.Append("c\n");
            trainingData.Append("\n");
            trainingData.Append("d\n");

            var untokenizedLineStream = new PlainTextByLineStream(new StringReader(trainingData.ToString()));
            var trainingStream = new NameSampleDataStream(untokenizedLineStream);

            Assert.False(trainingStream.Read().ClearAdaptiveData);
            Assert.False(trainingStream.Read().ClearAdaptiveData);
            Assert.False(trainingStream.Read().ClearAdaptiveData);
            Assert.True(trainingStream.Read().ClearAdaptiveData);
            Assert.Null(trainingStream.Read());
        }

        [Test]
        public void TestHtmlNameSampleParsing() {
            using (var file = Tests.OpenSample("opennlp/tools/namefind/html1.train")) {
                var ds = new NameSampleDataStream(new PlainTextByLineStream(file));

                NameSample ns = ds.Read();

                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("<html>", ns.Sentence[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("<head/>", ns.Sentence[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("<body>", ns.Sentence[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("<ul>", ns.Sentence[0]);

                // <li> <START:organization> Advanced Integrated Pest Management <END> </li>
                ns = ds.Read();
                Assert.AreEqual(6, ns.Sentence.Length);
                Assert.AreEqual("<li>", ns.Sentence[0]);
                Assert.AreEqual("Advanced", ns.Sentence[1]);
                Assert.AreEqual("Integrated", ns.Sentence[2]);
                Assert.AreEqual("Pest", ns.Sentence[3]);
                Assert.AreEqual("Management", ns.Sentence[4]);
                Assert.AreEqual("</li>", ns.Sentence[5]);
                Assert.AreEqual(new Span(1, 5, organization), ns.Names[0]);

                // <li> <START:organization> Bay Cities Produce Co., Inc. <END> </li>
                ns = ds.Read();
                Assert.AreEqual(7, ns.Sentence.Length);
                Assert.AreEqual("<li>", ns.Sentence[0]);
                Assert.AreEqual("Bay", ns.Sentence[1]);
                Assert.AreEqual("Cities", ns.Sentence[2]);
                Assert.AreEqual("Produce", ns.Sentence[3]);
                Assert.AreEqual("Co.,", ns.Sentence[4]);
                Assert.AreEqual("Inc.", ns.Sentence[5]);
                Assert.AreEqual("</li>", ns.Sentence[6]);
                Assert.AreEqual(new Span(1, 6, organization), ns.Names[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("</ul>", ns.Sentence[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("</body>", ns.Sentence[0]);

                ns = ds.Read();
                Assert.AreEqual(1, ns.Sentence.Length);
                Assert.AreEqual("</html>", ns.Sentence[0]);

                Assert.Null(ds.Read());
            }
        }
    }
}