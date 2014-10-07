using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNL.ML.Model;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;
using Sequence = SharpNL.ML.Model.Sequence;

namespace SharpNL.NameFind {
    public class NameSampleSequenceStream : ISequenceStream {

          private INameContextGenerator pcg;
          private readonly bool useOutcomes;
          private IObjectStream<NameSample> psi;
          private ISequenceCodec<String> seqCodec;


        /*
        public NameSampleSequenceStream(IObjectStream<NameSample> psi)
            : this(psi, new DefaultNameContextGenerator(), true) {
            
        }
        */

        public NameSampleSequenceStream(IObjectStream<NameSample> psi, IAdaptiveFeatureGenerator featureGen)
            : this(psi, new DefaultNameContextGenerator(featureGen), true) {
            
        }

        public NameSampleSequenceStream(IObjectStream<NameSample> psi, IAdaptiveFeatureGenerator featureGen,
            bool useOutcomes) : this(psi, new DefaultNameContextGenerator(featureGen), useOutcomes) {
            
        }

        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg)
            : this(psi, pcg, true) {
            
        }

        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg, bool useOutcomes)
            : this(psi, pcg, useOutcomes, new BioCodec()) {
            
        }

        public NameSampleSequenceStream(IObjectStream<NameSample> psi, INameContextGenerator pcg, bool useOutcomes, ISequenceCodec<string> seqCodec) {
            this.psi = psi;
            this.useOutcomes = useOutcomes;
            this.pcg = pcg;
            this.seqCodec = seqCodec;
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public Sequence Read() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new event array based on the outcomes predicted by the specified parameters for the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence to be evaluated.</param>
        /// <param name="model">The model.</param>
        /// <returns>The event array.</returns>
        public Event[] UpdateContext(Sequence sequence, AbstractModel model) {

            /*
               var tagger = new NameFinderME(new TokenNameFinderModel("x-unspecified", model, new Dictionary<string, string>(), null));

               String[] sentence = sequence.GetSource<NameSample>().Sentence;

    String[] tags = seqCodec.Encode(tagger.Find(sentence), sentence.Length);
    Event[] events = new Event[sentence.length];

            NameFinderEventStream.GenerateEvents(sentence,tags,pcg).toArray(events);

    return events;
            */
            throw new NotImplementedException();
        }
    }
}
