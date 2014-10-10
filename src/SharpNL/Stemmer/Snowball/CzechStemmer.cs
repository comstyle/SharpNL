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

namespace SharpNL.Stemmer.Snowball {
    public class CzechStemmer : AbstractStemmer {

        private static CzechStemmer instance;

        /// <summary>
        /// Gets the <see cref="CzechStemmer"/> instance.
        /// </summary>
        /// <value>The <see cref="CzechStemmer"/> instance.</value>
        public static CzechStemmer Instance {
            get { return instance ?? (instance = new CzechStemmer()); }
        }

        private CzechStemmer() { }

        public override string Stem(string word) {
            Current = word.ToLowerInvariant();

            // stemming...

            //removes case endings from nouns and adjectives
            removeCase();
            //removes possessive endings from names -ov- and -in-
            removePossessives();
            //removes comparative endings
            removeComparative();
            //removes diminutive endings
            removeDiminutive();
            //removes augmentatives endings
            removeAugmentative();
            //removes derivational suffixes from nouns
            removeDerivational();

            return Current;
        }

        protected void removeDerivational() {
            var len = sb.Length;
            if ((len > 8) &&
                sb.ToString().Substring(len - 6, 6).Equals("obinec")) {
                sb = sb.Remove(len - 6, 6);
                return;
            } //len >8
            if (len > 7) {
                if (sb.ToString().Substring(len - 5, 5).Equals("ion\u00e1\u0159")) {
                    // -ionář 

                    sb = sb.Remove(len - 4, 4);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 5, 5).Equals("ovisk") ||
                    sb.ToString().Substring(len - 5, 5).Equals("ovstv") ||
                    sb.ToString().Substring(len - 5, 5).Equals("ovi\u0161t") || //-ovišt
                    sb.ToString().Substring(len - 5, 5).Equals("ovn\u00edk")) {
                    //-ovník

                    sb = sb.Remove(len - 5, 5);
                    return;
                }
            } //len>7
            if (len > 6) {
                if (sb.ToString().Substring(len - 4, 4).Equals("\u00e1sek") || // -ásek 
                    sb.ToString().Substring(len - 4, 4).Equals("loun") ||
                    sb.ToString().Substring(len - 4, 4).Equals("nost") ||
                    sb.ToString().Substring(len - 4, 4).Equals("teln") ||
                    sb.ToString().Substring(len - 4, 4).Equals("ovec") ||
                    sb.ToString().Substring(len - 5, 5).Equals("ov\u00edk") || //-ovík
                    sb.ToString().Substring(len - 4, 4).Equals("ovtv") ||
                    sb.ToString().Substring(len - 4, 4).Equals("ovin") ||
                    sb.ToString().Substring(len - 4, 4).Equals("\u0161tin")) {
                    //-štin

                    sb = sb.Remove(len - 4, 4);
                    return;
                }
                if (sb.ToString().Substring(len - 4, 4).Equals("enic") ||
                    sb.ToString().Substring(len - 4, 4).Equals("inec") ||
                    sb.ToString().Substring(len - 4, 4).Equals("itel")) {
                    sb = sb.Remove(len - 3, 3);
                    palatalise();
                    return;
                }
            } //len>6
            if (len > 5) {
                if (sb.ToString().Substring(len - 3, 3).Equals("\u00e1rn")) {
                    //-árn
                    sb = sb.Remove(len - 3, 3);
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("\u011bnk")) {
                    //-ěnk

                    sb = sb.Remove(len - 2, 2);
                    palatalise();

                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("i\u00e1n") || //-ián
                    sb.ToString().Substring(len - 3, 3).Equals("ist") ||
                    sb.ToString().Substring(len - 3, 3).Equals("isk") ||
                    sb.ToString().Substring(len - 3, 3).Equals("i\u0161t") || //-išt
                    sb.ToString().Substring(len - 3, 3).Equals("itb") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00edrn")) {
                    //-írn

                    sb = sb.Remove(len - 2, 2);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("och") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ost") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ovn") ||
                    sb.ToString().Substring(len - 3, 3).Equals("oun") ||
                    sb.ToString().Substring(len - 3, 3).Equals("out") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ou\u0161")) {
                    //-ouš

                    sb = sb.Remove(len - 3, 3);
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("u\u0161k")) {
                    //-ušk

                    sb = sb.Remove(len - 3, 3);
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("kyn") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u010dan") || //-čan
                    sb.ToString().Substring(len - 3, 3).Equals("k\u00e1\u0159") || //kář
                    sb.ToString().Substring(len - 3, 3).Equals("n\u00e9\u0159") || //néř
                    sb.ToString().Substring(len - 3, 3).Equals("n\u00edk") || //-ník
                    sb.ToString().Substring(len - 3, 3).Equals("ctv") ||
                    sb.ToString().Substring(len - 3, 3).Equals("stv")) {
                    sb = sb.Remove(len - 3, 3);
                    return;
                }
            } //len>5
            if (len > 4) {
                if (sb.ToString().Substring(len - 2, 2).Equals("\u00e1\u010d") || // -áč
                    sb.ToString().Substring(len - 2, 2).Equals("a\u010d") || //-ač
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e1n") || //-án
                    sb.ToString().Substring(len - 2, 2).Equals("an") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e1\u0159") || //-ář
                    sb.ToString().Substring(len - 2, 2).Equals("as")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("ec") ||
                    sb.ToString().Substring(len - 2, 2).Equals("en") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u011bn") || //-ěn
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e9\u0159")) {
                    //-éř

                    sb = sb.Remove(len - 1, 1);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("\u00ed\u0159") || //-íř
                    sb.ToString().Substring(len - 2, 2).Equals("ic") ||
                    sb.ToString().Substring(len - 2, 2).Equals("in") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00edn") || //-ín
                    sb.ToString().Substring(len - 2, 2).Equals("it") ||
                    sb.ToString().Substring(len - 2, 2).Equals("iv")) {
                    sb = sb.Remove(len - 1, 1);
                    palatalise();
                    return;
                }

                if (sb.ToString().Substring(len - 2, 2).Equals("ob") ||
                    sb.ToString().Substring(len - 2, 2).Equals("ot") ||
                    sb.ToString().Substring(len - 2, 2).Equals("ov") ||
                    sb.ToString().Substring(len - 2, 2).Equals("o\u0148")) {
                    //-oň 

                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("ul")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("yn")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("\u010dk") || //-čk
                    sb.ToString().Substring(len - 2, 2).Equals("\u010dn") || //-čn
                    sb.ToString().Substring(len - 2, 2).Equals("dl") ||
                    sb.ToString().Substring(len - 2, 2).Equals("nk") ||
                    sb.ToString().Substring(len - 2, 2).Equals("tv") ||
                    sb.ToString().Substring(len - 2, 2).Equals("tk") ||
                    sb.ToString().Substring(len - 2, 2).Equals("vk")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
            } //len>4
            if (len > 3) {
                if (sb.ToString()[sb.Length - 1] == 'c' ||
                    sb.ToString()[sb.Length - 1] == '\u010d' || //-č
                    sb.ToString()[sb.Length - 1] == 'k' ||
                    sb.ToString()[sb.Length - 1] == 'l' ||
                    sb.ToString()[sb.Length - 1] == 'n' ||
                    sb.ToString()[sb.Length - 1] == 't') {
                    sb = sb.Remove(len - 1, 1);
                }
            } //len>3	
        } //removeDerivational

