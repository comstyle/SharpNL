// /*
//  * Copyright 2014 Gustavo J Knuppe (https://github.com/knuppe)
//  *
//  *  Licensed under the Apache License, Version 2.0 (the "License");
//  *  you may not use this file except in compliance with the License.
//  *  You may obtain a copy of the License at
//  *
//  *      http://www.apache.org/licenses/LICENSE-2.0
//  *
//  *  Unless required by applicable law or agreed to in writing, software
//  *  distributed under the License is distributed on an "AS IS" BASIS,
//  *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  *  See the License for the specific language governing permissions and
//  *  limitations under the License.
//  *
//  *  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  *  - May you do good and not evil.                                         -
//  *  - May you find forgiveness for yourself and forgive others.             -
//  *  - May you share freely, never taking more than you give.                -
//  *  - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
//  * 
//  */

using System.Collections.Generic;
using NUnit.Framework;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    internal class BeamSearchTest {
        private class IdentityFeatureGenerator : IBeamSearchContextGenerator<string> {
            private readonly string[] outcomeSequence;


            public IdentityFeatureGenerator(string[] outcomeSequence) {
                this.outcomeSequence = outcomeSequence;
            }

            public string[] GetContext(int index, string[] sequence, string[] priorDecisions, object[] additionalContext) {
                return new[] {outcomeSequence[index]};
            }
        }

        private class SequenceValidator : ISequenceValidator<string> {
            public bool ValidSequence(int index, string[] inputSequence, string[] outcomesSequence, string outcome) {
                return !outcome.Equals("2");
            }
        }

        private class IdentityModel : IMaxentModel {
            private const double bestOutcomeProb = 0.8d;

            private readonly string[] oc;
            private readonly double otherOutcomeProb;
            private readonly Dictionary<string, int> outcomeIndexMap = new Dictionary<string, int>();

            public IdentityModel(string[] outcomes) {
                oc = outcomes;

                for (var i = 0; i < outcomes.Length; i++) {
                    outcomeIndexMap[outcomes[i]] = i;
                }

                otherOutcomeProb = 0.2d/(outcomes.Length - 1);
            }

            public double[] Eval(string[] context) {
                var probs = new double[oc.Length];

                for (var i = 0; i < probs.Length; i++) {
                    if (oc[i].Equals(context[0])) {
                        probs[i] = bestOutcomeProb;
                    } else {
                        probs[i] = otherOutcomeProb;
                    }
                }

                return probs;
            }

            public double[] Eval(string[] context, double[] probs) {
                return Eval(context);
            }

            public double[] Eval(string[] context, float[] probs) {
                return Eval(context);
            }

            public string GetBestOutcome(double[] outcomes) {
                return null;
            }

            public string GetOutcome(int index) {
                return oc[index];
            }

            public int GetIndex(string outcome) {
                return 0;
            }

            public int GetNumOutcomes() {
                return oc.Length;
            }
        }

        /// <summary>Tests finding the best sequence on a short input sequence.</summary>
        [Test]
        public void testBestSequence() {
            string[] sequence = {"1", "2", "3", "2", "1"};
            var cg = new IdentityFeatureGenerator(sequence);

            var outcomes = new[] {"1", "2", "3"};
            var model = new IdentityModel(outcomes);

            var bs = new BeamSearch<string>(2, cg, model);

            var seq = bs.BestSequence(sequence, null);

            Assert.NotNull(seq);
            Assert.AreEqual(sequence.Length, seq.Outcomes.Count);
            Assert.AreEqual("1", seq.Outcomes[0]);
            Assert.AreEqual("2", seq.Outcomes[1]);
            Assert.AreEqual("3", seq.Outcomes[2]);
            Assert.AreEqual("2", seq.Outcomes[3]);
            Assert.AreEqual("1", seq.Outcomes[4]);
        }

        /// <summary>Tests finding a sequence of length one.</summary>
        [Test]
        public void testBestSequenceOneElementInput() {
            var sequence = new[] {"1"};

            var cg = new IdentityFeatureGenerator(sequence);

            var outcomes = new[] {"1", "2", "3"};

            var model = new IdentityModel(outcomes);

            var bs = new BeamSearch<string>(3, cg, model);

            var seq = bs.BestSequence(sequence, null);

            Assert.NotNull(seq);
            Assert.AreEqual(sequence.Length, seq.Outcomes.Count);
            Assert.AreEqual("1", seq.Outcomes[0]);
        }

        /// <summary>Tests finding the best sequence on a short input sequence.</summary>
        [Test]
        public void testBestSequenceWithValidator() {
            var sequence = new[] {"1", "2", "3", "2", "1"};
            var cg = new IdentityFeatureGenerator(sequence);

            var outcomes = new[] {"1", "2", "3"};
            var model = new IdentityModel(outcomes);

            var bs = new BeamSearch<string>(2, cg, model, new SequenceValidator(), 0);

            var seq = bs.BestSequence(sequence, null);

            Assert.NotNull(seq);
            Assert.AreEqual(sequence.Length, seq.Outcomes.Count);
            Assert.AreEqual("1", seq.Outcomes[0]);
            Assert.AreNotSame("2", seq.Outcomes[1]);
            Assert.AreEqual("3", seq.Outcomes[2]);
            Assert.AreNotSame("2", seq.Outcomes[3]);
            Assert.AreEqual("1", seq.Outcomes[4]);
        }

        /// <summary>Tests that beam search does not fail to detect an empty sequence.</summary>
        [Test]
        public void testBestSequenceZeroLengthInput() {
            var sequence = new string[0];
            IBeamSearchContextGenerator<string> cg = new IdentityFeatureGenerator(sequence);

            var outcomes = new[] {"1", "2", "3"};
            var model = new IdentityModel(outcomes);

            var bs = new BeamSearch<string>(3, cg, model);

            var seq = bs.BestSequence(sequence, null);

            Assert.NotNull(seq);
            Assert.AreEqual(sequence.Length, seq.Outcomes.Count);
        }
    }
}