using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    class BioCodec : ISequenceCodec<string> {

        /// <summary>
        /// Decodes a sequence string objects into its respective <see cref="Span"/> objects.
        /// </summary>
        /// <param name="objectList">The object list.</param>
        /// <returns>A array with the decoded objects.</returns>
        public Span[] Decode(string[] objectList) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes the specified names.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="length">The length.</param>
        /// <returns>T[].</returns>
        public string[] Encode(Span[] names, int length) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a sequence validator which can validate a sequence of outcomes.
        /// </summary>
        /// <returns>A sequence validator which can validate a sequence of outcomes.</returns>
        public ISequenceValidator<string> CreateSequenceValidator() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the outcomes of the model are compatible with the codec.
        /// </summary>
        /// <param name="outcomes">All possible model outcomes</param>
        /// <returns><c>true</c> if the outcomes of the model are compatible with the codec, <c>false</c> otherwise.</returns>
        public bool AreOutcomesCompatible(string[] outcomes) {
            throw new NotImplementedException();
        }
    }
}
