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

namespace SharpNL.Formats.Brat {
    /// <summary>
    /// Represents a Brat annotation.
    /// </summary>
    public abstract class BratAnnotation {

        #region . Constructor .

        protected BratAnnotation(string id, string type) {
            Id = id;
            Type = type;
        }

        #endregion

        #region + Properties .

        #region . Id .

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; private set; }

        #endregion

        #region . Type .

        /// <summary>
        /// Gets the annotation type.
        /// </summary>
        /// <value>The annotation type.</value>
        public string Type { get; private set; }

        #endregion

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current annotation.
        /// </summary>
        /// <returns>
        /// A string that represents the current annotation.
        /// </returns>
        public override string ToString() {
            return string.Format("{0} {1}", Id, Type);
        }

        #endregion

    }
}