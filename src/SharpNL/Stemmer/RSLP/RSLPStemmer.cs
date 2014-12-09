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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SharpNL.Extensions;

namespace SharpNL.Stemmer.RSLP {
    /// <summary>
    /// A Stemming Algorithm for the Portuguese Language. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This stemmer is based on the algorithm presented in the paper "A Stemming # Algorithm for 
    /// the Portuguese Language" by Viviane Moreira Orengo and Christian Huyck.
    /// <para>
    /// The original publication is available at:
    /// <see cref="http://www.inf.ufrgs.br/~viviane/rslp/"/>.
    /// </para>
    /// <para>
    /// The original code in C can be obtained here:
    /// <see cref="http://www.inf.ufrgs.br/~arcoelho/rslp/integrando_rslp.html"/>
    /// </para>
    /// </remarks>
    public sealed class RSLPStemmer : AbstractStemmer {

        /// <summary>
        /// Gets the default steps.
        /// </summary>
        public static readonly List<RSLPStep> DefaultSteps;

        #region % DefaultImplementation .

        /// <summary>
        /// Initializes static members of the <see cref="RSLPStemmer"/> class.
        /// </summary>
        static RSLPStemmer() {
            DefaultSteps = new List<RSLPStep> {
                // # Step 1: Plural Reduction
                new RSLPStep("Plural", 3, false, "s") {
                    new RSLPRule("ns", 1, "m"),
                    new RSLPRule("\u00f5es", 3, "\u00e3o"),
                    new RSLPRule("\u00e3es", 1, "\u00e3o", new[] {"m\u00e3e"}),
                    new RSLPRule("ais", 1, "al", new[] {"cais", "mais"}),
                    new RSLPRule("\u00e9is", 2, "el"),
                    new RSLPRule("eis", 2, "el"),
                    new RSLPRule("\u00f3is", 2, "ol"),
                    new RSLPRule("is", 2, "il", new[] {"l\u00e1pis", "cais", "mais", "cr\u00facis", "biqu\u00ednis", "pois", "depois", "dois", "leis"}),
                    new RSLPRule("les", 3, "l"),
                    new RSLPRule("res", 3, "r"),
                    new RSLPRule("s", 2, "", new[] {
                            "ali\u00e1s", "pires", "l\u00e1pis", "cais", "mais", "mas", "menos", "f\u00e9rias", "fezes", "p\u00easames",
                            "cr\u00facis", "g\u00e1s", "atr\u00e1s", "mois\u00e9s", "atrav\u00e9s", "conv\u00e9s", "\u00eas", "pa\u00eds", "ap\u00f3s", "ambas",
                            "ambos", "messias"
                        })
                },
                // Step 2: Feminine Reduction
                new RSLPStep("Feminine", 3, false, "a", "\u00e3") {
                    new RSLPRule("ona", 3, "\u00e3o", new[] {
                        "abandona", "lona", "iona", "cortisona", "mon\u00f3tona", "maratona", "acetona", "detona", "carona"
                    }),
                    new RSLPRule("ora", 3, "or"),
                    new RSLPRule("na", 4, "no", new[] {
                            "carona", "abandona", "lona", "iona", "cortisona", "mon\u00f3tona", "maratona", "acetona", "detona",
                            "guiana", "campana", "grana", "caravana", "banana", "paisana"
                        }),
                    new RSLPRule("inha", 3, "inho", "rainha", "linha", "minha"),
                    new RSLPRule("esa", 3, "\u00eas", new[] {
                        "mesa", "obesa", "princesa", "turquesa", "ilesa", "pesa", "presa"
                    }),
                    new RSLPRule("osa", 3, "oso", new[] {"mucosa", "prosa"}),
                    new RSLPRule("\u00edaca", 3, "\u00edaco"),
                    new RSLPRule("ica", 3, "ico", new[] {"dica"}),
                    new RSLPRule("ada", 2, "ado", new[] {"pitada"}),
                    new RSLPRule("ida", 3, "ido", new[] {"vida"}),
                    new RSLPRule("\u00edda", 3, "ido", new[] {"reca\u00edda", "sa\u00edda", "d\u00favida"}),
                    new RSLPRule("ima", 3, "imo", new[] {"v\u00edtima"}),
                    new RSLPRule("iva", 3, "ivo", new[] {"saliva", "oliva"}),
                    new RSLPRule("eira", 3, "eiro", new[] {
                            "beira", "cadeira", "frigideira", "bandeira", "feira", "capoeira", "barreira", "fronteira",
                            "besteira", "poeira"
                        }),
                    new RSLPRule("\u00e3", 2, "\u00e3o", new[] {"amanh\u00e3", "arapu\u00e3", "f\u00e3", "div\u00e3"})
                },

                // Step 3: Adverb Reduction
                new RSLPStep("Adverb", 0, false) {
                    new RSLPRule("mente", 4, "", new[] {"experimente"})
                },

                // Step 4: Augmentative/Diminutive Reduction
                new RSLPStep("Augmentative", 0, false) {
                    new RSLPRule("d\u00edssimo", 5),
                    new RSLPRule("abil\u00edssimo", 5),
                    new RSLPRule("\u00edssimo", 3),
                    new RSLPRule("\u00e9simo", 3),
                    new RSLPRule("\u00e9rrimo", 4),
                    new RSLPRule("zinho", 2),
                    new RSLPRule("quinho", 4, "c"),
                    new RSLPRule("uinho", 4),
                    new RSLPRule("adinho", 3),
                    new RSLPRule("inho", 3, "", new[] {"caminho", "cominho"}),
                    new RSLPRule("alh\u00e3o", 4),
                    new RSLPRule("u\u00e7a", 4),
                    new RSLPRule("a\u00e7o", 4, "", new[] {"antebra\u00e7o"}),
                    new RSLPRule("a\u00e7a", 4),
                    new RSLPRule("ad\u00e3o", 4),
                    new RSLPRule("id\u00e3o", 4),
                    new RSLPRule("\u00e1zio", 3, "", new[] {"top\u00e1zio"}),
                    new RSLPRule("arraz", 4),
                    new RSLPRule("zarr\u00e3o", 3),
                    new RSLPRule("arr\u00e3o", 4),
                    new RSLPRule("arra", 3),
                    new RSLPRule("z\u00e3o", 2, "", new[] {"coaliz\u00e3o"}),
                    new RSLPRule("\u00e3o", 3, "", new[] {
                        "camar\u00e3o", "chimarr\u00e3o", "can\u00e7\u00e3o", "cora\u00e7\u00e3o", "embri\u00e3o", "grot\u00e3o", "glut\u00e3o",
                        "fic\u00e7\u00e3o", "fog\u00e3o", "fei\u00e7\u00e3o", "furac\u00e3o", "gam\u00e3o", "lampi\u00e3o", "le\u00e3o", "macac\u00e3o", "na\u00e7\u00e3o",
                        "\u00f3rf\u00e3o", "org\u00e3o", "patr\u00e3o", "port\u00e3o", "quinh\u00e3o", "rinc\u00e3o", "tra\u00e7\u00e3o",
                        "falc\u00e3o", "espi\u00e3o", "mam\u00e3o", "foli\u00e3o", "cord\u00e3o", "aptid\u00e3o", "campe\u00e3o",
                        "colch\u00e3o", "lim\u00e3o", "leil\u00e3o", "mel\u00e3o", "bar\u00e3o", "milh\u00e3o", "bilh\u00e3o", "fus\u00e3o",
                        "crist\u00e3o", "ilus\u00e3o", "capit\u00e3o", "esta\u00e7\u00e3o", "sen\u00e3o"
                    })
                },

                // Step 5: Noun Suffix Reduction
                new RSLPStep("Noun", 0, false) {
                    new RSLPRule("encialista", 4),
                    new RSLPRule("alista", 5),
                    new RSLPRule("agem", 3, "", new[] {"coragem", "chantagem", "vantagem", "carruagem"}),
                    new RSLPRule("iamento", 4),
                    new RSLPRule("amento", 3, "", new[] {"firmamento", "fundamento", "departamento"}),
                    new RSLPRule("imento", 3),
                    new RSLPRule("mento", 6, "", new[] {"firmamento", "elemento", "complemento", "instrumento", "departamento"}),
                    new RSLPRule("alizado", 4),
                    new RSLPRule("atizado", 4),
                    new RSLPRule("tizado", 4, "", new[] {"alfabetizado"}),
                    new RSLPRule("izado", 5, "", new[] {"organizado", "pulverizado"}),
                    new RSLPRule("ativo", 4, "", new[] {"pejorativo", "relativo"}),
                    new RSLPRule("tivo", 4, "", new[] {"relativo"}),
                    new RSLPRule("ivo", 4, "", new[] {"passivo", "possessivo", "pejorativo", "positivo"}),
                    new RSLPRule("ado", 2, "", new[] {"grado"}),
                    new RSLPRule("ido", 3, "", new[] {"c00e2ndido", "consolido", "r\u00e1pido", "decido", "t\u00edmido", "duvido", "marido"}),
                    new RSLPRule("ador", 3),
                    new RSLPRule("edor", 3),
                    new RSLPRule("idor", 4, "", new[] {"ouvidor"}),
                    new RSLPRule("dor", 4, "", new[] {"ouvidor"}),
                    new RSLPRule("sor", 4, "", new[] {"assessor"}),
                    new RSLPRule("atoria", 5),
                    new RSLPRule("tor", 3, "", new[] {"benfeitor", "leitor", "editor", "pastor", "produtor", "promotor", "consultor"}),
                    new RSLPRule("or", 2, "",  new[] {
                            "motor", "melhor", "redor", "rigor", "sensor", "tambor", "tumor", "assessor", "benfeitor",
                            "pastor", "terior", "favor", "autor"
                        }),
                    new RSLPRule("abilidade", 5),
                    new RSLPRule("icionista", 4),
                    new RSLPRule("cionista", 5),
                    new RSLPRule("ionista", 5),
                    new RSLPRule("ionar", 5),
                    new RSLPRule("ional", 4),
                    new RSLPRule("\u00eancia", 3),
                    new RSLPRule("00e2ncia", 4, "", new[] {"ambul00e2ncia"}),
                    new RSLPRule("edouro", 3),
                    new RSLPRule("queiro", 3, "c"),
                    new RSLPRule("adeiro", 4, "", new[] {"desfiladeiro"}),
                    new RSLPRule("eiro", 3, "", new[] {"desfiladeiro", "pioneiro", "mosteiro"}),
                    new RSLPRule("uoso", 3),
                    new RSLPRule("oso", 3, "", new[] {"precioso"}),
                    new RSLPRule("aliza\u00e7", 5),
                    new RSLPRule("atiza\u00e7", 5),
                    new RSLPRule("tiza\u00e7", 5),
                    new RSLPRule("iza\u00e7", 5, "", new[] {"organiza\u00e7"}),
                    new RSLPRule("a\u00e7", 3, "", new[] {"equa\u00e7", "rela\u00e7"}),
                    new RSLPRule("i\u00e7", 3, "", new[] {"elei\u00e7\u00e3o"}),
                    new RSLPRule("\u00e1rio", 3, "", new[] {
                        "volunt\u00e1rio", "sal\u00e1rio", "anivers\u00e1rio", "di\u00e1rio", "lion\u00e1rio", "arm\u00e1rio"
                    }),
                    new RSLPRule("at\u00f3rio", 3),
                    new RSLPRule("rio", 5, "", new[] {
                            "volunt\u00e1rio", "sal\u00e1rio", "anivers\u00e1rio", "di\u00e1rio", "compuls\u00f3rio", "lion\u00e1rio", "pr\u00f3prio",
                            "st\u00e9rio", "arm\u00e1rio"
                        }),
                    new RSLPRule("\u00e9rio", 6),
                    new RSLPRule("\u00eas", 4),
                    new RSLPRule("eza", 3),
                    new RSLPRule("ez", 4),
                    new RSLPRule("esco", 4),
                    new RSLPRule("ante", 2, "", new[] {"gigante", "elefante", "adiante", "possante", "instante", "restaurante"}),
                    new RSLPRule("\u00e1stico", 4, "", new[] {"eclesi\u00e1stico"}),
                    new RSLPRule("al\u00edstico", 3),
                    new RSLPRule("\u00e1utico", 4),
                    new RSLPRule("\u00eautico", 4),
                    new RSLPRule("tico", 3, "", new[] {
                            "pol\u00edtico", "eclesi\u00e1stico", "diagnostico", "pr\u00e1tico", "dom\u00e9stico", "diagn\u00f3stico", "id\u00eantico",
                            "alop\u00e1tico", "art\u00edstico", "aut\u00eantico", "ecl\u00e9tico", "cr\u00edtico", "critico"
                        }),
                    new RSLPRule("ico", 4, "", new[] {"tico", "p\u00fablico", "explico"}),
                    new RSLPRule("ividade", 5),
                    new RSLPRule("idade", 4, "", new[] {"autoridade", "comunidade"}),
                    new RSLPRule("oria", 4, "", new[] {"categoria"}),
                    new RSLPRule("encial", 5),
                    new RSLPRule("ista", 4),
                    new RSLPRule("auta", 5),
                    new RSLPRule("quice", 4, "c"),
                    new RSLPRule("ice", 4, "", new[] {"c\u00famplice"}),
                    new RSLPRule("\u00edaco", 3),
                    new RSLPRule("ente", 4, "", new[] {
                        "freq00fcente", "alimente", "acrescente", "permanente", "oriente", "aparente"
                    }),
                    new RSLPRule("ense", 5),
                    new RSLPRule("inal", 3),
                    new RSLPRule("ano", 4),
                    new RSLPRule("\u00e1vel", 2, "", new[] {"af\u00e1vel", "razo\u00e1vel", "pot\u00e1vel", "vulner\u00e1vel"}),
                    new RSLPRule("\u00edvel", 3, "", new[] {"poss\u00edvel"}),
                    new RSLPRule("vel", 5, "", new[] {"poss\u00edvel", "vulner\u00e1vel", "sol\u00favel"}),
                    new RSLPRule("bil", 3, "vel"),
                    new RSLPRule("ura", 4, "", new[] {"imatura", "acupuntura", "costura"}),
                    new RSLPRule("ural", 4),
                    new RSLPRule("ual", 3, "", new[] {"bissexual", "virtual", "visual", "pontual"}),
                    new RSLPRule("ial", 3),
                    new RSLPRule("al", 4, "", new[] {
                            "afinal", "animal", "estatal", "bissexual", "desleal", "fiscal", "formal", "pessoal", "liberal",
                            "postal", "virtual", "visual", "pontual", "sideral", "sucursal"
                        }),
                    new RSLPRule("alismo", 4),
                    new RSLPRule("ivismo", 4),
                    new RSLPRule("ismo", 3, "", new[] {"cinismo"})
                },

                // Step 6: Verb Suffix Reduction
                new RSLPStep("Verb", 0, false) {
                    new RSLPRule("ar\u00edamo", 2),
                    new RSLPRule("\u00e1ssemo", 2),
                    new RSLPRule("er\u00edamo", 2),
                    new RSLPRule("\u00eassemo", 2),
                    new RSLPRule("ir\u00edamo", 3),
                    new RSLPRule("\u00edssemo", 3),
                    new RSLPRule("\u00e1ramo", 2),
                    new RSLPRule("\u00e1rei", 2),
                    new RSLPRule("aremo", 2),
                    new RSLPRule("ariam", 2),
                    new RSLPRule("ar\u00edei", 2),
                    new RSLPRule("\u00e1ssei", 2),
                    new RSLPRule("assem", 2),
                    new RSLPRule("\u00e1vamo", 2),
                    new RSLPRule("\u00earamo", 3),
                    new RSLPRule("eremo", 3),
                    new RSLPRule("eriam", 3),
                    new RSLPRule("er\u00edei", 3),
                    new RSLPRule("\u00eassei", 3),
                    new RSLPRule("essem", 3),
                    new RSLPRule("\u00edramo", 3),
                    new RSLPRule("iremo", 3),
                    new RSLPRule("iriam", 3),
                    new RSLPRule("ir\u00edei", 3),
                    new RSLPRule("\u00edssei", 3),
                    new RSLPRule("issem", 3),
                    new RSLPRule("ando", 2),
                    new RSLPRule("endo", 3),
                    new RSLPRule("indo", 3),
                    new RSLPRule("ondo", 3),
                    new RSLPRule("aram", 2),
                    new RSLPRule("ar\u00e3o", 2),
                    new RSLPRule("arde", 2),
                    new RSLPRule("arei", 2),
                    new RSLPRule("arem", 2),
                    new RSLPRule("aria", 2),
                    new RSLPRule("armo", 2),
                    new RSLPRule("asse", 2),
                    new RSLPRule("aste", 2),
                    new RSLPRule("avam", 2, "", new[] {"agravam"}),
                    new RSLPRule("\u00e1vei", 2),
                    new RSLPRule("eram", 3),
                    new RSLPRule("er\u00e3o", 3),
                    new RSLPRule("erde", 3),
                    new RSLPRule("erei", 3),
                    new RSLPRule("\u00earei", 3),
                    new RSLPRule("erem", 3),
                    new RSLPRule("eria", 3),
                    new RSLPRule("ermo", 3),
                    new RSLPRule("esse", 3),
                    new RSLPRule("este", 3, "", new[] {"faroeste", "agreste"}),
                    new RSLPRule("\u00edamo", 3),
                    new RSLPRule("iram", 3),
                    new RSLPRule("\u00edram", 3),
                    new RSLPRule("ir\u00e3o", 2),
                    new RSLPRule("irde", 2),
                    new RSLPRule("irei", 3, "", new[] {"admirei"}),
                    new RSLPRule("irem", 3, "", new[] {"adquirem"}),
                    new RSLPRule("iria", 3),
                    new RSLPRule("irmo", 3),
                    new RSLPRule("isse", 3),
                    new RSLPRule("iste", 4),
                    new RSLPRule("iava", 4, "", new[] {"ampliava"}),
                    new RSLPRule("amo", 2),
                    new RSLPRule("iona", 3),
                    new RSLPRule("ara", 2, "", new[] {"arara", "prepara"}),
                    new RSLPRule("ar\u00e1", 2, "", new[] {"alvar\u00e1"}),
                    new RSLPRule("are", 2, "", new[] {"prepare"}),
                    new RSLPRule("ava", 2, "", new[] {"agrava"}),
                    new RSLPRule("emo", 2),
                    new RSLPRule("era", 3, "", new[] {"acelera", "espera"}),
                    new RSLPRule("er\u00e1", 3),
                    new RSLPRule("ere", 3, "", new[] {"espere"}),
                    new RSLPRule("iam", 3, "", new[] {"enfiam", "ampliam", "elogiam", "ensaiam"}),
                    new RSLPRule("\u00edei", 3),
                    new RSLPRule("imo", 3, "", new[] {"reprimo", "intimo", "\u00edntimo", "nimo", "queimo", "ximo"}),
                    new RSLPRule("ira", 3, "", new[] {"fronteira", "s\u00e1tira"}),
                    new RSLPRule("\u00eddo", 3),
                    new RSLPRule("ir\u00e1", 3),
                    new RSLPRule("tizar", 4, "", new[] {"alfabetizar"}),
                    new RSLPRule("izar", 5, "", new[] {"organizar"}),
                    new RSLPRule("itar", 5, "", new[] {"acreditar", "explicitar", "estreitar"}),
                    new RSLPRule("ire", 3, "", new[] {"adquire"}),
                    new RSLPRule("omo", 3),
                    new RSLPRule("ai", 2),
                    new RSLPRule("am", 2),
                    new RSLPRule("ear", 4, "", new[] {"alardear", "nuclear"}),
                    new RSLPRule("ar", 2, "", new[] {"azar", "bazaar", "patamar"}),
                    new RSLPRule("uei", 3),
                    new RSLPRule("u\u00eda", 5, "u"),
                    new RSLPRule("ei", 3),
                    new RSLPRule("guem", 3, "g"),
                    new RSLPRule("em", 2, "", new[] {"alem", "virgem"}),
                    new RSLPRule("er", 2, "", new[] {"\u00e9ter", "pier"}),
                    new RSLPRule("eu", 3, "", new[] {"chapeu"}),
                    new RSLPRule("ia", 3, "", new[] {
                            "est\u00f3ria", "fatia", "acia", "praia", "elogia", "mania", "l\u00e1bia", "aprecia", "pol\u00edcia",
                            "arredia", "cheia", "\u00e1sia"
                        }),
                    new RSLPRule("ir", 3, "", new[] {"freir"}),
                    new RSLPRule("iu", 3),
                    new RSLPRule("eou", 5),
                    new RSLPRule("ou", 3),
                    new RSLPRule("i", 3)
                },

                // Step 7: Vowel Removal 
                new RSLPStep("Vowel", 0, false) {
                    new RSLPRule("bil", 2, "vel"),
                    new RSLPRule("gue", 2, "g", new[] {"gangue", "jegue"}),
                    new RSLPRule("\u00e1", 3),
                    new RSLPRule("\u00ea", 3, "", new[] {"beb\u00ea"}),
                    new RSLPRule("a", 3, "", new[] {"\u00e1sia"}),
                    new RSLPRule("e", 3),
                    new RSLPRule("o", 3, "", new[] {"\u00e3o"})
                }
            };

            /** 	
                Default step´s application flow, as written in the original RSPL code.
                It differs from the Orengo's paper, shown below:

                { 0 "Plural"       , 1 "Feminine"     , 1 "Feminine"     }  , 
                { 1 "Feminine"     , 3 "Augmentative" , 3 "Augmentative" }  , 
                { 3 "Augmentative" , 2 "Adverb"       , 2 "Adverb"       }  , 
                { 2 "Adverb"       , 4 "Noun"         , 4 "Noun"         }  , 
                { 4 "Noun"         ,   NULL           , 5 "Verb"         }  , 
                { 5 "Verb"         ,   NULL           , 6 "Vowel"        }  , 
                { 6 "Vowel"        ,   NULL           ,   NULL           }  , 
            */

            DefaultSteps[0].FlowPass = DefaultSteps[1];
            DefaultSteps[0].FlowFail = DefaultSteps[1];

            DefaultSteps[1].FlowPass = DefaultSteps[3];
            DefaultSteps[1].FlowFail = DefaultSteps[3];

            DefaultSteps[2].FlowPass = DefaultSteps[4];
            DefaultSteps[2].FlowFail = DefaultSteps[4];

            DefaultSteps[3].FlowPass = DefaultSteps[2];
            DefaultSteps[3].FlowFail = DefaultSteps[2];

            DefaultSteps[4].FlowPass = null;
            DefaultSteps[4].FlowFail = DefaultSteps[5];

            DefaultSteps[5].FlowPass = null;
            DefaultSteps[5].FlowFail = DefaultSteps[6];

            DefaultSteps[6].FlowPass = null;
            DefaultSteps[6].FlowFail = null;
        }

