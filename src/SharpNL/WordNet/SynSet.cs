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
using System.Globalization;
using System.Linq;
using System.Text;

namespace SharpNL.WordNet {
    /// <summary>
    /// Represents synonym ring or synset, is a group of data elements that are considered 
    /// semantically equivalent for the purposes of information retrieval.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is a modified/revamped version from Matt Gerber implementation: <br /> 
    /// <see href="http://ptl.sys.virginia.edu/ptl/software" />
    /// </para>
    /// <para>
    /// Original license: <br /> <b>Free.</b> Do whatever you want with it.
    /// </para>
    /// </remarks>
    public sealed class SynSet {

        /// <summary>
        /// Words for which the current synset is the most common sense
        /// </summary>
        private List<string> isMostCommonSynsetForWords;

        private Dictionary<SynSetRelation, Dictionary<SynSet, Dictionary<int, List<int>>>> lexicalRelations;
        private Dictionary<SynSetRelation, List<SynSet>> relationSynSets;
        private readonly WordNet wordNet;

        #region + Properties .

        #region . Gloss .
        /// <summary>
        /// Gets the gloss of the current SynSet
        /// </summary>
        public string Gloss { get; private set; }
        #endregion

        #region . Id .
        /// <summary>
        /// Gets the ID of this synset in the form POS:Offset
        /// </summary>
        public string Id { get; private set; }
        #endregion

        #region . Instantiated .
        /// <summary>
        /// Gets whether or not the current synset has been instantiated
        /// </summary>
        internal bool Instantiated { get; private set; }
        #endregion

        #region . LexicalRelations .
        /// <summary>
        /// Gets lexical relations that exist between words in this synset and words in another synset
        /// </summary>
        public IEnumerable<SynSetRelation> LexicalRelations {
            get { return lexicalRelations.Keys; }
        }
        #endregion

        #region . LexicographerFileName .
        /// <summary>
        /// Gets the lexicographer file name for this synset (see the lexnames file in the WordNet distribution).
        /// </summary>
        public LexicographerFileName LexicographerFileName { get; private set; }
        #endregion

        #region . Offset .
        /// <summary>
        /// Gets the byte offset of synset definition within the data file
        /// </summary>
        public int Offset { get; private set; }
        #endregion

        #region . Pos .
        /// <summary>
        /// Gets the part-of-speech of the current synset
        /// </summary>
        public WordNetPos Pos { get; private set; }
        #endregion

        #region . SearchBackPointer .
        /// <summary>
        /// Gets or sets the back-pointer used when searching WordNet
        /// </summary>
        internal SynSet SearchBackPointer { get; set; }
        #endregion

        #region . SemanticRelations .
        /// <summary>
        /// Gets semantic relations that exist between this synset and other synsets
        /// </summary>
        public IEnumerable<SynSetRelation> SemanticRelations {
            get { return relationSynSets.Keys; }
        }
        #endregion

        #region . Words .
        /// <summary>
        /// Gets the words in the current SynSet
        /// </summary>
        public List<string> Words { get; private set; }
        #endregion

        #endregion

        #region + Constructors .
        /// <summary>
        /// Constructor. Creates the shell of a SynSet without any actual information.
        /// To gain access to SynSet words, gloss, and related SynSets, call SynSet.Instantiate.
        /// </summary>
        /// <param name="pos">POS of SynSet</param>
        /// <param name="offset">Byte location of SynSet definition within data file</param>
        /// <param name="wordNet">WordNet engine used to instantiate this synset.</param>
        internal SynSet(WordNetPos pos, int offset, WordNet wordNet) {
            Id = string.Format("{0}:{1}", pos, offset);
            Pos = pos;
            Offset = offset;
            Instantiated = false;

            this.wordNet = wordNet;
        }
        #endregion

        #region + Instantiate .

        internal void Instantiate(IWordNetProvider provider) {
            Instantiate(provider.GetSynSetDefinition(Pos, Offset), null);
        }

