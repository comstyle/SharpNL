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

using System.Collections.Generic;
using NUnit.Framework;
using SharpNL.Stemmer.Porter;
using SharpNL.Stemmer.Porter.Algorithm;

namespace SharpNL.Tests.Stemmer.Porter {
    [TestFixture]
    internal class EnglishTest {

        private Dictionary<string, string> dic;

        #region . Setup .

        [TestFixtureSetUp]
        public void Setup() {
            dic = new Dictionary<string, string>() {
                {"consign", "consign"},
                {"consigned", "consign"},
                {"consigning", "consign"},
                {"consignment", "consign"},
                {"consist", "consist"},
                {"consisted", "consist"},
                {"consistency", "consist"},
                {"consistent", "consist"},
                {"consistently", "consist"},
                {"consisting", "consist"},
                {"consists", "consist"},
                {"consolation", "consol"},
                {"consolations", "consol"},
                {"consolatory", "consolatori"},
                {"console", "consol"},
                {"consoled", "consol"},
                {"consoles", "consol"},
                {"consolidate", "consolid"},
                {"consolidated", "consolid"},
                {"consolidating", "consolid"},
                {"consoling", "consol"},
                {"consolingly", "consol"},
                {"consols", "consol"},
                {"consonant", "conson"},
                {"consort", "consort"},
                {"consorted", "consort"},
                {"consorting", "consort"},
                {"conspicuous", "conspicu"},
                {"conspicuously", "conspicu"},
                {"conspiracy", "conspiraci"},
                {"conspirator", "conspir"},
                {"conspirators", "conspir"},
                {"conspire", "conspir"},
                {"conspired", "conspir"},
                {"conspiring", "conspir"},
                {"constable", "constabl"},
                {"constables", "constabl"},
                {"constance", "constanc"},
                {"constancy", "constanc"},
                {"constant", "constant"},
                {"knack", "knack"},
                {"knackeries", "knackeri"},
                {"knacks", "knack"},
                {"knag", "knag"},
                {"knave", "knave"},
                {"knaves", "knave"},
                {"knavish", "knavish"},
                {"kneaded", "knead"},
                {"kneading", "knead"},
                {"knee", "knee"},
                {"kneel", "kneel"},
                {"kneeled", "kneel"},
                {"kneeling", "kneel"},
                {"kneels", "kneel"},
                {"knees", "knee"},
                {"knell", "knell"},
                {"knelt", "knelt"},
                {"knew", "knew"},
                {"knick", "knick"},
                {"knif", "knif"},
                {"knife", "knife"},
                {"knight", "knight"},
                {"knightly", "knight"},
                {"knights", "knight"},
                {"knit", "knit"},
                {"knits", "knit"},
                {"knitted", "knit"},
                {"knitting", "knit"},
                {"knives", "knive"},
                {"knob", "knob"},
                {"knobs", "knob"},
                {"knock", "knock"},
                {"knocked", "knock"},
                {"knocker", "knocker"},
                {"knockers", "knocker"},
                {"knocking", "knock"},
                {"knocks", "knock"},
                {"knopp", "knopp"},
                {"knot", "knot"},
                {"knots", "knot"},
            }; 
        }

        #endregion

        [Test]
        public void TestSamples() {
            var stemmer = new English();

            foreach (var pair in dic) {
                var stem = stemmer.Stem(pair.Key);
                
                Assert.AreEqual(pair.Value, stem);
            }
        }

        [Test]
        public void TestFromPorter() {
            var stemmer = new PorterStemmer("en");

            foreach (var pair in dic) {
                var stem = stemmer.Stem(pair.Key);

                Assert.AreEqual(pair.Value, stem);
            }

        }

    }
}