        #endregion

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="RSLPStemmer"/> class.
        /// </summary>
        public RSLPStemmer() {
            RemoveDiacritics = true;
        }
        #endregion

        #region + Properties .

        #region . RemoveDiacritics .

        /// <summary>
        /// Gets or sets a value indicating whether the diacritics should be removed. The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if the diacritics should be removed; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool RemoveDiacritics { get; set; }

        #endregion

        #region . Steps .
        private List<RSLPStep> steps;

        /// <summary>
        /// Gets or sets the steps of this stemmer.
        /// </summary>
        /// <value>The steps of this stemmer.</value>
        public List<RSLPStep> Steps {
            get {
                return steps ?? DefaultSteps;
            }
            set { steps = value; }
        }
        #endregion

        #region . Start .

        private string start = "Plural";

        /// <summary>
        /// Gets or sets the starting step.
        /// </summary>
        /// <value>The starting step.</value>
        /// <exception cref="System.ArgumentException">The specified step does not exist.</exception>
        public string Start {
            get { return start; }
            set {
                if (GetStep(value) == null)
                    throw new ArgumentException(@"The specified step does not exist.", "value");

                start = value;
            }
        }
        #endregion

        #endregion

        #region . SetFlow .
        /// <summary>
        /// Sets the evaluation flow for the specified <paramref name="stepName"/>.
        /// </summary>
        /// <param name="stepName">Name of the step.</param>
        /// <param name="passStep">The pass step. This value can be null.</param>
        /// <param name="failStep">The fail step. This value can be null.</param>
        /// <exception cref="System.ArgumentNullException">stepName</exception>
        /// <exception cref="System.ArgumentException">
        /// The specified step does not exist.;stepName
        /// or
        /// The specified step does not exist.;passStep
        /// or
        /// The specified step does not exist.;failStep
        /// </exception>
        public void SetFlow(string stepName, string passStep, string failStep) {
            if (string.IsNullOrEmpty(stepName))
                throw new ArgumentNullException("stepName");

            var step = GetStep(stepName);
            var fail = GetStep(passStep);
            var pass = GetStep(failStep);

            if (step == null)
                throw new ArgumentException(@"The specified step does not exist.", "stepName");

            if (!string.IsNullOrEmpty(passStep) && pass == null)
                throw new ArgumentException(@"The specified step does not exist.", "passStep");

            if (!string.IsNullOrEmpty(failStep) && fail == null)
                throw new ArgumentException(@"The specified step does not exist.", "failStep");

            step.FlowPass = pass;
            step.FlowFail = fail;
        }
        #endregion

