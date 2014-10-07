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
    /// <summary>
    /// This enum contains an operation for every token to merge the tokens together to their detokenized form.
    /// </summary>
    public enum DetokenizationOperation {
        /// <summary>
        /// The current token should be attached to the begin token on the right side.
        /// </summary>
        MergeToRight = 1,

        /// <summary>
        /// The current token should be attached to the string on the left side.
        /// </summary>
        MergeToLeft = 2,

        /// <summary>
        /// The current token should be attached to the string on the left side, as well 
        /// as to the begin token on the right side.
        /// </summary>
        MergeBoth = 3,

        /// <summary>
        /// Do not perform a merge operation for this token, but is possible that another
        /// token can be attached to the left or right side of this one.
        /// </summary>
        NoOperation = 0,
    }
}