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
using System.Globalization;
using SharpNL.Extensions;
using SharpNL.Tokenize;
using SharpNL.Utility;
using System;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// This class reads the <see cref="TokenSample"/>s from the given <see cref="T:IObjectStream{string}"/>
    /// using floresta Sita(c)tica Arvores Deitadas corpus which can be used by the maxent library for training.
    /// </summary>
    public class AdTokenSampleStream : IObjectStream<TokenSample> {
        private const string Hyphen = "-";

        private readonly Monitor monitor;
        private readonly AdSentenceStream adSentenceStream;
        private readonly IDetokenizer detokenizer;
        private readonly bool splitHyphenatedTokens;
        private string leftContractionPart;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="AdTokenSampleStream" /> from a <paramref name="lineStream" /> object.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer used create the samples.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdTokenSampleStream(IObjectStream<string> lineStream, IDetokenizer detokenizer, bool splitHyphenatedTokens, bool safeParse) {
            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            if (detokenizer == null)
                throw new ArgumentNullException("detokenizer");

            adSentenceStream = new AdSentenceStream(lineStream, safeParse);
            this.detokenizer = detokenizer;
            this.splitHyphenatedTokens = splitHyphenatedTokens;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdTokenSampleStream"/> from a <paramref name="lineStream"/> object.
        /// </summary>
        /// <param name="monitor">The evaluation monitor.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="detokenizer">The detokenizer used create the samples.</param>
        /// <param name="splitHyphenatedTokens">if set to <c>true</c> hyphenated tokens will be separated: "carros-monstro" &gt; "carros" Hyphen "monstro".</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid data in the file will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdTokenSampleStream(Monitor monitor, IObjectStream<string> lineStream, IDetokenizer detokenizer, bool splitHyphenatedTokens,
            bool safeParse)
            : this(lineStream, detokenizer, splitHyphenatedTokens, safeParse) {
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

        #region . Process .
        /// <summary>
        /// Recursive method to process a node in Arvores Deitadas format.
        /// </summary>
        /// <param name="node">The node to be processed.</param>
        /// <param name="sentence">The sentence tokens we got so far.</param>
        private void Process(AdNode node, List<string> sentence) {
            if (node == null)
                return;

            foreach (var element in node.Elements) {
                if (element.IsLeaf) {
                    ProcessLeaf((AdLeaf)element, sentence);
                } else {
                    Process((AdNode)element, sentence);
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
        private void ProcessLeaf(AdLeaf leaf, List<string> sentence) {
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

                    if (monitor != null)
                        monitor.OnWarning(string.Format("Missing contraction for: {0} - {1}", leftContractionPart, leaf.Lexeme));

                    // keep alreadyAdded false.
                }
                leftContractionPart = null;
            }

            var leafTag = leaf.SecondaryTag;

            if (leafTag != null) {
                if (leafTag.Contains("<sam->") && !alreadyAdded) {
                    var lexemes = leaf.Lexeme.RegExSplit(Expressions.Expression.Underline);
                    if (lexemes.Length > 1) {
                        sentence.AddRange(lexemes.SubArray(0, lexemes.Length - 1));
                    }
                    leftContractionPart = lexemes[lexemes.Length - 1];
                    return;
                }
            }

            if (!alreadyAdded)
                sentence.AddRange(ProcessLexeme(leaf.Lexeme));
        }
        #endregion

        #region . ProcessLexeme .
        private IEnumerable<string> ProcessLexeme(string lexemeStr) {
            var list = new List<string>();
            var parts = lexemeStr.RegExSplit(Expressions.Expression.Underline);
            foreach (var tok in parts) {
                if (tok.Length > 1 && !AdNameSampleStream.alphanumericPattern.IsMatch(tok)) {
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
                var match = AdNameSampleStream.hyphenPattern.Match(tok);

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
                    && !AdNameSampleStream.alphanumericPattern.IsMatch(tok)) {
                    list.AddRange(ProcessTok(tok));
                } else {
                    list.Add(tok);
                }
            }
            list.AddRange(suffix);
            return list;
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
        public TokenSample Read() {
            AdSentence paragraph;

            if (monitor != null)
                monitor.Token.ThrowIfCancellationRequested();

            while ((paragraph = adSentenceStream.Read()) != null) {
                var root = paragraph.Root;
                var sentence = new List<string>();

                Process(root, sentence);

                return new TokenSample(detokenizer, sentence.ToArray());
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