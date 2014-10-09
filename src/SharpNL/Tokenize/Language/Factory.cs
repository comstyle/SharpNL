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

using System.Collections.Generic;

namespace SharpNL.Tokenize.Language {
    public static class Factory {

        public const string DefaultAlphanumeric = "^[A-Za-z0-9]+$";

        #region . GetAlphanumeric .
        /// <summary>
        /// Gets the alpha numeric pattern for the language. Please save the value
        /// locally because this call is expensive. 
        /// </summary>
        /// <param name="languageCode">The language code. If null or unknown the default pattern will be returned.</param>
        /// <returns>The alpha numeric pattern for the language or the default pattern.</returns>
        public static string GetAlphanumeric(string languageCode) {
            switch (languageCode) {
                case "pt":
                    return "^[0-9a-záãâàéêíóõôúüçA-ZÁÃÂÀÉÊÍÓÕÔÚÜÇ]+$";
                default:
                    return DefaultAlphanumeric;
            }
        }
        #endregion

        public static ITokenContextGenerator CreateTokenContextGenerator(string languageCode, List<string> abbreviations) {
            switch (languageCode) {
                case "pt":
                    return new pt.PtTokenContextGenerator(abbreviations);
                default:
                    return new DefaultTokenContextGenerator(abbreviations);
            }
        }
    }
}