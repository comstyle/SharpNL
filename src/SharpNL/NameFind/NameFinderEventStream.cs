using System;
using System.Collections.Generic;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.NameFind {
    /// <summary>
    /// Class for creating an event stream out of data files for training an name finder.
    /// </summary>
    public class NameFinderEventStream : AbstractEventStream<NameSample> {

        private readonly ISequenceCodec<string> codec;
        private readonly INameContextGenerator contextGenerator;
        private readonly AdditionalContextFeatureGenerator additionalContextFeatureGenerator;

        private static readonly CachedFeatureGenerator defaultGenerators;

        #region + Constructors .
        static NameFinderEventStream() {
            // TODO: check if these are the defaults and run properly
            defaultGenerators = new CachedFeatureGenerator(
                new WindowFeatureGenerator(new TokenFeatureGenerator(), 2, 2),
                new WindowFeatureGenerator(new TokenClassFeatureGenerator(true), 2, 2),
                new OutcomePriorFeatureGenerator(),
                new PreviousMapFeatureGenerator(),
                new BigramNameFeatureGenerator()
                );
        }

        public NameFinderEventStream(IObjectStream<NameSample> dataStream)
            : this(dataStream, new DefaultNameContextGenerator(defaultGenerators), null) {
        }

#if DEBUG
        [Obsolete("The argument type is deprecated and useless in the newest implementations.")]
        // ReSharper disable once UnusedParameter.Local
        public NameFinderEventStream(IObjectStream<NameSample> dataStream, string type, INameContextGenerator contextGenerator,
            ISequenceCodec<string> codec) : this(dataStream, contextGenerator, codec) {
            
        }
#endif

        public NameFinderEventStream(IObjectStream<NameSample> dataStream, INameContextGenerator contextGenerator, ISequenceCodec<string> codec) : base(dataStream) {
            this.codec = codec ?? new BioCodec();

            additionalContextFeatureGenerator = new AdditionalContextFeatureGenerator();

            this.contextGenerator = contextGenerator;
            this.contextGenerator.AddFeatureGenerator(new WindowFeatureGenerator(additionalContextFeatureGenerator, 8, 8));

        }
        #endregion

        #region . AdditionalContext .

        /// <summary>
        /// Generated previous decision features for each token based on contents of the specified map.
        /// </summary>
        /// <param name="tokens">The token for which the context is generated.</param>
        /// <param name="prevMap">A mapping of tokens to their previous decisions.</param>
        /// <returns>An additional context array with features for each token.</returns>
        public static string[][] AdditionalContext(string[] tokens, Dictionary<string, string> prevMap) {
            var ac = new string[tokens.Length][];
            for (int i = 0; i < tokens.Length; i++) {
                ac[i] = new[] { "pd="+ prevMap[tokens[i]] };
            }
            return ac;
        }

        #endregion

        #region . CreateEvents .
        /// <summary>
        /// Creates events for the provided sample.
        /// </summary>
        /// <param name="sample">The sample the sample for which training <see cref="T:Event"/>s are be created.</param>
        /// <returns>The events enumerator.</returns>
        protected override IEnumerator<Event> CreateEvents(NameSample sample) {

            if (sample.ClearAdaptiveData)
                contextGenerator.ClearAdaptiveData();

            var tokens = new string[sample.Sentence.Length];
            var outcomes = codec.Encode(sample.Names, tokens.Length);

            additionalContextFeatureGenerator.SetCurrentContext(sample.AdditionalContext);

            for (int i = 0; i < sample.Sentence.Length; i++) {
                tokens[i] = sample.Sentence[i];
            }

            return GenerateEvents(tokens, outcomes, contextGenerator).GetEnumerator();
        }
        #endregion

        #region . GenerateEvents .
        internal static List<Event> GenerateEvents(String[] sentence, String[] outcomes, INameContextGenerator cg) {
            var events = new List<Event>(outcomes.Length);
            for (int i = 0; i < outcomes.Length; i++) {
                events.Add(new Event(outcomes[i], cg.GetContext(i, sentence, outcomes, null)));
            }
            cg.UpdateAdaptiveData(sentence, outcomes);
            return events;
        }
        #endregion

    }
}
