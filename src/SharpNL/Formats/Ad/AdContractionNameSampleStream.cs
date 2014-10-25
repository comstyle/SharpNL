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
using System.IO;
using System.Text;
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
    public class AdContractionNameSampleStream : IObjectStream<NameSample> {
        private readonly IObjectStream<AdSentence> adSentenceStream;
        private readonly Monitor monitor;
        private string leftContractionPart;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="AdContractionNameSampleStream"/> from a <paramref name="lineStream"/> object.
        /// </summary>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid Ad sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">lineStream</exception>
        public AdContractionNameSampleStream(IObjectStream<string> lineStream, bool safeParse) {
            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            adSentenceStream = new AdSentenceStream(lineStream, safeParse);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AdContractionNameSampleStream" /> from a <paramref name="inputStream"/> with the given <paramref name="encoding"/>.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid Ad sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream" />
        /// or
        /// <paramref name="encoding" />
        /// </exception>
        /// <exception cref="System.ArgumentException">The input stream does not support reading.</exception>
        public AdContractionNameSampleStream(Stream inputStream, Encoding encoding, bool safeParse) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (!inputStream.CanRead)
                throw new ArgumentException("The input stream does not support reading.");

            adSentenceStream = new AdSentenceStream(new PlainTextByLineStream(inputStream, encoding), safeParse);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdContractionNameSampleStream" /> from a <paramref name="lineStream" /> object.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="lineStream">The line stream.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid Ad sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="monitor"/>
        /// or
        /// <paramref name="lineStream"/>
        /// </exception>
        public AdContractionNameSampleStream(Monitor monitor, IObjectStream<string> lineStream, bool safeParse)
           : this(lineStream, safeParse)  {
           
            if (monitor == null)
                throw new ArgumentNullException("monitor");

            this.monitor = monitor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdContractionNameSampleStream" /> from a <paramref name="inputStream"/> with the given <paramref name="encoding"/>.
        /// </summary>
        /// <param name="monitor">The execution monitor.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid Ad sentences will be skipped.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="monitor"/>
        /// or
        /// <paramref name="inputStream" />
        /// or
        /// <paramref name="encoding" />
        /// </exception>
        /// <exception cref="System.ArgumentException">The input stream does not support reading.</exception>
        public AdContractionNameSampleStream(Monitor monitor, Stream inputStream, Encoding encoding, bool safeParse)
            : this(inputStream, encoding, safeParse) {

            if (monitor == null)
                throw new ArgumentNullException("monitor");

            this.monitor = monitor;
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
        /// <param name="names">The names we got so far.</param>
        private void Process(AdNode node, List<String> sentence, List<Span> names) {
            if (node == null) return;

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

        private void ProcessLeaf(AdLeaf leaf, List<String> sentence, List<Span> names) {
            if (leaf == null)
                return;

            if (leftContractionPart == null) {
                var leafTag = leaf.SecondaryTag;

                if (leafTag != null) {
                    if (leafTag.Contains("<sam->")) {
                        var lexemes = leaf.Lexeme.RegExSplit(Expressions.Expression.Underline);
                        if (lexemes.Length > 1) {
                            for (var i = 0; i < lexemes.Length - 1; i++) {
                                sentence.Add(lexemes[i]);

                                var expand = PortugueseContractionUtility.Expand(lexemes[i]);
                                if (expand == null) 
                                    continue;

                                var end = sentence.Count;
                                var start = end - 1;
                                var s = new Span(start, end, "default");
                                names.Add(s);
                            }
                        }
                        leftContractionPart = lexemes[lexemes.Length - 1];
                        return;
                    }
                }
                sentence.Add(leaf.Lexeme);
                return;
            }

            // will handle the contraction
            var tag = leaf.SecondaryTag;
            var right = leaf.Lexeme;
            if (tag != null && tag.Contains("<-sam>")) {
                var parts = leaf.Lexeme.RegExSplit(Expressions.Expression.Underline);
                if (parts != null) {
                    // try to join only the first
                    var c = PortugueseContractionUtility.ToContraction(leftContractionPart, parts[0]);

                    if (c != null) {
                        sentence.Add(c);
                        names.Add(new Span(sentence.Count - 1, sentence.Count, "default"));
                    }

                    for (var i = 1; i < parts.Length; i++) {
                        sentence.Add(parts[i]);
                    }
                } else {
                    right = leaf.Lexeme;
                    var c = PortugueseContractionUtility.ToContraction(leftContractionPart, right);

                    if (c != null) {
                        sentence.Add(c);
                        names.Add(new Span(sentence.Count - 1, sentence.Count, "default"));
                    } else {
                        if (monitor != null)
                            monitor.OnError("ContractionNameSample: Missing " + leftContractionPart + " + " + right);
                        sentence.Add(leftContractionPart);
                        sentence.Add(right);
                    }
                }
            } else {
                if (monitor != null)
                    monitor.OnError("ContractionNameSample: No match " + leftContractionPart + " + " + right);
            }
            leftContractionPart = null;
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
            while ((paragraph = adSentenceStream.Read()) != null) {

                if (monitor != null)
                    monitor.Token.ThrowIfCancellationRequested();

                var root = paragraph.Root;
                var sentence = new List<String>();
                var names = new List<Span>();

                Process(root, sentence, names);

                return new NameSample(sentence.ToArray(), names.ToArray(), true);
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