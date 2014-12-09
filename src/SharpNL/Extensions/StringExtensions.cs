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
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpNL.Extensions {
    /// <summary>
    /// Provides a set of extension methods for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions {

        #region . Contains .
        /// <summary>
        /// Determines whether the string array contains the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="array">The string array.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the string array contains the specified <paramref name="value"/>; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string[] array, string value) {
            if (array == null || array.Length == 0)
                return false;

            return array.Any(value.Equals);
        }
        #endregion

        #region . EndsWith .
        /// <summary>
        /// Determines whether the end of this string instance matches any specified <paramref name="prefixes"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="prefixes">The prefixes to compare.</param>
        /// <returns><c>true</c> if a prefix from <paramref name="prefixes"/> matches the end of this string, <c>false</c> otherwise.</returns>
        public static bool EndsWith(this string input, params string[] prefixes) {
            return prefixes.Any(input.EndsWith);
        }
        /// <summary>
        /// Determines whether the end of this string instance matches any specified <paramref name="prefixes"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="comparison">One of the enumeration values that determines how this string and prefix are compared.</param>
        /// <param name="prefixes">The prefixes to compare.</param>
        /// <returns><c>true</c> if a prefix from <paramref name="prefixes"/> matches the end of this string, <c>false</c> otherwise.</returns>
        public static bool EndsWith(this string input, StringComparison comparison, params string[] prefixes) {
            return prefixes.Any(prefix => input.EndsWith(prefix, comparison));
        }
        #endregion

        #region . Left .
        /// <summary>
        /// Returns the left part of a string. If the length is negative a relative length will be returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>The left part of a string value.</returns>
        public static string Left(this string value, int length) {
            if (string.IsNullOrEmpty(value) || length == 0)
                return value;

            if (length > 0) {
                return length < value.Length
                    ? value.Substring(0, length)
                    : string.Empty;
            }

            // negative length
            length = value.Length + length;

            if (length < 1)
                return string.Empty;

            return length < value.Length
                ? value.Substring(0, length)
                : string.Empty;
        }

        #endregion

        #region . Replace .
        /// <summary>
        /// Returns a new string in which all occurrences of a specified Unicode characters in this instance are replaced with another specified Unicode character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldChars">The Unicode characters to be replaced.</param>
        /// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChars"/>.</param>
        /// <returns>
        /// A string that is equivalent to this instance except that all instances of <paramref name="oldChars"/> are replaced with <paramref name="newChar"/>. 
        /// If any <paramref name="oldChars"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        public static string Replace(this string value, char[] oldChars, char newChar) {
            var sb = new StringBuilder(value);
            for (var i = 0; i < sb.Length; i++) {
                if (sb[i].In(oldChars))
                    sb[i] = newChar;
            }
            return sb.ToString();
        }
        #endregion

        #region . Right .
        /// <summary>
        /// Returns the right part of a string. If the length is negative a relative length will be returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>The right part of a string value.</returns>
        public static string Right(this string value, int length) {
            if (string.IsNullOrEmpty(value) || length == 0)
                return value;

            if (length < 0) 
                length = value.Length + length;

            if (length < 1)
                return string.Empty;

            return length < value.Length
                    ? value.Substring(value.Length - length)
                    : string.Empty;

        }
        #endregion

        #region + StartsWith .
        /// <summary>
        /// Determines whether the beginning of this string instance matches any specified <paramref name="prefixes"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="prefixes">The prefixes to compare.</param>
        /// <returns><c>true</c> if a prefix from <paramref name="prefixes"/> matches the beginning of this string, <c>false</c> otherwise.</returns>
        public static bool StartsWith(this string input, params string[] prefixes) {
            return prefixes.Any(input.StartsWith);
        }
        /// <summary>
        /// Determines whether the beginning of this string instance matches any specified <paramref name="prefixes"/>.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="comparison">One of the enumeration values that determines how this string and prefix are compared.</param>
        /// <param name="prefixes">The prefixes to compare.</param>
        /// <returns><c>true</c> if a prefix from <paramref name="prefixes"/> matches the beginning of this string, <c>false</c> otherwise.</returns>
        public static bool StartsWith(this string input, StringComparison comparison, params string[] prefixes) {
            return prefixes.Any(prefix => input.StartsWith(prefix, comparison));
        }

        #endregion

        #region . TrimEnd .
        /// <summary>
        /// Trims the <paramref name="suffix"/> from the end of the input string.
        /// </summary>
        /// <param name="value">The input string.</param>
        /// <param name="suffix">The suffix to remove from the end of the input string.</param>
        /// <param name="comparison">One of the enumeration values that determines how this string and suffix are compared.</param>
        /// <returns>The trimmed result string.</returns>
        public static string TrimEnd(this string value, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase) {
            if (suffix.Length > value.Length || !value.EndsWith(suffix, comparison))
                return value;

            return value.Substring(0, value.Length - suffix.Length);
        }
        #endregion

        #region . RemoveDiacritics .
        /// <summary>
        /// Removes diacritics (diacritical mark, diacritical point, diacritical sign) from the input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <seealso href="http://en.wikipedia.org/wiki/Diacritic"/>
        public static string RemoveDiacritics(this string input) {
            if (input == null)
                return null;

            if (input.Length > 0) {
                var chars = new char[input.Length];
                var charIndex = 0;

                input = input.Normalize(NormalizationForm.FormD);
                foreach (var c in input) {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                        chars[charIndex++] = c;
                }

                return new string(chars, 0, charIndex).Normalize(NormalizationForm.FormC);
            }

            return input;
        }

        #endregion
    }
}