        #region . GetStep .
        private RSLPStep GetStep(string name) {
            return Steps.FirstOrDefault(step => step.StepName == name);
        }
        #endregion

        #region . Stemming .

        /// <summary>
        /// Performs stemming on the specified word.
        /// </summary>
        /// <param name="word">The word to be stemmed.</param>
        /// <param name="posTag">The part-of-speech tag or a <c>null</c> value if none.</param>
        /// <returns>The stemmed word.</returns>
        protected override string Stemming(string word, string posTag) {

            // Do the stemming only if the word doesn't contain numbers
            if (word.ToCharArray().Any(char.IsDigit))
                return word;

            // word too small to be stemmed
            if (word.Length <= 3)
                return word;

            var stem = word;
            var step = GetStep(start);

            while (step != null) {
                if (step.MinWordLen > 0 && stem.Length < step.MinWordLen)
                    goto fail;

                if (step.EndWords.Length > 0) {
                    foreach (var end in step.EndWords) {
                        if (stem.EndsWith(end))
                            goto apply;
                    }
                    goto fail;
                }

            apply:

                foreach (var rule in step.Rules) {
                    if (rule.Exceptions != null && rule.Exceptions.Contains(stem))
                        continue;

                    if (stem.Length < rule.MinStemSize)
                        continue;

                    if (step.EntireWord && stem.Equals(rule.Suffix, StringComparison.Ordinal) ||
                        !step.EntireWord && stem.EndsWith(rule.Suffix)) {

                            stem = stem.Left(-rule.Suffix.Length);

                        if (!string.IsNullOrEmpty(rule.Replacement))
                            stem += rule.Replacement;

                        step = step.FlowPass;
                        goto next;
                    }
                }

            fail:
                step = step.FlowFail;
            next:
                ;
            }

            return RemoveDiacritics
                ? stem.RemoveDiacritics()
                : stem;
        }

        #endregion

    }
}