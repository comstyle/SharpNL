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
using System.Text.RegularExpressions;
using SharpNL.Utility;
using Dic = SharpNL.Dictionary.Dictionary;

namespace SharpNL.POSTag {
    /// <summary>
    /// A context generator for the POS Tagger.
    /// </summary>
    public class DefaultPOSContextGenerator : IPOSContextGenerator {
        protected const string SE = "*SE*";
        protected const string SB = "*SB*";

        private const int PREFIX_LENGTH = 4;
        private const int SUFFIX_LENGTH = 4;
        private static readonly Regex hasCap;
        private static readonly Regex hasNum;

        private readonly Cache contextsCache;

        private readonly Dic dict;
        private readonly string[] dictGram;
        private object wordsKey;

        static DefaultPOSContextGenerator() {
            hasCap = new Regex("[A-Z]", RegexOptions.Compiled);
            hasNum = new Regex("[0-9]", RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPOSContextGenerator"/> without cache.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DefaultPOSContextGenerator(Dic dictionary) : this(0, dictionary) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPOSContextGenerator"/> with the specified cache size.
        /// </summary>
        /// <param name="cacheSize">The cache size.</param>
        /// <param name="dictionary">The dictionary.</param>
        public DefaultPOSContextGenerator(int cacheSize, Dic dictionary) {
            dict = dictionary;
            dictGram = new string[1];

            if (cacheSize > 0) {
                contextsCache = new Cache(cacheSize);
            }
        }

        #region . GetPrefixes .

        /// <summary>
        /// Gets the prefixes.
        /// </summary>
        /// <param name="lex">The lex.</param>
        /// <returns>The prefixes.</returns>
        protected static string[] GetPrefixes(string lex) {
            var prefixes = new string[PREFIX_LENGTH];
            for (int li = 0, ll = PREFIX_LENGTH; li < ll; li++) {
                prefixes[li] = lex.Substring(0, Math.Min(li + 1, lex.Length));
            }
            return prefixes;
        }

        #endregion

        #region . GetSuffixes .

        /// <summary>
        /// Gets the suffixes
        /// </summary>
        /// <param name="lex">The lex.</param>
        /// <returns>The suffixes.</returns>
        protected static string[] GetSuffixes(string lex) {
            var suffixes = new string[SUFFIX_LENGTH];
            for (int li = 0, ll = SUFFIX_LENGTH; li < ll; li++) {
                suffixes[li] = lex.Substring(Math.Max(lex.Length - li - 1, 0));
            }
            return suffixes;
        }

        #endregion

        /// <summary>Gets the context for the specified position in the specified sequence (list).</summary>
        /// <param name="index">The index of the sequence.</param>
        /// <param name="sequence">The sequence of items over which the beam search is performed.</param>
        /// <param name="priorDecisions">The sequence of decisions made prior to the context for which this decision is being made.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for the specified position in the specified sequence.</returns>
        string[] IBeamSearchContextGenerator<string>.GetContext(int index, string[] sequence, string[] priorDecisions,
            object[] additionalContext) {
            return GetContext(index, sequence, priorDecisions, additionalContext);
        }

        /// <summary>
        /// Returns the context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
        /// </summary>
        /// <param name="index">The index of the token for which the context is provided.</param>
        /// <param name="tokens">The tokens in the sentence.</param>
        /// <param name="prevTags">The tags assigned to the previous words in the sentence.</param>
        /// <param name="additionalContext">Any addition context specific to a class implementing this interface.</param>
        /// <returns>The context for making a pos tag decision at the specified token index given the specified tokens and previous tags.</returns>
        public virtual string[] GetContext(int index, string[] tokens, string[] prevTags, object[] additionalContext) {
            return GetContext(index, Array.ConvertAll(tokens, input => (object) input), prevTags);
        }

        /// <summary>
        ///  Returns the context for making a pos tag decision at the specified token index given the specified tokens and previous tags.
        /// </summary>
        /// <param name="index">The index of the token for which the context is provided.</param>
        /// <param name="tokens">The tokens in the sentence.</param>
        /// <param name="tags">The tags assigned to the previous words in the sentence.</param>
        /// <returns>The context for making a pos tag decision at the specified token index given the specified tokens and previous tags.</returns>
        public virtual string[] GetContext(int index, object[] tokens, string[] tags) {
            string next, prev, prevPrev;
            string tagPrevPrev;
            string tagPrev = tagPrevPrev = null;
            string nextNext = prevPrev = null;
            string lex = tokens[index].ToString();

            if (tokens.Length > index + 1) {
                next = tokens[index + 1].ToString();
                if (tokens.Length > index + 2)
                    nextNext = tokens[index + 2].ToString();
                else
                    nextNext = SE; // Sentence End
            } else {
                next = SE; // Sentence End
            }

            if (index - 1 >= 0) {
                prev = tokens[index - 1].ToString();
                tagPrev = tags[index - 1];

                if (index - 2 >= 0) {
                    prevPrev = tokens[index - 2].ToString();
                    tagPrevPrev = tags[index - 2];
                } else {
                    prevPrev = SB; // Sentence Beginning
                }
            } else {
                prev = SB; // Sentence Beginning
            }
            var cacheKey = index + tagPrev + tagPrevPrev;
            if (contextsCache != null) {
                if (wordsKey == tokens) {
                    var cachedContexts = (string[]) contextsCache.Get(cacheKey);
                    if (cachedContexts != null) {
                        return cachedContexts;
                    }
                } else {
                    contextsCache.Clear();
                    wordsKey = tokens;
                }
            }

            var e = new List<String> {
                "default",
                "w=" + lex // add the word itself
            };

            dictGram[0] = lex;
            if (dict == null || !dict.Contains(new StringList(dictGram))) {
                // do some basic suffix analysis
                var suffixes = GetSuffixes(lex);
                for (var i = 0; i < suffixes.Length; i++) {
                    e.Add("suf=" + suffixes[i]);
                }

                var prefixes = GetPrefixes(lex);
                for (var i = 0; i < prefixes.Length; i++) {
                    e.Add("pre=" + prefixes[i]);
                }
                // see if the word has any special characters
                if (lex.IndexOf('-') != -1) {
                    e.Add("h");
                }

                if (hasCap.IsMatch(lex)) {
                    e.Add("c");
                }

                if (hasNum.IsMatch(lex)) {
                    e.Add("d");
                }
            }

            // Add the words and POS's of the surrounding context
            // if (prev != null) { removed... always true
            e.Add("p=" + prev);
            if (tagPrev != null) {
                e.Add("t=" + tagPrev);
            }
            if (prevPrev != null) {
                e.Add("pp=" + prevPrev);
                if (tagPrevPrev != null) {
                    e.Add("t2=" + tagPrevPrev + "," + tagPrev);
                }
            }

            // if (next != null) { removed... always true
            e.Add("n=" + next);
            if (nextNext != null) {
                e.Add("nn=" + nextNext);
            }

            var contexts = e.ToArray();
            if (contextsCache != null) {
                contextsCache.Put(cacheKey, contexts);
            }
            return contexts;
        }
    }
}