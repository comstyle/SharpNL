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

using System.Linq;
using System.Windows.Forms;

namespace SharpNL.Gui {
    internal static class Extensions {

        #region . Between .
        /// <summary>
        /// Returns if the value is "between" the upper and lower values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="lower">The lowest desired value.</param>
        /// <param name="upper">The highest desired value.</param>
        /// <param name="inclusive">Should consider the value as between.</param>
        /// <returns><c>true</c> if the value is between the <paramref name="lower"/> and <paramref name="upper"/>, <c>false</c> otherwise.</returns>
        public static bool Between(this int value, int lower, int upper, bool inclusive = false) {
            return inclusive
                ? lower <= value && value <= upper
                : lower < value && value < upper;
        }
        #endregion

        #region . Contains .
        /// <summary>
        /// Determines whether the <see cref="TreeView"/> contains the specified <paramref name="node"/>.
        /// </summary>
        /// <param name="treeView">The tree view.</param>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if the <see cref="TreeView"/> contains the specified tree node; otherwise, <c>false</c>.</returns>
        internal static bool Contains(this TreeView treeView, TreeNode node) {
            if (treeView == null || node == null)
                return false;
            return treeView.Nodes.Cast<TreeNode>().Any(child => TreeContains(child, node));
        }
        private static bool TreeContains(TreeNode current, TreeNode node) {
            return current == node || current.Nodes.Cast<TreeNode>().Any(child => TreeContains(child, node));
        }

        #endregion


    }
}