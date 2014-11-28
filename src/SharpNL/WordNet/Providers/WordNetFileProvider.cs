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
    /// Represents a default WordNet data provider, which uses files to provide the data
    /// for the WordNet engine. This class cannot be inherited.
    /// </summary>
    public sealed class WordNetFileProvider : IWordNetProvider {

        private WordNet wordNet;

        private readonly Dictionary<WordNetPos, Index> index;
        private readonly Dictionary<WordNetPos, StreamReader> data;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="WordNetFileProvider"/> class.
        /// </summary>
        /// <param name="dataPath">The data path.</param>
        /// <exception cref="System.ArgumentNullException">dataPath</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The data directory does not exist.</exception>
        /// <exception cref="System.IO.FileNotFoundException">A required WordNet file does not exist: [filename]</exception>
        public WordNetFileProvider(string dataPath) {
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

            index = new Dictionary<WordNetPos, Index>(4);
            foreach (var info in indexPaths) {
                index[GetFilePos(info.FullName)] = new Index(info);
            }

            data = new Dictionary<WordNetPos, StreamReader>(4);
            foreach (var info in dataPaths) {
                data[GetFilePos(info.FullName)] = new StreamReader(info.FullName);
            }
        }

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting 
        /// unmanaged resources.
        /// </summary>
        public void Dispose() {
            data.Clear();
            index.Clear();           
        }
        #endregion

        #region . GetFilePos .
        internal static WordNetPos GetFilePos(string path) {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext))
                return WordNetPos.None;

            switch (ext.ToLowerInvariant()) {
                case ".adj":
                    return WordNetPos.Adjective;
                case ".adv":
                    return WordNetPos.Adverb;
                case ".noun":
                    return WordNetPos.Noun;
                case ".verb":
                    return WordNetPos.Verb;
                default:
                    throw new InvalidOperationException("Unkown data file extension: " + ext);
            }
        }
        #endregion

        #region . Initialize .
        /// <summary>
        /// Initializes the WordNet provider.
        /// </summary>
        /// <param name="wordnet">The WordNet.</param>
        public void Initialize(WordNet wordnet) {
            wordNet = wordnet;
        }
        #endregion

        #region . GetAllWords .
        /// <summary>
        /// Gets all words with the specified part-of-speech.
        /// </summary>
        /// <param name="pos">The part-of-speech to get words for.</param>
        /// <returns>A readonly collection containing all the words with the specified part-of-speech tag.</returns>
        public IReadOnlyCollection<string> GetAllWords(WordNetPos pos) {
            return pos == WordNetPos.None ? null : index[pos].GetAllWords();
        }
        #endregion

        #region . GetSynSetDefinition .
        /// <summary>
        /// Gets the definition for a synset
        /// </summary>
        /// <param name="pos">Part-of-speech to get definition for.</param>
        /// <param name="offset">Offset or a index into data file.</param>
        public string GetSynSetDefinition(WordNetPos pos, int offset) {

            data[pos].DiscardBufferedData();
            data[pos].BaseStream.Position = offset;

            // read synset definition
            var synSetDefinition = data[pos].ReadLine();
            if (string.IsNullOrEmpty(synSetDefinition))
                return null;

            // make sure file positions line up
            if (int.Parse(synSetDefinition.Substring(0, synSetDefinition.IndexOf(' '))) != offset)
                throw new Exception("Position mismatch:  passed " + offset + " and got definition line \"" + synSetDefinition + "\"");

            return synSetDefinition;
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

            word = word.ToLowerInvariant().Replace(' ', '_');

            var indexLine = index[pos].Search(word);

            if (indexLine == null)
                return new ReadOnlyCollection<SynSet>(new SynSet[0]);

            SynSet mostCommonSynset;

            var synsets = GetSynSetShells(indexLine, pos, out mostCommonSynset, wordNet);
            foreach (var synset in synsets) {
                synset.Instantiate(this);
            }

            // we only need to set this flag if there is more than one synset for the word-pos pair
            if (synsets.Count > 1)
                mostCommonSynset.SetAsMostCommonSynsetFor(word);

            return new ReadOnlyCollection<SynSet>(synsets);
        }
        #endregion

        #region . GetSynSetShells .

        /// <summary>
        /// Gets synset shells from a word index line. A synset shell is an instance of SynSet with only the POS and Offset
        /// members initialized. These members are enough to look up the full synset within the corresponding data file. This
        /// method is static to prevent inadvertent references to a current WordNetEngine, which should be passed via the
        /// corresponding parameter.
        /// </summary>
        /// <param name="wordIndexLine">Word index line from which to get synset shells</param>
        /// <param name="pos">POS of the given index line</param>
        /// <param name="mostCommonSynSet">Returns the most common synset for the word</param>
        /// <param name="wordNet">The WordNet instance</param>
        /// <returns>Synset shells for the given index line</returns>
        /// <exception cref="System.Exception">Failed to get most common synset</exception>
        internal static List<SynSet> GetSynSetShells(string wordIndexLine, WordNetPos pos, out SynSet mostCommonSynSet, WordNet wordNet) {
            var synsets = new List<SynSet>();
            mostCommonSynSet = null;

            // get number of synsets
            var parts = wordIndexLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var numSynSets = int.Parse(parts[2]);

            // grab each synset shell, from last to first
            int firstOffsetIndex = parts.Length - numSynSets;
            for (int i = parts.Length - 1; i >= firstOffsetIndex; --i) {
                // create synset
                int offset = int.Parse(parts[i]);

                // add synset to collection                        
                var synset = new SynSet(pos, offset, wordNet);
                synsets.Add(synset);

                // if this is the last synset offset to get (since we grabbed them in reverse order), record it as the most common synset
                if (i == firstOffsetIndex)
                    mostCommonSynSet = synset;
            }

            if (mostCommonSynSet == null)
                throw new Exception("Failed to get most common synset");

            return synsets;
        }
        
        #endregion

        #region . GetSynSetDefinition .

        /// <summary>
        /// Gets definition line for synset from data file
        /// </summary>
        /// <param name="pos">POS to get definition for</param>
        /// <param name="offset">Offset into data file</param>
        internal string GetSynSetDefinition(WordNetPos pos, long offset) {
            // set data file to synset location

            data[pos].DiscardBufferedData();
            data[pos].BaseStream.Position = offset;

            // read synset definition
            var synSetDefinition = data[pos].ReadLine();

            if (string.IsNullOrEmpty(synSetDefinition))
                return null;

            // make sure file positions line up
            if (int.Parse(synSetDefinition.Substring(0, synSetDefinition.IndexOf(' '))) != offset)
                throw new Exception("Position mismatch:  passed " + offset + " and got definition line \"" + synSetDefinition + "\"");

            return synSetDefinition;
        }

        #endregion

        #region @ Index .
        private class Index : IDisposable {


            private readonly StreamReader Stream;

            public Index(FileInfo file) {
                Stream = new StreamReader(file.OpenRead());
            }

            private string ReadLine(ref uint position) {
                var text = Stream.ReadLine();
                if (text == null) {
                    return null;
                }
                position += (uint)Stream.CurrentEncoding.GetByteCount(text + "\n");
                return text;
            }
            public void Dispose() {
                Stream.Dispose();
            }

            public IReadOnlyCollection<string> GetAllWords() {
                Stream.BaseStream.Seek(0, SeekOrigin.Begin);
                Stream.DiscardBufferedData();

                var list = new List<string>();
                string line;
                while ((line = Stream.ReadLine()) != null)
                    if (!line.StartsWith(" "))
                        list.Add(line.Substring(0, line.IndexOf(' ')));

                return list.AsReadOnly();
            }

            public string Search(string word) {
                return Stream.BaseStream.Length == 0
                    ? null
                    : Search(word, 0, Stream.BaseStream.Length - 1);
            }

            private string Search(string word, long start, long end) {

                while (start <= end) {
                    Stream.BaseStream.Position = (long)((start + end) / 2.0);
                    int num = 0;
                    while (Stream.BaseStream.Position > 0L) {
                        int num2;
                        if ((num2 = Stream.BaseStream.ReadByte()) == -1) {
                            throw new Exception("Failed to read byte");
                        }
                        var c = (char)num2;
                        if (++num > 1 && c == '\n') {
                            break;
                        }
                        Stream.BaseStream.Position -= 2L;
                    }
                    long position = Stream.BaseStream.Position;
                    uint num3 = (uint)position;
                    if (num3 != (ulong)position) {
                        throw new Exception("uint overflow");
                    }
                    Stream.DiscardBufferedData();
                    var text = ReadLine(ref num3);

                    //num3 -= 1u;

                    int comparer;
                    if (text[0] == ' ' || string.IsNullOrEmpty(text))
                        comparer = 1; // header, search further down
                    else {
                        var curWord = text.Substring(0, text.IndexOf(' '));

                        comparer = String.Compare(word, curWord, StringComparison.OrdinalIgnoreCase);
                    }

                    if (comparer == 0)
                        return text;

                    if (comparer < 0) {
                        end = position - 1L;
                    } else {
                        start = num3 + 1u;
                    }
                }
                return null;
            }

        }
        #endregion

    }
}