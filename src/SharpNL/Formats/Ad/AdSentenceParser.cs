using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SharpNL.Formats.Ad {
    /// <summary>
    /// Parses a sample of AD corpus. A sentence in AD corpus is represented by a 
    /// Tree. In this class we declare some types to represent that tree. Today we get only
    /// the first alternative (A1).
    /// </summary>
    public class AdSentenceParser {

        private readonly static Regex nodePattern;
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
        /// </summary>
        /// <param name="sentenceString">The sentence string.</param>
        /// <param name="para">The para.</param>
        /// <param name="isTitle">if set to <c>true</c> [is title].</param>
        /// <param name="isBox">if set to <c>true</c> [is box].</param>
        /// <param name="safeParse">if set to <c>true</c> the bizarre cases are not parsed.</param>
        /// <returns>AdSentence.</returns>
        /// <exception cref="System.IO.InvalidDataException"></exception>
        public static AdSentence Parse(string sentenceString, int para, bool isTitle, bool isBox, bool safeParse) {

            string text = null;
            string meta = null;
            var sentence = new AdSentence();

            try {
                using (var reader = new StringReader(sentenceString)) {

                    // first line is <s ...>
                    string line = reader.ReadLine();

                    if (line == null) {
                        throw new InvalidDataException();
                    }

                    bool useSameTextAndMeta = false; // to handle cases where there are diff sug of parse (&&)

                    while (!line.StartsWith("SOURCE")) {
                        if (line.Equals("&&")) {
                            useSameTextAndMeta = true;
                            break;
                        }
                        line = reader.ReadLine();
                        if (line == null) {
                            return null;
                        }
                    }

                    if (!useSameTextAndMeta) {
                        string metaFromSource = line.Substring(7);

                        line = reader.ReadLine();

                        if (line == null) {
                            throw new InvalidDataException();
                        }

                        int start = line.IndexOf(" ", StringComparison.InvariantCulture);

                        text = FixPunctuation(line.Substring(start + 1).Trim());

                        if (start > 0) {
                            meta = line.Substring(0, start) + " p=" + para;
                            if (isTitle) {
                                meta += " title";
                            }
                            if (isBox) {
                                meta += " box";
                            }
                            meta += metaFromSource;
                        } else {
                            throw new Exception("Let me see!");
                            // rare case were there is no space between id and the sentence.
                            // will use previous meta for now
                        }
                    }
                    sentence.Text = text;
                    sentence.Metadata = meta;

                    // skip lines starting with ###
                    line = reader.ReadLine();
                    while (line != null && line.StartsWith("###")) {
                        line = reader.ReadLine();
                    }

                    var nodeStack = new List<AdNode>();

                    sentence.Root = new AdNode {
                        SyntacticTag = "ROOT",
                        Level = 0
                    };

                    nodeStack.Add(sentence.Root);

                    while (!string.IsNullOrEmpty(line) && !line.StartsWith("</s>") && !line.Equals("&&")) {
                        var element = ParseElement(line, !safeParse);

                        if (element != null) {
                            // The idea here is to keep a stack of nodes that are candidates for
                            // parenting the following elements (nodes and leafs).

                            // 1) When we get a new element, we check its level and remove from
                            // the top of the stack nodes that are brothers or nephews.
                            while (nodeStack.Count != 0 && element.Level > 0 && element.Level <= nodeStack[nodeStack.Count - 1].Level) {
                                nodeStack.RemoveAt(nodeStack.Count - 1); // pop
                            }

                            if (element.IsLeaf) {
                                // 2b) There are parent candidates.
                                // look for the node with the correct level

                                if (element.Level == 0) {
                                    sentence.Root.Elements.Add(element);
                                } else {
                                    var peek = nodeStack[nodeStack.Count - 1];
                                    var index = nodeStack.Count - 1;
                                    AdNode parent = null;
                                    while (parent == null) {
                                        if (peek.Level < element.Level) {
                                            parent = peek;
                                        }
                                        index--;
                                        if (index > -1) {
                                            peek = nodeStack[index];
                                        } else {
                                            parent = sentence.Root;
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
                        }
                        line = reader.ReadLine();
                    }
                }
            } catch (Exception ex) {
#if DEBUG
                throw new InvalidDataException("Something went wrong.", ex);
#else
                Trace.TraceError(ex.Message);
                return sentence;
#endif
            }

            return sentence;
        }

        /// <summary>
        /// Parse a tree element from a AD line
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="parseBizarre">Parse the bizarre elements.</param>
        /// <returns>AdTreeElement.</returns>
        private static AdTreeElement ParseElement(string line, bool parseBizarre) {

            Match m = nodePattern.Match(line);
            if (m.Success) {
                return new AdNode {
                    Level = m.Groups[1].Length + 1,
                    SyntacticTag = m.Groups[2].Value
                };
            }

            m = leafPattern.Match(line);
            if (m.Success) {
                return new AdLeaf {
                    Level = m.Groups[1].Length + 1,
                    SyntacticTag = m.Groups[2].Value,
                    FunctionalTag = m.Groups[3].Value,
                    Lemma = m.Groups[4].Value,
                    SecondaryTag = m.Groups[5].Value,
                    MorphologicalTag = m.Groups[6].Value,
                    Lexeme = m.Groups[7].Value
                };
            }

            m = punctuationPattern.Match(line);
            if (m.Success) {
                return new AdLeaf {
                    Level = m.Groups[1].Length + 1,
                    Lexeme = m.Groups[2].Value

                };
            }


            // TODO: Implement a tracking system for this weird AD elements.

            // process the bizarre cases.
            if (line.Equals("_") || line.StartsWith("<lixo") || line.StartsWith("pause")) {
                return null;
            }

            if (!parseBizarre) {
                Trace.TraceError("Couldn't parse leaf: {0}", line);
                return new AdLeaf {
                    Level = 1,
                    Lexeme = line
                };
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
                    return leaf;
                }

                int level = line.LastIndexOf("=", StringComparison.InvariantCulture) + 1;

                var leaf2 = new AdLeaf {
                    Level = level + 1,
                    Lexeme = line.Substring(level + 1)
                };

                if (Regex.IsMatch(leaf2.Lexeme, "\\w.*?[\\.<>].*")) {
                    return null;
                }

                return leaf2;
            }

            return null;
        }

        private static string FixPunctuation(string text) {
            return Regex.Replace(Regex.Replace(text, "\\»\\s+\\.", "»."), "\\»\\s+\\,", "»,");
        }
    }
}
