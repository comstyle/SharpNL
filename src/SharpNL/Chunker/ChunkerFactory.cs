using System;
using SharpNL.Utility;

namespace SharpNL.Chunker {
    public class ChunkerFactory : BaseToolFactory {

        /// <summary>
        /// Gets the sequence validator.
        /// </summary>
        public virtual ISequenceValidator<string> GetSequenceValidator() {
            return new DefaultChunkerSequenceValidator();
        }

        /// <summary>
        /// Gets the context generator.
        /// </summary>
        public virtual IChunkerContextGenerator GetContextGenerator() {
            return new DefaultChunkerContextGenerator();
        }
        

    }
}
