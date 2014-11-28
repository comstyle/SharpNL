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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SharpNL.WordNet.Providers {

    /// <summary>
    /// Represents a WordNet memory provider, which reads the standard WordNet files and store everyting in 
    /// memory improving performance but this provider requires ~200mb of RAM using the default WordNet db.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class WordNetMemoryProvider : IWordNetProvider {

        /// <summary>
        /// In-memory pos-word synsets lookup
        /// </summary>
        private readonly Dictionary<WordNetPos, Dictionary<string, List<SynSet>>> posWordSynSets;

        /// <summary>
        /// in-memory id-synset lookup where id is POS:Offset
        /// </summary>
        private readonly Dictionary<string, SynSet> idSynset;

        private WordNet wordNet;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="WordNetMemoryProvider"/> class.
        /// </summary>
        /// <param name="dataPath">The data path.</param>
        /// <exception cref="System.ArgumentNullException">dataPath</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The data directory does not exist.</exception>
        /// <exception cref="System.IO.FileNotFoundException">A required WordNet file does not exist: [filename]</exception>
        public WordNetMemoryProvider(string dataPath) {
            if (string.IsNullOrEmpty(dataPath))
                throw new ArgumentNullException("dataPath");

            var dir = new DirectoryInfo(dataPath);

            if (!dir.Exists)
                throw new DirectoryNotFoundException("The data directory does not exist.");


            var dataPaths = new [] {
                new FileInfo(Path.Combine(dataPath, "data.adj")),
                new FileInfo(Path.Combine(dataPath, "data.adv")),
                new FileInfo(Path.Combine(dataPath, "data.noun")),
                new FileInfo(Path.Combine(dataPath, "data.verb"))
            };

            var indexPaths = new [] {
                new FileInfo(Path.Combine(dataPath, "index.adj")),
                new FileInfo(Path.Combine(dataPath, "index.adv")),
                new FileInfo(Path.Combine(dataPath, "index.noun")),
                new FileInfo(Path.Combine(dataPath, "index.verb"))
            };

            foreach (var file in dataPaths.Union(indexPaths).Where(file => !file.Exists))
                throw new FileNotFoundException("A required WordNet file does not exist: " + file.Name);

            // Pass 1: Get total number of synsets
            var totalSynsets = 0;
            foreach (var dataInfo in dataPaths) {
                // scan synset data file for lines that don't start with a space... 
                // these are synset definition lines
                using (var dataFile = new StreamReader(dataInfo.FullName)) {
                    string line;
                    while ((line = dataFile.ReadLine()) != null) {
                        var firstSpace = line.IndexOf(' ');
                        if (firstSpace > 0)
                            ++totalSynsets;
                    }
                }

            }

            // Pass 2: Create synset shells (pos and offset only)
            idSynset = new Dictionary<string, SynSet>(totalSynsets);
            foreach (var dataInfo in dataPaths) {
                var pos = WordNetFileProvider.GetFilePos(dataInfo.FullName);

                // scan synset data file
                using (var dataFile = new StreamReader(dataInfo.FullName)) {
                    string line;
                    while ((line = dataFile.ReadLine()) != null) {
                        var firstSpace = line.IndexOf(' ');
                        if (firstSpace <= 0) 
                            continue;

                        // get offset and create synset shell
                        var offset = int.Parse(line.Substring(0, firstSpace));
                        var synset = new SynSet(pos, offset, null);

                        idSynset.Add(synset.Id, synset);
                    }
                }

            }

            // Pass 3: Instantiate synsets (hooks up relations, set glosses, etc.)
            foreach (var dataInfo in dataPaths) {
                var pos = WordNetFileProvider.GetFilePos(dataInfo.FullName);

                // scan synset data file
                using (var dataFile = new StreamReader(dataInfo.FullName)) {
                    string line;
                    while ((line = dataFile.ReadLine()) != null) {
                        var firstSpace = line.IndexOf(' ');
                        if (firstSpace > 0)
                            // instantiate synset defined on current line, using the instantiated synsets for all references
                            idSynset[pos + ":" + int.Parse(line.Substring(0, firstSpace))].Instantiate(line, idSynset);
                    }
                }

            }

            // organize synsets by pos and words... 
            // also set most common synset for word-pos pairs that have multiple synsets

            posWordSynSets = new Dictionary<WordNetPos, Dictionary<string, List<SynSet>>>();

            foreach (var indexInfo in indexPaths) {
                var pos = WordNetFileProvider.GetFilePos(indexInfo.FullName);

                posWordSynSets.EnsureContainsKey(pos, typeof(Dictionary<string, List<SynSet>>));

                // scan word index file, skipping header lines
                var indexFile = new StreamReader(indexInfo.FullName);
                string line;
                while ((line = indexFile.ReadLine()) != null) {
                    var firstSpace = line.IndexOf(' ');
                    if (firstSpace <= 0) 
                        continue;

                    // grab word and synset shells, along with the most common synset
                    var word = line.Substring(0, firstSpace);
                    SynSet mostCommonSynSet;
                    var synsets = WordNetFileProvider.GetSynSetShells(line, pos, out mostCommonSynSet, wordNet);

                    // set flag on most common synset if it's ambiguous
                    if (synsets.Count > 1)
                        idSynset[mostCommonSynSet.Id].SetAsMostCommonSynsetFor(word);

                    // use reference to the synsets that we instantiated in our three-pass routine above
                    posWordSynSets[pos].Add(word, new List<SynSet>(synsets.Count));
                    foreach (var synset in synsets)
                        posWordSynSets[pos][word].Add(idSynset[synset.Id]);
                }
            }
        }

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            idSynset.Clear();
            posWordSynSets.Clear();
        }
        #endregion

        #region . Initialize .
        /// <summary>
        /// Initializes the WordNet provider.
        /// </summary>
        /// <param name="wordnet">The WordNet instance.</param>
        public void Initialize(WordNet wordnet) {
            wordNet = wordnet;
        }
        #endregion

        #region . GetSynSets .
        /// <summary>
        /// Gets all synsets for a word, optionally restricting the returned synsets to one or more parts of speech. This
        /// method does not perform any morphological analysis to match up the given word.
        /// </summary>
        /// <param name="word">Word to get SynSets for.</param>
        /// <param name="pos">Part-of-speech to search.</param>
        /// <returns>A readonly collection of SynSets that contain the requested word.</returns>
        public IReadOnlyCollection<SynSet> GetSynSets(string word, WordNetPos pos) {
            List<SynSet> list;
            return posWordSynSets[pos].TryGetValue(word, out list)
                ? new ReadOnlyCollection<SynSet>(list)
                : new ReadOnlyCollection<SynSet>(new SynSet[0]);
        }
        #endregion

        #region . GetSynSetDefinition .
        /// <summary>
        /// Gets the definition for a synset
        /// </summary>
        /// <param name="pos">Part-of-speech to get definition for.</param>
        /// <param name="offset">Offset or a index into data file.</param>
        public string GetSynSetDefinition(WordNetPos pos, int offset) {
            throw new NotSupportedException();
        }
        #endregion

        #region . GetAllWords .
        /// <summary>
        /// Gets all words with the specified part-of-speech.
        /// </summary>
        /// <param name="pos">The part-of-speech to get words for.</param>
        /// <returns>A readonly collection containing all the words with the specified part-of-speech tag.</returns>
        public IReadOnlyCollection<string> GetAllWords(WordNetPos pos) {
            return new ReadOnlyCollection<string>(posWordSynSets[pos].Keys.ToList());
        }
        #endregion

    }
}