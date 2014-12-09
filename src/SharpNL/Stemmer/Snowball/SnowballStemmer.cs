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
 *  Port of Snowball stemmers on C#
 *  Original stemmers can be found on http://snowball.tartarus.org
 *  Licence still BSD: http://snowball.tartarus.org/license.php
 *  
 *  Most of stemmers are ported from Java by Iveonik Systems ltd. (www.iveonik.com)
 *  
 *  German stemmer's port found on SourceForge site
 */

using System.Text;

namespace SharpNL.Stemmer.Snowball {
    /// <summary>
    /// Represents a abstract stemmer.
    /// </summary>
    public abstract class SnowballStemmer : IStemmer {
        protected int bra;
        protected int cursor;
        protected int ket;
        protected int limit;
        protected int limit_backward;
        protected StringBuilder sb;


        protected SnowballStemmer() {
            sb = new StringBuilder();
            Current = string.Empty;
        }

        #region . Stem .
        /// <summary>
        /// Reduces the given word into its stem.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>The stemmed word.</returns>
        public abstract string Stem(string word);
        #endregion

        #region + Properties .

        #region . Current .

        /// <summary>
        /// Gets or sets the current string in the <see cref="sb"/> object.
        /// </summary>
        /// <value>The current.</value>
        protected string Current {
            get { return sb.ToString(); }
            set {
                sb.Remove(0, sb.Length);
                sb.Append(value);
                cursor = 0;
                limit = sb.Length;
                limit_backward = 0;
                bra = cursor;
                ket = limit;
            }
        }

        #endregion

        #endregion

        protected void copy_from(SnowballStemmer other) {
            sb = other.sb;
            cursor = other.cursor;
            limit = other.limit;
            limit_backward = other.limit_backward;
            bra = other.bra;
            ket = other.ket;
        }

        protected bool in_grouping(char[] s, int min, int max) {
            if (cursor >= limit) return false;
            //           char ch = current.charAt(cursor);
            int ch = sb[cursor];
            if (ch > max || ch < min) return false;
            //           ch -= min;
            ch -= min;
            if ((s[ch >> 3] & (0X1 << (ch & 0X7))) == 0) return false;
            cursor++;
            return true;
        }

        protected bool in_grouping_b(char[] s, int min, int max) {
            if (cursor <= limit_backward) return false;
            //           char ch = current.charAt(cursor - 1);
            int ch = sb[cursor - 1];
            if (ch > max || ch < min) return false;
            ch -= min;
            if ((s[ch >> 3] & (0X1 << (ch & 0X7))) == 0) return false;
            cursor--;
            return true;
        }

        protected bool out_grouping(char[] s, int min, int max) {
            if (cursor >= limit) return false;
            //           char ch = current.charAt(cursor);
            int ch = sb[cursor];
            if (ch > max || ch < min) {
                cursor++;
                return true;
            }
            ch -= min;
            if ((s[ch >> 3] & (0X1 << (ch & 0X7))) == 0) {
                cursor++;
                return true;
            }
            return false;
        }

        protected bool out_grouping_b(char[] s, int min, int max) {
            if (cursor <= limit_backward) return false;
            //           char ch = current.charAt(cursor - 1);
            int ch = sb[cursor - 1];
            if (ch > max || ch < min) {
                cursor--;
                return true;
            }
            ch -= min;
            if ((s[ch >> 3] & (0X1 << (ch & 0X7))) == 0) {
                cursor--;
                return true;
            }
            return false;
        }

        protected bool in_range(int min, int max) {
            if (cursor >= limit) return false;
            //           char ch = current.charAt(cursor);
            int ch = sb[cursor];
            if (ch > max || ch < min) return false;
            cursor++;
            return true;
        }

        protected bool in_range_b(int min, int max) {
            if (cursor <= limit_backward) return false;
            //           char ch = current.charAt(cursor - 1);
            int ch = sb[cursor - 1];
            if (ch > max || ch < min) return false;
            cursor--;
            return true;
        }

        protected bool out_range(int min, int max) {
            if (cursor >= limit) return false;
            //           char ch = current.charAt(cursor);
            int ch = sb[cursor];
            if (!(ch > max || ch < min)) return false;
            cursor++;
            return true;
        }

