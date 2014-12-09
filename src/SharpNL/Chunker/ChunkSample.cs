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
using System.Text;
using SharpNL.Extensions;
using SharpNL.Utility;

namespace SharpNL.Chunker {
    /// <summary>
    /// Represents chunks for a single unit of text.
    /// </summary>
    public class ChunkSample : IEquatable<ChunkSample> {
        private readonly ReadOnlyCollection<string> preds;
        private readonly ReadOnlyCollection<string> sentence;
        private readonly ReadOnlyCollection<string> tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkSample"/> class.
        /// </summary>
        /// <param name="sentence">The training sentence.</param>
        /// <param name="tags">The POS Tags for the sentence.</param>
        /// <param name="preds">The Chunk tags in B-* I-* notation.</param>
        /// <exception cref="System.ArgumentException">The arguments must have the same length.</exception>
        public ChunkSample(string[] sentence, string[] tags, string[] preds) {
            if (sentence.Length != tags.Length || tags.Length != preds.Length) {
                throw new ArgumentException("The arguments must have the same length.");
            }

            this.sentence = new ReadOnlyCollection<string>(sentence);
            this.tags = new ReadOnlyCollection<string>(tags);
            this.preds = new ReadOnlyCollection<string>(preds);
        }

        #region . Equals .

        /// <summary>
        /// Indicates whether the current chunk sample is equal to another chunk sample.
        /// </summary>
        /// <returns>
        /// true if the current chunk sample is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An chunk sample to compare with this chunk sample.</param>
        public bool Equals(ChunkSample other) {
            if (other == null) {
                return false;
            }

            return sentence.SequenceEqual(other.sentence) &&
                   tags.SequenceEqual(other.tags) &&
                   preds.SequenceEqual(other.preds);
        }

        #endregion

        #region . GetPhrasesAsSpanList .

        /// <summary>
        /// Gets the phrases as an array of spans
        /// </summary>
        /// <returns>The spans array.</returns>
        public Span[] GetPhrasesAsSpanList() {
            return PhrasesAsSpanList(sentence.ToArray(), tags.ToArray(), preds.ToArray());
        }

        #endregion

        #region . NicePrint .

        private string nicePrint;

        /// <summary>
        /// Gets a nice to read string for the phrases formatted as following: 
        /// <code>
        /// [NP Rockwell_NNP ] [VP said_VBD ] [NP the_DT agreement_NN ] [VP calls_VBZ ] [SBAR for_IN ] [NP it_PRP ] [VP to_TO supply_VB ] [NP 200_CD additional_JJ so-called_JJ shipsets_NNS ] [PP for_IN ] [NP the_DT planes_NNS ] ._.
        /// </code>
        /// </summary>
        /// <value>A nice to read string representation of the chunk phases.</value>
        public string NicePrint {
            get {
                if (nicePrint != null) {
                    return nicePrint;
                }

                var sb = new StringBuilder(" ");
                var spans = GetPhrasesAsSpanList();

                for (var tokenIndex = 0; tokenIndex < sentence.Count; tokenIndex++) {
                    for (var nameIndex = 0; nameIndex < spans.Length; nameIndex++) {
                        if (spans[nameIndex].Start == tokenIndex) {
                            sb.AppendFormat("[{0} ", spans[nameIndex].Type);
                        }

                        if (spans[nameIndex].End == tokenIndex) {
                            sb.Append("] ");
                        }
                    }
                    sb.AppendFormat("{0}_{1} ", sentence[tokenIndex], tags[tokenIndex]);
                }

                if (sentence.Count > 1) {
                    sb.Remove(sb.Length - 1, 1);
                }

                foreach (var span in spans) {
                    if (span.End == sentence.Count) {
                        sb.Append(']');
                    }
                }

                nicePrint = sb.ToString();
                return nicePrint;
            }
        }

        #endregion

        #region . PhrasesAsSpanList .

        /// <summary>
        /// Phrases as span list.
        /// </summary>
        /// <param name="sentence">The training sentence.</param>
        /// <param name="tags">POS Tags for the sentence.</param>
        /// <param name="preds">Chunk tags in B-* I-* notation.</param>
        /// <returns>The phrases as an array of spans.</returns>
        /// <exception cref="System.ArgumentException">The arguments must have the same length.</exception>
        public static Span[] PhrasesAsSpanList(string[] sentence, string[] tags, string[] preds) {
            if (sentence.Length != tags.Length || tags.Length != preds.Length) {
                throw new ArgumentException("The arguments must have the same length.");
            }

            var list = new List<Span>(sentence.Length);
            var startIndex = 0;
            var foundPhrase = false;
            var startTag = string.Empty;

            for (var i = 0; i < preds.Length; i++) {
                if (preds[i].StartsWith("B-") || (!preds[i].Equals("I-" + startTag) && !preds[i].Equals("O"))) {
                    if (foundPhrase) {
                        list.Add(new Span(startIndex, i, startTag));
                    }
                    startIndex = i;
                    startTag = preds[i].Substring(2);
                    foundPhrase = true;
                } else if (preds[i].Equals("I-" + startTag)) {
                    // middle
                    // do nothing
                } else if (foundPhrase) {
                    list.Add(new Span(startIndex, i, startTag));
                    foundPhrase = false;
                    startTag = "";
                }
            }
            if (foundPhrase) {
                list.Add(new Span(startIndex, preds.Length, startTag));
            }

            return list.ToArray();
        }

        #endregion

        #region . Preds .

        /// <summary>
        /// Gets the Chunk tags in B-* I-* notation.
        /// </summary>
        /// <value>The the Chunk tags in B-* I-* notation.</value>
        public ReadOnlyCollection<string> Preds {
            get { return preds; }
        }

        #endregion

        #region . Sentence .

        /// <summary>
        /// Gets the training sentence
        /// </summary>
        /// <value>The training sentence.</value>
        public ReadOnlyCollection<string> Sentence {
            get { return sentence; }
        }

        #endregion

        #region . Tags .

        /// <summary>
        /// Gets the POS Tags for the sentence.
        /// </summary>
        /// <value>The POS Tags for the sentence..</value>
        public ReadOnlyCollection<string> Tags {
            get { return tags; }
        }

        #endregion

        #region . ToString .

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            var sb = new StringBuilder();
            for (var i = 0; i < sentence.Count; i++) {
                sb.AppendFormat("{0} {1} {2}\n", sentence[i], tags[i], preds[i]);
            }
            return sb.ToString();
        }

        #endregion
    }
}