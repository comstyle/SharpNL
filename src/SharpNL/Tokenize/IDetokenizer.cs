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

namespace SharpNL.Tokenize {
    /// <summary>A Detokenizer merges tokens back to their untokenized representation.</summary>
    public interface IDetokenizer {
        /// <summary>Detokenizes the specified tokens.</summary>
        /// <param name="tokens">The tokens to detokenize.</param>
        /// <returns>The merge operations to detokenize the input tokens.</returns>
        DetokenizationOperation[] Detokenize(string[] tokens);


        /// <summary>Detokenize the input tokens into a String. Tokens which are connected without a space inbetween can be
        /// separated by a split marker.</summary>
        /// <param name="tokens">The tokens to detokenize.</param>
        /// <param name="splitMarker">The split marker or null.</param>
        /// <returns>The detokenized string.</returns>
        string Detokenize(string[] tokens, string splitMarker);
    }
}