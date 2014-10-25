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

using System.Text;
using System.Collections.Generic;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Utility class to handle Portuguese contractions.
    /// </summary>
    /// <remarks>
    /// Some Corpora splits contractions in its parts, for example, "da" &gt; "de" + "a",
    /// but according to the phase of language processing, NER for instance, we can't decide
    /// if to split a contraction or not, specially because contractions
    /// inside names are not separated, but outside are.
    /// </remarks>
    public class PortugueseContractionUtility {
        private static readonly Dictionary<string, string> Contractions;

        static PortugueseContractionUtility() {
            Contractions = new Dictionary<string, string> {
                {"a+a", "\u00e0"},
                {"a+as", "\u00e0s"},
                {"a+aquele", "\u00e0quele"},
                {"a+aqueles", "\u00e0queles"},
                {"a+aquela", "\u00e0quela"},
                {"a+aquelas", "\u00e0quelas"},
                {"a+aquilo", "\u00e0quilo"},
                {"a+o", "ao"},
                {"a+os", "aos"},
                {"com+mim", "comigo"},
                {"com+n\u00f2s", "conosco"},
                {"com+si", "consigo"},
                {"com+ti", "contigo"},
                {"com+v\u00f2s", "convosco"},
                {"de+a\u00ed", "da\u00ed"},
                {"de+algu\u00e9m", "dalgu\u00e9m"},
                {"de+algum", "dalgum"},
                {"de+alguma", "dalguma"},
                {"de+alguns", "dalguns"},
                {"de+algumas", "dalgumas"},
                {"de+ali", "dali"},
                {"de+aqu\u00e9m", "daqu\u00e9m"},
                {"de+aquele", "daquele"},
                {"de+aquela", "daquela"},
                {"de+aqueles", "daqueles"},
                {"de+aquelas", "daquelas"},
                {"de+aqui", "daqui"},
                {"de+aquilo", "daquilo"},
                {"de+ele", "dele"},
                {"de+ela", "dela"},
                {"de+eles", "deles"},
                {"de+elas", "delas"},
                {"de+entre", "dentre"},
                {"de+esse", "desse"},
                {"de+essa", "dessa"},
                {"de+esses", "desses"},
                {"de+essas", "dessas"},
                {"de+este", "deste"},
                {"de+esta", "desta"},
                {"de+estes", "destes"},
                {"de+estas", "destas"},
                {"de+isso", "disso"},
                {"de+isto", "disto"},
                {"de+o", "do"},
                {"de+a", "da"},
                {"de+os", "dos"},
                {"de+as", "das"},
                {"de+outrem", "doutrem"},
                {"de+outro", "doutro"},
                {"de+outra", "doutra"},
                {"de+outros", "doutros"},
                {"de+outras", "doutras"},
                {"de+um", "dum"},
                {"de+uma", "duma"},
                {"de+uns", "duns"},
                {"de+umas", "dumas"},
                {"esse+outro", "essoutro"},
                {"essa+outra", "essoutra"},
                {"este+outro", "estoutro"},
                {"este+outra", "estoutra"},
                {"ele+o", "lho"},
                {"ele+a", "lha"},
                {"ele+os", "lhos"},
                {"ele+as", "lhas"},
                {"em+algum", "nalgum"},
                {"em+alguma", "nalguma"},
                {"em+alguns", "nalguns"},
                {"em+algumas", "nalgumas"},
                {"em+aquele", "naquele"},
                {"em+aquela", "naquela"},
                {"em+aqueles", "naqueles"},
                {"em+aquelas", "naquelas"},
                {"em+aquilo", "naquilo"},
                {"em+ele", "nele"},
                {"em+ela", "nela"},
                {"em+eles", "neles"},
                {"em+elas", "nelas"},
                {"em+esse", "nesse"},
                {"em+essa", "nessa"},
                {"em+esses", "nesses"},
                {"em+essas", "nessas"},
                {"em+este", "neste"},
                {"em+esta", "nesta"},
                {"em+estes", "nestes"},
                {"em+estas", "nestas"},
                {"em+isso", "nisso"},
                {"em+isto", "nisto"},
                {"em+o", "no"},
                {"em+a", "na"},
                {"em+os", "nos"},
                {"em+as", "nas"},
                {"em+outro", "noutro"},
                {"em+outra", "noutra"},
                {"em+outros", "noutros"},
                {"em+outras", "noutras"},
                {"em+um", "num"},
                {"em+uma", "numa"},
                {"em+uns", "nuns"},
                {"em+umas", "numas"},
                {"por+o", "pelo"},
                {"por+a", "pela"},
                {"por+os", "pelos"},
                {"por+as", "pelas"},
                {"para+a", "pra"},
                {"para+o", "pro"},
                {"para+as", "pras"},
                {"para+os", "pros"}
            };
        }

        /// <summary>
        /// Merges a contraction.
        /// </summary>
        /// <param name="left">The left component.</param>
        /// <param name="right">The right component.</param>
        /// <returns>The merged contraction.</returns>
        public static string ToContraction(string left, string right) {
            var key = left + "+" + right;
            if (Contractions.ContainsKey(key)) {
                return Contractions[key];
            }
            var sb = new StringBuilder();
            var parts = left.Split('_');
            for (var i = 0; i < parts.Length - 1; i++) {
                sb.Append(parts[i]).Append(" ");
            }
            key = parts[parts.Length - 1] + "+" + right;
            if (Contractions.ContainsKey(key)) {
                sb.Append(Contractions[key]);
                return sb.ToString();
            }

            if (right.Contains("_")) {
                parts = right.Split('_');

                key = left + "+" + parts[0];
                if (Contractions.ContainsKey(key)) {
                    sb.Append(Contractions[key]).Append(" ");

                    for (var i = 1; i < parts.Length; i++) {
                        sb.Append(parts[i]).Append(" ");
                    }

                    return sb.ToString();
                }
            }

            var leftLower = parts[parts.Length - 1].ToLowerInvariant();
            key = leftLower + "+" + right;

            if (!Contractions.ContainsKey(key)) 
                return null;

            var r = Contractions[key];
            r = char.ToUpper(r[0]) + r.Substring(1);
            sb.Append(r);
            return sb.ToString();
        }

        /// <summary>
        /// Expands the specified contraction into its components.
        /// </summary>
        /// <param name="contraction">The contraction.</param>
        /// <returns>The expanded contraction or <c>null</c> if the contraction cannot be found.</returns>
        public static string[] Expand(string contraction) {
            contraction = contraction.ToLowerInvariant();
            return Contractions.ContainsValue(contraction) 
                ? Contractions.GetKey(contraction).Split('+') 
                : null;
        }

    }
}