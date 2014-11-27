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
using System.Linq;
using System.Text.RegularExpressions;

namespace SharpNL.Inflecter.Lang.en {

    /// <summary>
    /// Represents a english inflector.
    /// </summary>
    /// <remarks>
    /// The plurilizer is based on "An Algorithmic Approach to English Pluralization" by Damian Conway:
    /// <seealso href="http://www.csse.monash.edu.au/~damian/papers/HTML/Plurals.html"/> 
    /// 
    /// The singularize is Adapted from Bermi Ferrer's Inflector for Python
    /// <see href="http://www.bermi.org/inflector/"/>
    /// 
    /// License:
    /// 
    /// Copyright (c) 2006 Bermi Ferrer Martinez
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software to deal in this software without restriction, including
    /// without limitation the rights to use, copy, modify, merge, publish,
    /// distribute, sublicense, and/or sell copies of this software, and to permit
    /// persons to whom this software is furnished to do so, subject to the following
    /// condition:
    /// 
    /// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THIS SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THIS SOFTWARE.
    /// </remarks>
    public class Inflecter : IInfleter {

        // plural
        private static readonly Dictionary<string, List<string>> pluralCategories;
        private static readonly List<string> pluralPrepositions;
        private static readonly List<List<PluralRule>> pluralRules;


        // singular
        private static readonly List<SingularRule> singularRules;
        private static readonly List<string> singularUninflected;
        private static readonly List<string> singularUncountable;
        private static readonly List<string> singularIe;
        private static readonly Dictionary<string, string> singularIrregular;

        private class PluralRule {
            public Regex Regex { get; private set; }
            public string Plural { get; private set; }
            public string Category { get; private set; }
            public bool Classic { get; private set; }

            public PluralRule(string pattern, string plural, string category, bool classical) {
                Regex = new Regex(pattern, RegexOptions.Compiled);
                Plural = plural;
                Category = category;
                Classic = classical;
            }
        }

        private class SingularRule {
            public Regex Regex { get; private set; }
            public string Replace { get; private set; }

            public SingularRule(string pattern, string replace) {
                Regex = new Regex(pattern, RegexOptions.Compiled);
                Replace = replace;
            }
        }

        #region + Static members .



