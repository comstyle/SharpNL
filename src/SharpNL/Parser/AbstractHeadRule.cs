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

using SharpNL.Extensions;

namespace SharpNL.Parser {
    /// <summary>
    /// Represents an abstract head rule.
    /// </summary>
    public abstract class AbstractHeadRule {

        #region + Properties .

        #region . LeftToRight .

        public bool LeftToRight { get; protected set; }

        #endregion

        #region . Tags .

        public string[] Tags { get; protected set; }

        #endregion

        #endregion

        #region + Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:AbstractHeadRule"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (Equals(this, obj))
                return true;

            var headRule = obj as AbstractHeadRule;
            if (headRule != null) {
                return LeftToRight == headRule.LeftToRight &&
                       Tags.SequenceEqual(headRule.Tags);
            }
            return false;
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return (LeftToRight.GetHashCode()*397) ^ (Tags != null ? Tags.GetHashCode() : 0);
            }
        }

        #endregion
    }
}