using SharpNL.Chunker;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    public class ADChunkSampleStream : IObjectStream<ChunkSample> {

        //protected readonly IObjectStream<> 

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public ChunkSample Read() {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            throw new System.NotImplementedException();
        }

    }
}
