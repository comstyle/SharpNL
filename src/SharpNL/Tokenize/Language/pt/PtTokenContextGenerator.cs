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
//  Note: 
//   This class is based/inspired on code extracted from the CoGrOO (http://cogroo.sourceforge.net/)
//   under Apache V2 license.
//

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpNL.Tokenize.Language.pt {
    /// <summary>
    /// Represents a portuguese token context generator.
    /// </summary>
    public class PtTokenContextGenerator : DefaultTokenContextGenerator {

        private static readonly Regex itemRegex;

        static PtTokenContextGenerator() {
            itemRegex = new Regex("^\\p{Nd}[\\.)]$", RegexOptions.Compiled);
        }

        public PtTokenContextGenerator() : base(new List<string>()) {}
        public PtTokenContextGenerator(List<string> inducedAbbreviations) : base(inducedAbbreviations) { }


        /// <summary>
        /// Returns an array of features for the specified sentence string at the specified index. 
        /// Extensions of this class can override this method to create a customized <see cref="ITokenContextGenerator"/>
        /// </summary>
        /// <param name="sentence">The string for a sentence.</param>
        /// <param name="index">The index to consider splitting as a token.</param>
        /// <returns>An array of features for the specified sentence string at the specified index.</returns>
        protected override List<string> CreateContext(string sentence, int index) {
            var preds = base.CreateContext(sentence, index);

            if (sentence.Length == 2) {
                var current = sentence[1];
                var prev = sentence[0];

                if (current == '.' && char.IsLetter(prev) && char.IsUpper(prev)) {
                    preds.Add("abbname");
                }

                if ((current == '.' || current == ')') && itemRegex.IsMatch(sentence)) {
                    preds.Add("item");
                }
            }

            return preds;
        }

        /// <summary>
        /// Adds the character preds. Helper function for getContext.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="c">The c.</param>
        /// <param name="preds">The preds.</param>
        protected override void AddCharPreds(string key, char c, List<string> preds) {
            base.AddCharPreds(key, c, preds);

            if (c == ':' || c == ',' || c == ';') {
                preds.Add(key + "_sep");
            } else if (c == '»' || c == '«') {
                preds.Add(key + "_quote");
            }

        }
    }
}