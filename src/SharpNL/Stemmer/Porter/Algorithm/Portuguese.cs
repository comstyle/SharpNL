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
using SharpNL.Extensions;

namespace SharpNL.Stemmer.Porter.Algorithm {
    /// <summary>
    /// Represents a Portuguese stemmer using Porter's algorithm. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This implementation follows the definition in:
    /// <see cref="http://snowball.tartarus.org/algorithms/portuguese/stemmer.html"/>
    /// 
    /// Note: I remove the Step1 3rd rule [ución -> uciones], it's clearly not Portuguese :P
    /// 
    /// </remarks>
    public sealed class Portuguese : PorterAlgorithm {

        private static readonly Suffixes[] rules;

        static Portuguese() {
            rules = new[] {
                // step 1
                new Suffixes("amentos", "imentos", "amento", "imento", "adoras", "adores", "a\u00e7o~es", "ismos", "istas", "adora", "a\u00e7a~o", "antes", "\u00e2ncia", "ezas", "icos", "icas", "ismo", "\u00e1vel", "\u00edvel", "ista", "osos", "osas", "ador", "ante", "eza", "ico", "ica", "oso", "osa"), 
                new Suffixes("log\u00edas", "log\u00eda"), 
                new Suffixes("\u00eancias", "\u00eancia"),
                new Suffixes("amente"),
                new Suffixes("mente"),
                new Suffixes("idades", "idade"),
                new Suffixes("ivas", "ivos", "iva", "ivo"),
                new Suffixes("iras", "ira"),

                // step 2
                new Suffixes("ar\u00edamos", "er\u00edamos", "ir\u00edamos", "\u00e1ssemos", "\u00eassemos", "\u00edssemos", "ar\u00edeis", "er\u00edeis", "ir\u00edeis", "\u00e1sseis", "\u00e9sseis", "\u00edsseis", "\u00e1ramos", "\u00e9ramos", "\u00edramos", "\u00e1vamos", "aremos", "eremos", "iremos", "ariam", "eriam", "iriam", "assem", "essem", "issem", "ara~o", "era~o", "ira~o", "arias", "erias", "irias", "ardes", "erdes", "irdes", "asses", "esses", "isses", "astes", "estes", "istes", "\u00e1reis", "areis", "\u00e9reis", "ereis", "\u00edreis", "ireis", "\u00e1veis", "\u00edamos", "armos", "ermos", "irmos", "aria", "eria", "iria", "asse", "esse", "isse", "aste", "este", "iste", "arei", "erei", "irei", "aram", "eram", "iram", "avam", "arem", "erem", "irem", "ando", "endo", "indo", "adas", "idas", "ar\u00e1s", "aras", "er\u00e1s", "eras", "ir\u00e1s", "avas", "ares", "eres", "ires", "\u00edeis", "ados", "idos", "\u00e1mos", "amos", "emos", "imos", "iras", "ada", "ida", "ar\u00e1", "ara", "er\u00e1", "era", "ir\u00e1", "ava", "iam", "ado", "ido", "ias", "ais", "eis", "ira", "ia", "ei", "am", "em", "ar", "er", "ir", "as", "es", "is", "eu", "iu", "ou"),
                
                // step 4
                new Suffixes("os", "a", "i", "o", "\u00e1", "\u00ed", "\u00f3"),

                // step 5
                new Suffixes("e", "\u00e9", "\u00ea")
            };
        }

        public Portuguese() {
            Vowels = new HashSet<char>(new[] {
                'a', 'e', 'i', 'o', 'u', '\u00e1', '\u00e9', '\u00ed', '\u00f3', '\u00fa', '\u00e2', '\u00ea', '\u00f4'
            });
        }

