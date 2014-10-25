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
using System.Text.RegularExpressions;
using NUnit.Framework;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tests.ML {
    internal static class PrepAttachDataUtility {
        private static IEnumerable<Event> readPpaFile(String filename) {
            var events = new List<Event>();

            using (var reader = new StreamReader(Tests.OpenFile("opennlp/data/ppa/" + filename))) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var items = Regex.Split(line, "\\s+");

                    events.Add(new Event(items[5], new[] {
                        "verb=" + items[1], "noun=" + items[2], "prep=" + items[3], "prep_obj=" + items[4]
                    }));
                }
            }
            return events;
        }

        internal static IObjectStream<Event> CreateTrainingStream() {
            return new GenericObjectStream<Event>(readPpaFile("training"));
        }

        internal static void TestModel(IMaxentModel model, double expecedAccuracy) {
            var devEvents = readPpaFile("devset");

            var total = 0;
            var correct = 0;
            foreach (var ev in devEvents) {
                //String targetLabel = ev.getOutcome();
                var ocs = model.Eval(ev.Context);

                var best = 0;
                for (var i = 1; i < ocs.Length; i++)
                    if (ocs[i] > ocs[best])
                        best = i;

                var predictedLabel = model.GetOutcome(best);

                if (ev.Outcome.Equals(predictedLabel))
                    correct++;
                total++;
            }

            var accuracy = correct/(double) total;

            Console.Out.WriteLine("Accuracy on PPA devSet: (" + correct + "/" + total + ") " + accuracy);

            Assert.AreEqual(expecedAccuracy, accuracy, .00001);
        }
    }
}