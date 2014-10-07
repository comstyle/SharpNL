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


using System.Text.RegularExpressions;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// This class provide common utilities for feature generation.
    /// </summary>
    public static class FeatureGeneratorUtil {
        private static readonly Regex capPeriod;

        /// <summary>
        /// Initializes static members of the <see cref="FeatureGeneratorUtil"/> class.
        /// </summary>
        static FeatureGeneratorUtil() {
            capPeriod = new Regex("^[A-Z]\\.$", RegexOptions.Compiled);
        }

        #region . TokenFeature .

        /// <summary>
        /// Generates a class name for the specified token.
        /// The classes are as follows where the first matching class is used:
        /// <ul>
        /// <li>lc - lowercase alphabetic</li>
        /// <li>2d - two digits </li>
        /// <li>4d - four digits </li>
        /// <li>an - alpha-numeric </li>
        /// <li>dd - digits and dashes </li>
        /// <li>ds - digits and slashes </li>
        /// <li>dc - digits and commas </li>
        /// <li>dp - digits and periods </li>
        /// <li>num - digits </li>
        /// <li>sc - single capital letter </li>
        /// <li>ac - all capital letters </li>
        /// <li>ic - initial capital letter </li>
        /// <li>other - other </li>
        /// </ul>
        /// </summary>
        /// <param name="token">A token or word.</param>
        /// <returns>The class name that the specified token belongs in.</returns>
        public static string TokenFeature(string token) {
            var pattern = StringPattern.Recognize(token);

            string feat;
            if (pattern.AllCapitalLetter) {
                feat = "lc";
            } else if (pattern.Digits == 2) {
                feat = "2d";
            } else if (pattern.Digits == 4) {
                feat = "4d";
            } else if (pattern.ContainsDigit) {
                if (pattern.ContainsLetters) {
                    feat = "an";
                } else if (pattern.ContainsHyphen) {
                    feat = "dd";
                } else if (pattern.ContainsSlash) {
                    feat = "ds";
                } else if (pattern.ContainsComma) {
                    feat = "dc";
                } else if (pattern.ContainsPeriod) {
                    feat = "dp";
                } else {
                    feat = "num";
                }
            } else if (pattern.AllCapitalLetter && token.Length == 1) {
                feat = "sc";
            } else if (pattern.AllCapitalLetter) {
                feat = "ac";
            } else if (capPeriod.IsMatch(token)) {
                feat = "cp";
            } else if (pattern.InitialCapitalLetter) {
                feat = "ic";
            } else {
                feat = "other";
            }

            return feat;
        }

        #endregion
    }
}