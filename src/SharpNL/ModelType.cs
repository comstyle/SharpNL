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

namespace SharpNL {
    /// <summary>
    /// Enumerates model types in the SharpNL library.
    /// </summary>
    public enum Models {
        /// <summary>
        /// Unknown model.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Represents a chunker model.
        /// </summary>
        Chunker,
        /// <summary>
        /// The document categorizer model.
        /// </summary>
        DocumentCategorizer,
        /// <summary>
        /// Represents a tokenizer model.
        /// </summary>
        Tokenizer,
        /// <summary>
        /// Represents a name finder model.
        /// </summary>
        NameFind,
        /// <summary>
        /// Represents a parser model.
        /// </summary>
        Parser,
        /// <summary>
        /// Represents a POS tagger model.
        /// </summary>
        POSTag,
        /// <summary>
        /// Represents a sentence detector model.
        /// </summary>
        SentenceDetector
    }
}