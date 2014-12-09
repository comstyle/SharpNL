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
    /// Represents a English Porter Stemmer algorithm. 
    /// This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// The English stemming algorithm can be found in
    /// <see cref="http://snowball.tartarus.org/algorithms/english/stemmer.html"/>
    /// </remarks>
    public sealed class English : PorterAlgorithm {

        private static readonly Dictionary<string, string> exceptions;
        private static readonly string[] doubleConsonants;
        private static readonly char[] liEnding;
        private static readonly Suffixes[] rules;


        static English() {
            exceptions = new Dictionary<string, string> {
                {"skis", "ski"},
                {"skies", "sky"},
                {"dying", "die"},
                {"lying", "lie"},
                {"tying", "tie"},
                {"idly", "idl"},
                {"gently", "gentl"},
                {"ugly", "ugli"},
                {"early", "earli"},
                {"only", "onli"},
                {"singly", "singl"},
                {"sky", "sky"},
                {"news", "news"},
                {"howe", "howe"},
                {"atlas", "atlas"},
                {"cosmos", "cosmos"},
                {"bias", "bias"},
                {"andes", "andes"},
                {"inning", "inning"},
                {"innings", "inning"},
                {"outing", "outing"},
                {"outings", "outing"},
                {"canning", "canning"},
                {"cannings", "canning"},
                {"herring", "herring"},
                {"herrings", "herring"},
                {"earring", "earring"},
                {"earrings", "earring"},
                {"proceed", "proceed"},
                {"proceeds", "proceed"},
                {"proceeded", "proceed"},
                {"proceeding", "proceed"},
                {"exceed", "exceed"},
                {"exceeds", "exceed"},
                {"exceeded", "exceed"},
                {"exceeding", "exceed"},
                {"succeed", "succeed"},
                {"succeeds", "succeed"},
                {"succeeded", "succeed"},
                {"succeeding", "succeed"}
            };

            doubleConsonants = new[] {
                "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt"
            };

            liEnding = new[] {
                'c', 'd', 'e', 'g', 'h', 'k', 'm', 'n', 'r', 't'
            };

            rules = new[] {
                // step0 #0
                new Suffixes("'s'", "'s", "'"), 

                // step1 #1-2
                new Suffixes("sses", "ied", "ies", "us", "ss", "s"), 
                new Suffixes("eedly", "ingly", "edly", "eed", "ing", "ed"), 

                // step2 #3
                new Suffixes("ization", "ational", "fulness", "ousness", "iveness", "tional", "biliti", "lessli", "entli", "ation", "alism", "aliti", "ousli", "iviti", "fulli", "enci", "anci", "abli", "izer", "ator", "alli", "bli", "ogi", "li"), 

                // step3 #4
                new Suffixes("ational", "tional", "alize", "icate", "iciti", "ative", "ical", "ness", "ful"), 

                // step4 #5
                new Suffixes("ement", "ance", "ence", "able", "ible", "ment", "ant", "ent", "ism", "ate", "iti", "ous", "ive", "ize", "ion", "al", "er", "ic"),

                // step5 #6
                new Suffixes("e", "l")

            };

        }

        public English() {
            Vowels = new HashSet<char>(new [] {
                'a', 'e', 'i', 'o', 'u', 'y'
            });
        }

        /// <summary>
        /// Stems the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stem.</returns>
        public override string Stem(string word) {
            if (word.Length <= 2)
                return word;

            if (exceptions.ContainsKey(word))
                return exceptions[word];

            // Apostrophe characters [’‘‛]
            word = word.Replace('\u2019', '\'').Replace('\u2018', '\'').Replace('\u201B', '\'');

            if (word.StartsWith("'"))
                word = word.Substring(1);

            if (word[0] == 'y')
                word = "Y" + word.Substring(1);

            for (var i = 1; i < word.Length; i++)
                if (word[i] == 'y' && IsVowel(word[i - 1]))
                    word = string.Format("{0}Y{1}", word.Substring(0, i), word.Substring(i + 1));

            string r1;
            string r2 = string.Empty;
            string suffix;

            if (word.StartsWith("gener", "commun", "arsen")) {
                r1 = word.StartsWith("gener", "arsen")
                    ? word.Substring(5)
                    : word.Substring(6);

                for (var i = 1; i < r1.Length; i++) {
                    if (IsConsonant(r1[i]) && IsVowel(r1[i - 1])) {
                        r2 = r1.Substring(i + 1);
                        break;
                    }
                }
            } else {
                r1 = FindR(word);
                r2 = FindR(r1);
            }

            // Step 0
            if (rules[0].TryMatch(word, out suffix)) {
                r1 = r1.Length > suffix.Length
                    ? r1.Left(-suffix.Length)
                    : string.Empty;

                r2 = r2.Length > suffix.Length
                    ? r2.Left(-suffix.Length)
                    : string.Empty;

                word = word.Left(-suffix.Length);
            }

            // Step 1a
            if (rules[1].TryMatch(word, out suffix)) {
                switch (suffix) {
                    case "sses":
                        r1 = r1.Left(-2);
                        r2 = r2.Left(-2);
                        word = word.Left(-2);
                        break;
                    case "ied":
                    case "ies":
                        if (word.Left(-suffix.Length).Length > 1) {
                            r1 = r1.Left(-2);
                            r2 = r2.Left(-2);
                            word = word.Left(-2);
                        } else {
                            r1 = r1.Left(-1);
                            r2 = r2.Left(-1);
                            word = word.Left(-1);
                        }
                        break;
                    case "s":
                        if (ContainsVowel(word.Left(-2))) {
                            r1 = r1.Left(-1);
                            r2 = r2.Left(-1);

                            word = word.Left(-1);
                            
                        }
                        break;
                }
            }

            // Step 1b
            if (rules[2].TryMatch(word, out suffix)) {
                if (suffix.In("eed", "eedly")) {
                    if (r1.EndsWith(suffix)) {
                        r1 = r1.Length >= suffix.Length
                            ? r1.Left(-suffix.Length) + "ee"
                            : string.Empty;

                        r2 = r2.Length >= suffix.Length
                            ? r2.Left(-suffix.Length) + "ee"
                            : string.Empty;

                        word = word.Left(-suffix.Length) + "ee";
                    }

                } else {
                    if (ContainsVowel(word.Left(-suffix.Length))) {

                        r1 = r1.Left(-suffix.Length);
                        r2 = r2.Left(-suffix.Length);

                        word = word.Left(-suffix.Length);

                        if (word.EndsWith("at", "bl", "iz")) {
                            r1 += "e";

                            if (word.Length > 5 || r1.Length >= 3)
                                r2 += "e";

                            word += "e";
                        } else if (word.EndsWith(doubleConsonants)) {
                            r1 = r1.Left(-1);
                            r2 = r2.Left(-1);

                            word = word.Left(-1);

                        } else if (
                            (r1 == string.Empty && word.Length >= 3 && IsConsonant(word[word.Length - 1]) &&
                             !word[word.Length - 1].In('w', 'x', 'Y') && IsVowel(word[word.Length - 2]) &&
                             IsConsonant(word[word.Length - 3])) ||
                            (r1 == string.Empty && word.Length == 2 && IsVowel(word[0]) && IsConsonant(word[1]))) {

                            word += "e";

                            if (r2.Length > 0)
                                r2 += "e";
                        }
                    }
                }
            }



            // Step 1c
            if (word.Length > 2 && word[word.Length - 1].In('y', 'Y') && IsConsonant(word[word.Length - 2])) {
                word = word.Left(-1) + "i";

                r1 = r1.Length >= 1
                    ? r1.Left(-1) + "i"
                    : string.Empty;

                r2 = r2.Length >= 2
                    ? r2.Left(-1) + "i"
                    : string.Empty;
            }

            // Step 2
            if (rules[3].TryMatch(word, out suffix)) {
                if (r1.EndsWith(suffix)) {
                    switch (suffix) {
                        case "entli":
                        case "tional":
                        case "fulli":
                        case "lessli":
                            r1 = r1.Left(-2);
                            r2 = r2.Left(-2);
                            word = word.Left(-2);
                            break;
                        case "abli":
                        case "anci":
                        case "enci":
                            r1 = r1.Length >= 1
                                ? r1.Left(-1) + "e"
                                : string.Empty;

                            r2 = r2.Length >= 1
                                ? r2.Left(-1) + "e"
                                : string.Empty;

                            word = word.Left(-1) + "e";
                            break;
                        case "izer":
                        case "ization":

                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ize"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ize"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "ize";

                            break;
                        case "ational":
                        case "ation":
                        case "ator":

                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ate"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ate"
                                : "e";

                            word = word.Left(-suffix.Length) + "ate";

                            break;
                        case "alism":
                        case "aliti":
                        case "alli":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "al"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "al"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "al";

                            break;
                        case "fulness":
                            r1 = r1.Left(-4);
                            r2 = r2.Left(-4);
                            word = word.Left(-4);
                            break;
                        case "ousli":
                        case "ousness":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ous"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ous"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "ous";

                            break;
                        case "iveness":
                        case "iviti":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ive"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ive"
                                : "e";

                            word = word.Left(-suffix.Length) + "ive";

                            break;
                        case "biliti":
                        case "bli":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ble"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ble"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "ble";

                            break;
                        case "li":
                            if (word.Length > 3 && word[word.Length - 3].In(liEnding)) {
                                r1 = r1.Left(-2);
                                r2 = r2.Left(-2);
                                word = word.Left(-2);
                            }
                            break;
                    }
                }
            }

            // Step 3
            if (rules[4].TryMatch(word, out suffix)) {
                if (r1.EndsWith(suffix)) {
                    switch (suffix) {
                        case "tional":
                            r1 = r1.Left(-2);
                            r2 = r2.Left(-2);
                            word = word.Left(-2);
                            break;
                        case "ational":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ate"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ate"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "ate";
                            break;
                        case "alize":
                            r1 = r1.Left(-3);
                            r2 = r2.Left(-3);
                            word = word.Left(-3);
                            break;
                        case "icate":
                        case "iciti":
                        case "ical":
                            r1 = r1.Length >= suffix.Length
                                ? r1.Left(-suffix.Length) + "ic"
                                : string.Empty;

                            r2 = r2.Length >= suffix.Length
                                ? r2.Left(-suffix.Length) + "ic"
                                : string.Empty;

                            word = word.Left(-suffix.Length) + "ic";
                            break;
                        case "ful":
                        case "ness":
                            r1 = r1.Left(-suffix.Length);
                            r2 = r2.Left(-suffix.Length);
                            word = word.Left(-suffix.Length);
                            break;
                        case "ative":
                            if (r2.EndsWith(suffix)) {
                                r1 = r1.Left(-5);
                                r2 = r2.Left(-5);
                                word = word.Left(-5);
                            }
                            break;

                    }
                }
            }

            // Step 4
            if (rules[5].TryMatch(word, out suffix)) {
                if (r2.EndsWith(suffix)) {
                    if (suffix == "ion") {
                        if (word.Length >= 4 && word[word.Length - 4].In('s', 't')) {
                            r1 = r1.Left(-3);
                            r2 = r2.Left(-3);
                            word = word.Left(-3);
                        }
                    } else {
                        r1 = r1.Left(-suffix.Length);
                        r2 = r2.Left(-suffix.Length);
                        word = word.Left(-suffix.Length);
                    }
                }
            }

            // Step 5
            if (word.Length > 2 && r2.EndsWith("l") && word[word.Length - 2] == 'l')
                word = word.Left(-1);
            else if (r2.EndsWith("e"))
                word = word.Left(-1);
            else if (r1.EndsWith("e")) {
                if (word.Length >= 4 && (
                    IsVowel(word[word.Length - 2]) ||
                    word[word.Length - 2].In('w','x','Y') ||
                    IsConsonant(word[word.Length - 3]) ||
                    IsVowel(word[word.Length - 4])))

                    word = word.Left(-1);
            }

            return word.Replace("Y", "y");
        }
    }
}