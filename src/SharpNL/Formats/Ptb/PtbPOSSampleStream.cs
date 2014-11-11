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

using System.Collections.Generic;
using SharpNL.POSTag;

namespace SharpNL.Formats.Ptb {
    /// <summary>
    /// This stream provides <see cref="POSSample"/> samples readed from a Penn Treebank input. This class cannot be inherited.
    /// </summary>
    public sealed class PtbPosSampleStream : PtbSampleStream<POSSample> {
        /// <summary>
        /// Initializes a new instance of the <see cref="PtbPosSampleStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="stream"/>
        /// </exception>
        public PtbPosSampleStream(PtbStreamReader stream) : base(stream) {}

        /// <summary>
        /// Returns the next <see cref="POSSample"/> object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="POSSample"/> object or <c>null</c> to signal that the stream is exhausted.
        /// </returns>
        public override POSSample Read() {
            var node = Stream.Read();
            if (node == null)
                return null;

            var tokens = new List<string>(node.Tokens);
            var tags = new List<string>(tokens.Count);

            AddNode(tags, node);

            var index = 0;
            while (index < tokens.Count) {
                if (tags[index] == "-NONE-") {
                    tags.RemoveAt(index);
                    tokens.RemoveAt(index);
                }

                index++;
            }

            return new POSSample(tokens.ToArray(), tags.ToArray());
        }

        private static void AddNode(List<string> list, PtbNode node) {
            if (node.Token != null)
                list.Add(node.Type);

            if (!node.HasChildren)
                return;

            foreach (var child in node.Children)
                AddNode(list, child);
        }
    }
}