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

namespace SharpNL.Project {
    /// <summary>
    /// Internal extensions.
    /// </summary>
    internal static class Extensions {

        #region . GetProperty .

        /// <summary>
        /// Gets the property value with the specified name. If the property cannot be found the <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="name">The property name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>T.</returns>
        internal static T GetProperty<T>(this object obj, string name, T defaultValue) {
            if (obj == null)
                return defaultValue;

            var type = obj.GetType();

            var p = type.GetProperty(name, typeof (T));
            if (p != null)
                return (T) p.GetValue(obj);

            return defaultValue;
        }

        #endregion

    }
}