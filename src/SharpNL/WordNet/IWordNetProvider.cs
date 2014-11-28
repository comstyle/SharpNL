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
    /// Represents a WordNet provider, which provides the data for the WordNet engine.
    /// </summary>
    public interface IWordNetProvider : IDisposable {

        /// <summary>
        /// Initializes the WordNet provider.
        /// </summary>
        /// <param name="wordNet">The WordNet.</param>
        void Initialize(WordNet wordNet);

        /// <summary>
        /// Gets all synsets for a word, optionally restricting the returned synsets to one or more parts of speech. This
        /// method does not perform any morphological analysis to match up the given word.
        /// </summary>
        /// <param name="word">Word to get SynSets for.</param>
        /// <param name="pos">Part-of-speech to search.</param>
        /// <returns>A readonly collection of SynSets that contain the requested word.</returns>
        IReadOnlyCollection<SynSet> GetSynSets(string word, WordNetPos pos);

        /// <summary>
        /// Gets the definition for a synset
        /// </summary>
        /// <param name="pos">Part-of-speech to get definition for.</param>
        /// <param name="offset">Offset or a index into data file.</param>
        string GetSynSetDefinition(WordNetPos pos, int offset);


        /// <summary>
        /// Gets all words with the specified part-of-speech.
        /// </summary>
        /// <param name="pos">The part-of-speech to get words for.</param>
        /// <returns>A readonly collection containing all the words with the specified part-of-speech tag.</returns>
        IReadOnlyCollection<string> GetAllWords(WordNetPos pos);

    }
}