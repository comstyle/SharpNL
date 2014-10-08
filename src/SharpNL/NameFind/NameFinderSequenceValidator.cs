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

using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Validates the sequences from the name finder.
    /// </summary>
    public class NameFinderSequenceValidator : ISequenceValidator<string> {

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

            // outcome is formatted like "cont" or "sometype-cont", so we
            // can check if it ends with "cont".

            if (!outcome.EndsWith(NameFinderME.Continue)) 
                return true;

            var li = outcomesSequence.Length - 1;

            if (li == -1) 
                return false;
                
            if (outcomesSequence[li].EndsWith(NameFinderME.Other))
                return false;
                
            if (!outcomesSequence[li].EndsWith(NameFinderME.Continue)) 
                return true;

            var nameType = NameFinderME.ExtractNameType(outcome);
            var previousNameType = NameFinderME.ExtractNameType(outcomesSequence[li]);
                
            if (previousNameType == null && nameType == null) 
                return true;

            return nameType != null && nameType.Equals(previousNameType);
        }
    }
}