        /// <summary>
        /// Stems the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stem.</returns>
        public override string Stem(string word) {

            word = word.Replace("\u00e3", "a~").Replace("\u00f5", "o~");

            var rv = FindRV(word);
            var r1 = FindR(word);
            var r2 = FindR(r1);

            string stem;

            var step1 = Step1(out stem, word, r2, rv);
            var step2 = false;

            if (!step1) {
                step2 = Step2(out stem, stem, rv);
            }

            if (step1 || step2) {
                rv = FindRV(stem);
                stem = Step3(stem, rv);
            }

            if (!step1 && !step2) {
                rv = FindRV(stem);
                stem = Step4(stem, rv);
            }
                
            rv = FindRV(stem);
            stem = Step5(stem, rv);

            return stem.Replace("a~", "\u00e3").Replace("o~", "\u00f5");
        }

        private static bool Step1(out string stem, string word, string r2, string rv) {
            string suffix;

            for (var i = 0; i <= 6; i++) {
                if (!rules[i].TryMatch(r2, out suffix)) 
                    continue;

                stem = word.Left(-suffix.Length);

                switch (i) {
                    case 0:
                        return true;
                    case 1:
                        stem += "log";
                        return true;
                    case 2:
                        stem += "ente";
                        return true;
                    case 3: // amente
                        if (stem.EndsWith("iv") && r2.EndsWith("iv" + suffix)) {
                            stem = stem.Left(-2);

                            if (stem.EndsWith("at") && r2.EndsWith("ativ" + suffix))
                                stem = stem.Left(-2);

                        } else if (
                            (stem.EndsWith("os") && r2.EndsWith("os" + suffix)) ||
                            (stem.EndsWith("ic") && r2.EndsWith("ic" + suffix)) || 
                            (stem.EndsWith("ad") && r2.EndsWith("ad" + suffix)))

                            stem = stem.Left(-2);

                        return true;
                    case 4: // mente
                        if ((stem.EndsWith("ante") && r2.EndsWith("ante" + suffix)) ||
                            (stem.EndsWith("avel") && r2.EndsWith("avel" + suffix)) ||
                            (stem.EndsWith("ível") && r2.EndsWith("ível" + suffix)))

                            stem = stem.Left(-4);

                        return true;
                    case 5:
                        if (stem.EndsWith("abil") && r2.EndsWith("abil" + suffix))
                            stem = stem.Left(-4);
                        else if (
                            (stem.EndsWith("ic") && r2.EndsWith("ic" + suffix)) ||
                            (stem.EndsWith("iv") && r2.EndsWith("iv" + suffix)))

                            stem = stem.Left(-2);

                        return true;
                    case 6:
                        if (stem.EndsWith("at") && r2.EndsWith("at" + suffix))
                            stem = stem.Left(-2);

                        return true;
                }
            }

            // ira iras
            if (rules[7].TryMatch(rv, out suffix) && word.EndsWith("e" + suffix)) {

                stem = word.Left(-suffix.Length) + "ir";

                return true;
            }

            stem = word;
            return false;
        }

        private static bool Step2(out string stem, string word, string rv) {
            string suffix;
            if (rules[8].TryMatch(rv, out suffix)) {
                stem = word.Left(-suffix.Length);
                return true;
            }
            stem = word;
            return false;
        }

        private static string Step3(string word, string rv) {
            return rv.EndsWith("i") && word.EndsWith("ci")
                ? word.Left(-1)
                : word;
        }

        private static string Step4(string word, string rv) {
            string suffix;
            return rules[9].TryMatch(rv, out suffix)
                ? word.Left(-suffix.Length)
                : word;
        }

        private static string Step5(string word, string rv) {
            string suffix;
            if (rules[10].TryMatch(rv, out suffix)) {

                word = word.Left(-suffix.Length);

                if ((word.EndsWith("gu") && rv.EndsWith("u" + suffix)) ||
                    (word.EndsWith("ci") && rv.EndsWith("i" + suffix)))
                    word = word.Left(-1);

                return word;
            }

            if (word.EndsWith("\xE7"))
                word = word.Left(-1) + "c";

            return word;
        }
    }
}