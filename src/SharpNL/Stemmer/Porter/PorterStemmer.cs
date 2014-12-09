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
using SharpNL.Stemmer.Porter.Algorithm;
using SharpNL.Utility;

namespace SharpNL.Stemmer.Porter {
    /// <summary>
    /// Represents a Porter stemmer using the Snowball stemmers developed by Martin Porter.
    /// This class cannot be inherited.
    /// </summary>
    [TypeClass("opennlp.tools.stemmer.PorterStemmer")]
    public sealed class PorterStemmer : AbstractStemmer {

        private readonly PorterAlgorithm algorithm;

        /// <summary>
        /// Initializes a new instance of the <see cref="PorterStemmer"/> using the english algorithm to stem the inputs.
        /// </summary>
        /// <remarks>
        /// This constructor exists for OpenNLP compatibility.
        /// </remarks>
        internal PorterStemmer() : this("en") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PorterStemmer"/> class.
        /// </summary>
        /// <param name="language">The language code.</param>
        /// <exception cref="System.NotSupportedException">The specified language is not supported.</exception>
        public PorterStemmer(string language) {
            switch (Library.GetLang(language)) {
                case "en":
                    algorithm = new English();
                    break;
                case "pt":
                    algorithm = new Portuguese();
                    break;
                default:
                    throw new NotSupportedException("The specified language is not supported.");                   
            }
        }

        /// <summary>
        /// Performs stemming on the specified word.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <param name="posTag">The part-of-speech tag or a <c>null</c> value if none.</param>
        /// <returns>The stemmed word.</returns>
        protected override string Stemming(string word, string posTag) {
            return algorithm.Stem(word);
        }
    }
}