        protected void removeAugmentative() {
            var len = sb.Length;
            //
            if ((len > 6) &&
                sb.ToString().Substring(len - 4, 4).Equals("ajzn")) {
                sb = sb.Remove(len - 4, 4);
                return;
            }
            if ((len > 5) &&
                (sb.ToString().Substring(len - 3, 3).Equals("izn") ||
                 sb.ToString().Substring(len - 3, 3).Equals("isk"))) {
                sb = sb.Remove(len - 2, 2);
                palatalise();
                return;
            }
            if ((len > 4) &&
                sb.ToString().Substring(len - 2, 2).Equals("\00e1k")) {
                //-ák

                sb = sb.Remove(len - 2, 2);
            }
        }

        protected void removeDiminutive() {
            var len = sb.Length;
            // 
            if ((len > 7) &&
                sb.ToString().Substring(len - 5, 5).Equals("ou\u0161ek")) {
                //-oušek

                sb = sb.Remove(len - 5, 5);
                return;
            }
            if (len > 6) {
                if (sb.ToString().Substring(len - 4, 4).Equals("e\u010dek") || //-eček
                    sb.ToString().Substring(len - 4, 4).Equals("\u00e9\u010dek") || //-éček
                    sb.ToString().Substring(len - 4, 4).Equals("i\u010dek") || //-iček
                    sb.ToString().Substring(len - 4, 4).Equals("\u00ed\u010dek") || //íček
                    sb.ToString().Substring(len - 4, 4).Equals("enek") ||
                    sb.ToString().Substring(len - 4, 4).Equals("\u00e9nek") || //-ének
                    sb.ToString().Substring(len - 4, 4).Equals("inek") ||
                    sb.ToString().Substring(len - 4, 4).Equals("\u00ednek")) {
                    //-ínek

                    sb = sb.Remove(len - 3, 3);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 4, 4).Equals("\u00e1\u010dek") || //áček
                    sb.ToString().Substring(len - 4, 4).Equals("a\u010dek") || //aček
                    sb.ToString().Substring(len - 4, 4).Equals("o\u010dek") || //oček
                    sb.ToString().Substring(len - 4, 4).Equals("u\u010dek") || //uček
                    sb.ToString().Substring(len - 4, 4).Equals("anek") ||
                    sb.ToString().Substring(len - 4, 4).Equals("onek") ||
                    sb.ToString().Substring(len - 4, 4).Equals("unek") ||
                    sb.ToString().Substring(len - 4, 4).Equals("\u00e1nek")) {
                    //-ánek

                    sb = sb.Remove(len - 4, 4);
                    return;
                }
            } //len>6
            if (len > 5) {
                if (sb.ToString().Substring(len - 3, 3).Equals("e\u010dk") || //-ečk
                    sb.ToString().Substring(len - 3, 3).Equals("\u00e9\u010dk") || //-éčk 
                    sb.ToString().Substring(len - 3, 3).Equals("i\u010dk") || //-ičk
                    sb.ToString().Substring(len - 3, 3).Equals("\u00ed\u010dk") || //-íčk
                    sb.ToString().Substring(len - 3, 3).Equals("enk") || //-enk
                    sb.ToString().Substring(len - 3, 3).Equals("\u00e9nk") || //-énk 
                    sb.ToString().Substring(len - 3, 3).Equals("ink") || //-ink
                    sb.ToString().Substring(len - 3, 3).Equals("\u00ednk")) {
                    //-ínk

                    sb = sb.Remove(len - 3, 3);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("\u00e1\u010dk") || //-áčk
                    sb.ToString().Substring(len - 3, 3).Equals("au010dk") || //-ačk
                    sb.ToString().Substring(len - 3, 3).Equals("o\u010dk") || //-očk
                    sb.ToString().Substring(len - 3, 3).Equals("u\u010dk") || //-učk 
                    sb.ToString().Substring(len - 3, 3).Equals("ank") ||
                    sb.ToString().Substring(len - 3, 3).Equals("onk") ||
                    sb.ToString().Substring(len - 3, 3).Equals("unk")) {
                    sb = sb.Remove(len - 3, 3);
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("\u00e1tk") || //-átk
                    sb.ToString().Substring(len - 3, 3).Equals("\u00e1nk") || //-ánk
                    sb.ToString().Substring(len - 3, 3).Equals("u\u0161k")) {
                    //-ušk

                    sb = sb.Remove(len - 3, 3);
                    return;
                }
            } //len>5
            if (len > 4) {
                if (sb.ToString().Substring(len - 2, 2).Equals("ek") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e9k") || //-ék
                    sb.ToString().Substring(len - 2, 2).Equals("\u00edk") || //-ík
                    sb.ToString().Substring(len - 2, 2).Equals("ik")) {
                    sb = sb.Remove(len - 1, 1);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("\u00e1k") || //-ák
                    sb.ToString().Substring(len - 2, 2).Equals("ak") ||
                    sb.ToString().Substring(len - 2, 2).Equals("ok") ||
                    sb.ToString().Substring(len - 2, 2).Equals("uk")) {
                    sb = sb.Remove(len - 1, 1);
                    return;
                }
            }
            if ((len > 3) &&
                sb.ToString().Substring(len - 1, 1).Equals("k")) {
                sb = sb.Remove(len - 1, 1);
            }
        } //removeDiminutives

        protected void removeComparative() {
            var len = sb.Length;
            // 
            if ((len > 5) &&
                (sb.ToString().Substring(len - 3, 3).Equals("ej\u0161") || //-ejš
                 sb.ToString().Substring(len - 3, 3).Equals("\u011bj\u0161"))) {
                //-ějš

                sb = sb.Remove(len - 2, 2);
                palatalise();
            }
        }

        private void palatalise() {
            var len = sb.Length;

            if (sb.ToString().Substring(len - 2, 2).Equals("ci") ||
                sb.ToString().Substring(len - 2, 2).Equals("ce") ||
                sb.ToString().Substring(len - 2, 2).Equals("\u010di") || //-či
                sb.ToString().Substring(len - 2, 2).Equals("\u010de")) {
                //-če

                sb = StringBufferReplace(len - 2, len, sb, "k");
                return;
            }
            if (sb.ToString().Substring(len - 2, 2).Equals("zi") ||
                sb.ToString().Substring(len - 2, 2).Equals("ze") ||
                sb.ToString().Substring(len - 2, 2).Equals("\u017ei") || //-ži
                sb.ToString().Substring(len - 2, 2).Equals("\u017ee")) {
                //-že

                sb = StringBufferReplace(len - 2, len, sb, "h");
                return;
            }
            if (sb.ToString().Substring(len - 3, 3).Equals("\u010dt\u011b") || //-čtě
                sb.ToString().Substring(len - 3, 3).Equals("\u010dti") || //-čti
                sb.ToString().Substring(len - 3, 3).Equals("\u010dt\u00ed")) {
                //-čtí

                sb = StringBufferReplace(len - 3, len, sb, "ck");
                return;
            }
            if (sb.ToString().Substring(len - 2, 2).Equals("\u0161t\u011b") || //-ště
                sb.ToString().Substring(len - 2, 2).Equals("\u0161ti") || //-šti
                sb.ToString().Substring(len - 2, 2).Equals("\u0161t\u00ed")) {
                //-ští

                sb = StringBufferReplace(len - 2, len, sb, "sk");
                return;
            }
            sb = sb.Remove(len - 1, 1);
        } //palatalise

        protected void removePossessives() {
            var len = sb.Length;

            if (len > 5) {
                if (sb.ToString().Substring(len - 2, 2).Equals("ov")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("\u016fv")) {
                    //-ův
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("in")) {
                    sb = sb.Remove(len - 1, 1);
                    palatalise();
                }
            }
        } //removePossessives

        protected void removeCase() {
            var len = sb.Length;
            // 
            if ((len > 7) &&
                sb.ToString().Substring(len - 5, 5).Equals("atech")) {
                sb = sb.Remove(len - 5, 5);
                return;
            } //len>7
            if (len > 6) {
                if (sb.ToString().Substring(len - 4, 4).Equals("\u011btem")) {
                    //-ětem

                    sb = sb.Remove(len - 3, 3);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 4, 4).Equals("at\u016fm")) {
                    //-atům
                    sb = sb.Remove(len - 4, 4);
                    return;
                }
            }
            if (len > 5) {
                if (sb.ToString().Substring(len - 3, 3).Equals("ech") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ich") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00edch")) {
                    //-ích

                    sb = sb.Remove(len - 2, 2);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("\u00e9ho") || //-ého
                    sb.ToString().Substring(len - 3, 3).Equals("\u011bmi") || //-ěmu
                    sb.ToString().Substring(len - 3, 3).Equals("emi") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00e9mu") ||
                    // -ému				                                                                current.substring( len-3,len).equals("ete")||
                    sb.ToString().Substring(len - 3, 3).Equals("eti") ||
                    sb.ToString().Substring(len - 3, 3).Equals("iho") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00edho") || //-ího
                    sb.ToString().Substring(len - 3, 3).Equals("\u00edmi") || //-ími
                    sb.ToString().Substring(len - 3, 3).Equals("imu")) {
                    sb = sb.Remove(len - 2, 2);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 3, 3).Equals("\u00e1ch") || //-ách
                    sb.ToString().Substring(len - 3, 3).Equals("ata") ||
                    sb.ToString().Substring(len - 3, 3).Equals("aty") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00fdch") || //-ých
                    sb.ToString().Substring(len - 3, 3).Equals("ama") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ami") ||
                    sb.ToString().Substring(len - 3, 3).Equals("ov\u00e9") || //-ové
                    sb.ToString().Substring(len - 3, 3).Equals("ovi") ||
                    sb.ToString().Substring(len - 3, 3).Equals("\u00fdmi")) {
                    //-ými

                    sb = sb.Remove(len - 3, 3);
                    return;
                }
            }
            if (len > 4) {
                if (sb.ToString().Substring(len - 2, 2).Equals("em")) {
                    sb = sb.Remove(len - 1, 1);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("es") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e9m") || //-ém
                    sb.ToString().Substring(len - 2, 2).Equals("\u00edm")) {
                    //-ím

                    sb = sb.Remove(len - 2, 2);
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("\u016fm")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
                if (sb.ToString().Substring(len - 2, 2).Equals("at") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00e1m") || //-ám
                    sb.ToString().Substring(len - 2, 2).Equals("os") ||
                    sb.ToString().Substring(len - 2, 2).Equals("us") ||
                    sb.ToString().Substring(len - 2, 2).Equals("\u00fdm") || //-ým
                    sb.ToString().Substring(len - 2, 2).Equals("mi") ||
                    sb.ToString().Substring(len - 2, 2).Equals("ou")) {
                    sb = sb.Remove(len - 2, 2);
                    return;
                }
            } //len>4
            if (len > 3) {
                if (sb.ToString().Substring(len - 1, 1).Equals("e") ||
                    sb.ToString().Substring(len - 1, 1).Equals("i")) {
                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 1, 1).Equals("\u00ed") || //-é
                    sb.ToString().Substring(len - 1, 1).Equals("\u011b")) {
                    //-ě

                    palatalise();
                    return;
                }
                if (sb.ToString().Substring(len - 1, 1).Equals("u") ||
                    sb.ToString().Substring(len - 1, 1).Equals("y") ||
                    sb.ToString().Substring(len - 1, 1).Equals("\u016f")) {
                    //-ů

                    sb = sb.Remove(len - 1, 1);
                    return;
                }
                if (sb.ToString().Substring(len - 1, 1).Equals("a") ||
                    sb.ToString().Substring(len - 1, 1).Equals("o") ||
                    sb.ToString().Substring(len - 1, 1).Equals("\u00e1") || // -á
                    sb.ToString().Substring(len - 1, 1).Equals("\u00e9") || //-é
                    sb.ToString().Substring(len - 1, 1).Equals("\u00fd")) {
                    //-ý

                    sb = sb.Remove(len - 1, 1);
                }
            } //len>3
        }

    }
}
