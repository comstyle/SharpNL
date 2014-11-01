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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharpNL.NameFind;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Parser for Floresta Sita(c)tica Arvores Deitadas corpus, output to for the Portuguese NER training.
    /// <para>
    /// The data contains common multi word expressions. The categories are:<br /> 
    /// intj, spec, conj-s, num, pron-indef, n, prop, adj, prp, adv
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Data can be found on this web site: <see href="http://www.linguateca.pt/floresta/corpus.html" />
    /// </para>
    /// <para>
    /// Information about the format: <br />
    /// "Árvores deitadas: Descrição do formato e das opções de análise na Floresta Sintáctica" <br />
    /// <see href="http://www.linguateca.pt/documentos/Afonso2006ArvoresDeitadas.pdf"/>
    /// </para>
    /// <para>
    /// Detailed info about the NER tagset: <see href="http://beta.visl.sdu.dk/visl/pt/info/portsymbol.html#semtags_names"/>
    /// </para>
    /// </remarks>
    public class AdNameSampleStream : IObjectStream<NameSample> {
        private const string Hyphen = "-";

        private readonly IObjectStream<AdSentence> adSentenceStream;
        private readonly Monitor monitor;
        private readonly bool splitHyphenatedTokens;
        private Type corpusType = Type.None;
        private string leftContractionPart;
        private int textId = -1;
        private int textIdMeta2 = -1;
        private string textMeta2 = string.Empty;

        internal static readonly Regex AmaMetaRegex;
        internal static readonly Regex CieMetaRegex;
        internal static readonly Regex LitMetaRegex;
        internal static readonly Regex tagPattern;
        internal static readonly Regex alphanumericPattern;
        internal static readonly Regex hyphenPattern;

        private static readonly IReadOnlyDictionary<string, string> mapNER;

        #region + Constructors .

        /// <summary>
        /// Initializes static members of the <see cref="AdNameSampleStream"/> class.
        /// </summary>
        static AdNameSampleStream() {
            AmaMetaRegex = new Regex("^(?:[a-zA-Z\\-]*(\\d+)).*?p=(\\d+).*", RegexOptions.Compiled);
            CieMetaRegex = new Regex("^.*?source=\"(.*?)\".*", RegexOptions.Compiled);
            LitMetaRegex = new Regex("^([a-zA-Z\\-]+)(\\d+).*?p=(\\d+).*", RegexOptions.Compiled);

            alphanumericPattern = new Regex("^[\\p{L}\\p{Nd}]+$", RegexOptions.Compiled);
            hyphenPattern = new Regex("((\\p{L}+)-$)|(^-(\\p{L}+)(.*))|((\\p{L}+)-(\\p{L}+)(.*))", RegexOptions.Compiled);
            tagPattern = new Regex("<(NER:)?(.*?)>", RegexOptions.Compiled);

            var map = new Dictionary<string, string>();

            const string person = "person";
            map.Add("hum", person);
            map.Add("official", person);
            map.Add("member", person);
            map.Add("author", person);

            const string organization = "organization";
            map.Add("admin", organization);
            map.Add("org", organization);
            map.Add("inst", organization);
            map.Add("media", organization);
            map.Add("party", organization);
            map.Add("suborg", organization);

            const string group = "group";
            map.Add("groupind", group);
            map.Add("groupofficial", group);

            const string place = "place";
            map.Add("top", place);
            map.Add("civ", place);
            map.Add("address", place);
            map.Add("site", place);
            map.Add("virtual", place);
            map.Add("astro", place);

            const string ev = "event";
            map.Add("occ", ev);
            map.Add("event", ev);
            map.Add("history", ev);

            const string artprod = "artprod";
            map.Add("tit", artprod);
            map.Add("pub", artprod);
            map.Add("product", artprod);
            map.Add("V", artprod);
            map.Add("artwork", artprod);

            const string abs = "abstract";
            map.Add("brand", abs);
            map.Add("genre", abs);
            map.Add("school", abs);
            map.Add("idea", abs);
            map.Add("plan", abs);
            
            map.Add("absname", abs);
            map.Add("disease", abs);

            const string thing = "thing";
            map.Add("object", thing);
            map.Add("common", thing);
            map.Add("mat", thing);
            map.Add("class", thing);
            map.Add("plant", thing);

            const string time = "time";
            map.Add("date", time);
            map.Add("hour", time);
            map.Add("period", time);
            map.Add("cyclic", time);

            const string numeric = "numeric";
            map.Add("quantity", numeric);
            map.Add("prednum", numeric);
            map.Add("currency", numeric);

            mapNER = new ReadOnlyDictionary<string, string>(map);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AdNameSampleStream" /> from a <paramref name="lineStream" /> object.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdNameSampleStream(IObjectStream<string> lineStream, bool splitHyphenatedTokens, bool safeParse) {
            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            adSentenceStream = new AdSentenceStream(lineStream, safeParse);
            this.splitHyphenatedTokens = splitHyphenatedTokens;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdNameSampleStream" /> from a <paramref name="inputStream" /> object.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding used to read the stream.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// or
        /// <paramref name="encoding"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The <paramref name="inputStream" /> does not support reading.</exception>
        public AdNameSampleStream(Stream inputStream, Encoding encoding, bool splitHyphenatedTokens, bool safeParse) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (!inputStream.CanRead)
                throw new ArgumentException("The input stream does not support reading.");

            adSentenceStream = new AdSentenceStream(new PlainTextByLineStream(inputStream, encoding), safeParse);
            this.splitHyphenatedTokens = splitHyphenatedTokens;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AdNameSampleStream"/> from a <paramref name="lineStream"/> object.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdNameSampleStream(Monitor monitor, IObjectStream<string> lineStream, bool splitHyphenatedTokens,
            bool safeParse)
            : this(lineStream, splitHyphenatedTokens, safeParse) {
            if (monitor == null)
                throw new ArgumentNullException("monitor");

            this.monitor = monitor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdNameSampleStream" /> from a <paramref name="inputStream" /> object.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding used to read the stream.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="inputStream"/> does not support reading.</exception>
        public AdNameSampleStream(Monitor monitor, Stream inputStream, Encoding encoding, bool splitHyphenatedTokens,
            bool safeParse)
            : this(inputStream, encoding, splitHyphenatedTokens, safeParse) {
            if (monitor == null)
                throw new ArgumentNullException("monitor");

            this.monitor = monitor;
        }

        #endregion

        #region . AddIfNotEmpty .
        private void AddIfNotEmpty(string firstTok, List<string> list) {
            if (!string.IsNullOrEmpty(firstTok)) {
                list.AddRange(ProcessTok(firstTok));
            }
        }
        #endregion

        #region . Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            adSentenceStream.Dispose();
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public NameSample Read() {
            AdSentence paragraph;

            if (monitor != null)
                monitor.Token.ThrowIfCancellationRequested();

            while ((paragraph = adSentenceStream.Read()) != null) {
                var clearData = false;

                var currentTextID = GetTextId(paragraph);
                if (currentTextID != textId) {
                    clearData = true;
                    textId = currentTextID;
                }

                var root = paragraph.Root;
                var sentence = new List<string>();
                var names = new List<Span>();

                Process(root, sentence, names);

                return new NameSample(sentence.ToArray(), names.ToArray(), clearData);
            }

            return null;
        }

        #endregion

        #region . GetNER .

        private static string GetNER(string tags) {
            if (tags.Contains("<NER2>")) {
                return null;
            }
            var tag = tags.RegExSplit(Expressions.Expression.Space);
            return (from t in tag
                select tagPattern.Match(t)
                into match
                where match.Success
                select match.Groups[2].Value
                into ner
                where mapNER.ContainsKey(ner)
                select mapNER[ner]).FirstOrDefault();
        }

        #endregion

        #region . GetTextId .

        private int GetTextId(AdSentence sentence) {
            if (corpusType == Type.None && !string.IsNullOrEmpty(sentence.Metadata)) {
                if (sentence.Metadata.StartsWith("LIT")) {
                    corpusType = Type.Lit;
                } else if (sentence.Metadata.StartsWith("CIE")) {
                    corpusType = Type.Cie;
                } else {
                    corpusType = Type.Ama;
                }
            }

            Match match;
            string text;
            switch (corpusType) {
                case Type.Ama:
                    match = AmaMetaRegex.Match(sentence.Metadata);
                    if (match.Success) {
                        return int.Parse(match.Groups[1].Value);
                    }
                    throw new InvalidFormatException("Invalid metadata: " + sentence.Metadata);
                case Type.Cie:
                    match = CieMetaRegex.Match(sentence.Metadata);

                    if (match.Success) {
                        text = match.Groups[1].Value;

                        if (text.Equals(textMeta2))
                            return textIdMeta2;

                        textIdMeta2++;
                        textMeta2 = text;

                        return textIdMeta2;
                    }

                    throw new InvalidFormatException("Invalid metadata: " + sentence.Metadata);
                case Type.Lit:
                    match = LitMetaRegex.Match(sentence.Metadata);

                    if (match.Success) {
                        text = match.Groups[1].Value;
                        if (textId == textIdMeta2)
                            return textIdMeta2;

                        textIdMeta2++;
                        textMeta2 = text;

                        return textIdMeta2;
                    }

                    throw new InvalidFormatException("Invalid metadata: " + sentence.Metadata);
                default:
                    return 0;
            }
        }

        #endregion

        #region . Process .

        /// <summary>
        /// Recursive method to process a node in Arvores Deitadas format.
        /// </summary>
        /// <param name="node">The node to be processed.</param>
        /// <param name="sentence">The sentence tokens we got so far.</param>
        /// <param name="names">The names we got so far</param>
        private void Process(AdNode node, List<string> sentence, List<Span> names) {
            if (node == null)
                return;

            foreach (var element in node.Elements) {
                if (element.IsLeaf) {
                    ProcessLeaf((AdLeaf) element, sentence, names);
                } else {
                    Process((AdNode) element, sentence, names);
                }
            }
        }

        #endregion

        #region . ProcessLeaf .

        /// <summary>
        /// Process a Leaf of Arvores Detaitadas format.
        /// </summary>
        /// <param name="leaf">The leaf to be processed.</param>
        /// <param name="sentence">The sentence tokens we got so far.</param>
        /// <param name="names">The names we got so far.</param>
        private void ProcessLeaf(AdLeaf leaf, List<string> sentence, List<Span> names) {
            if (leaf == null)
                return;

            var alreadyAdded = false;

            if (leftContractionPart != null) {
                // will handle the contraction

                var c = PortugueseContractionUtility.ToContraction(leftContractionPart, leaf.Lexeme);
                if (c != null) {
                    sentence.AddRange(c.RegExSplit(Expressions.Expression.Space));
                    alreadyAdded = true;
                } else {
                    // contraction was missing! why?
                    sentence.Add(leftContractionPart);
                    // keep alreadyAdded false.
                }
                leftContractionPart = null;
            }

            string namedEntityTag = null;
            var startOfNamedEntity = -1;

            var leafTag = leaf.SecondaryTag;
            var expandLastNER = false; // used when we find a <NER2> tag

            if (leafTag != null) {
                if (leafTag.Contains("<sam->") && !alreadyAdded) {
                    var lexemes = leaf.Lexeme.RegExSplit(Expressions.Expression.Underline);
                    if (lexemes.Length > 1) {
                        sentence.AddRange(lexemes.SubArray(0, lexemes.Length - 1));
                    }
                    leftContractionPart = lexemes[lexemes.Length - 1];
                    return;
                }
                if (leafTag.Contains("<NER2>")) {
                    // this one an be part of the last name
                    expandLastNER = true;
                }
                namedEntityTag = GetNER(leafTag);
            }

            if (namedEntityTag != null) {
                startOfNamedEntity = sentence.Count;
            }

            if (!alreadyAdded) {
                sentence.AddRange(ProcessLexeme(leaf.Lexeme));
            }

            if (namedEntityTag != null) {
                names.Add(new Span(startOfNamedEntity, sentence.Count, namedEntityTag));
            }

            if (expandLastNER) {
                // if the current leaf has the tag <NER2>, it can be the continuation of
                // a NER.
                // we check if it is true, and expand the last NER
                var lastIndex = names.Count - 1;
                var error = false;
                if (names.Count > 0) {
                    var last = names[lastIndex];
                    if (last.End == sentence.Count - 1) {
                        names[lastIndex] = new Span(last.Start, sentence.Count, last.Type);
                    } else {
                        error = true;
                    }
                } else {
                    error = true;
                }
                if (error) {
                    //           Maybe it is not the same NER, skip it.
                    //           System.err.println("Missing NER start for sentence [" + sentence
                    //           + "] node [" + leaf + "]");
                }
            }
        }

        #endregion

        #region . ProcessLexeme .
        private IEnumerable<string> ProcessLexeme(string lexemeStr) {
            var list = new List<string>();
            var parts = lexemeStr.RegExSplit(Expressions.Expression.Underline);
            foreach (var tok in parts) {
                if (tok.Length > 1 && !alphanumericPattern.IsMatch(tok)) {
                    list.AddRange(ProcessTok(tok));
                } else {
                    list.Add(tok);
                }
            }
            return list;
        }
        #endregion

        #region . ProcessTok .

        private IEnumerable<string> ProcessTok(string tok) {
            var tokAdded = false;
            var original = tok;
            var list = new List<string>();
            var suffix = new List<string>();
            var first = tok[0];
            if (first == '«') {
                list.Add(first.ToString(CultureInfo.InvariantCulture));
                tok = tok.Substring(1);
            }
            var last = tok[tok.Length - 1];
            if (last == '»' || last == ':' || last == ',' || last == '!') {
                suffix.Add(last.ToString(CultureInfo.InvariantCulture));
                tok = tok.Substring(0, tok.Length - 1);
            }

            // lets split all hyphens
            if (splitHyphenatedTokens && tok.Contains(Hyphen) && tok.Length > 1) {
                var match = hyphenPattern.Match(tok);

                string firstTok = null;
                
                string secondTok = null;
                string rest = null;

                if (match.Success) {
                    if (match.Groups[1].Success) {
                        firstTok = match.Groups[2].Value;
                    } else if (match.Groups[3].Success) {
                        secondTok = match.Groups[4].Value;
                        rest = match.Groups[5].Value;
                    } else if (match.Groups[6].Success) {
                        firstTok = match.Groups[7].Value;
                        secondTok = match.Groups[8].Value;
                        rest = match.Groups[9].Value;
                    }

                    AddIfNotEmpty(firstTok, list);
                    AddIfNotEmpty(Hyphen, list);
                    AddIfNotEmpty(secondTok, list);
                    AddIfNotEmpty(rest, list);
                    tokAdded = true;
                }
            }
            if (!tokAdded) {
                if (!original.Equals(tok) && tok.Length > 1
                    && !alphanumericPattern.IsMatch(tok)) {
                    list.AddRange(ProcessTok(tok));
                } else {
                    list.Add(tok);
                }
            }
            list.AddRange(suffix);
            return list;
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            if (adSentenceStream != null)
                adSentenceStream.Reset();
        }

        #endregion

        internal enum Type {
            None,
            Lit,
            Cie,
            Ama
        }
    }
}