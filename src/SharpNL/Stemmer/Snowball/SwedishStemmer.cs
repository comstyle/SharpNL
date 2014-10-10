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
/*

Copyright (c) 2001, Dr Martin Porter
Copyright (c) 2002, Richard Boulton
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
    * this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
    * notice, this list of conditions and the following disclaimer in the
    * documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright holders nor the names of its contributors
    * may be used to endorse or promote products derived from this software
    * without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

// This file was generated automatically by the Snowball to OpenNLP and
// ported to SharpNL

namespace SharpNL.Stemmer.Snowball {
    public class SwedishStemmer : AbstractStemmer {

        private static SwedishStemmer instance;

        /// <summary>
        /// Gets the <see cref="SwedishStemmer"/> instance.
        /// </summary>
        /// <value>The <see cref="SwedishStemmer"/> instance.</value>
        public static SwedishStemmer Instance {
            get { return instance ?? (instance = new SwedishStemmer()); }
        }

        private SwedishStemmer() { }

        private static readonly Among[] a_0 = {
            new Among("a", -1, 1, null),
            new Among("arna", 0, 1, null),
            new Among("erna", 0, 1, null),
            new Among("heterna", 2, 1, null),
            new Among("orna", 0, 1, null),
            new Among("ad", -1, 1, null),
            new Among("e", -1, 1, null),
            new Among("ade", 6, 1, null),
            new Among("ande", 6, 1, null),
            new Among("arne", 6, 1, null),
            new Among("are", 6, 1, null),
            new Among("aste", 6, 1, null),
            new Among("en", -1, 1, null),
            new Among("anden", 12, 1, null),
            new Among("aren", 12, 1, null),
            new Among("heten", 12, 1, null),
            new Among("ern", -1, 1, null),
            new Among("ar", -1, 1, null),
            new Among("er", -1, 1, null),
            new Among("heter", 18, 1, null),
            new Among("or", -1, 1, null),
            new Among("s", -1, 2, null),
            new Among("as", 21, 1, null),
            new Among("arnas", 22, 1, null),
            new Among("ernas", 22, 1, null),
            new Among("ornas", 22, 1, null),
            new Among("es", 21, 1, null),
            new Among("ades", 26, 1, null),
            new Among("andes", 26, 1, null),
            new Among("ens", 21, 1, null),
            new Among("arens", 29, 1, null),
            new Among("hetens", 29, 1, null),
            new Among("erns", 21, 1, null),
            new Among("at", -1, 1, null),
            new Among("andet", -1, 1, null),
            new Among("het", -1, 1, null),
            new Among("ast", -1, 1, null)
        };

        private static readonly Among[] a_1 = {
            new Among("dd", -1, -1, null),
            new Among("gd", -1, -1, null),
            new Among("nn", -1, -1, null),
            new Among("dt", -1, -1, null),
            new Among("gt", -1, -1, null),
            new Among("kt", -1, -1, null),
            new Among("tt", -1, -1, null)
        };

        private static readonly Among[] a_2 = {
            new Among("ig", -1, 1, null),
            new Among("lig", 0, 1, null),
            new Among("els", -1, 1, null),
            new Among("fullt", -1, 3, null),
            new Among("l\u00F6st", -1, 2, null)
        };

        private static readonly char[] g_v = {
            (char) 17, (char) 65, (char) 16, (char) 1, (char) 0, (char) 0, (char) 0,
            (char) 0, (char) 0, (char) 0, (char) 0, (char) 0, (char) 0, (char) 0, (char) 0, (char) 0, (char) 24,
            (char) 0, (char) 32
        };

        private static readonly char[] g_s_ending = {(char) 119, (char) 127, (char) 149};

        private int I_x;
        private int I_p1;

        private bool r_mark_regions() {
            int v_1;
            int v_2;
            // (, line 26
            I_p1 = limit;
            // test, line 29
            v_1 = cursor;
            // (, line 29
            // hop, line 29
            {
                int c = cursor + 3;
                if (0 > c || c > limit) {
                    return false;
                }
                cursor = c;
            }
            // setmark x, line 29
            I_x = cursor;
            cursor = v_1;
            // goto, line 30
            golab0:
            while (true) {
                v_2 = cursor;
                lab1:
                do {
                    if (!(in_grouping(g_v, 97, 246))) {
                        break;
                    }
                    cursor = v_2;
                    break;
                } while (false);
                cursor = v_2;
                if (cursor >= limit) {
                    return false;
                }
                cursor++;
            }
            // gopast, line 30
            golab2:
            while (true) {
                lab3:
                do {
                    if (!(out_grouping(g_v, 97, 246))) {
                        break;
                    }
                    break;
                } while (false);
                if (cursor >= limit) {
                    return false;
                }
                cursor++;
            }
            // setmark p1, line 30
            I_p1 = cursor;
            // try, line 31
            lab4:
            do {
                // (, line 31
                if (!(I_p1 < I_x)) {
                    break;
                }
                I_p1 = I_x;
            } while (false);
            return true;
        }

        private bool r_main_suffix() {
            int among_var;
            int v_1;
            int v_2;
            // (, line 36
            // setlimit, line 37
            v_1 = limit - cursor;
            // tomark, line 37
            if (cursor < I_p1) {
                return false;
            }
            cursor = I_p1;
            v_2 = limit_backward;
            limit_backward = cursor;
            cursor = limit - v_1;
            // (, line 37
            // [, line 37
            ket = cursor;
            // substring, line 37
            among_var = find_among_b(a_0, 37);
            if (among_var == 0) {
                limit_backward = v_2;
                return false;
            }
            // ], line 37
            bra = cursor;
            limit_backward = v_2;
            switch (among_var) {
                case 0:
                    return false;
                case 1:
                    // (, line 44
                    // delete, line 44
                    slice_del();
                    break;
                case 2:
                    // (, line 46
                    if (!(in_grouping_b(g_s_ending, 98, 121))) {
                        return false;
                    }
                    // delete, line 46
                    slice_del();
                    break;
            }
            return true;
        }

        private bool r_consonant_pair() {
            int v_1;
            int v_2;
            int v_3;
            // setlimit, line 50
            v_1 = limit - cursor;
            // tomark, line 50
            if (cursor < I_p1) {
                return false;
            }
            cursor = I_p1;
            v_2 = limit_backward;
            limit_backward = cursor;
            cursor = limit - v_1;
            // (, line 50
            // and, line 52
            v_3 = limit - cursor;
            // among, line 51
            if (find_among_b(a_1, 7) == 0) {
                limit_backward = v_2;
                return false;
            }
            cursor = limit - v_3;
            // (, line 52
            // [, line 52
            ket = cursor;
            // next, line 52
            if (cursor <= limit_backward) {
                limit_backward = v_2;
                return false;
            }
            cursor--;
            // ], line 52
            bra = cursor;
            // delete, line 52
            slice_del();
            limit_backward = v_2;
            return true;
        }

        private bool r_other_suffix() {
            int among_var;
            int v_1;
            int v_2;
            // setlimit, line 55
            v_1 = limit - cursor;
            // tomark, line 55
            if (cursor < I_p1) {
                return false;
            }
            cursor = I_p1;
            v_2 = limit_backward;
            limit_backward = cursor;
            cursor = limit - v_1;
            // (, line 55
            // [, line 56
            ket = cursor;
            // substring, line 56
            among_var = find_among_b(a_2, 5);
            if (among_var == 0) {
                limit_backward = v_2;
                return false;
            }
            // ], line 56
            bra = cursor;
            switch (among_var) {
                case 0:
                    limit_backward = v_2;
                    return false;
                case 1:
                    // (, line 57
                    // delete, line 57
                    slice_del();
                    break;
                case 2:
                    // (, line 58
                    // <-, line 58
                    slice_from("l\u00F6s");
                    break;
                case 3:
                    // (, line 59
                    // <-, line 59
                    slice_from("full");
                    break;
            }
            limit_backward = v_2;
            return true;
        }

        /// <summary>
        /// Reduces the given word into its stem.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stemmed word.</returns>
        public override string Stem(string word) {
            Current = word.ToLowerInvariant();

            int v_1;
            int v_2;
            int v_3;
            int v_4;
            // (, line 64
            // do, line 66
            v_1 = cursor;
            lab0:
            do {
                // call mark_regions, line 66
                if (!r_mark_regions()) {
                    break;
                }
            } while (false);
            cursor = v_1;
            // backwards, line 67
            limit_backward = cursor;
            cursor = limit;
            // (, line 67
            // do, line 68
            v_2 = limit - cursor;
            lab1:
            do {
                // call main_suffix, line 68
                if (!r_main_suffix()) {
                    break;
                }
            } while (false);
            cursor = limit - v_2;
            // do, line 69
            v_3 = limit - cursor;
            lab2:
            do {
                // call consonant_pair, line 69
                if (!r_consonant_pair()) {
                    break;
                }
            } while (false);
            cursor = limit - v_3;
            // do, line 70
            v_4 = limit - cursor;
            lab3:
            do {
                // call other_suffix, line 70
                if (!r_other_suffix()) {
                    break;
                }
            } while (false);
            cursor = limit - v_4;
            cursor = limit_backward;

            return Current;
        }
    }
}