        protected bool out_range_b(int min, int max) {
            if (cursor <= limit_backward) return false;
            //           char ch = current.charAt(cursor - 1);
            int ch = sb[cursor - 1];
            if (!(ch > max || ch < min)) return false;
            cursor--;
            return true;
        }

        protected bool eq_s(int s_size, string s) {
            if (limit - cursor < s_size) return false;
            int i;
            for (i = 0; i != s_size; i++) {
                if (sb[cursor + i] != s[i]) return false;
                //               if (current[cursor + i] != s[i]) return false;
            }
            cursor += s_size;
            return true;
        }

        protected bool eq_s_b(int s_size, string s) {
            if (cursor - limit_backward < s_size) return false;
            int i;
            for (i = 0; i != s_size; i++) {
                //               if (current.charAt(cursor - s_size + i) != s.charAt(i)) return false;
                if (sb[cursor - s_size + i] != s[i]) return false;
            }
            cursor -= s_size;
            return true;
        }

        protected bool eq_v(StringBuilder s) {
            return eq_s(s.Length, s.ToString());
        }

        protected bool eq_v_b(StringBuilder s) {
            return eq_s_b(s.Length, s.ToString());
        }


        internal int find_among(Among[] v, int v_size) {
            var i = 0;
            var j = v_size;

            var c = cursor;
            var l = limit;

            var common_i = 0;
            var common_j = 0;

            var first_key_inspected = false;
            while (true) {
                var k = i + ((j - i) >> 1);
                var diff = 0;
                var common = common_i < common_j ? common_i : common_j; // smaller
                var w = v[k];
                int i2;

                for (i2 = common; i2 < w.s_size; i2++) {
                    if (c + common == l) {
                        diff = -1;
                        break;
                    }
                    diff = sb[c + common] - w.s[i2];
                    if (diff != 0) break;
                    common++;
                }
                if (diff < 0) {
                    j = k;
                    common_j = common;
                } else {
                    i = k;
                    common_i = common;
                }
                if (j - i <= 1) {
                    if (i > 0) break; // v->s has been inspected
                    if (j == i) break; // only one item in v
                    // - but now we need to go round once more to get
                    // v->s inspected. This looks messy, but is actually
                    // the optimal approach.
                    if (first_key_inspected) break;
                    first_key_inspected = true;
                }
            }
            while (true) {
                var w = v[i];
                if (common_i >= w.s_size) {
                    cursor = c + w.s_size;
                    if (w.method == null) return w.result;
                    //bool res;
                    //try
                    //{
                    //    Object resobj = w.method.invoke(w.methodobject,new Object[0]);
                    //    res = resobj.toString().equals("true");
                    //}
                    //catch (InvocationTargetException e)
                    //{
                    //    res = false;
                    //    // FIXME - debug message
                    //}
                    //catch (IllegalAccessException e)
                    //{
                    //    res = false;
                    //// FIXME - debug message
                    //}
                    //cursor = c + w.s_size;
                    //if (res) return w.result;
                }
                i = w.substring_i;
                if (i < 0) return 0;
            }
        }

        //    // find_among_b is for backwards processing. Same comments apply

        internal int find_among_b(Among[] v, int v_size) {
            var i = 0;
            var j = v_size;
            var c = cursor;
            var lb = limit_backward;
            var common_i = 0;
            var common_j = 0;
            var first_key_inspected = false;
            while (true) {
                var k = i + ((j - i) >> 1);
                var diff = 0;
                var common = common_i < common_j ? common_i : common_j;
                var w = v[k];
                int i2;
                for (i2 = w.s_size - 1 - common; i2 >= 0; i2--) {
                    if (c - common == lb) {
                        diff = -1;
                        break;
                    }
                    //                   diff = current.charAt(c - 1 - common) - w.s[i2];
                    diff = sb[c - 1 - common] - w.s[i2];
                    if (diff != 0) break;
                    common++;
                }
                if (diff < 0) {
                    j = k;
                    common_j = common;
                } else {
                    i = k;
                    common_i = common;
                }
                if (j - i <= 1) {
                    if (i > 0) break;
                    if (j == i) break;
                    if (first_key_inspected) break;
                    first_key_inspected = true;
                }
            }
            while (true) {
                var w = v[i];
                if (common_i >= w.s_size) {
                    cursor = c - w.s_size;
                    if (w.method == null) return w.result;
                    //boolean res;
                    //try 
                    //{
                    //    Object resobj = w.method.invoke(w.methodobject,
                    //        new Object[0]);
                    //    res = resobj.toString().equals("true");
                    // } 
                    //catch (InvocationTargetException e) 
                    //{
                    //    res = false;
                    //    // FIXME - debug message
                    // } 
                    //catch (IllegalAccessException e) 
                    //{
                    //    res = false;
                    //    // FIXME - debug message
                    // }
                    //cursor = c - w.s_size;
                    //if (res) return w.result;
                }
                i = w.substring_i;
                if (i < 0) return 0;
            }
        }

