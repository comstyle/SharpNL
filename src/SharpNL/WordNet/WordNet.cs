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

namespace SharpNL.WordNet {
    /// <summary>
    /// Represents a WordNet 3.0 API. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This class is a revamped version from Matt Gerber implementation 
    /// <see href="http://ptl.sys.virginia.edu/ptl/software"/>
    /// 
    /// Original license:
    ///  Free. Do whatever you want with it.
    /// </remarks>
    public sealed class WordNet : IDisposable {

        private static readonly Dictionary<WordNetPos, Dictionary<string, SynSetRelation>> symbolRelation;

        static WordNet() {
            symbolRelation = new Dictionary<WordNetPos, Dictionary<string, SynSetRelation>> {
                {WordNetPos.Noun, new Dictionary<string, SynSetRelation> {
                    {"!", SynSetRelation.Antonym},
                    {"@", SynSetRelation.Hypernym},
                    {"@i", SynSetRelation.InstanceHypernym},
                    {"~", SynSetRelation.Hyponym},
                    {"~i", SynSetRelation.InstanceHyponym},
                    {"#m", SynSetRelation.MemberHolonym},
                    {"#s", SynSetRelation.SubstanceHolonym},
                    {"#p", SynSetRelation.PartHolonym},
                    {"%m", SynSetRelation.MemberMeronym},
                    {"%s", SynSetRelation.SubstanceMeronym},
                    {"%p", SynSetRelation.PartMeronym},
                    {"=", SynSetRelation.Attribute},
                    {"+", SynSetRelation.DerivationallyRelated},
                    {";c", SynSetRelation.TopicDomain},
                    {"-c", SynSetRelation.TopicDomainMember},
                    {";r", SynSetRelation.RegionDomain},
                    {"-r", SynSetRelation.RegionDomainMember},
                    {";u", SynSetRelation.UsageDomain},
                    {"-u", SynSetRelation.UsageDomainMember},
                    {"\\", SynSetRelation.DerivedFromAdjective},  // appears in WordNet 3.1
                }},
                {WordNetPos.Verb, new Dictionary<string, SynSetRelation> {
                    {"!", SynSetRelation.Antonym},
                    {"@", SynSetRelation.Hypernym},
                    {"~", SynSetRelation.Hyponym},
                    {"*", SynSetRelation.Entailment},
                    {">", SynSetRelation.Cause},
                    {"^", SynSetRelation.AlsoSee},
                    {"$", SynSetRelation.VerbGroup},
                    {"+", SynSetRelation.DerivationallyRelated},
                    {";c", SynSetRelation.TopicDomain},
                    {";r", SynSetRelation.RegionDomain},
                    {";u", SynSetRelation.UsageDomain},                    
                }},
                {WordNetPos.Adjective, new Dictionary<string, SynSetRelation> {
                    {"!", SynSetRelation.Antonym},
                    {"&", SynSetRelation.SimilarTo},
                    {"<", SynSetRelation.ParticipleOfVerb},
                    {@"\", SynSetRelation.Pertainym},
                    {"=", SynSetRelation.Attribute},
                    {"^", SynSetRelation.AlsoSee},
                    {";c", SynSetRelation.TopicDomain},
                    {";r", SynSetRelation.RegionDomain},
                    {";u", SynSetRelation.UsageDomain},
                    {"+", SynSetRelation.DerivationallyRelated},  // not in documentation                    
                }},
                {WordNetPos.Adverb, new Dictionary<string, SynSetRelation> {
                    {"!", SynSetRelation.Antonym},
                    {"\\", SynSetRelation.DerivedFromAdjective},
                    {";c", SynSetRelation.TopicDomain},
                    {";r", SynSetRelation.RegionDomain},
                    {";u", SynSetRelation.UsageDomain},
                    {"+", SynSetRelation.DerivationallyRelated},  // not in documentation                    
                }}
            };

        }

        public WordNet(IWordNetProvider provider) {

            if (provider == null)
                throw new ArgumentNullException("provider");

            Provider = provider;
            Provider.Initialize(this);
        }

        #region + Properties .

        #region . Provider .
        /// <summary>
        /// Gets the WordNet provider from this instance.
        /// </summary>
        /// <value>The WordNet provider from this instance.</value>
        public IWordNetProvider Provider { get; private set; }
        #endregion

        #endregion

        #region . Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting 
        /// unmanaged resources.
        /// </summary>
        public void Dispose() {
            Provider.Dispose();
        }
        #endregion

        #region . GetAllWords .

        /// <summary>
        /// Gets a dictionary containing all words in the current instance organized by part-of-speech.
        /// </summary>
        /// <returns>A dictionary containing all words in the instance by part-of-speech</returns>
        public Dictionary<WordNetPos, IReadOnlyCollection<string>> GetAllWords() {
            var list = new Dictionary<WordNetPos, IReadOnlyCollection<string>>(512);

            foreach (var pos in new [] {
                WordNetPos.Adjective,
                WordNetPos.Adverb,
                WordNetPos.Noun, 
                WordNetPos.Verb }) {

                list.Add(pos, Provider.GetAllWords(pos));

            }

            return list;
        }

        #endregion

        #region . GetSynSets .
        /// <summary>
        /// Gets all synsets for a word, optionally restricting the returned synsets to one or more parts of speech. This
        /// method does not perform any morphological analysis to match up the given word. It does, however, replace all 
        /// spaces with underscores and call String.ToLower to normalize case.
        /// </summary>
        /// <param name="word">Word to get SynSets for. This method will replace all spaces with underscores and
        /// call ToLower() to normalize the word's case.</param>
        /// <param name="posRestriction">POSs to search. Cannot contain POS.None. Will search all POSs if no restriction
        /// is given.</param>
        /// <returns>Set of SynSets that contain word</returns>
        public List<SynSet> GetSynSets(string word, params WordNetPos[] posRestriction) {

            // use all POSs if none are supplied
            if (posRestriction == null || posRestriction.Length == 0)
                posRestriction = new[] {
                    WordNetPos.Adjective,
                    WordNetPos.Adverb,
                    WordNetPos.Noun, 
                    WordNetPos.Verb
                };

            var posSet = new List<WordNetPos>(posRestriction);
            if (posSet.Contains(WordNetPos.None))
                throw new InvalidOperationException("Invalid SynSet POS request: None");

            // all words are lower case and space-replaced
            word = word.ToLower().Replace(' ', '_');

            // gather synsets for each POS
            var allSynsets = new List<SynSet>();
            foreach (var pos in posSet)
                allSynsets.AddRange(Provider.GetSynSets(word, pos));

            return allSynsets;
        }
        #endregion

        #region . GetSynSetRelation .
        /// <summary>
        /// Gets the relation for a given POS and symbol
        /// </summary>
        /// <param name="pos">POS to get relation for</param>
        /// <param name="symbol">Symbol to get relation for</param>
        /// <returns>SynSet relation</returns>
        public static SynSetRelation GetSynSetRelation(WordNetPos pos, string symbol) {
            if (pos == WordNetPos.None)
                throw new ArgumentException(@"The pos argument must not be None", "pos");

            return symbolRelation[pos][symbol];
        }
        #endregion

        #region . GetMostCommonSynSet .
        /// <summary>
        /// Gets the most common synset for a given word/pos pair. This is only available for memory-based
        /// engines (see constructor).
        /// </summary>
        /// <param name="word">Word to get SynSets for. This method will replace all spaces with underscores and
        /// will call String.ToLower to normalize case.</param>
        /// <param name="pos">Part of speech to find</param>
        /// <returns>Most common synset for given word/pos pair</returns>
        public SynSet GetMostCommonSynSet(string word, WordNetPos pos) {
            // all words are lower case and space-replaced...we need to do this here, even though it gets done in GetSynSets (we use it below)
            word = word.ToLower().Replace(' ', '_');

            // get synsets for word-pos pair
            var synsets = GetSynSets(word, pos);

            // get most common synset
            SynSet mostCommon = null;
            if (synsets.Count == 1)
                return synsets.First();

            if (synsets.Count <= 1) 
                return null;

            // one (and only one) of the synsets should be flagged as most common
            foreach (var synset in synsets)
                if (synset.IsMostCommonSynsetFor(word))
                    if (mostCommon == null)
                        mostCommon = synset;
                    else
                        throw new Exception("Multiple most common synsets found");

            if (mostCommon == null)
                throw new NullReferenceException("Failed to find most common synset");

            return mostCommon;
        }
        #endregion

    }
}