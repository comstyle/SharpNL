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

using System;
using SharpNL.Utility;

namespace SharpNL.POSTag {
    /// <summary>
    /// Represents a default POS sequence validator.
    /// </summary>
    public class DefaultPOSSequenceValidator : ISequenceValidator<string> {
        private readonly ITagDictionary dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPOSSequenceValidator"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DefaultPOSSequenceValidator(ITagDictionary dictionary) {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Determines whether a particular continuation of a sequence is valid.
        /// This is used to restrict invalid sequences such as those used in start/continue tag-based chunking or could be used to implement tag dictionary restrictions.
        /// </summary>
        /// <param name="index">The index in the input sequence for which the new outcome is being proposed.</param>
        /// <param name="inputSequence">The input sequence.</param>
        /// <param name="outcomesSequence">The outcomes so far in this sequence.</param>
        /// <param name="outcome">The next proposed outcome for the outcomes sequence.</param>
        /// <returns><c>true</c> if the sequence would still be valid with the new outcome, <c>false</c> otherwise.</returns>
        public bool ValidSequence(int index, string[] inputSequence, string[] outcomesSequence, string outcome) {
            if (dictionary == null) {
                return true;
            }

            var tags = dictionary.GetTags(inputSequence[index]);
            if (tags == null) {
                return true;
            }
            return Array.IndexOf(tags, outcome) != -1;
        }
    }
}