        /// <summary>
        /// Instantiates the current synset. If idSynset is non-null, related synsets references are set to those from 
        /// idSynset; otherwise, related synsets are created as shells.
        /// </summary>
        /// <param name="definition">Definition line of synset from data file</param>
        /// <param name="idSynset">Lookup for related synsets. If null, all related synsets will be created as shells.</param>
        internal void Instantiate(string definition, Dictionary<string, SynSet> idSynset) {
            // don't re-instantiate
            if (Instantiated)
                throw new Exception("Synset has already been instantiated");

            /* get lexicographer file name...the enumeration lines up precisely with the wordnet spec (see the lexnames file) except that
             * it starts with None, so we need to add 1 to the definition line's value to get the correct file name */
            var lexicographerFileNumber = int.Parse(GetField(definition, 1)) + 1;
            if (lexicographerFileNumber <= 0)
                throw new Exception("Invalid lexicographer file name number. Should be >= 1.");

            LexicographerFileName = (LexicographerFileName) lexicographerFileNumber;

            // get number of words in the synset and the start character of the word list
            int wordStart;
            var numWords = int.Parse(GetField(definition, 3, out wordStart), NumberStyles.HexNumber);
            wordStart = definition.IndexOf(' ', wordStart) + 1;

            // get words in synset
            Words = new List<string>(numWords);
            for (var i = 0; i < numWords; ++i) {
                var wordEnd = definition.IndexOf(' ', wordStart + 1) - 1;
                var wordLen = wordEnd - wordStart + 1;
                var word = definition.Substring(wordStart, wordLen);
                if (word.Contains(' '))
                    throw new Exception("Unexpected space in word:  " + word);

                Words.Add(word);

                // skip lex_id field
                wordStart = definition.IndexOf(' ', wordEnd + 2) + 1;
            }

            // get gloss
            Gloss = definition.Substring(definition.IndexOf('|') + 1).Trim();
            if (Gloss.Contains('|'))
                throw new Exception("Unexpected pipe in gloss");

            // get number and start of relations
            var relationCountField = 3 + (Words.Count*2) + 1;
            int relationFieldStart;
            var numRelations = int.Parse(GetField(definition, relationCountField, out relationFieldStart));
            relationFieldStart = definition.IndexOf(' ', relationFieldStart) + 1;

            // grab each related synset
            relationSynSets = new Dictionary<SynSetRelation, List<SynSet>>();
            lexicalRelations = new Dictionary<SynSetRelation, Dictionary<SynSet, Dictionary<int, List<int>>>>();
            for (var relationNum = 0; relationNum < numRelations; ++relationNum) {
                string relationSymbol = null;
                var relatedSynSetOffset = -1;
                var relatedSynSetPOS = WordNetPos.None;
                var sourceWordIndex = -1;
                var targetWordIndex = -1;

                // each relation has four columns
                for (var relationField = 0; relationField <= 3; ++relationField) {
                    var fieldEnd = definition.IndexOf(' ', relationFieldStart + 1) - 1;
                    var fieldLen = fieldEnd - relationFieldStart + 1;
                    var fieldValue = definition.Substring(relationFieldStart, fieldLen);

                    // relation symbol
                    if (relationField == 0)
                        relationSymbol = fieldValue;
                        // related synset offset
                    else if (relationField == 1)
                        relatedSynSetOffset = int.Parse(fieldValue);
                        // related synset POS
                    else if (relationField == 2)
                        relatedSynSetPOS = GetPos(fieldValue);
                        // source/target word for lexical relation
                    else if (relationField == 3) {
                        sourceWordIndex = int.Parse(fieldValue.Substring(0, 2), NumberStyles.HexNumber);
                        targetWordIndex = int.Parse(fieldValue.Substring(2), NumberStyles.HexNumber);
                    } else
                        throw new Exception();

                    relationFieldStart = definition.IndexOf(' ', relationFieldStart + 1) + 1;
                }

                // get related synset...create shell if we don't have a lookup
                var relatedSynSet = idSynset != null 
                    ? idSynset[relatedSynSetPOS + ":" + relatedSynSetOffset]
                    : new SynSet(relatedSynSetPOS, relatedSynSetOffset, wordNet);

                // get relation
                var relation = WordNet.GetSynSetRelation(Pos, relationSymbol);

                // add semantic relation if we have neither a source nor a target word index
                if (sourceWordIndex == 0 && targetWordIndex == 0) {
                    relationSynSets.EnsureContainsKey(relation, typeof(List<SynSet>));
                    relationSynSets[relation].Add(relatedSynSet);
                }
                    // add lexical relation
                else {
                    lexicalRelations.EnsureContainsKey(relation, typeof(Dictionary<SynSet, Dictionary<int, List<int>>>));
                    lexicalRelations[relation].EnsureContainsKey(relatedSynSet, typeof(Dictionary<int, List<int>>));
                    lexicalRelations[relation][relatedSynSet].EnsureContainsKey(sourceWordIndex, typeof(List<int>));

                    if (!lexicalRelations[relation][relatedSynSet][sourceWordIndex].Contains(targetWordIndex))
                        lexicalRelations[relation][relatedSynSet][sourceWordIndex].Add(targetWordIndex);
                }

            }

            Instantiated = true;
        }

