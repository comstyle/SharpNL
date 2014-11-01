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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SharpNL.Java;
using SharpNL.POSTag;
using SharpNL.Utility;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// This class reads the <see cref="POSSample"/>s from the given <see cref="T:IObjectStream{string}"/>
    /// using floresta Sita(c)tica Arvores Deitadas corpus which can be used by the maxent library for training.
    /// </summary>
    public class AdPosSampleStream : IObjectStream<POSSample> {
        private const string hyphen = "-";
        private static readonly Regex genderM;
        private static readonly Regex genderF;
        //private static readonly Regex genderN;

        private static readonly Regex hyphenRegex;

        // this is used to control changing quotation mark representation, 
        // some sentences we keep as original, others we change to "
        private int callsCount;

        private readonly IObjectStream<AdSentence> adSentenceStream;
        private readonly bool expandMe;
        private readonly bool includeFeatures;
        private readonly bool additionalContext;

        #region + Constructors .

        /// <summary>
        /// Initializes static members of the <see cref="AdPosSampleStream"/> class.
        /// </summary>
        static AdPosSampleStream() {
            genderM = new Regex(".*\\bM\\b.*", RegexOptions.Compiled);
            genderF = new Regex(".*\\bF\\b.*", RegexOptions.Compiled);
            //genderN = new Regex(".*\\bM/F\\b.*", RegexOptions.Compiled);

            hyphenRegex = new Regex("((\\p{L}+)-$)|(^-(\\p{L}+)(.*))|((\\p{L}+)-(\\p{L}+)(.*))", RegexOptions.Compiled);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdPosSampleStream" /> from a <paramref name="lineStream"/> object.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="expandMe">if set to <c>true</c> will expand the multiword expressions, each word of the expression will have the POS Tag that was attributed to the expression plus the prefix B- or I- (CONLL convention).</param>
        /// <param name="includeFeatures">if set to <c>true</c> will combine the POS Tag with the feature tags.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid AD sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdPosSampleStream(IObjectStream<string> lineStream,
            bool expandMe,
            bool includeFeatures,
            bool safeParse) : this(lineStream, expandMe, includeFeatures, false, safeParse) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdPosSampleStream" /> from a <paramref name="lineStream"/> object.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="expandMe">if set to <c>true</c> will expand the multiword expressions, each word of the expression will have the POS Tag that was attributed to the expression plus the prefix B- or I- (CONLL convention).</param>
        /// <param name="includeFeatures">if set to <c>true</c> will combine the POS Tag with the feature tags.</param>
        /// <param name="additionalContext">if set to <c>true</c> the additional context will be included in the POS sample.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid AD sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdPosSampleStream(
            IObjectStream<string> lineStream, 
            bool expandMe, 
            bool includeFeatures,
            bool additionalContext, 
            bool safeParse) {

            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            adSentenceStream = new AdSentenceStream(lineStream, safeParse);

            this.expandMe = expandMe;
            this.includeFeatures = includeFeatures;
            this.additionalContext = additionalContext;
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

        #region . AddGender .

        private static string AddGender(string tag, string morphologicalTag) {
            if ((tag != "n" && tag != "art") || morphologicalTag == null)
                return tag;

            if (genderF.IsMatch(morphologicalTag))
                tag = tag + "f";
            else if (genderM.IsMatch(morphologicalTag))
                tag = tag + "m";

            return tag;
        }

        #endregion

        #region . Process .
        private void Process(AdNode node, List<String> sentence, List<String> tags, List<String> con, List<String> prop) {
            if (node == null) return;

            foreach (var element in node.Elements) {
                if (element.IsLeaf) {
                    ProcessLeaf((AdLeaf)element, sentence, tags, con, prop);
                } else {
                    Process((AdNode)element, sentence, tags, con, prop);
                }
            }
        }
        #endregion

        #region . ProcessLeaf .

        private void ProcessLeaf(AdLeaf leaf, List<string> sentence, List<string> tags, List<string> con, List<string> prop) {
            if (leaf == null) 
                return;

            var lexeme = leaf.Lexeme;

            // this will change half of the quotation marks 
            if ("«" == lexeme || "»" == lexeme) {
                if (callsCount%2 == 0) {
                    lexeme = "\"";
                }
            }
            var tag = leaf.FunctionalTag;

            string contraction = null;
            if (leaf.SecondaryTag != null) {
                if (leaf.SecondaryTag.Contains("<sam->")) {
                    contraction = "B";
                } else if (leaf.SecondaryTag.Contains("<-sam>")) {
                    contraction = "E";
                }
            }

            if (tag == null)
                tag = lexeme;

            if (includeFeatures && !string.IsNullOrEmpty(leaf.MorphologicalTag)) {
                tag += " " + leaf.MorphologicalTag;
            }

            tag = tag.RegExReplace(Expressions.Expression.Space, "=") ?? lexeme;
            //tag = tag.replaceAll("\\s+", "=");

            if (expandMe && lexeme.Contains("_")) {
                var tokenizer = new StringTokenizer(lexeme, "_");

                if (tag == "prop") {
                    sentence.Add(lexeme);
                    tags.Add(tag);
                    con.Add(null);
                    prop.Add("P");
                } else if (tokenizer.CountTokens > 0) {
                    var toks = new List<string>(tokenizer.CountTokens);
                    var tagsWithCont = new List<string>(tokenizer.CountTokens);
                    toks.Add(tokenizer.NextToken);
                    tagsWithCont.Add("B-" + tag);
                    while (tokenizer.HasMoreTokens) {
                        toks.Add(tokenizer.NextToken);
                        tagsWithCont.Add("I-" + tag);
                    }
                    if (contraction != null) {
                        con.AddRange(new string[toks.Count - 1]);
                        con.Add(contraction);
                    } else {
                        con.AddRange(new string[toks.Count]);
                    }

                    sentence.AddRange(toks);
                    tags.AddRange(tagsWithCont);
                    prop.AddRange(new string[toks.Count]);
                } else {
                    sentence.Add(lexeme);
                    tags.Add(tag);
                    prop.Add(null);
                    con.Add(contraction);
                }
            } else if (lexeme.Contains(hyphen) && lexeme.Length > 1) {
                string firstTok = null;

                string secondTok = null;
                string rest = null;

                var match = hyphenRegex.Match(lexeme);

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
                    } else {
                        throw new InvalidFormatException("Wrong hyphen pattern.");
                    }

                    if (!string.IsNullOrEmpty(firstTok)) {
                        sentence.Add(firstTok);
                        tags.Add(tag);
                        prop.Add(null);
                        con.Add(contraction);
                    }
                    if (!string.IsNullOrEmpty(hyphen)) {
                        sentence.Add(hyphen);
                        tags.Add(hyphen);
                        prop.Add(null);
                        con.Add(contraction);
                    }
                    if (!string.IsNullOrEmpty(secondTok)) {
                        sentence.Add(secondTok);
                        tags.Add(tag);
                        prop.Add(null);
                        con.Add(contraction);
                    }
                    if (!string.IsNullOrEmpty(rest)) {
                        sentence.Add(rest);
                        tags.Add(tag);
                        prop.Add(null);
                        con.Add(contraction);
                    }
                } else {
                    sentence.Add(lexeme);
                    tags.Add(tag);
                    prop.Add(null);
                    con.Add(contraction);
                }
            } else {
                tag = AddGender(tag, leaf.MorphologicalTag);

                sentence.Add(lexeme);
                tags.Add(tag);
                prop.Add(null);
                con.Add(contraction);
            }
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
        public POSSample Read() {
            callsCount++;

            AdSentence paragraph;

            while ((paragraph = adSentenceStream.Read()) != null) {
                var root = paragraph.Root;
                var sentence = new List<String>();
                var tags = new List<String>();
                var contractions = new List<String>();
                var prop = new List<String>();
                Process(root, sentence, tags, contractions, prop);

                if (sentence.Count != contractions.Count || sentence.Count != prop.Count)
                    throw new InvalidOperationException("The processed information must have the same length.");

                if (additionalContext) {
                    //String[][] ac = new String[2][sentence.size()];

                    var ac = new string[2][];

                    ac[0] = new string[sentence.Count];
                    ac[1] = new string[sentence.Count];

                    // line 0: contractions
                    // line 1: props
                    for (var i = 0; i < sentence.Count; i++) {
                        if (contractions[i] != null) {
                            ac[0][i] = contractions[i];
                        }
                        if (prop[i] != null) {
                            ac[1][i] = prop[i];
                        }
                    }

                    return new POSSample(sentence.ToArray(), tags.ToArray(), ac);
                }
                return new POSSample(sentence.ToArray(), tags.ToArray());
            }
            return null;
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            adSentenceStream.Reset();
        }

        #endregion

    }
}