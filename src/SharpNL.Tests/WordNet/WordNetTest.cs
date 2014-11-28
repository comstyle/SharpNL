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

using net.didion.jwnl.data;
using NUnit.Framework;

using SharpNL.WordNet;
using SharpNL.WordNet.Providers;


namespace SharpNL.Tests.WordNet {
    [TestFixture]
    internal class WordNetTest {


        private SharpNL.WordNet.WordNet wordNet;

        [TestFixtureSetUp]
        public void Setup() {
            wordNet = new SharpNL.WordNet.WordNet(new WordNetFileProvider(@"F:\NLP\WNdb-3.0\dict"));
            //wordNet = new SharpNL.WordNet.WordNet(new WordNetMemoryProvider(@"F:\NLP\WNdb-3.0\dict"));
        }


        [Test]
        public void GetSynSetsTest() {
            var synsets = wordNet.GetSynSets("nice");
            
            Assert.NotNull(synsets);
            Assert.AreEqual(6, synsets.Count);           
        }

        [Test]
        public void GetMostCommonSynSetTest() {

            var synset = wordNet.GetMostCommonSynSet("dog", WordNetPos.Noun);

            Assert.NotNull(synset);
        }

        [Test]
        public void GetAllWords() {

            var alot = wordNet.GetAllWords();

            Assert.NotNull(alot);

            Assert.AreEqual(true, alot.ContainsKey(WordNetPos.Adjective));
            Assert.AreEqual(true, alot.ContainsKey(WordNetPos.Adverb));
            Assert.AreEqual(true, alot.ContainsKey(WordNetPos.Noun));
            Assert.AreEqual(true, alot.ContainsKey(WordNetPos.Verb));

            Assert.AreEqual(false, alot.ContainsKey(WordNetPos.None));

            Assert.Greater(alot[WordNetPos.Adjective].Count, 1);
            Assert.Greater(alot[WordNetPos.Adverb].Count, 1);
            Assert.Greater(alot[WordNetPos.Noun].Count, 1);
            Assert.Greater(alot[WordNetPos.Verb].Count, 1);
        }
    }
}