        #endregion

        #region . GetField .
        /// <summary>
        /// Gets a space-delimited field from a synset definition line
        /// </summary>
        /// <param name="line">SynSet definition line</param>
        /// <param name="fieldNum">Number of field to get</param>
        /// <returns>Field value</returns>
        private static string GetField(string line, int fieldNum) {
            int dummy;
            return GetField(line, fieldNum, out dummy);
        }
        #endregion

        #region . GetField .

        /// <summary>
        /// Gets a space-delimited field from a synset definition line
        /// </summary>
        /// <param name="line">SynSet definition line</param>
        /// <param name="fieldNum">Number of field to get</param>
        /// <param name="startIndex">Start index of field within the line</param>
        /// <returns>Field value</returns>
        private static string GetField(string line, int fieldNum, out int startIndex) {
            if (fieldNum < 0)
                throw new Exception("Invalid field number:  " + fieldNum);

            // scan fields until we hit the one we want
            var currField = 0;
            startIndex = 0;
            while (true) {
                if (currField == fieldNum) {
                    // get the end of the field
                    var endIndex = line.IndexOf(' ', startIndex + 1) - 1;

                    // watch out for end of line
                    if (endIndex < 0)
                        endIndex = line.Length - 1;

                    // get length of field
                    var fieldLen = endIndex - startIndex + 1;

                    // return field value
                    return line.Substring(startIndex, fieldLen);
                }

                // move to start of next field (one beyond next space)
                startIndex = line.IndexOf(' ', startIndex) + 1;

                // if there are no more spaces and we haven't found the field, the caller requested an invalid field
                if (startIndex == 0)
                    throw new Exception("Failed to get field number:  " + fieldNum);

                ++currField;
            }
        }

        #endregion

        #region . GetPos .
        /// <summary>
        /// Gets the POS from its code
        /// </summary>
        /// <param name="pos">POS code</param>
        /// <returns>POS</returns>
        private static WordNetPos GetPos(string pos) {
            switch (pos) {
                case "a":
                case "s":
                    return WordNetPos.Adjective;
                case "r":
                    return WordNetPos.Adverb;
                case "n":
                    return WordNetPos.Noun;
                case "v":
                    return WordNetPos.Verb;
                default:
                    throw new FormatException("Unexpected part-of-speech: " + pos);
            }
        }
        #endregion

        #region . GetRelatedSynSetCount .
        /// <summary>
        /// Gets the number of synsets related to the current one by the given relation
        /// </summary>
        /// <param name="relation">Relation to check</param>
        /// <returns>Number of synset related to the current one by the given relation</returns>
        public int GetRelatedSynSetCount(SynSetRelation relation) {
            return relationSynSets.ContainsKey(relation)
                ? relationSynSets[relation].Count
                : 0;
        }
        #endregion

