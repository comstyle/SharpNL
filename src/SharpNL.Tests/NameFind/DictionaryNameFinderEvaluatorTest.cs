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
using NUnit.Framework;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Tests.NameFind {
    [TestFixture]
    internal class DictionaryNameFinderEvaluatorTest {

        private static IObjectStream<NameSample> CreateSample() {
            return
                new NameSampleStream(
                    new PlainTextByLineStream(Tests.OpenFile("opennlp/tools/namefind/AnnotatedSentences.txt"),
                        "ISO-8859-1"));
        }

        private static SharpNL.Dictionary.Dictionary CreateDictionary() {
            var sampleStream = CreateSample();
            var sample = sampleStream.Read();
            var entries = new List<string[]>();

            while (sample != null) {
                Span[] names = sample.Names;
                if (names != null && names.Length > 0) {
                    var toks = sample.Sentence;
                    foreach (Span name in names) {
                        var nameToks = new string[name.Length];
                        Array.Copy(toks, name.Start, nameToks, 0, name.Length);
                        entries.Add(nameToks);
                    }
                }
                sample = sampleStream.Read();
            }
            sampleStream.Dispose();
            var dictionary = new SharpNL.Dictionary.Dictionary(true);
            foreach (var entry in entries) {
                var dicEntry = new StringList(entry);
                dictionary.Add(dicEntry);
            }
            return dictionary;
        }

        [Test]
        public void TestEvaluator() {
            var nameFinder = new DictionaryNameFinder(CreateDictionary());

            // TODO: Add the evaluation listener.
            var evaluator = new TokenNameFinderEvaluator(nameFinder);
            var sample = CreateSample();

            evaluator.Evaluate(sample);
            sample.Dispose();

            Assert.AreEqual(1d, evaluator.FMeasure.Value);
            Assert.AreEqual(1d, evaluator.FMeasure.RecallScore);
        }

    }
}