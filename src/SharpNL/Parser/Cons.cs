using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNL.Parser {
    /// <summary>
    /// Holds feature information about a specific parse node.
    /// </summary>
    public struct Cons {

        public readonly string cons;
        public readonly string consbo;
        public readonly bool unigram;
        public readonly int index;

        public Cons(string cons, string consbo, int index, bool unigram) {
            this.cons = cons;
            this.consbo = consbo;
            this.index = index;
            this.unigram = unigram;
        }
    }
}