        #region . GetRelatedSynSets .
        /// <summary>
        /// Gets synsets related to the current synset
        /// </summary>
        /// <param name="relation">Synset relation to follow</param>
        /// <param name="recursive">Whether or not to follow the relation recursively for all related synsets</param>
        /// <returns>Synsets related to the given one by the given relation</returns>
        public List<SynSet> GetRelatedSynSets(SynSetRelation relation, bool recursive) {
            return GetRelatedSynSets(new[] { relation }, recursive);
        }
        #endregion

        #region . GetRelatedSynSets .
        /// <summary>
        /// Gets synsets related to the current synset
        /// </summary>
        /// <param name="relations">Synset relations to follow</param>
        /// <param name="recursive">Whether or not to follow the relations recursively for all related synsets</param>
        /// <returns>Synsets related to the given one by the given relations</returns>
        public List<SynSet> GetRelatedSynSets(IEnumerable<SynSetRelation> relations, bool recursive) {
            var synsets = new List<SynSet>();

            GetRelatedSynSets(relations, recursive, synsets);

            return synsets;
        }
        #endregion

        #region . GetRelatedSynSets .
        /// <summary>
        /// Private version of GetRelatedSynSets that avoids cyclic paths in WordNet. The current synset must itself be instantiated.
        /// </summary>
        /// <param name="relations">Synset relations to get</param>
        /// <param name="recursive">Whether or not to follow the relation recursively for all related synsets</param>
        /// <param name="currSynSets">Current collection of synsets, which we'll add to.</param>
        private void GetRelatedSynSets(IEnumerable<SynSetRelation> relations, bool recursive, List<SynSet> currSynSets) {
            var synSetRelations = relations as SynSetRelation[] ?? Enumerable.ToArray(relations);
            foreach (var relation in synSetRelations)
                if (relationSynSets.ContainsKey(relation))
                    foreach (var relatedSynset in relationSynSets[relation])
                        // only add synset if it isn't already present (wordnet contains cycles)
                        if (!currSynSets.Contains(relatedSynset)) {
                            // instantiate synset if it isn't already (for disk-based storage)
                            if (!relatedSynset.Instantiated)
                                relatedSynset.Instantiate(wordNet.Provider);

                            currSynSets.Add(relatedSynset);

                            if (recursive)
                                relatedSynset.GetRelatedSynSets(synSetRelations, true, currSynSets);
                        }
        }
        #endregion

        #region . GetShortestPathTo .
        /// <summary>
        /// Gets the shortest path from the current synset to another, following the given synset relations.
        /// </summary>
        /// <param name="destination">Destination synset</param>
        /// <param name="relations">Relations to follow, or null for all relations.</param>
        /// <returns>Synset path, or null if none exists.</returns>
        public List<SynSet> GetShortestPathTo(SynSet destination, IEnumerable<SynSetRelation> relations) {

            var synSetRelations = relations == null
                ? Enum.GetValues(typeof(SynSetRelation)) as SynSetRelation[]
                : (relations as SynSetRelation[] ?? Enumerable.ToArray(relations));

            // make sure the backpointer on the current synset is null - can't predict what other functions might do
            SearchBackPointer = null;

            // avoid cycles
            var synsetsEncountered = new List<SynSet> { this };

            // start search queue
            var searchQueue = new Queue<SynSet>();
            searchQueue.Enqueue(this);

            // run search
            List<SynSet> path = null;
            while (searchQueue.Count > 0 && path == null) {
                var currSynSet = searchQueue.Dequeue();

                // see if we've finished the search
                if (currSynSet == destination) {
                    // gather synsets along path
                    path = new List<SynSet>();
                    while (currSynSet != null) {
                        path.Add(currSynSet);
                        currSynSet = currSynSet.SearchBackPointer;
                    }

                    // reverse for the correct order
                    path.Reverse();
                }
                    // expand the search one level
                else {

                    foreach (var synset in currSynSet.GetRelatedSynSets(synSetRelations, false))
                        if (!synsetsEncountered.Contains(synset)) {
                            synset.SearchBackPointer = currSynSet;
                            searchQueue.Enqueue(synset);

                            synsetsEncountered.Add(synset);
                        }
                }
            }

            // null-out all search backpointers
            foreach (var synset in synsetsEncountered)
                synset.SearchBackPointer = null;

            return path;
        }
        #endregion

