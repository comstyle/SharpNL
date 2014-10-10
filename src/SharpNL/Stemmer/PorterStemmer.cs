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
using SharpNL.Java;
using SharpNL.Utility;

namespace SharpNL.Stemmer {

    /// <summary>
    /// Stemmer, implementing the Porter Stemming Algorithm.
    /// <para>
    /// The Stemmer class transforms a word into its root form. 
    /// The input word can be provided a character at time (by calling <see cref="Add"/>), or at once
    /// by calling one of the various <see cref="Stem(string)"/> methods.
    /// </para>
    /// </summary>
    [TypeClass("opennlp.tools.stemmer.PorterStemmer")]
    public class PorterStemmer : IStemmer {
        private const int INC = 50;
        private bool dirty;

        private int j, k, k0;

        public PorterStemmer() {
            ResultBuffer = new char[INC];
            ResultLength = 0;
        }

        #region + Properties .

        #region . ResultLength .
        /// <summary>
        /// Gets the length of the word resulting from the stemming process.
        /// </summary>
        /// <value>The length of the word resulting from the stemming process.</value>
        public int ResultLength { get; private set; }
        #endregion

        #region . ResultBuffer .
        /// <summary>
        /// Gets a character buffer containing the results of the stemming process.
        /// You also need to consult <see cref="ResultLength"/> to determine the length of the result.
        /// </summary>
        /// <value>The result buffer.</value>
        public char[] ResultBuffer { get; private set; }
        #endregion

        #endregion


        /// <summary>
        /// Reduces the given word into its stem.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stemmed word.</returns>
        public string Stem(string word) {
            return Stem(word.ToCharArray(), word.Length) ? ToString() : word;
        }


        /// <summary>
        /// Stem a word contained in a char array.
        /// Returns true if the stemming process resulted in a word different from the input.
        /// </summary>
        /// <param name="word">The word.</param>
        public bool Stem(char[] word) {
            return Stem(word, word.Length);
        }


        /// <summary>
        /// Stem a word contained in a char array.
        /// Returns true if the stemming process resulted in a word different from the input.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public bool Stem(char[] word, int offset, int length) {
            Reset();
            if (ResultBuffer.Length < length) {
                ResultBuffer = new char[length - offset];
            }
            Array.Copy(word, offset, ResultBuffer, 0, length);
            ResultLength = length;
            return Stem(0);
        }

        /// <summary>
        /// Stem a word contained in a leading portion of a char[] array.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="wordLen">Length of the word.</param>
        /// <returns><c>true</c> the stemming process resulted in a word different 
        /// from the input, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// You can retrieve the result with <see cref="ResultBuffer"/>, <see cref="ResultLength"/> or <see cref="ToString"/>.
        /// </remarks>
        public bool Stem(char[] word, int wordLen) {
            return Stem(word, 0, wordLen);
        }

        /// <summary>
        /// Stem the word placed into the Stemmer buffer through calls to <see cref="Add"/>.
        /// </summary>
        /// <returns><c>true</c> if the stemming process resulted in a word different from the input, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// You can retrieve the result with <see cref="ResultBuffer"/>, <see cref="ResultLength"/> or <see cref="ToString"/>.
        /// </remarks>
        public bool Stem() {
            return Stem(0);
        }

        private bool Stem(int i0) {
            k = ResultLength - 1;
            k0 = i0;
            if (k > k0 + 1) {
                Step1();
                Step2();
                Step3();
                Step4();
                Step5();
                Step6();
            }
            // Also, a word is considered dirty if we lopped off letters
            // Thanks to Ifigenia Vairelles for pointing this out.
            if (ResultLength != k + 1)
                dirty = true;

            ResultLength = k + 1;
            return dirty;
        }

        /// <summary>
        /// Resets the stemmer so it can stem another word.
        /// <para>
        /// If you invoke the stemmer by calling <see cref="Add(char)"/> and then <see cref="Stem()"/>, 
        /// you must call <see cref="Reset()"/> before starting another word.
        /// </para>
        /// </summary>
        public void Reset() {
            ResultLength = 0;
            dirty = false;
        }

        /// <summary>
        /// Add a character to the word being stemmed.
        /// <para>
        /// When you are finished adding characters, you can call <see cref="Stem()"/> to process the word.
        /// </para>
        /// </summary>
        /// <param name="ch">The ch.</param>
        public void Add(char ch) {
            if (ResultBuffer.Length == ResultLength) {
                var new_b = new char[ResultLength + INC];
                for (var c = 0; c < ResultLength; c++) new_b[c] = ResultBuffer[c];
                {
                    ResultBuffer = new_b;
                }
            }
            ResultBuffer[ResultLength++] = ch;
        }

