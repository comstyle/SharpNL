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
using System.Text.RegularExpressions;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Parses a sample of AD corpus. A sentence in AD corpus is represented by a 
    /// Tree. In this class we declare some types to represent that tree. Today we get only
    /// the first alternative (A1).
    /// </summary>
    public class AdSentenceParser {
        private static readonly Regex nodePattern;
        private static readonly Regex leafPattern;

        private static readonly Regex bizarreLeafPattern;
        private static readonly Regex punctuationPattern;

        static AdSentenceParser() {
            nodePattern = new Regex("([=-]*)([^:=]+:[^\\(\\s]+)(\\(([^\\)]+)\\))?\\s*(?:(\\((<.+>)\\))*)\\s*$", RegexOptions.Compiled);
            leafPattern = new Regex("^([=-]*)([^:=]+):([^\\(\\s]+)\\([\"'](.+)[\"']\\s*((?:<.+>)*)\\s*([^\\)]+)?\\)\\s+(.+)", RegexOptions.Compiled);
            bizarreLeafPattern = new Regex("^([=-]*)([^:=]+=[^\\(\\s]+)\\(([\"'].+[\"'])?\\s*([^\\)]+)?\\)\\s+(.+)", RegexOptions.Compiled);
            punctuationPattern = new Regex("^(=*)(\\W+)$", RegexOptions.Compiled);
        }

        /// <summary>
        /// Parses the specified sentence string.
        /// Converts the string representation of a sentence in a specified attributes and culture-specific
        /// format to its <see cref="AdSentence" /> equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// </summary>
        /// <param name="sentence">The sentence.</param>
        /// <param name="sentenceString">The sentence string.</param>
        /// <param name="para">The para.</param>
        /// <param name="isTitle">if set to <c>true</c> [is title].</param>
        /// <param name="isBox">if set to <c>true</c> [is box].</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid sentences will be ignored.</param>
        /// <param name="monitor">The evaluation monitor. This value can be a <c>null</c> value.</param>
        /// <returns><c>true</c> if the <paramref name="sentenceString"/> parameter was converted successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="System.IO.InvalidDataException">
        /// Something went wrong.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">Should not happen!</exception>
        public static bool TryParse(
            out AdSentence sentence,
            string sentenceString,
            int para,
            bool isTitle,
            bool isBox,
            bool safeParse,
            Monitor monitor) {
            string text = null;
            string meta = null;
            var sent = new AdSentence();

            try {
                using (var reader = new StringReader(sentenceString)) {
                    // first line is <s ...>
                    var line = reader.ReadLine();

                    if (line == null) {
                        sentence = null;
                        return false;
                    }

                    var useSameTextAndMeta = false; // to handle cases where there are diff sug of parse (&&)

                    while (!line.StartsWith("SOURCE")) {
                        if (line.Equals("&&")) {
                            useSameTextAndMeta = true;
                            break;
                        }
                        line = reader.ReadLine();

                        if (line == null) {
                            sentence = null;
                            return false;
                        }
                    }

                    if (!useSameTextAndMeta) {
                        var metaFromSource = line.Substring(7);

                        line = reader.ReadLine();

                        if (line == null) {
                            sentence = null;
                            return false;
                        }

                        var start = line.IndexOf(" ", StringComparison.InvariantCulture);

                        text = FixPunctuation(line.Substring(start + 1).Trim());

                        if (start > 0) {
                            meta = line.Substring(0, start) + " p=" + para;
                            if (isTitle)
                                meta += " title";

                            if (isBox)
                                meta += " box";

                            meta += metaFromSource;
                        } else {
                            // rare case were there is no space between id and the sentence.

                            if (monitor != null)
                                monitor.OnWarning("A sentence was skipped due a possible integrity loss.");

                            // The OpenNLP uses previous meta, but its better to just ignore the sentence
                            // since the previous meta its not related to the current.

                            sentence = null;
                            return false;                           
                        }
                    }
                    sent.Text = text;
                    sent.Metadata = meta;

                    // skip lines starting with ###
                    line = reader.ReadLine();
                    while (line != null && line.StartsWith("###")) {
                        line = reader.ReadLine();
                    }

                    var nodeStack = new List<AdNode>();

                    sent.Root = new AdNode {
                        SyntacticTag = "ROOT",
                        Level = 0
                    };

                    nodeStack.Add(sent.Root);

                    while (!string.IsNullOrEmpty(line) && !line.StartsWith("</s>") && !line.Equals("&&")) {
                        AdTreeElement element;

                        if (TryParseElement(out element, line, safeParse, monitor)) {
                            // The idea here is to keep a stack of nodes that are candidates for
                            // parenting the following elements (nodes and leafs).

                            // 1) When we get a new element, we check its level and remove from
                            // the top of the stack nodes that are brothers or nephews.
                            while (nodeStack.Count != 0 && element.Level > 0 &&
                                   element.Level <= nodeStack[nodeStack.Count - 1].Level) {
                                nodeStack.RemoveAt(nodeStack.Count - 1); // pop
                            }

                            if (element.IsLeaf) {
                                // 2b) There are parent candidates.
                                // look for the node with the correct level

                                if (element.Level == 0) {
                                    nodeStack[0].Elements.Add(element);
                                } else {
                                    var peek = nodeStack[nodeStack.Count - 1];
                                    var index = nodeStack.Count - 1;
                                    AdNode parent = null;
                                    while (parent == null) {
                                        if (peek.Level < element.Level) {
                                            parent = peek;
                                            break;
                                        }
                                        index--;
                                        if (index > -1) {
                                            peek = nodeStack[index];
                                        } else {
                                            parent = nodeStack[0];
                                        }
                                    }
                                    parent.AddElement(element);
                                }
                            } else {
                                // 3) Check if the element that is at the top of the stack is this
                                // node parent, if yes add it as a son

                                if (nodeStack.Count != 0 && nodeStack[nodeStack.Count - 1].Level < element.Level) {
                                    nodeStack[nodeStack.Count - 1].AddElement(element);
                                } else {
                                    throw new InvalidOperationException("Should not happen!");
                                }

                                nodeStack.Add((AdNode) element);
                            }
                        } else if (safeParse) {
                            // invalid element, so we skip this sentence...
                            sentence = null;
                            return false;
                        }
                        line = reader.ReadLine();
                    }
                }
            } catch (Exception ex) {
                if (monitor != null)
                    monitor.OnException(new InvalidDataException("Something went wrong during the AdSentence parse.", ex));

                sentence = null;
                return false;
            }

            sentence = sent;
            return true;
        }

        /// <summary>
        /// Converts the specified string representation of a tree element to its <see cref="AdTreeElement"/> 
        /// equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="element">
        /// When this method returns, contains the <see cref="AdTreeElement"/> value equivalent to the element 
        /// contained in <paramref name="line"/>, if the conversion succeeded, or <c>null</c> if the conversion 
        /// failed. The conversion fails if the <paramref name="line"/> parameter is null, is an empty string (""),
        /// or does not contain a valid string representation of a AdElement. This parameter is passed 
        /// uninitialized.
        /// </param>
        /// <param name="line">The string representation of the element.</param>
        /// <param name="safeParse">if set to <c>true</c> the invalid sentences will be ignored.</param>
        /// <param name="monitor">The evaluation monitor.</param>
        /// <returns><c>true</c> if the s parameter was converted successfully; otherwise, <c>false</c>.</returns>
        private static bool TryParseElement(out AdTreeElement element, string line, bool safeParse, Monitor monitor) {
            var m = nodePattern.Match(line);
            if (m.Success) {
                element = new AdNode {
                    Level = m.Groups[1].Length + 1,
                    SyntacticTag = m.Groups[2].Value
                };
                return true;
            }

            m = leafPattern.Match(line);
            if (m.Success) {
                element = new AdLeaf {
                    Level = m.Groups[1].Length + 1,
                    SyntacticTag = m.Groups[2].Value,
                    FunctionalTag = m.Groups[3].Value,
                    Lemma = m.Groups[4].Value,
                    SecondaryTag = m.Groups[5].Value,
                    MorphologicalTag = m.Groups[6].Value,
                    Lexeme = m.Groups[7].Value
                };
                return true;
            }

            m = punctuationPattern.Match(line);
            if (m.Success) {
                element = new AdLeaf {
                    Level = m.Groups[1].Length + 1,
                    Lexeme = m.Groups[2].Value
                };
                return true;
            }

            if (safeParse) {
                element = null;
                return false;
            }

            // Knuppe: The most bizarre cases I found, were invalid data (like HTML, inside the sentences)
            //         so I decided to implement the safeParse attribute, to ignore this junk...
            //
            //         I think any program should adapt to an error in a file. otherwise the files will never
            //         be fixed...                      

            // process the bizarre cases.
            if (line.Equals("_") || line.StartsWith("<lixo") || line.StartsWith("pause")) {
                element = null;
                return false;
            }

            if (line.StartsWith("=")) {
                m = bizarreLeafPattern.Match(line);
                if (m.Success) {
                    var leaf = new AdLeaf {
                        Level = m.Groups[1].Length + 1,
                        SyntacticTag = m.Groups[2].Value,
                        Lemma = m.Groups[3].Value,
                        MorphologicalTag = m.Groups[4].Value,
                        Lexeme = m.Groups[5].Value
                    };

                    if (!string.IsNullOrEmpty(leaf.Lemma) && leaf.Lemma.Length > 2) {
                        leaf.Lemma = leaf.Lemma.Substring(1);
                    }
                    element = leaf;
                    return true;
                }

                var level = line.LastIndexOf("=", StringComparison.InvariantCulture) + 1;
                if (level > 0 && level < line.Length - 2 && Regex.IsMatch(line.Substring(level + 1), "\\w.*?[\\.<>].*")) {
                    element = new AdLeaf {
                        Level = level + 1,
                        Lexeme = line.Substring(level + 1)
                    };
                    return true;
                }
            }

            if (monitor != null) {
                monitor.OnWarning("Couldn't parse leaf: " + line);
            }

            element = null;
            return false;
        }

        private static string FixPunctuation(string text) {
            return Regex.Replace(Regex.Replace(text, "\\»\\s+\\.", "»."), "\\»\\s+\\,", "»,");
        }
    }
}