using System;

using SharpNL.ML.Model;

namespace SharpNL.Utility {

    /// <summary>
    /// Performs k-best search over sequence.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    /// <remarks>This is based on the description in Ratnaparkhi (1998), PhD diss, Univ. of Pennsylvania.</remarks>
    public class BeamSearch<T> {

        private const double zeroLog = -100000;

        protected IBeamSearchContextGenerator<T> cg;
        protected ISequenceValidator<T> validator;
        protected IMaxentModel model;
        protected int size;

        private readonly double[] probs;
        private readonly Cache contextsCache;

        #region + Constructors .

        /// <summary>
        /// Creates new search object.
        /// </summary>
        /// <param name="size">The size of the beam (k).</param>
        /// <param name="cg">The context generator for the model.</param>
        /// <param name="model">The model for assigning probabilities to the sequence outcomes.</param>
        public BeamSearch(int size, IBeamSearchContextGenerator<T> cg, IMaxentModel model)
            : this(size, cg, model, null, 0) {
            
        }

        /// <summary>
        /// Creates new search object.
        /// </summary>
        /// <param name="size">The size of the beam (k).</param>
        /// <param name="cg">The context generator for the model.</param>
        /// <param name="model">The model for assigning probabilities to the sequence outcomes.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public BeamSearch(int size, IBeamSearchContextGenerator<T> cg, IMaxentModel model, int cacheSize)
            : this(size, cg, model, null, cacheSize) {
            
        }


        /// <summary>
        /// Creates new search object.
        /// </summary>
        /// <param name="size">The size of the beam (k).</param>
        /// <param name="cg">The context generator for the model.</param>
        /// <param name="model">The model for assigning probabilities to the sequence outcomes.</param>
        /// <param name="validator">The sequence validator.</param>
        /// <param name="cacheSize">Size of the cache.</param>
        public BeamSearch(int size, IBeamSearchContextGenerator<T> cg, IMaxentModel model, ISequenceValidator<T> validator, int cacheSize) {

            this.cg = cg;
            this.size = size;
            this.model = model;
            this.validator = validator;

            if (cacheSize > 0) {
                contextsCache = new Cache(cacheSize);
            }

            probs = new double[model.GetNumOutcomes()];
        }

        #endregion

        #region . BestSequence .

        /// <summary>
        /// Returns the best sequence of outcomes based on model for this object.
        /// </summary>
        /// <param name="sequence">The input sequence.</param>
        /// <param name="additionalContext">An <see cref="T:object[]"/> of additional context. This is passed to the context generator blindly with the assumption that the context are appropriate.</param>
        /// <returns>The top ranked sequence of outcomes or null if no sequence could be found.</returns>
        public Sequence BestSequence(T[] sequence, object[] additionalContext) {

            var sequences = BestSequences(1, sequence, additionalContext, zeroLog);

            if (sequences.Length > 0) {
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
        /// <returns>The n most probable sequences.</returns>
        public Sequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext) {
            return BestSequences(numSequences, sequence, additionalContext, zeroLog);
        }

        /// <summary>
        /// Finds the n most probable sequences.
        /// </summary>
        /// <param name="numSequences">The number sequences.</param>
        /// <param name="sequence">The sequence.</param>
        /// <param name="additionalContext">The additional context.</param>
        /// <param name="minSequenceScore">The minimum sequence score.</param>
        public Sequence[] BestSequences(int numSequences, T[] sequence, object[] additionalContext, double minSequenceScore) {

            IHeap<Sequence> prev = new ListHeap<Sequence>(size);
            IHeap<Sequence> next = new ListHeap<Sequence>(size);

            prev.Add(new Sequence());

            if (additionalContext == null) {
                additionalContext = new object[] { }; // EMPTY_ADDITIONAL_CONTEXT
            }

            for (int i = 0; i < sequence.Length; i++) {
                int sz = Math.Min(size, prev.Size());

                for (int sc = 0; prev.Size() > 0 && sc < sz; sc++) {
                    var top = prev.Extract();
                    string[] outcomes = top.Outcomes.ToArray();
                    string[] contexts = cg.GetContext(i, sequence, outcomes, additionalContext);
                    double[] scores;

                    if (contextsCache != null) {
                        scores = (double[])contextsCache.Get(contexts);
                        if (scores == null) {
                            scores = model.Eval(contexts, probs);
                            contextsCache.Put(contexts, scores);
                        }
                    } else {
                        scores = model.Eval(contexts, probs);
                    }

                    var temp_scores = new double[scores.Length];
                    for (int c = 0; c < scores.Length; c++) {
                        temp_scores[c] = scores[c];
                    }

                    Array.Sort(temp_scores);

                    double min = temp_scores[Math.Max(0, scores.Length - size)];

                    for (int p = 0; p < scores.Length; p++) {
                        if (scores[p] < min) {
                            continue; //only advance first "size" outcomes
                        }
                        string outcome = model.GetOutcome(p);
                        if (ValidSequence(i, sequence, outcomes, outcome)) {
                            var ns = new Sequence(top, outcome, scores[p]);
                            if (ns.Score > minSequenceScore) {
                                next.Add(ns);
                            }
                        }
                    }

                    if (next.Size() == 0) {
                        //if no advanced sequences, advance all valid
                        for (int p = 0; p < scores.Length; p++) {
                            string outcome = model.GetOutcome(p);
                            if (ValidSequence(i, sequence, outcomes, outcome)) {
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

                IHeap<Sequence> tmp = prev;
                prev = next;
                next = tmp;
            }

            int numSeq = Math.Min(numSequences, prev.Size());
            var topSequences = new Sequence[numSeq];

            for (int seqIndex = 0; seqIndex < numSeq; seqIndex++) {
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
            for (int i = 0; i < outcomes.Length; i++) {
                outcomes[i] = model.GetOutcome(i);
            }
            return outcomes;
        }
        #endregion

        #region . ValidSequence .
        private bool ValidSequence(int index, T[] inputSequence, string[] outcomesSequence, string outcome) {
            if (validator != null) {
                return validator.ValidSequence(index, inputSequence, outcomesSequence, outcome);
            }
            return true;
        }
        #endregion

    }
}
