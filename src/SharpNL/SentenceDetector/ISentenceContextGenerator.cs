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

namespace SharpNL.SentenceDetector {
    /// <summary>
    /// Interface for <see cref="T:SentenceDetectorME"/> context generators.
    /// </summary>
    public interface ISentenceContextGenerator {

        /// <summary>
        /// Returns an array of contextual features for the potential sentence boundary at the
        /// specified position within the specified string buffer.
        /// </summary>
        /// <param name="value">The value for which sentences are being determined.</param>
        /// <param name="position">An index into the specified string buffer when a sentence boundary may occur.</param>
        /// <returns>An array of contextual features for the potential sentence boundary at the specified position within the specified string buffer.</returns>
        string[] GetContext(string value, int position);
    }
}