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
using System.Collections.Generic;

namespace SharpNL.SentenceDetector.Language.th {
    /// <summary>
    /// Creates contexts/features for end-of-sentence detection in Thai text.
    /// </summary>
    public class ThSentenceContextGenerator : DefaultSentenceContextGenerator {
        
        public readonly static char[] eosCharacters = {' ', '\n'};


        public ThSentenceContextGenerator() : base(eosCharacters) {
            
        }

        public ThSentenceContextGenerator(char[] eosCharacters) : base(eosCharacters) {
            
        }

        public ThSentenceContextGenerator(List<string> inducedAbbreviations, char[] eosCharacters)
            : base(inducedAbbreviations, eosCharacters) {
            
        }

        #region . CollectFeatures .

        /// <summary>
        /// Determines some of the features for the sentence detector and adds them to list features.
        /// </summary>
        /// <param name="prefix">String preceding the eos character in the eos token.</param>
        /// <param name="suffix">String following the eos character in the eos token.</param>
        /// <param name="previous">Space delimited token preceding token containing eos character.</param>
        /// <param name="next">Space delimited token following token containing eos character.</param>
        /// <param name="eosChar">The EOS character been analyzed.</param>
        protected override void CollectFeatures(string prefix, string suffix, string previous, string next, char? eosChar) {

            buf.Append("p=");
            buf.Append(prefix);
            collectFeats.Add(buf.ToString());
            buf.Clear();

            buf.Append("s=");
            buf.Append(suffix);
            collectFeats.Add(buf.ToString());
            buf.Clear();

            collectFeats.Add("p1=" + prefix.Substring(Math.Max(prefix.Length - 1, 0)));
            collectFeats.Add("p2=" + prefix.Substring(Math.Max(prefix.Length - 2, 0)));
            collectFeats.Add("p3=" + prefix.Substring(Math.Max(prefix.Length - 3, 0)));
            collectFeats.Add("p4=" + prefix.Substring(Math.Max(prefix.Length - 4, 0)));
            collectFeats.Add("p5=" + prefix.Substring(Math.Max(prefix.Length - 5, 0)));
            collectFeats.Add("p6=" + prefix.Substring(Math.Max(prefix.Length - 6, 0)));
            collectFeats.Add("p7=" + prefix.Substring(Math.Max(prefix.Length - 7, 0)));

            collectFeats.Add("n1=" + suffix.Substring(0, Math.Max(1, suffix.Length)));
            collectFeats.Add("n2=" + suffix.Substring(0, Math.Max(2, suffix.Length)));
            collectFeats.Add("n3=" + suffix.Substring(0, Math.Max(3, suffix.Length)));
            collectFeats.Add("n4=" + suffix.Substring(0, Math.Max(4, suffix.Length)));
            collectFeats.Add("n5=" + suffix.Substring(0, Math.Max(5, suffix.Length)));
            collectFeats.Add("n6=" + suffix.Substring(0, Math.Max(6, suffix.Length)));
            collectFeats.Add("n7=" + suffix.Substring(0, Math.Max(7, suffix.Length)));

        }
        #endregion
        
    }
}