        #region . GetClosestMutuallyReachableSynset .
        /// <summary>
        /// Gets the closest synset that is reachable from the current and another synset along the given relations. For example, 
        /// given two synsets and the Hypernym relation, this will return the lowest synset that is a hypernym of both synsets. If 
        /// the hypernym hierarchy forms a tree, this will be the lowest common ancestor.
        /// </summary>
        /// <param name="synset">Other synset</param>
        /// <param name="relations">Relations to follow</param>
        /// <returns>Closest mutually reachable synset</returns>
        public SynSet GetClosestMutuallyReachableSynset(SynSet synset, IEnumerable<SynSetRelation> relations) {
            var synSetRelations = relations as SynSetRelation[] ?? Enumerable.ToArray(relations);
            var synsetsEncountered = new List<SynSet> { this };
            var searchQueue = new Queue<SynSet>();

            searchQueue.Enqueue(this);

            // run search
            SynSet closest = null;
            while (searchQueue.Count > 0 && closest == null) {
                var currSynSet = searchQueue.Dequeue();

                /* check for a path between the given synset and the current one. if such a path exists, the current
                 * synset is the closest mutually reachable synset. */

                if (synset.GetShortestPathTo(currSynSet, synSetRelations) != null)
                    closest = currSynSet;
                // otherwise, expand the search along the given relations
                else
                    foreach (var relatedSynset in currSynSet.GetRelatedSynSets(synSetRelations, false))
                        if (!synsetsEncountered.Contains(relatedSynset)) {
                            searchQueue.Enqueue(relatedSynset);
                            synsetsEncountered.Add(relatedSynset);
                        }
            }

            return closest;
        }
        #endregion

        #region + GetDepth .
        /// <summary>
        /// Computes the depth of the current synset following a set of relations. Returns the minimum of all possible depths. Root nodes
        /// have a depth of zero.
        /// </summary>
        /// <param name="relations">Relations to follow</param>
        /// <returns>Depth of current SynSet</returns>
        public int GetDepth(IEnumerable<SynSetRelation> relations) {
            var synsets = new List<SynSet> { this };
            return GetDepth(relations, ref synsets);
        }

        /// <summary>
        /// Computes the depth of the current synset following a set of relations. Returns the minimum of all possible depths. Root
        /// nodes have a depth of zero.
        /// </summary>
        /// <param name="relations">Relations to follow</param>
        /// <param name="synsetsEncountered">Synsets that have already been encountered. Prevents cycles from being entered.</param>
        /// <returns>Depth of current SynSet</returns>
        private int GetDepth(IEnumerable<SynSetRelation> relations, ref List<SynSet> synsetsEncountered) {
            // get minimum depth through all relatives
            var minimumDepth = -1;
            var synSetRelations = relations as SynSetRelation[] ?? Enumerable.ToArray(relations);
            foreach (var relatedSynset in GetRelatedSynSets(synSetRelations, false))
                if (!synsetsEncountered.Contains(relatedSynset)) {
                    // add this before recursing in order to avoid cycles
                    synsetsEncountered.Add(relatedSynset);

                    // get depth from related synset
                    var relatedDepth = relatedSynset.GetDepth(synSetRelations, ref synsetsEncountered);

                    // use depth if it's the first or it's less than the current best
                    if (minimumDepth == -1 || relatedDepth < minimumDepth)
                        minimumDepth = relatedDepth;
                }

            // depth is one plus minimum depth through any relative synset...for synsets with no related synsets, this will be zero
            return minimumDepth + 1;
        }

        #endregion