        //    /* to replace chars between c_bra and c_ket in current by the
        //     * chars in s.
        //     */
        protected int replace_s(int c_bra, int c_ket, string s) {
            var adjustment = s.Length - (c_ket - c_bra);
            //           current.replace(c_bra, c_ket, s);
            sb = StringBufferReplace(c_bra, c_ket, sb, s);
            limit += adjustment;
            if (cursor >= c_ket) cursor += adjustment;
            else if (cursor > c_bra) cursor = c_bra;
            return adjustment;
        }

        protected static StringBuilder StringBufferReplace(int start, int end, StringBuilder s, string s1) {
            var bufferReplace = new StringBuilder();
            for (var i = 0; i < start; i++) {
                bufferReplace.Insert(bufferReplace.Length, s[i]);
            }
            //           for (int i = 1; i < end - start + 1; i++)
            //           {
            bufferReplace.Insert(bufferReplace.Length, s1);
            //           }
            for (var i = end; i < s.Length; i++) {
                bufferReplace.Insert(bufferReplace.Length, s[i]);
            }

            return bufferReplace;
            //string temp = s.ToString();
            //temp = temp.Substring(start - 1, end - start + 1);
            //s = s.Replace(temp, s1, start - 1, end - start + 1);
            //return s;
        }

        protected void slice_check() {
            if (bra < 0 ||
                bra > ket ||
                ket > limit ||
                limit > sb.Length) // this line could be removed
            {
                //System.err.println("faulty slice operation");
                // FIXME: report error somehow.
                /*
                    fprintf(stderr, "faulty slice operation:\n");
                    debug(z, -1, 0);
                    exit(1);
                    */
            }
        }

        protected void slice_from(string s) {
            slice_check();
            replace_s(bra, ket, s);
        }

        protected void slice_from(StringBuilder s) {
            slice_from(s.ToString());
        }

        protected void slice_del() {
            slice_from("");
        }

        protected void insert(int c_bra, int c_ket, string s) {
            var adjustment = replace_s(c_bra, c_ket, s);
            if (c_bra <= bra) bra += adjustment;
            if (c_bra <= ket) ket += adjustment;
        }

        protected void insert(int c_bra, int c_ket, StringBuilder s) {
            insert(c_bra, c_ket, s.ToString());
        }

        //    /* Copy the slice into the supplied StringBuffer */
        protected StringBuilder slice_to(StringBuilder s) {
            slice_check();
            var len = ket - bra;
            //           s.replace(0, s.length(), current.substring(bra, ket));
            //           int lengh = string.IsNullOrEmpty(s.ToString())!= true ? s.Length : 0;
            //           if (ket == current.Length) ket--;
            //string ss = current.ToString().Substring(bra, len);
            //StringBufferReplace(0, s.Length, s, ss);
            //return s;
            return StringBufferReplace(0, s.Length, s, sb.ToString().Substring(bra, len));
            //           return StringBufferReplace(0, lengh, s, current.ToString().Substring(bra, ket));
            //           return s;
        }

        protected StringBuilder assign_to(StringBuilder s) {
            //s.replace(0, s.length(), current.substring(0, limit));
            //return s;
            return StringBufferReplace(0, s.Length, s, sb.ToString().Substring(0, limit));
        }       
    }
}