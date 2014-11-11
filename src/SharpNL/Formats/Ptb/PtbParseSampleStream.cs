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

using System.Text;
using SharpNL.Parser;
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {
    public sealed class PtbParseSampleStream : PtbSampleStream<Parse> {

        /// <summary>
        /// Initializes a new instance of the <see cref="PtbSampleStream{T}"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/>
        /// </exception>
        public PtbParseSampleStream(PtbStreamReader stream) : base(stream) { }

        /// <summary>
        /// Returns the next <see cref="Parse"/> object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="Parse"/> object or <c>null</c> to signal that the stream is exhausted.
        /// </returns>
        public override Parse Read() {
            var node = Stream.Read();
            if (node == null || node.Tokens == null)
                return null;

            var sb = new StringBuilder();

            var tokens = node.Tokens;

            for (var i = 0; i < tokens.Length; i++)
                sb.Append(node.Tokens[i]).Append(' ');

            var p = new Parse(sb.ToString(), new Span(0, sb.Length), AbstractBottomUpParser.TOP_NODE, 1, 0);
            var index = 0;

            foreach (var child in node.Children)
                AddNode(p, child, ref index);

            return p;
        }

        private static void AddNode(Parse parse, PtbNode node, ref int index) {
            var head = ++index;
            
            // TODO: How to deal with this gaps here?

            //if (node.Type != "-NONE-")
            parse.Insert(new Parse(parse.Text, node.Span, AbstractBottomUpParser.TOK_NODE, 1, head));

            // Add the children first, otherwise the order of the nodes will be a mess :P
            if (node.HasChildren) {
                foreach (var child in node.Children) {
                    if (child.HasChildren)
                        AddNode(parse, child, ref index);
                    else
                        parse.Insert(new Parse(parse.Text, child.Span, child.Type, 1, head));

                }
            }

            parse.Insert(new Parse(parse.Text, node.Span, node.Type, 1, head));
        }
    }
}