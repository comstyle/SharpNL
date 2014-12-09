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

using SharpNL.Stemmer.RSLP;

using NUnit.Framework;

namespace SharpNL.Tests.Stemmer.RSLP {
    internal class RSLPStemmerTest {

        private Dictionary<string, string> dic;
            
        [TestFixtureSetUp]
        public void Setup() {
            dic = new Dictionary<string, string> {
                {"coração", "coraca"},
                {"coraçãozinho", "coraca"},
                {"funcionamento","funcion"},
                {"nervosos", "nerv"},
                {"continuar", "continu"},
                {"continuando", "continu"},
                {"demonstração", "demonstr"},
                {"finalidades", "final"},
                {"utilizar-se", "utilizar-s"},
                {"infelizmente", "infeliz"},
                {"comentário", "coment"},
                {"comentários", "coment"}
                
            };
        }

        [Test]
        public void StemmerTest() {
            var stemmer = new RSLPStemmer();

            foreach (var pair in dic) {
                var stem = stemmer.Stem(pair.Key);

                Assert.AreEqual(pair.Value, stem);
            }
        }

    }
}