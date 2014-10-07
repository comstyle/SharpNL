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

namespace SharpNL.Text.Tree {

    /// <summary>
    /// Represents a abstract tree element.
    /// </summary>
    public abstract class TreeElement {

        #region . FeatureTag .
        /// <summary>
        /// Gets or sets the feature tag.
        /// </summary>
        /// <value>The feature tag.</value>
        public string FeatureTag { get; set; }
        #endregion

        #region . Level .
        /// <summary>
        /// Gets or sets the element level.
        /// </summary>
        /// <value>The element level.</value>
        public int Level { get; set; }
        #endregion

        #region . MorphologicalTag .
        /// <summary>
        /// Gets or sets the morphological tag.
        /// </summary>
        /// <value>The morphological tag.</value>
        public string MorphologicalTag { get; set; }
        #endregion

        #region . Parent .
        /// <summary>
        /// Gets or sets the parent element.
        /// </summary>
        /// <value>The parent element.</value>
        public TreeElement Parent { get; set; }
        #endregion

        #region . SyntacticTag .
        /// <summary>
        /// Gets or sets the syntactic tag.
        /// </summary>
        /// <value>The syntactic tag.</value>
        public string SyntacticTag { get; set; }
        #endregion

        #region . SyntaxTree .
        /// <summary>
        /// Gets the syntax tree.
        /// </summary>
        /// <value>The syntax tree.</value>
        public abstract string SyntaxTree { get; }
        #endregion

        #region . TreeBank .
        /// <summary>
        /// Gets the tree bank.
        /// </summary>
        /// <value>The tree bank.</value>
        public abstract string TreeBank { get; }
        #endregion

    }
}