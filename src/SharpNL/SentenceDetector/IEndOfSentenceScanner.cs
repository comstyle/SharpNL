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

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Scans for the offsets of sentence ending characters.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface can use regular expressions,
    /// hand-coded DFAs, and other scanning techniques to locate end of
    /// sentence offsets.
    /// </remarks>
    public interface IEndOfSentenceScanner {
        /// <summary>
        /// Returns an array of character which can indicate the end of a sentence.
        /// </summary>
        /// <returns>An array of character which can indicate the end of a sentence.</returns>
        char[] GetEndOfSentenceCharacters();


        /// <summary>
        /// Scans the specified string for sentence ending characters and
        /// returns their offsets.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The positions list.</returns>
        List<int> GetPositions(string value);

        /// <summary>
        /// Scans the characters for sentence ending characters and returns their offsets.
        /// </summary>
        /// <param name="chars">The chars to scan.</param>
        /// <returns>Positions.</returns>
        List<int> GetPositions(char[] chars);
    }
}