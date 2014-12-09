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
using SharpNL.Stemmer.Porter.Algorithm;
using NUnit.Framework;

namespace SharpNL.Tests.Stemmer.Porter {
    [TestFixture]
    internal class PortugueseTest {

        [Test]
        public void TestSamples() {

            var dic = new Dictionary<string, string>() {
                {"boa", "boa"},
                {"boainain", "boainain"},
                {"boas", "boas"},
                {"bôas", "bôas"},
                {"boassu", "boassu"},
                {"boataria", "boat"},
                {"boate", "boat"},
                {"boates", "boat"},
                {"boatos", "boat"},
                {"bob", "bob"},
                {"boba", "bob"},
                {"bobagem", "bobag"},
                {"bobagens", "bobagens"},
                {"bobalhões", "bobalhõ"},
                {"bobear", "bob"},
                {"bobeira", "bobeir"},
                {"bobinho", "bobinh"},
                {"bobinhos", "bobinh"},
                {"bobo", "bob"},
                {"bobs", "bobs"},
                {"boca", "boc"},
                {"bocadas", "boc"},
                {"bocadinho", "bocadinh"},
                {"bocado", "boc"},
                {"bocaiúva", "bocaiúv"},
                {"boçal", "boçal"},
                {"bocarra", "bocarr"},
                {"bocas", "boc"},
                {"bode", "bod"},
                {"bodoque", "bodoqu"},
                {"body", "body"},
                {"boeing", "boeing"},
                {"boem", "boem"},
                {"boemia", "boem"},
                {"boêmio", "boêmi"},
                {"boêmios", "boêmi"},
                {"bogotá", "bogot"},
                {"boi", "boi"},
                {"bóia", "bói"},
                {"boiando", "boi"},
                {"quiabo", "quiab"},
                {"quicaram", "quic"},
                {"quickly", "quickly"},
                {"quieto", "quiet"},
                {"quietos", "quiet"},
                {"quilate", "quilat"},
                {"quilates", "quilat"},
                {"quilinhos", "quilinh"},
                {"quilo", "quil"},
                {"quilombo", "quilomb"},
                {"quilométricas", "quilométr"},
                {"quilométricos", "quilométr"},
                {"quilômetro", "quilômetr"},
                {"quilômetros", "quilômetr"},
                {"quilos", "quil"},
                {"química", "químic"},
                {"químicas", "químic"},
                {"químico", "químic"},
                {"químicos", "químic"},
                {"quimioterapia", "quimioterap"},
                {"quimioterápicos", "quimioteráp"},
                {"quimono", "quimon"},
                {"quincas", "quinc"},
                {"quinhão", "quinhã"},
                {"quinhentos", "quinhent"},
                {"quinn", "quinn"},
                {"quino", "quin"},
                {"quinta", "quint"},
                {"quintal", "quintal"},
                {"quintana", "quintan"},
                {"quintanilha", "quintanilh"},
                {"quintão", "quintã"},
                {"quintessência", "quintessent"},
                {"quintino", "quintin"},
                {"quinto", "quint"},
                {"quintos", "quint"},
                {"quintuplicou", "quintuplic"},
                {"quinze", "quinz"},
                {"quinzena", "quinzen"},
                {"quiosque", "quiosqu"},
            };

            var stemmer = new Portuguese();

            foreach (var pair in dic) {
                var stem = stemmer.Stem(pair.Key);
                Assert.AreEqual(pair.Value, stem);
            }
        }

    }
}