        /// <summary>
        /// After a word has been stemmed, it can be retrieved by <see cref="ToString"/>.
        /// </summary>
        /// <returns>A string that represents the current stemed value.</returns>
        public override string ToString() {
            return new string(ResultBuffer, 0, ResultLength);
        }



        /// <summary>
        /// cons(i) is true &lt;=&gt; b[i] is a consonant.
        /// </summary>
        private bool Cons(int index) {
            switch (ResultBuffer[index]) {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return false;
                case 'y':
                    return (index == k0) || !Cons(index - 1);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Measures the number of consonant sequences between k0 and j.
        /// </summary>
        private int m() {
            /* if c is a consonant sequence and v a vowel sequence, 
               and <..> indicates arbitrary presence,

                  <c><v>       gives 0
                  <c>vc<v>     gives 1
                  <c>vcvc<v>   gives 2
                  <c>vcvcvc<v> gives 3
                  ....
            */

            var n = 0;
            var i = k0;
            while (true) {
                if (i > j)
                    return n;
                if (! Cons(i))
                    break;
                i++;
            }
            i++;
            while (true) {
                while (true) {
                    if (i > j)
                        return n;
                    if (Cons(i))
                        break;
                    i++;
                }
                i++;
                n++;
                while (true) {
                    if (i > j)
                        return n;
                    if (! Cons(i))
                        break;
                    i++;
                }
                i++;
            }
        }

        /// <summary>
        /// k0,...j contains a vowel
        /// </summary>
        private bool containsVowel() {
            int i;
            for (i = k0; i <= j; i++)
                if (! Cons(i))
                    return true;
            return false;
        }

        /// <summary>
        /// j,(j-1) contain a double consonant.
        /// </summary>
        private bool doubleConstant(int j0) {
            if (j0 < k0 + 1)
                return false;

            return ResultBuffer[j0] == ResultBuffer[j0 - 1] && Cons(j0);
        }

        /// <summary>
        /// i-2,i-1,i has the form consonant - vowel - consonant and also if the second c is not w,x or y.
        /// </summary>
        /// <remarks>
        /// this is used when trying to restore an e at the end of a short word. 
        /// 
        /// e.g.
        /// cav(e), lov(e), hop(e), crim(e), but
        /// snow, box, tray.
        /// </remarks>
        private bool cvc(int i) {
            if (i < k0 + 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2))
                return false;
            int ch = ResultBuffer[i];
            if (ch == 'w' || ch == 'x' || ch == 'y') return false;
            return true;
        }

        private bool ends(string s) {
            var l = s.Length;
            var o = k - l + 1;
            if (o < k0)
                return false;
            for (var i = 0; i < l; i++)
                if (ResultBuffer[o + i] != s[i])
                    return false;
            j = k - l;
            return true;
        }

        /// <summary>
        /// sets (j+1),...k to the characters in the string s, readjusting k.
        /// </summary>
        private void setTo(string s) {
            var l = s.Length;
            var o = j + 1;
            for (var i = 0; i < l; i++)
                ResultBuffer[o + i] = s[i];
            k = j + l;
            dirty = true;
        }

        /// <summary>
        /// is used further down.
        /// </summary>
        private void r(string s) {
            if (m() > 0) setTo(s);
        }

        /// <summary>
        /// gets rid of plurals and -ed or -ing.
        /// </summary>
        /// <remarks>
        /// e.g.
        /// <para>
        /// caresses  ->  caress
        /// ponies    ->  poni
        /// ties      ->  ti
        /// caress    ->  caress
        /// cats      ->  cat
        /// 
        /// feed      ->  feed
        /// agreed    ->  agree
        /// disabled  ->  disable
        /// 
        /// matting   ->  mat
        /// mating    ->  mate
        /// meeting   ->  meet
        /// milling   ->  mill
        /// messing   ->  mess
        /// 
        /// meetings  ->  meet
        /// </para>
        /// </remarks>
        private void Step1() {
            if (ResultBuffer[k] == 's') {
                if (ends("sses")) k -= 2;
                else if (ends("ies")) setTo("i");
                else if (ResultBuffer[k - 1] != 's') k--;
            }
            if (ends("eed")) {
                if (m() > 0)
                    k--;
            } else if ((ends("ed") || ends("ing")) && containsVowel()) {
                k = j;
                if (ends("at")) setTo("ate");
                else if (ends("bl")) setTo("ble");
                else if (ends("iz")) setTo("ize");
                else if (doubleConstant(k)) {
                    int ch = ResultBuffer[k--];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        k++;
                } else if (m() == 1 && cvc(k))
                    setTo("e");
            }
        }

        /// <summary>
        /// turns terminal y to i when there is another vowel in the stem.
        /// </summary>
        private void Step2() {
            if (ends("y") && containsVowel()) {
                ResultBuffer[k] = 'i';
                dirty = true;
            }
        }

        /// <summary>
        /// maps double suffices to single ones. so -ization ( = -ize plus -ation) 
        /// maps to -ize etc. note that the string before the suffix must give m() > 0.
        /// </summary>
        private void Step3() {
            if (k == k0) return;
            switch (ResultBuffer[k - 1]) {
                case 'a':
                    if (ends("ational")) {
                        r("ate");
                        break;
                    }
                    if (ends("tional")) {
                        r("tion");
                    }
                    break;
                case 'c':
                    if (ends("enci")) {
                        r("ence");
                        break;
                    }
                    if (ends("anci")) {
                        r("ance");
                    }
                    break;
                case 'e':
                    if (ends("izer")) {
                        r("ize");
                    }
                    break;
                case 'l':
                    if (ends("bli")) {
                        r("ble");
                        break;
                    }
                    if (ends("alli")) {
                        r("al");
                        break;
                    }
                    if (ends("entli")) {
                        r("ent");
                        break;
                    }
                    if (ends("eli")) {
                        r("e");
                        break;
                    }
                    if (ends("ousli")) {
                        r("ous");
                    }
                    break;
                case 'o':
                    if (ends("ization")) {
                        r("ize");
                        break;
                    }
                    if (ends("ation")) {
                        r("ate");
                        break;
                    }
                    if (ends("ator")) {
                        r("ate");
                    }
                    break;
                case 's':
                    if (ends("alism")) {
                        r("al");
                        break;
                    }
                    if (ends("iveness")) {
                        r("ive");
                        break;
                    }
                    if (ends("fulness")) {
                        r("ful");
                        break;
                    }
                    if (ends("ousness")) {
                        r("ous");
                    }
                    break;
                case 't':
                    if (ends("aliti")) {
                        r("al");
                        break;
                    }
                    if (ends("iviti")) {
                        r("ive");
                        break;
                    }
                    if (ends("biliti")) {
                        r("ble");
                    }
                    break;
                case 'g':
                    if (ends("logi")) {
                        r("log");
                    }
                    break;
            }
        }

        /// <summary>
        /// deals with -ic-, -full, -ness etc. similar strategy to step3.
        /// </summary>
        private void Step4() {
            switch (ResultBuffer[k]) {
                case 'e':
                    if (ends("icate")) {
                        r("ic");
                        break;
                    }
                    if (ends("ative")) {
                        r("");
                        break;
                    }
                    if (ends("alize")) {
                        r("al");
                    }
                    break;
                case 'i':
                    if (ends("iciti")) {
                        r("ic");
                    }
                    break;
                case 'l':
                    if (ends("ical")) {
                        r("ic");
                        break;
                    }
                    if (ends("ful")) {
                        r("");
                    }
                    break;
                case 's':
                    if (ends("ness")) {
                        r("");
                    }
                    break;
            }
        }

        /// <summary>
        /// Takes off -ant, -ence etc., in context vcvc.
        /// </summary>
        private void Step5() {
            if (k == k0) return;
            switch (ResultBuffer[k - 1]) {
                case 'a':
                    if (ends("al")) break;
                    return;
                case 'c':
                    if (ends("ance")) break;
                    if (ends("ence")) break;
                    return;
                case 'e':
                    if (ends("er")) break;
                    return;
                case 'i':
                    if (ends("ic")) break;
                    return;
                case 'l':
                    if (ends("able")) break;
                    if (ends("ible")) break;
                    return;
                case 'n':
                    if (ends("ant")) break;
                    if (ends("ement")) break;
                    if (ends("ment")) break;
                    /* element etc. not stripped before the m */
                    if (ends("ent")) break;
                    return;
                case 'o':
                    if (ends("ion") && j >= 0 && (ResultBuffer[j] == 's' || ResultBuffer[j] == 't')) break;
                    if (ends("ou")) break;
                    return;
                    /* takes care of -ous */
                case 's':
                    if (ends("ism")) break;
                    return;
                case 't':
                    if (ends("ate")) break;
                    if (ends("iti")) break;
                    return;
                case 'u':
                    if (ends("ous")) break;
                    return;
                case 'v':
                    if (ends("ive")) break;
                    return;
                case 'z':
                    if (ends("ize")) break;
                    return;
                default:
                    return;
            }
            if (m() > 1)
                k = j;
        }

        /// <summary>
        /// Removes a readonly -e if m() > 1.
        /// </summary>
        private void Step6() {
            j = k;
            if (ResultBuffer[k] == 'e') {
                var a = m();
                if (a > 1 || a == 1 && !cvc(k - 1))
                    k--;
            }
            if (ResultBuffer[k] == 'l' && doubleConstant(k) && m() > 1)
                k--;
        }

    }
}