        #region . GetLexicallyRelatedWords .
        /// <summary>
        /// Gets lexically related words for the current synset. Many of the relations in WordNet are lexical instead of semantic. Whereas
        /// the latter indicate relations between entire synsets (e.g., hypernym), the former indicate relations between specific 
        /// words in synsets. This method retrieves all lexical relations and the words related thereby.
        /// </summary>
        /// <returns>Mapping from relations to mappings from words in the current synset to related words in the related synsets</returns>
        public Dictionary<SynSetRelation, Dictionary<string, List<string>>> GetLexicallyRelatedWords() {
            var relatedWords = new Dictionary<SynSetRelation, Dictionary<string, List<string>>>();
            foreach (var relation in lexicalRelations.Keys) {
                relatedWords.EnsureContainsKey(relation, typeof(Dictionary<string, List<string>>));

                foreach (var relatedSynSet in lexicalRelations[relation].Keys) {
                    // make sure related synset is initialized
                    if (!relatedSynSet.Instantiated)
                        relatedSynSet.Instantiate(wordNet.Provider);

                    foreach (var sourceWordIndex in lexicalRelations[relation][relatedSynSet].Keys) {
                        var sourceWord = Words[sourceWordIndex - 1];

                        relatedWords[relation].EnsureContainsKey(sourceWord, typeof(List<string>), false);

                        foreach (var targetWordIndex in lexicalRelations[relation][relatedSynSet][sourceWordIndex]) {
                            var targetWord = relatedSynSet.Words[targetWordIndex - 1];
                            relatedWords[relation][sourceWord].Add(targetWord);
                        }
                    }
                }
            }

            return relatedWords;
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Gets hash code for this synset
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
        #endregion

        #region + Operators .
        /// <summary>
        /// Checks whether two synsets are equal
        /// </summary>
        /// <param name="first">First synset</param>
        /// <param name="second">Second synset</param>
        /// <returns>True if synsets are equal, false otherwise</returns>
        public static bool operator ==(SynSet first, SynSet second) {
            // check object reference
            if (ReferenceEquals(first, second))
                return true;

            // check if either (but not both) are null
            if ((object)second == null ^ (object)first == null)
                return false;

            return first.Equals(second);
        }

        /// <summary>
        /// Checks whether two synsets are unequal
        /// </summary>
        /// <param name="first">First synset</param>
        /// <param name="second">Second synset</param>
        /// <returns>True if synsets are unequal, false otherwise</returns>
        public static bool operator !=(SynSet first, SynSet second) {
            return !(first == second);
        }

        #endregion

        #region . Equals .
        /// <summary>
        /// Checks whether the current synset equals another
        /// </summary>
        /// <param name="obj">Other synset</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object obj) {
            if (!(obj is SynSet))
                return false;

            var synSet = obj as SynSet;

            return Pos == synSet.Pos && Offset == synSet.Offset;
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Gets description of synset
        /// </summary>
        /// <returns>Description</returns>
        public override string ToString() {
            var sb = new StringBuilder();

            // if the synset is instantiated, include words and gloss
            if (Instantiated) {
                sb.Append("{");
                var prependComma = false;
                foreach (var word in Words) {
                    sb.Append((prependComma ? ", " : "") + word);
                    prependComma = true;
                }

                sb.Append("}: " + Gloss);
            }
                // if it's not instantiated, just include the ID
            else
                sb.Append(Id);

            return sb.ToString();
        }
        #endregion

        #region . IsMostCommonSynsetFor .
        /// <summary>
        /// Checks whether this is the most common synset for a word
        /// </summary>
        /// <param name="word">Word to check</param>
        /// <returns>True if this is the most common synset, false otherwise</returns>
        internal bool IsMostCommonSynsetFor(string word) {
            return isMostCommonSynsetForWords != null && isMostCommonSynsetForWords.Contains(word);
        }
        #endregion

        #region . SetAsMostCommonSynsetFor .
        /// <summary>
        /// Set the current synset as the most common for a word
        /// </summary>
        /// <param name="word">Word to set</param>
        internal void SetAsMostCommonSynsetFor(string word) {
            if (isMostCommonSynsetForWords == null)
                isMostCommonSynsetForWords = new List<string>();

            isMostCommonSynsetForWords.Add(word);
        }
        #endregion

    }
}