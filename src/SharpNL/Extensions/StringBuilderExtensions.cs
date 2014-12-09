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
using System.Text;

namespace SharpNL.Extensions {
    /// <summary>
    /// Provides a set of extension methods for <see cref="StringBuilder"/>.
    /// </summary>
    public static class StringBuilderExtensions {

        #region + IndexOf .
        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of a specified Unicode character in the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The Unicode character you want to find.</param>
        /// <returns>The character position of the value parameter if the specified character is found, or -1 if it is not found.</returns>
        /// <remarks>
        /// The search for the value parameter is both case-sensitive and culture-sensitive.
        /// </remarks>
        public static int IndexOf(this StringBuilder sb, char value) {
            return IndexOf(sb, value, 0);
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of a specified Unicode character in the current <see cref="StringBuilder"/> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The Unicode character you want to find.</param>
        /// <param name="startIndex">The starting index number for the search.</param>
        /// <returns>The character position of the value parameter if the specified character is found, or -1 if it is not found.</returns>
        /// <remarks>
        /// <para>Index numbering starts at 0 (zero). The search ranges from <paramref name="startIndex"/> to the end of the string.</para>
        /// <para>The search for the value parameter is both case-sensitive and culture-sensitive.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex) {
            if (startIndex < 0 || startIndex > sb.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            for (var i = startIndex; i < sb.Length; i++) {
                if (sb[i] == value)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of the specified String object in the current <see cref="StringBuilder" /> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The <see cref="string" /> object you want to find.</param>
        /// <returns>The character position of the <paramref name="value" /> parameter if the specified string is found, or -1 if it is not found. If value is empty, the return value is 0 (zero).</returns>
        public static int IndexOf(this StringBuilder sb, string value) {
            if (String.IsNullOrEmpty(value))
                return 0;

            return IndexOf(sb, value, 0, false);
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of the specified String object in the current <see cref="StringBuilder" /> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The <see cref="string" /> object you want to find.</param>
        /// <param name="startIndex">The starting index number for the search.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the case will be ignored during the comparison.</param>
        /// <returns>The character position of the <paramref name="value" /> parameter if the specified string is found, or -1 if it is not found. If value is empty, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase) {
            if (startIndex > sb.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (sb.Length == 0 || value == null)
                return -1;

            if (value == String.Empty)
                return startIndex;

            int index;
            var length = value.Length;
            var count = (sb.Length - length) + 1;
            if (!ignoreCase) {
                for (var i = startIndex; i < count; i++) {
                    if (sb[i] != value[0])
                        continue;

                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index])) {
                        index++;
                    }
                    if (index == length) {
                        return i;
                    }
                }
            } else {
                for (var i = startIndex; i < count; i++) {
                    if (Char.ToLower(sb[i]) != Char.ToLower(value[0]))
                        continue;

                    index = 1;
                    while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index]))) {
                        index++;
                    }
                    if (index == length) {
                        return i;
                    }
                }
            }
            return -1;
        }
        #endregion
        
    }
}