        /// <summary>
        /// Initializes static members of the <see cref="Inflecter"/> class.
        /// </summary>
        static Inflecter() {
            pluralCategories = new Dictionary<string, List<string>> {
                {"uninflected", new List<string> {
                     "bison", "bream", "breeches", "britches", "carp", "chassis", "clippers", "cod", "contretemps",
                     "corps", "debris", "diabetes", "djinn", "eland", "elk", "flounder", "gallows", "graffiti",
                     "headquarters", "herpes", "high-jinks", "homework", "innings", "jackanapes", "mackerel",
                     "measles", "mews", "mumps", "news", "pincers", "pliers", "proceedings", "rabies", "salmon",
                     "scissors", "series", "shears", "species", "swine", "trout", "tuna", "whiting", "wildebeest"}},
                {"uncountable", new List<string> {
                     "advice", "bread", "butter", "cheese", "electricity", "equipment", "fruit", "furniture",
                     "garbage", "gravel", "happiness", "information", "ketchup", "knowledge", "love", "luggage",
                     "mathematics", "mayonnaise", "meat", "mustard", "news", "progress", "research", "rice",
                     "sand", "software", "understanding", "water"}},
                {"s-singular", new List<string> {
                     "acropolis", "aegis", "alias", "asbestos", "bathos", "bias", "caddis", "cannabis", "canvas",
                     "chaos", "cosmos", "dais", "digitalis", "epidermis", "ethos", "gas", "glottis", "glottis",
                     "ibis", "lens", "mantis", "marquis", "metropolis", "pathos", "pelvis", "polis", "rhinoceros",
                     "sassafras", "trellis"}},
                {"ex-ices", new List<string> {"codex", "murex", "silex"}},
                {"ex-ices-classical", new List<string> {
                     "apex", "cortex", "index", "latex", "pontifex", "simplex", "vertex", "vortex"}},
                {"um-a", new List<string> {
                     "agendum", "bacterium", "candelabrum", "datum", "desideratum", "erratum", "extremum",
                     "ovum", "stratum"}},
                {"um-a-classical", new List<string> {
                     "aquarium", "compendium", "consortium", "cranium", "curriculum", "dictum", "emporium",
                     "enconium", "gymnasium", "honorarium", "interregnum", "lustrum", "maximum", "medium",
                     "memorandum", "millenium", "minimum", "momentum", "optimum", "phylum", "quantum", "rostrum",
                     "spectrum", "speculum", "stadium", "trapezium", "ultimatum", "vacuum", "velum"}},
                {"on-a", new List<string> {
                     "aphelion", "asyndeton", "criterion", "hyperbaton", "noumenon", "organon", "perihelion",
                     "phenomenon", "prolegomenon"}},
                {"a-ae", new List<string> {"alga", "alumna", "vertebra"}},
                {"a-ae-classical", new List<string> {
                     "abscissa", "amoeba", "antenna", "aurora", "formula", "hydra", "hyperbola", "lacuna",
                     "medusa", "nebula", "nova", "parabola"}},
                {"en-ina-classical", new List<string> {"foramen", "lumen", "stamen"}},
                {"a-ata-classical", new List<string> {
                     "anathema", "bema", "carcinoma", "charisma", "diploma", "dogma", "drama", "edema", "enema",
                     "enigma", "gumma", "lemma", "lymphoma", "magma", "melisma", "miasma", "oedema", "sarcoma",
                     "schema", "soma", "stigma", "stoma", "trauma"}},
                {"is-ides-classical", new List<string> {"clitoris", "iris"}},
                {"us-i-classical", new List<string> {
                     "focus", "fungus", "genius", "incubus", "nimbus", "nucleolus", "radius", "stylus", "succubus",
                     "torus", "umbilicus", "uterus"}},
                {"us-us-classical", new List<string> {
                     "apparatus", "cantus", "coitus", "hiatus", "impetus", "nexus", "plexus", "prospectus",
                     "sinus", "status"}},
                {"o-i-classical", new List<string> {"alto", "basso", "canto", "contralto", "crescendo", "solo", "soprano", "tempo"}},
                {"-i-classical", new List<string> {"afreet", "afrit", "efreet"}},
                {"-im-classical", new List<string> {"cherub", "goy", "seraph"}},
                {"o-os", new List<string> {
                     "albino", "archipelago", "armadillo", "commando", "ditto", "dynamo", "embryo", "fiasco",
                     "generalissimo", "ghetto", "guano", "inferno", "jumbo", "lingo", "lumbago", "magneto",
                     "manifesto", "medico", "octavo", "photo", "pro", "quarto", "rhino", "stylo"}},
                {"general-generals", new List<string> {
                     "Adjutant", "Brigadier", "Lieutenant", "Major", "Quartermaster",
                     "adjutant", "brigadier", "lieutenant", "major", "quartermaster"}}
            };

            pluralPrepositions = new List<string> {
                "about", "above", "across", "after", "among", "around", "at", "athwart", "before", "behind",
                "below", "beneath", "beside", "besides", "between", "betwixt", "beyond", "but", "by", "during",
                "except", "for", "from", "in", "into", "near", "of", "off", "on", "onto", "out", "over",
                "since", "till", "to", "under", "until", "unto", "upon", "with"
            };


            pluralRules = new List<List<PluralRule>> {
                // 0) Indefinite articles and demonstratives.
                new List<PluralRule> {
                    new PluralRule("^a$|^an$", "some", null, false),
                    new PluralRule("^this$", "these", null, false),
                    new PluralRule("^that$", "those", null, false),
                    new PluralRule("^any$", "all", null, false)
                },
                // 1) Possessive adjectives.
                // Overlaps with 1/ for "his" and "its".
                // Overlaps with 2/ for "her".
                new List<PluralRule> {
                    new PluralRule("^my$", "our", null, false),
                    new PluralRule("^your$|^thy$", "your", null, false),
                    new PluralRule("^her$|^his$|^its$|^their$", "their", null, false)
                },
                // 2) Possessive pronouns.
                new List<PluralRule> {
                    new PluralRule("^mine$", "ours", null, false),
                    new PluralRule("^yours$|^thine$", "yours", null, false),
                    new PluralRule("^hers$|^his$|^its$|^theirs$", "theirs", null, false)
                },
                // 3) Personal pronouns.
                new List<PluralRule> {
                    new PluralRule("^I$", "we", null, false),
                    new PluralRule("^me$", "us", null, false),
                    new PluralRule("^myself$", "ourselves", null, false),
                    new PluralRule("^you$", "you", null, false),
                    new PluralRule("^thou$|^thee$", "ye", null, false),
                    new PluralRule("^yourself$|^thyself$", "yourself", null, false),
                    new PluralRule("^she$|^he$|^it$|^they$", "they", null, false),
                    new PluralRule("^her$|^him$|^it$|^them$", "them", null, false),
                    new PluralRule("^herself$|^himself$|^itself$|^themself$", "themselves", null, false),
                    new PluralRule("^oneself$", "oneselves", null, false)
                },
                // 4) Words that do not inflect.
                new List<PluralRule> {
                    new PluralRule("$", "", "uninflected", false),
                    new PluralRule("$", "", "uncountable", false),
                    new PluralRule("s$", "s", "s-singular", false),
                    new PluralRule("fish$", "fish", null, false),
                    new PluralRule("([- ])bass$", "$1bass", null, false),
                    new PluralRule("ois$", "ois", null, false),
                    new PluralRule("sheep$", "sheep", null, false),
                    new PluralRule("deer$", "deer", null, false),
                    new PluralRule("pox$", "pox", null, false),
                    new PluralRule("(new PluralRule(A-Z).*)ese$", "$1ese", null, false),
                    new PluralRule("itis$", "itis", null, false),
                    new PluralRule("(fruct|gluc|galact|lact|ket|malt|rib|sacchar|cellul)ose$", "$1ose", null, false)
                },
                // 5) Irregular plurals (mongoose, oxen).
                new List<PluralRule> {
                    new PluralRule("atlas$", "atlantes", null, true),
                    new PluralRule("atlas$", "atlases", null, false),
                    new PluralRule("beef$", "beeves", null, true),
                    new PluralRule("brother$", "brethren", null, true),
                    new PluralRule("child$", "children", null, false),
                    new PluralRule("corpus$", "corpora", null, true),
                    new PluralRule("corpus$", "corpuses", null, false),
                    new PluralRule("^cow$", "kine", null, true),
                    new PluralRule("ephemeris$", "ephemerides", null, false),
                    new PluralRule("ganglion$", "ganglia", null, true),
                    new PluralRule("genie$", "genii", null, true),
                    new PluralRule("genus$", "genera", null, false),
                    new PluralRule("graffito$", "graffiti", null, false),
                    new PluralRule("loaf$", "loaves", null, false),
                    new PluralRule("money$", "monies", null, true),
                    new PluralRule("mongoose$", "mongooses", null, false),
                    new PluralRule("mythos$", "mythoi", null, false),
                    new PluralRule("octopus$", "octopodes", null, true),
                    new PluralRule("opus$", "opera", null, true),
                    new PluralRule("opus$", "opuses", null, false),
                    new PluralRule("^ox$", "oxen", null, false),
                    new PluralRule("penis$", "penes", null, true),
                    new PluralRule("penis$", "penises", null, false),
                    new PluralRule("soliloquy$", "soliloquies", null, false),
                    new PluralRule("testis$", "testes", null, false),
                    new PluralRule("trilby$", "trilbys", null, false),
                    new PluralRule("turf$", "turves", null, true),
                    new PluralRule("numen$", "numena", null, false),
                    new PluralRule("occiput$", "occipita", null, true)
                },
                // 6) Irregular inflections for common suffixes (synopses, mice, men).
                new List<PluralRule> {
                    new PluralRule("man$", "men", null, false),
                    new PluralRule("person$", "people", null, false),
                    new PluralRule("([lm])ouse$", "$1ice", null, false),
                    new PluralRule("tooth$", "teeth", null, false),
                    new PluralRule("goose$", "geese", null, false),
                    new PluralRule("foot$", "feet", null, false),
                    new PluralRule("zoon$", "zoa", null, false),
                    new PluralRule("([csx])is$", "$1es", null, false)
                },
                // 7) Fully assimilated classical inflections (vertebrae, codices).
                new List<PluralRule> {
                    new PluralRule("ex$", "ices", "ex-ices", false),
                    new PluralRule("ex$", "ices", "ex-ices-classical", true),
                    new PluralRule("um$", "a", "um-a", false),
                    new PluralRule("um$", "a", "um-a-classical", true),
                    new PluralRule("on$", "a", "on-a", false),
                    new PluralRule("a$", "ae", "a-ae", false),
                    new PluralRule("a$", "ae", "a-ae-classical", true)
                },
                // 8) Classical variants of modern inflections (stigmata, soprani).
                new List<PluralRule> {
                    new PluralRule("trix$", "trices", null, true),
                    new PluralRule("eau$", "eaux", null, true),
                    new PluralRule("ieu$", "ieu", null, true),
                    new PluralRule("(new PluralRule(iay))nx$", "$1nges", null, true),
                    new PluralRule("en$", "ina", "en-ina-classical", true),
                    new PluralRule("a$", "ata", "a-ata-classical", true),
                    new PluralRule("is$", "ides", "is-ides-classical", true),
                    new PluralRule("us$", "i", "us-i-classical", true),
                    new PluralRule("us$", "us", "us-us-classical", true),
                    new PluralRule("o$", "i", "o-i-classical", true),
                    new PluralRule("$", "i", "-i-classical", true),
                    new PluralRule("$", "im", "-im-classical", true)
                },
                // 9) -ch, -sh and -ss take -es in the plural (churches, classes).
                new List<PluralRule> {
                    new PluralRule("([cs])h$", "$1hes", null, false),
                    new PluralRule("ss$", "sses", null, false),
                    new PluralRule("x$", "xes", null, false)
                },
                // 10) Certain words ending in -f or -fe take -ves in the plural (lives, wolves).
                new List<PluralRule> {
                    new PluralRule("([aeo]l)f$", "$1ves", null, false),
                    new PluralRule("([^d]ea)f$", "$1ves", null, false),
                    new PluralRule("arf$", "arves", null, false),
                    new PluralRule("([nlw]i)fe$", "$1ves", null, false),
                },
                // 11) -y takes -ys if preceded by a vowel or when a proper noun,
                // but -ies if preceded by a consonant (storeys, Marys, stories).
                new List<PluralRule> {
                    new PluralRule("([aeiou])y$", "$1ys", null, false),
                    new PluralRule("([A-Z].*)y$", "$1ys", null, false),
                    new PluralRule("y$", "ies", null, false)
                },
                // 12) Some words ending in -o take -os, the rest take -oes.
                // Words in which the -o is preceded by a vowel always take -os (lassos, potatoes, bamboos).
                new List<PluralRule> {
                    new PluralRule("o$", "os", "o-os", false),
                    new PluralRule("([aeiou])o$", "$1os", null, false),
                    new PluralRule("o$", "oes", null, false)
                },
                // 13) Miltary stuff (Major Generals).
                new List<PluralRule> {
                    new PluralRule("l$", "ls", "general-generals", false)
                },
                // 14) Otherwise, assume that the plural just adds -s (cats, programmes).
                new List<PluralRule> {
                    new PluralRule("$", "s", null, false)
                },
            };

            singularRules = new List<SingularRule> {
                new SingularRule("(?i)(.)ae$", "$1a"),
                new SingularRule("(?i)(.)itis$", "$1itis"),
                new SingularRule("(?i)(.)eaux$", "$1eau"),
                new SingularRule("(?i)(quiz)zes$", "$1"),
                new SingularRule("(?i)(matr)ices$", "$1ix"),
                new SingularRule("(?i)(ap|vert|ind)ices$", "$1ex"),
                new SingularRule("(?i)^(ox)en", "$1"),
                new SingularRule("(?i)(alias|status)es$", "$1"),
                new SingularRule("(?i)([octop|vir])i$", "$1us"),
                new SingularRule("(?i)(cris|ax|test)es$", "$1is"),
                new SingularRule("(?i)(shoe)s$", "$1"),
                new SingularRule("(?i)(o)es$", "$1"),
                new SingularRule("(?i)(bus)es$", "$1"),
                new SingularRule("(?i)([m|l])ice$", "$1ouse"),
                new SingularRule("(?i)(x|ch|ss|sh)es$", "$1"),
                new SingularRule("(?i)(m)ovies$", "$1ovie"),
                new SingularRule("(?i)(.)ombies$", "$1ombie"),
                new SingularRule("(?i)(s)eries$", "$1eries"),
                new SingularRule("(?i)([^aeiouy]|qu)ies$", "$1y"),

                // Certain words ending in -f or -fe take -ves in the plural (lives, wolves).
                new SingularRule("([aeo]l)ves$", "$1f"),
                new SingularRule("([^d]ea)ves$", "$1f"),
                new SingularRule("arves$", "arf"),
                new SingularRule("erves$", "erve"),
                new SingularRule("([nlw]i)ves$", "$1fe"),
                new SingularRule("(?i)([lr])ves$", "$1f"),
                new SingularRule("([aeo])ves$", "$1ve"),
                new SingularRule("(?i)(sive)s$", "$1"),
                new SingularRule("(?i)(tive)s$", "$1"),
                new SingularRule("(?i)(hive)s$", "$1"),
                new SingularRule("(?i)([^f])ves$", "$1fe"),

                // -es suffix.
                new SingularRule("(?i)(^analy)ses$", "$1sis"),
                new SingularRule("(?i)((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis"),
                new SingularRule("(?i)(.)opses$", "$1opsis"),
                new SingularRule("(?i)(.)yses$", "$1ysis"),
                new SingularRule("(?i)(h|d|r|o|n|b|cl|p)oses$", "$1ose"),
                new SingularRule("(?i)(fruct|gluc|galact|lact|ket|malt|rib|sacchar|cellul)ose$", "$1ose"),
                new SingularRule("(?i)(.)oses$", "$1osis"),

                // -a
                new SingularRule("(?i)([ti])a$", "$1um"),
                new SingularRule("(?i)(n)ews$", "$1ews"),
                new SingularRule("(?i)s$", ""),
            };

            singularUninflected = new List<string> {
                "bison", "bream", "breeches", "britches", "carp", "chassis", "christmas", "clippers", "cod",
                "contretemps", "corps", "debris", "diabetes", "djinn", "eland", "elk", "flounder", "gallows",
                "georgia", "graffiti", "headquarters", "herpes", "high-jinks", "homework", "innings",
                "jackanapes", "mackerel", "measles", "mews", "mumps", "news", "pincers", "pliers", "proceedings",
                "rabies", "salmon", "scissors", "series", "shears", "species", "swine", "swiss", "trout", "tuna",
                "whiting", "wildebeest"
            };

            singularUncountable = new List<string> {
                "advice", "bread", "butter", "cheese", "electricity", "equipment", "fruit", "furniture",
                "garbage", "gravel", "happiness", "information", "ketchup", "knowledge", "love", "luggage",
                "mathematics", "mayonnaise", "meat", "mustard", "news", "progress", "research", "rice", "sand",
                "software", "understanding", "water"                
            };

            singularIe = new List<string> {
                "algerie", "auntie", "beanie", "birdie", "bogie", "bombie", "bookie", "collie", "cookie", "cutie",
                "doggie", "eyrie", "freebie", "goonie", "groupie", "hankie", "hippie", "hoagie", "hottie",
                "indie", "junkie", "laddie", "laramie", "lingerie", "meanie", "nightie", "oldie", "^pie",
                "pixie", "quickie", "reverie", "rookie", "softie", "sortie", "stoolie", "sweetie", "techie",
                "^tie", "toughie", "valkyrie", "veggie", "weenie", "yuppie", "zombie"
            };

            singularIrregular = new Dictionary<string, string> {
                {"atlantes","atlas"},
                {"atlases","atlas"},
                {"axes","axe"},
                {"beeves","beef"},
                {"brethren","brother"},
                {"children","child"},
                {"corpora","corpus"},
                {"corpuses","corpus"},
                {"ephemerides","ephemeris"},
                {"feet","foot"},
                {"ganglia","ganglion"},
                {"geese","goose"},
                {"genera","genus"},
                {"genii","genie"},
                {"graffiti","graffito"},
                {"helves","helve"},
                {"kine","cow"},
                {"leaves","leaf"},
                {"loaves","loaf"},
                {"men","man"},
                {"mongooses","mongoose"},
                {"monies","money"},
                {"moves","move"},
                {"mythoi","mythos"},
                {"numena","numen"},
                {"occipita","occiput"},
                {"octopodes","octopus"},
                {"opera","opus"},
                {"opuses","opus"},
                {"our","my"},
                {"oxen","ox"},
                {"penes","penis"},
                {"penises","penis"},
                {"people","person"},
                {"sexes","sex"},
                {"soliloquies","soliloquy"},
                {"teeth","tooth"},
                {"testes","testis"},
                {"trilbys","trilby"},
                {"turves","turf"},
                {"zoa","zoon"}
            };
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Inflecter"/> class using the classical inflection as default.
        /// </summary>
        public Inflecter() {
            Classical = true;

            AdjectiveTag = "JJ";
            AdverbTag = "RB";
            VerbTag = "VB";
            NounTag = "NN";
        }

        #region . Properties .

        #region . AdjectiveTag .
        /// <summary>
        /// Gets or sets the adjective tag.
        /// </summary>
        /// <value>The adjective tag.</value>
        public string AdjectiveTag { get; set; }
        #endregion

        #region . AdverbTag .
        /// <summary>
        /// Gets or sets the adverb tag.
        /// </summary>
        /// <value>The adverb tag.</value>
        public string AdverbTag { get; set; }
        #endregion

        #region . Classical .
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Inflecter"/> is using the classical inflection. e.g. where "matrix" pluralizes to "matrices" instead of "matrixes"
        /// </summary>
        /// <value><c>true</c> if using classical inflection; otherwise, <c>false</c>.</value>
        public bool Classical { get; set; }
        #endregion
        
        #region . NounTag .
        /// <summary>
        /// Gets or sets the noun tag.
        /// </summary>
        /// <value>The noun tag.</value>
        public string NounTag { get; set; }
        #endregion

        #region . VerbTag .
        /// <summary>
        /// Gets or sets the verb tag.
        /// </summary>
        /// <value>The verb tag.</value>
        public string VerbTag { get; set; }
        #endregion

        #endregion

        #region . Pluralize .

        /// <summary>
        /// Returns the plural of a given <paramref name="word" />.
        /// </summary>
        /// <param name="word">The word to pluralize.</param>
        /// <param name="pos">The part-of-speec tag.</param>
        /// <returns>The plurilized word.</returns>
        public virtual string Pluralize(string word, string pos) {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            // Recursion of genitives.
            // Remove the apostrophe and any trailing -s,
            // form the plural of the resultant noun, and then append an apostrophe (dog's -> dogs').
            if (word.EndsWith("'") || word.EndsWith("'s", StringComparison.OrdinalIgnoreCase)) {
                var owner = word.TrimEnd(new[] {'\'', 's'});
                var owners = Pluralize(owner, pos);

                return owners.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                    ? owners + "'"
                    : owners + "'s";
            }

            // Recursion of compound words (Postmasters General, mothers-in-law, Roman deities).
            var words = word.Replace("-", " ").Split(' ');
            if (words.Length > 1) {
                if ((words[1] == "general" || words[1] == "General") &&
                    !pluralCategories["general-generals"].Contains(words[0]))
                    return word.Replace(words[0], Pluralize(words[0], pos));

                if (pluralPrepositions.Contains(words[1]))
                    return word.Replace(words[0], Pluralize(words[0], pos));

                return word.Replace(words.Last(), Pluralize(words.Last(), pos));
            }

            var count = pos != null && pos.StartsWith(AdjectiveTag) 
                ? 1 
                : pluralRules.Count;

            for (var i = 0; i < count; i++) {
                foreach (var rule in pluralRules[i]) {
                    if (!rule.Classic || (rule.Classic && Classical)) {
                        if (rule.Category == null) {
                            if (rule.Regex.IsMatch(word))
                                return rule.Regex.Replace(word, rule.Plural);

                        } else {
                            if (pluralCategories[rule.Category].Contains(word) && rule.Regex.IsMatch(word))
                                return rule.Regex.Replace(word, rule.Plural);

                        }
                    }

                }
            }
            return word;
        }

        #endregion

        #region . Singularize .

        /// <summary>
        /// Singularizes the specified <paramref name="word"/>.
        /// </summary>
        /// <param name="word">The word to singularize.</param>
        /// <param name="pos">The part-of-speech tag.</param>
        /// <returns>The singularized world.</returns>
        public virtual string Singularize(string word, string pos) {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            if (pos == null)
                pos = NounTag;

            if (word.Contains("-")) {
                var words = word.Split(new []{'-'}, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 1 && pluralPrepositions.Contains(words[1]))
                    return string.Format("{0}-{1}", Singularize(words[0], pos), string.Join("-", words.Skip(1)));
            }

            // dogs' => dog's
            if (word.EndsWith("'"))
                return string.Format("{0}'s", Singularize(word.TrimEnd('\''), pos));

            var lower = word.ToLowerInvariant();

            if (singularUninflected.Any(w => w.EndsWith(lower)))
                return word;

            if (singularUncountable.Any(w => w.EndsWith(lower)))
                return word;

            foreach (var w in singularIe) {
                if (lower.EndsWith(w + "s"))
                    return w;
            }

            foreach (var pair in singularIrregular) {
                if (lower.EndsWith(pair.Key))
                    return Regex.Replace(word, string.Format("(?i){0}$", pair.Key), pair.Value);               
            }

            foreach (var rule in singularRules) {
                var m = rule.Regex.Match(word);
                var i = rule.Replace;
                
                if (m.Success) {
                    for (int k = 0; k < m.Groups.Count; k++) {
                        if (!m.Groups[k].Success)
                            i = i.Replace(string.Format("${0}", k + 1), string.Empty);
                    }
                    return rule.Regex.Replace(word, i);
                }
            }

            return word;
        }

        #endregion
    }
}