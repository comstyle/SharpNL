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
using SharpNL.ML.Model;
using SharpNL.Utility;

using Sequence = SharpNL.Utility.Sequence;

namespace SharpNL.ML {
    /// <summary>
    /// Performs k-best search over sequence.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <remarks>This is based on the description in Ratnaparkhi (1998), PhD diss, Univ. of Pennsylvania.</remarks>
    public class BeamSearch<T> : Model.ISequenceClassificationModel<T> {
        private const double zeroLog = -100000;
        private readonly Cache contextsCache;
        private readonly double[] probs;

        protected IMaxentModel model;
        protected int size;

        #region + Constructors .

        /// <summary>
        /// Creates new search object.
        /// </summary>
        /// <param name="size">The size of the beam (k).</param>
        /// <param name="model">The model for assigning probabilities to the sequence outcomes.</param>
        public BeamSearch(int size, IMaxentModel model) : this(size, model, 0) {}


        /// <summary>
        /// Creates new search object with the specified cache size.
        /// </summary>
        /// <param name="size">The size of the beam (k).</param>
        /// <param name="model">The model for assigning probabilities to the sequence outcomes.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public BeamSearch(int size, IMaxentModel model, int cacheSize) {
            this.size = size;
            this.model = model;

            if (cacheSize > 0) {
                contextsCache = new Cache(cacheSize);
            }

            probs = new double[model.GetNumOutcomes()];
        }

        #endregion

        #region . BestSequence .

        /// <summary>
        /// Finds the sequence with the highest probability.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>The sequence with the highest probability.</returns>
        public Sequence BestSequence(T[] sequence, object[] additionalContext,
            IBeamSearchContextGenerator<T> beamSearch,
            ISequenceValidator<T> validator) {
            var sequences = BestSequences(1, sequence, additionalContext, beamSearch, validator);

            if (sequence.Length > 0) {
                return sequences[0];
            }

            return null;
        }

        #endregion

        #region + BestSequences .

        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        /// <returns>The n most probable sequences.</returns>
        public Sequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext,
            IBeamSearchContextGenerator<T> beamSearch, ISequenceValidator<T> validator) {
            return BestSequences(numSequences, sequence, additionalContext, zeroLog, beamSearch, validator);
        }

        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="minSequenceScore">The minimum sequence score.</param>
        /// <param name="beamSearch">The beam search.</param>
        /// <param name="validator">The validator.</param>
        public Sequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext,
            double minSequenceScore,
            IBeamSearchContextGenerator<T> beamSearch, ISequenceValidator<T> validator) {
            IHeap<Sequence> prev = new ListHeap<Sequence>(size);
            IHeap<Sequence> next = new ListHeap<Sequence>(size);

            prev.Add(new Sequence());

            if (additionalContext == null) {
                additionalContext = new object[] {}; // EMPTY_ADDITIONAL_CONTEXT
            }

            for (var i = 0; i < sequence.Length; i++) {
                var sz = Math.Min(size, prev.Size());

                for (var sc = 0; prev.Size() > 0 && sc < sz; sc++) {
                    var top = prev.Extract();

                    var tmpOutcomes = top.Outcomes;
                    var outcomes = tmpOutcomes.ToArray();
                    var contexts = beamSearch.GetContext(i, sequence, outcomes, additionalContext);
                    double[] scores;
                    if (contextsCache != null) {
                        scores = (double[]) contextsCache.Get(contexts);
                        if (scores == null) {
                            scores = model.Eval(contexts, probs);
                            contextsCache.Put(contexts, scores);
                        }
                    } else {
                        scores = model.Eval(contexts, probs);
                    }

                    var temp_scores = new double[scores.Length];
                    for (var c = 0; c < scores.Length; c++) {
                        temp_scores[c] = scores[c];
                    }

                    Array.Sort(temp_scores);

                    var min = temp_scores[Math.Max(0, scores.Length - size)];

                    for (var p = 0; p < scores.Length; p++) {
                        if (scores[p] < min) {
                            continue; //only advance first "size" outcomes
                        }
                        var outcome = model.GetOutcome(p);
                        if (validator.ValidSequence(i, sequence, outcomes, outcome)) {
                            var ns = new Sequence(top, outcome, scores[p]);
                            if (ns.Score > minSequenceScore) {
                                next.Add(ns);
                            }
                        }
                    }

                    if (next.Size() == 0) {
                        //if no advanced sequences, advance all valid
                        for (var p = 0; p < scores.Length; p++) {
                            var outcome = model.GetOutcome(p);
                            if (validator.ValidSequence(i, sequence, outcomes, outcome)) {
                                var ns = new Sequence(top, outcome, scores[p]);
                                if (ns.Score > minSequenceScore) {
                                    next.Add(ns);
                                }
                            }
                        }
                    }
                }

                // make prev = next; and re-init next (we reuse existing prev set once we clear it)
                prev.Clear();

                var tmp = prev;
                prev = next;
                next = tmp;
            }

            var numSeq = Math.Min(numSequences, prev.Size());
            var topSequences = new Sequence[numSeq];

            for (var seqIndex = 0; seqIndex < numSeq; seqIndex++) {
                topSequences[seqIndex] = prev.Extract();
            }

            return topSequences;
        }

        #endregion

        #region . GetOutcomes .

        /// <summary>
        /// Gets all possible outcomes.
        /// </summary>
        /// <returns>all possible outcomes.</returns>
        public string[] GetOutcomes() {
            var outcomes = new string[model.GetNumOutcomes()];
            for (var i = 0; i < outcomes.Length; i++) {
                outcomes[i] = model.GetOutcome(i);
            }
            return outcomes;
        }

        #endregion
    }
}