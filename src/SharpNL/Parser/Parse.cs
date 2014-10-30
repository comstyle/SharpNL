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
using System.Text;
using System.Text.RegularExpressions;
using SharpNL.Utility;

namespace SharpNL.Parser {
    /// <summary>
    /// Data structure for holding parse constituents.
    /// </summary>
    public class Parse : ICloneable, IComparable<Parse> {
        private const string BracketLRB = "(";
        private const string BracketRRB = ")";
        private const string BracketLCB = "{";
        private const string BracketRCB = "}";
        private const string BracketLSB = "[";
        private const string BracketRSB = "]";

        /// <summary>
        /// The pattern used to find the base constituent label of a Penn Treebank labeled constituent.
        /// </summary>
        private static readonly Regex typePattern;

        /// <summary>
        /// The pattern used to find the function tags.
        /// </summary>
        private static readonly Regex funTypePattern;

        /// <summary>
        /// The patter used to identify tokens in Penn Treebank labeled constituents.
        /// </summary>
        private static readonly Regex tokenPattern;


        private List<Parse> parts;

        #region + Constructors .

        /// <summary>
        /// Initializes static members of the <see cref="Parse"/>.
        /// </summary>
        static Parse() {
            typePattern = new Regex("^([^ =-]+)", RegexOptions.Compiled);

            funTypePattern = new Regex("^[^ =-]+-([^ =-]+)", RegexOptions.Compiled);

            tokenPattern = new Regex("^[^ ()]+ ([^ ()]+)\\s*\\)", RegexOptions.Compiled);
        }

        /// <summary>
        /// Creates a new parse node for this specified text and span of the specified type
        /// with the specified probability and the specified head index.
        /// </summary>
        /// <param name="text">The text of the sentence for which this node is a part of.</param>
        /// <param name="span">The character offsets for this node within the specified text.</param>
        /// <param name="type">The constituent label of this node.</param>
        /// <param name="probability">The probability of this parse.</param>
        /// <param name="headIndex">The token index of the head of this parse.</param>
        public Parse(string text, Span span, string type, double probability, int headIndex) {
            Text = text;
            Span = span;
            Type = type;
            Head = this;
            Probability = probability;
            HeadIndex = headIndex;
            parts = new List<Parse>();
        }

        /// <summary>
        /// Creates a new parse node for this specified text and span of the specified type 
        /// with the specified probability and the specified head and head index.
        /// </summary>
        /// <param name="text">The text of the sentence for which this node is a part of.</param>
        /// <param name="span">The character offsets for this node within the specified text.</param>
        /// <param name="type">The constituent label of this node.</param>
        /// <param name="probability">The probability of this parse.</param>
        /// <param name="head">The head token of this parse.</param>
        public Parse(string text, Span span, string type, double probability, Parse head)
            : this(text, span, type, probability, 0) {
            if (head == null) 
                return;

            Head = head;
            HeadIndex = head.HeadIndex;
        }

        #endregion

        #region + Properties .

        #region . ChildCount .
        /// <summary>
        /// Gets the number of children for this parse node.
        /// </summary>
        /// <value>The number of children for this parse node.</value>
        public int ChildCount {
            get { return parts.Count; }
        }
        #endregion

        #region . Children .

        /// <summary>
        /// Gets the children constituents.
        /// </summary>
        /// <value>The children constituents.</value>
        public Parse[] Children {
            get { return parts.ToArray(); }
        }

        #endregion

        #region . Complete .

        /// <summary>
        /// Gets a value indicating whether this <see cref="Parse"/> is complete.
        /// </summary>
        /// <value><c>true</c> if the parse contains a single top-most node; otherwise, <c>false</c>.</value>
        public bool Complete {
            get { return parts.Count == 1; }
        }

        #endregion

        #region . CoveredText .

        /// <summary>
        /// Gets the covered text by the current span.
        /// </summary>
        /// <value>The covered text by the current span.</value>
        public string CoveredText {
            get { return Span.GetCoveredText(Text); }
        }

        #endregion

        #region . Derivation .

        /// <summary>
        /// Gets or sets the derivation string for this parse if one has been created.
        /// </summary>
        /// <value>The derivation string for this parse or null if no derivation string has been created.</value>
        public StringBuilder Derivation { get; set; }

        #endregion

        #region . Head .

        /// <summary>
        /// Gets the head constituent associated with this constituent.
        /// </summary>
        /// <value>The head constituent associated with this constituent.</value>
        public Parse Head { get; private set; }

        #endregion

        #region . HeadIndex .

        /// <summary>
        /// Gets the index within a sentence of the head token for this parse.
        /// </summary>
        /// <value>The index within a sentence of the head token for this parse.</value>
        public int HeadIndex { get; private set; }

        #endregion

        #region . IsChunk .

        /// <summary>
        /// Gets or sets a value indicating whether this instance is chunk.
        /// </summary>
        /// <value><c>true</c> if this instance is chunk; otherwise, <c>false</c>.</value>
        public bool IsChunk { get; set; }

        #endregion

        #region . IsFlat .

        /// <summary>
        /// Gets a value indicating whether this constituent contains no sub-constituents.
        /// </summary>
        /// <value><c>true</c> if this constituent contains no sub-constituents; otherwise, <c>false</c>.</value>
        public bool IsFlat {
            get {
                var flat = true;
                for (var ci = 0; ci < parts.Count; ci++) {
                    flat &= (parts[ci]).IsPosTag;
                }
                return flat;
            }
        }

        #endregion

        #region . IsPosTag .

        /// <summary>
        /// Gets a value indicating whether this instance is position tag.
        /// </summary>
        /// <value><c>true</c> if this instance is position tag; otherwise, <c>false</c>.</value>
        public bool IsPosTag {
            get {
                return parts.Count == 1 &&
                       parts[0].Type == AbstractBottomUpParser.TOK_NODE;
            }
        }

        #endregion

        #region . Label .

        /// <summary>
        /// Gets or sets the label assigned to this parse node during parsing
        /// which specifies how this node will be formed into a constituent.
        /// </summary>
        /// <value>The outcome label assigned to this node during parsing.</value>
        public string Label { get; set; }

        #endregion

        #region . NextPunctuationSet .

        /// <summary>
        /// Gets or sets the set of punctuation tags which follow this parse.
        /// </summary>
        /// <value>The set of punctuation tags which follow this parse.</value>
        public SortedSet<Parse> NextPunctuationSet { get; set; }

        #endregion

        #region . Parent .

        /// <summary>
        /// Gets or sets the parent parse node for this constituent.
        /// </summary>
        /// <value>The parent parse node for this constituent.</value>
        public Parse Parent { get; set; }

        #endregion

        #region . PreviousPunctuationSet .

        /// <summary>
        /// Gets or sets the set of punctuation parses that occur immediately before this parse.
        /// </summary>
        /// <value>The set of punctuation parses that occur immediately before this parse.</value>
        public SortedSet<Parse> PreviousPunctuationSet { get; set; }

        #endregion

        #region . Probability .

        /// <summary>
        /// Gets the log of the product of the probability associated with all the decisions which formed this constituent.
        /// </summary>
        /// <value>The log of the product of the probability associated with all the decisions which formed this constituent.</value>
        public double Probability { get; private set; }

        #endregion

        #region . Span .

        /// <summary>
        /// Gets the character offsets for this constituent.
        /// </summary>
        /// <value>The the character offsets for this constituent.</value>
        public Span Span { get; private set; }

        #endregion

        #region . Text .

        /// <summary>
        /// Gets the text of the sentence over which this parse was formed.
        /// </summary>
        /// <value>The text of the sentence over which this parse was formed.</value>
        public string Text { get; private set; }

        #endregion

        #region . this .
        /// <summary>
        /// Gets the child <see cref="Parse"/> at the specified index.
        /// </summary>
        /// <param name="index">The child index.</param>
        /// <returns>The child <see cref="Parse"/> object.</returns>
        public Parse this[int index] {
            get {
                if (index < 0 || index > parts.Count)
                    return null;

                return parts[index];
            }
        }
        #endregion

        #region . Type .

        /// <summary>
        /// Gets or sets the type of this constituent to the specified type.
        /// </summary>
        /// <value>The type of this constituent to the specified type.</value>
        public string Type { get; set; }

        #endregion

        #region . UseFunctionTags .
        /// <summary>
        /// Specifies whether constituent labels should include parts specified after minus character.
        /// </summary>
        /// <value><c>true</c> if they should be included; otherwise, <c>false</c>.</value>
        public static bool UseFunctionTags { get; set; }
        #endregion

        #endregion

        #region . Add .
        public void Add(Parse daughter, IHeadRules rules) {
            if (daughter.PreviousPunctuationSet != null) {
                parts.AddRange(daughter.PreviousPunctuationSet);
            }
            parts.Add(daughter);
            Span = new Span(Span.Start, daughter.Span.End);
            Head = rules.GetHead(Children, Type);
            if (Head == null) {
                throw new ArgumentNullException();
            }
            HeadIndex = Head.HeadIndex;
        }
        #endregion

        #region . AddNames .

        /// <summary>
        /// Adds named entities.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="names">The names.</param>
        /// <param name="tokens">The tokens.</param>
        public static void AddNames(string tag, Span[] names, Parse[] tokens) {
            for (int ni = 0, nn = names.Length; ni < nn; ni++) {
                var nameTokenSpan = names[ni];
                var startToken = tokens[nameTokenSpan.Start];
                var endToken = tokens[nameTokenSpan.End - 1];
                var commonParent = startToken.GetCommonParent(endToken);
                //System.err.println("addNames: "+startToken+" .. "+endToken+" commonParent = "+commonParent);
                if (commonParent != null) {
                    var nameSpan = new Span(startToken.Span.Start, endToken.Span.End);
                    if (nameSpan.Equals(commonParent.Span)) {
                        commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0, endToken.HeadIndex));
                    } else {
                        var kids = commonParent.Children;
                        var crossingKids = false;
                        for (int ki = 0, kn = kids.Length; ki < kn; ki++) {
                            if (nameSpan.Crosses(kids[ki].Span)) {
                                crossingKids = true;
                            }
                        }
                        if (!crossingKids) {
                            commonParent.Insert(new Parse(commonParent.Text, nameSpan, tag, 1.0, endToken.HeadIndex));
                        } else {
                            if (commonParent.Type.Equals("NP")) {
                                var grandKids = kids[0].Children;
                                if (grandKids.Length > 1 && nameSpan.Contains(grandKids[grandKids.Length - 1].Span)) {
                                    commonParent.Insert(new Parse(commonParent.Text, commonParent.Span, tag, 1.0,
                                        commonParent.HeadIndex));
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region . AddPreviousPunctuation .
        /// <summary>
        /// Designates that the specified punctuation should is prior to this parse.
        /// </summary>
        /// <param name="punct">The punctuation.</param>
        public void AddPreviousPunctuation(Parse punct) {
            if (PreviousPunctuationSet == null) {
                PreviousPunctuationSet = new SortedSet<Parse>();
            }
            PreviousPunctuationSet.Add(punct);
        }
        #endregion

        #region . AddProbability .

        /// <summary>
        /// Adds the specified probability log to this current log for this parse.
        /// </summary>
        /// <param name="logProb">The probability of an action performed on this parse.</param>
        public void AddProbability(double logProb) {
            Probability += logProb;
        }

        #endregion

        #region . AddNextPunctuation .

        /// <summary>
        /// Designates that the specified punctuation follows this parse.
        /// </summary>
        /// <param name="punctuation">The punctuation set.</param>
        public void AddNextPunctuation(Parse punctuation) {
            if (NextPunctuationSet == null)
                NextPunctuationSet = new SortedSet<Parse>();

            NextPunctuationSet.Add(punctuation);
        }

        #endregion

        #region . AdJoin .
        /// <summary>
        /// Sister adjoins this node's last child and the specified sister node and returns their
        /// new parent node. The new parent node replace this nodes last child.
        /// </summary>
        /// <param name="sister">The node to be adjoined.</param>
        /// <param name="rules">The head rules for the parser.</param>
        /// <returns>The new parent node of this node and the specified sister node.</returns>
        public Parse AdJoin(Parse sister, IHeadRules rules) {
            var lastChild = parts[parts.Count - 1];
            var adjNode = new Parse(Text, new Span(lastChild.Span.Start, sister.Span.End), lastChild.Type, 1, rules.GetHead(new[] {lastChild, sister}, lastChild.Type));

            adjNode.parts.Add(lastChild);
            if (sister.PreviousPunctuationSet != null) {
                adjNode.parts.AddRange(sister.PreviousPunctuationSet);
            }
            adjNode.parts.Add(sister);
            parts[parts.Count - 1] = adjNode;
            Span = new Span(Span.Start, sister.Span.End);
            Head = rules.GetHead(Children, Type);
            HeadIndex = Head.HeadIndex;
            return adjNode;
        }
        #endregion

        #region . AdJoinRoot .

        public Parse AdJoinRoot(Parse node, AbstractHeadRules rules, int parseIndex) {
            var lastChild = parts[parseIndex];
            var adjNode = new Parse(Text, new Span(lastChild.Span.Start, node.Span.End), lastChild.Type, 1, rules.GetHead(new[] { lastChild, node }, lastChild.Type));
            adjNode.parts.Add(lastChild);
            if (node.PreviousPunctuationSet != null) {
                adjNode.parts.AddRange(node.PreviousPunctuationSet);
            }
            adjNode.parts.Add(node);
            parts[parseIndex] = adjNode;
            return adjNode;
        }

        #endregion

        #region + Clone .

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone() {
            var p = new Parse(Text, Span, Type, Probability, Head) {
                Label = Label
            };

            p.parts.AddRange(parts);

            if (Derivation != null)
                p.Derivation = new StringBuilder(Derivation.ToString());

            return p;
        }

        /// <summary>
        /// Clones the right frontier of parse up to the specified node.
        /// </summary>
        /// <param name="node">The last node in the right frontier of the parse tree which should be cloned.</param>
        /// <returns>A clone of this parse and its right frontier up to and including the specified node.</returns>
        public Parse Clone(Parse node) {
            if (Equals(this, node)) {
                return (Parse) Clone();
            }

            var c = (Parse) Clone();
            var l = c.parts[parts.Count - 1];
            c.parts[parts.Count] = l.Clone(node);
            return c;
        }

        #endregion

        #region . CloneRoot .

        /// <summary>
        /// Clones the right frontier of this root parse up to and including the specified node.
        /// </summary>
        /// <param name="node">The last node in the right frontier of the parse tree which should be cloned.</param>
        /// <param name="parseIndex">The child index of the parse for this root node.</param>
        /// <returns>A clone of this root parse and its right frontier up to and including the specified node.</returns>
        public Parse CloneRoot(Parse node, int parseIndex) {
            var c = (Parse) Clone();
            var fc = c.parts[parseIndex];
            c.parts[parseIndex] = fc.Clone(node);
            return c;
        }

        #endregion

        #region . CodeTree .

        private static void CodeTree(StringBuilder sb, Parse p, int[] levels) {
            var kids = p.Children;
            var levelsBuff = new StringBuilder();
            levelsBuff.Append("[");
            var n = new int[levels.Length + 1];
            for (var li = 0; li < levels.Length; li++) {
                n[li] = levels[li];
                levelsBuff.Append(levels[li]).Append(".");
            }
            for (var ki = 0; ki < kids.Length; ki++) {
                n[levels.Length] = ki;

                sb.AppendFormat("{0}{1}] {2} {3} -> {4} {5} -> {6}",
                    levelsBuff,
                    ki,
                    kids[ki].Type,
                    kids[ki].GetHashCode(),
                    kids[ki].Parent.GetHashCode(),
                    kids[ki].Parent.Type,
                    kids[ki].CoveredText);

                CodeTree(sb, kids[ki], n);
            }
        }

        #endregion

        #region . CompareTo .

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Parse other) {
            if (Probability > other.Probability)
                return -1;

            if (Probability < other.Probability)
                return 1;

            return 0;
        }

        #endregion

        #region . DecodeToken .

        private static string DecodeToken(string token) {
            switch (token) {
                case "-LRB-":
                    return BracketLRB;
                case "-RRB-":
                    return BracketRRB;
                case "-LCB-":
                    return BracketLCB;
                case "-RCB-":
                    return BracketRCB;
                case "-LSB-":
                    return BracketLSB;
                case "-RSB-":
                    return BracketRSB;
                default:
                    return token;
            }
        }

        #endregion

        #region . EncodeToken .

        private static string EncodeToken(string token) {
            switch (token) {
                case BracketLRB:
                    return "-LRB-";
                case BracketRRB:
                    return "-RRB-";
                case BracketLCB:
                    return "-LCB-";
                case BracketRCB:
                    return "-RCB-";
                case BracketLSB:
                    return "-LSB-";
                case BracketRSB:
                    return "-RSB-";
                default:
                    return token;
            }
        }

        #endregion

        #region . Equals .

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            var p = obj as Parse;
            if (p != null) {
                if (Label == null && p.Label != null)
                    return false;

                if (Label != p.Label)
                    return false;

                if (!Span.Equals(p.Span))
                    return false;

                if (!Text.Equals(p.Text))
                    return false;

                if (parts.Count != p.parts.Count)
                    return false;

                for (var i = 0; i < parts.Count; i++) {
                    if (!parts[i].Equals(p.parts[i]))
                        return false;
                }

                return true;
            }
            return false;
        }

        #endregion

        #region . ExpandTopNode .

        public void ExpandTopNode(Parse root) {
            bool beforeRoot = true;
            for (int pi = 0, ai = 0; pi < parts.Count; pi++, ai++) {
                Parse node = parts[pi];
                if (Equals(node, root)) {
                    beforeRoot = false;
                } else if (beforeRoot) {
                    root.parts[ai] = node;
                    parts.RemoveAt(pi);
                    pi--;
                } else {
                    root.parts.Add(node);
                    parts.RemoveAt(pi);
                    pi--;
                }
            }
            root.UpdateSpan();
        }

        #endregion

        #region . FixPossessives .

        public static void FixPossessives(Parse parse) {
            var tags = parse.GetTagNodes();
            for (var ti = 0; ti < tags.Length; ti++) {
                if (tags[ti].Type == "POS") {
                    if (ti + 1 < tags.Length && Equals(tags[ti + 1].Parent, tags[ti].Parent.Parent)) {
                        var start = tags[ti + 1].Span.Start;
                        var end = tags[ti + 1].Span.End;
                        for (var npi = ti + 2; npi < tags.Length; npi++) {
                            if (Equals(tags[npi].Parent, tags[npi - 1].Parent)) {
                                end = tags[npi].Span.End;
                            } else {
                                break;
                            }
                        }
                        var npPos = new Parse(parse.Text, new Span(start, end), "NP", 1, tags[ti + 1]);
                        parse.Insert(npPos);
                    }
                }
            }
        }

        #endregion

        #region . GetCodeTree .

        /// <summary>
        /// Gets a representation of the specified parse which contains hash codes so 
        /// that parent/child relationships can be explicitly seen.
        /// </summary>
        /// <param name="parse">The parse.</param>
        /// <returns>A representation of the specified parse.</returns>
        public static StringBuilder GetCodeTree(Parse parse) {
            var sb = new StringBuilder();
            CodeTree(sb, parse, new int[0]);

            return sb;
        }

        #endregion

        #region . GetCommonParent .

        /// <summary>
        /// Gets the deepest shared parent of this node and the specified node.
        /// If the nodes are identical then their parent is returned.
        /// If one node is the parent of the other then the parent node is returned.
        /// </summary>
        /// <param name="node">The node from which parents are compared to this node's parents.</param>
        /// <returns>The deepest shared parent of this node and the specified node.</returns>
        public Parse GetCommonParent(Parse node) {
            if (Equals(this, node)) {
                return Parent;
            }

            var parents = new HashSet<Parse>();
            var cparent = this;
            while (cparent != null) {
                parents.Add(cparent);
                cparent = cparent.Parent;
            }
            while (node != null) {
                if (parents.Contains(node)) {
                    return node;
                }
                node = node.Parent;
            }
            return null;
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:Parse"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return ((Span != null ? Span.GetHashCode() : 0)*397) ^
                        (Text != null ? Text.GetHashCode() : 0);
            }
        }

        #endregion

        #region . GetType .

        private static string GetType(string rest) {
            if (rest.StartsWith("-LCB-"))
                return "-LCB-";

            if (rest.StartsWith("-RCB-"))
                return "-RCB-";

            if (rest.StartsWith("-LRB-"))
                return "-LRB-";

            if (rest.StartsWith("-RRB-"))
                return "-RRB-";

            if (rest.StartsWith("-RSB-"))
                return "-RSB-";

            if (rest.StartsWith("-LSB-"))
                return "-LSB-";

            if (rest.StartsWith("-NONE-"))
                return "-NONE-";

            var typeMatcher = typePattern.Match(rest);
            if (typeMatcher.Success) {
                var type = typeMatcher.Groups[1].Value;
                if (UseFunctionTags) {
                    var funMatcher = funTypePattern.Match(rest);
                    if (funMatcher.Success) {
                        var ftag = funMatcher.Groups[1].Value;
                        type = type + "-" + ftag;
                    }
                }
                return type;
            }
            return null;
        }

        #endregion

        #region . GetTagNodes .

        /// <summary>
        /// Gets the parse nodes which are children of this node and which are pos tags.
        /// </summary>
        /// <returns>The parse nodes which are children of this node and which are pos tags.</returns>
        public Parse[] GetTagNodes() {
            var tags = new List<Parse>();
            var nodes = new List<Parse>();
            nodes.AddRange(parts);
            while (nodes.Count != 0) {
                var p = nodes[0];
                nodes.RemoveAt(0);
                if (p.IsPosTag) {
                    tags.Add(p);
                } else {
                    nodes.InsertRange(0, p.parts);
                }
            }
            return tags.ToArray();
        }

        #endregion

        #region . GetToken .

        /// <summary>
        /// Returns the string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </summary>
        /// <param name="rest">The portion of the parse string remaining to be processed.</param>
        /// <returns>
        /// The string containing the token for the specified portion of the parse string or
        /// null if the portion of the parse string does not represent a token.
        /// </returns>
        private static string GetToken(string rest) {
            var tokenMatcher = tokenPattern.Match(rest);
            if (tokenMatcher.Success) {
                return DecodeToken(tokenMatcher.Groups[1].Value);
            }
            return null;
        }

        #endregion

        #region . Insert .

        /// <summary>
        /// Inserts the specified constituent into this parse based on its text span. 
        /// This method assumes that the specified constituent can be inserted into this parse.
        /// </summary>
        /// <param name="constituent">The constituent to be inserted.</param>
        /// <exception cref="System.InvalidOperationException">Inserting constituent not contained in the sentence!</exception>
        public void Insert(Parse constituent) {
            var ic = constituent.Span;

            if (Span.Contains(ic)) {
                var pi = 0;
                var pn = parts.Count;
                for (; pi < pn; pi++) {
                    var subPart = parts[pi];
                    var sp = subPart.Span;

                    if (sp.Start >= ic.End)
                        break;

                    if (ic.Contains(sp)) {
                        parts.RemoveAt(pi);
                        pi--;
                        constituent.parts.Add(subPart);
                        subPart.Parent = constituent;

                        pn = parts.Count;
                        continue;
                    }

                    if (!sp.Contains(ic)) 
                        continue;

                    subPart.Insert(constituent);
                    return;
                }
                parts.Insert(pi, constituent);
                constituent.Parent = this;
            } else {
                throw new InvalidOperationException("Inserting constituent not contained in the sentence!");
            }
        }

        #endregion

        #region . ParseParse .

        /// <summary>
        /// Parses the specified tree-bank style parse string and return a Parse structure for that string.
        /// </summary>
        /// <param name="parse">A tree-bank style parse string.</param>
        /// <returns>A <see cref="Parse"/> structure for the specified tree-bank style parse string.</returns>
        public static Parse ParseParse(string parse) {
            return ParseParse(parse, null);
        }

        /// <summary>
        /// Parses the specified tree-bank style parse string and return a Parse structure for that string.
        /// </summary>
        /// <param name="parse">A tree-bank style parse string.</param>
        /// <param name="gl">The gap labeler.</param>
        /// <returns>A <see cref="Parse"/> structure for the specified tree-bank style parse string.</returns>
        public static Parse ParseParse(string parse, IGapLabeler gl) {
            var text = new StringBuilder();
            var offset = 0;
            var stack = new Stack<Constituent>();
            var cons = new List<Constituent>();
            for (int ci = 0, cl = parse.Length; ci < cl; ci++) {
                var c = parse[ci];
                if (c == '(') {
                    var rest = parse.Substring(ci + 1);
                    var type = GetType(rest);
                    if (type == null) {
                        throw new InvalidFormatException("Null type for: " + rest);
                    }
                    var token = GetToken(rest);
                    stack.Push(new Constituent(type, new Span(offset, offset)));
                    if (token != null) {
                        if (type.Equals("-NONE-") && gl != null) {
                            gl.LabelGaps(stack);
                        } else {
                            cons.Add(new Constituent(AbstractBottomUpParser.TOK_NODE,
                                new Span(offset, offset + token.Length)));
                            text.Append(token).Append(" ");
                            offset += token.Length + 1;
                        }
                    }
                } else if (c == ')') {
                    var con = stack.Pop();
                    var start = con.Span.Start;
                    if (start < offset) {
                        cons.Add(new Constituent(con.Label, new Span(start, offset - 1)));
                    }
                }
            }
            var txt = text.ToString();
            var tokenIndex = -1;
            var p = new Parse(txt, new Span(0, txt.Length), AbstractBottomUpParser.TOP_NODE, 1, 0);
            for (var ci = 0; ci < cons.Count; ci++) {
                var con = cons[ci];
                var type = con.Label;
                if (!type.Equals(AbstractBottomUpParser.TOP_NODE)) {
                    if (type == AbstractBottomUpParser.TOK_NODE) {
                        tokenIndex++;
                    }
                    var c = new Parse(txt, con.Span, type, 1, tokenIndex);
                    //System.err.println("insert["+ci+"] "+type+" "+c.toString()+" "+c.hashCode());
                    p.Insert(c);
                    //codeTree(p);
                }
            }
            return p;
        }

        #endregion

        #region . PurneParse .

        /// <summary>
        /// Prune the specified sentence parse of vacuous productions.
        /// </summary>
        /// <param name="parse">The sentence parse to be purned.</param>
        public static void PurneParse(Parse parse) {
            var nodes = new List<Parse> {parse};
            while (nodes.Count != 0) {
                var node = nodes[0];
                var children = node.Children;
                if (children.Length == 1 && node.Type == children[0].Type) {
                    var index = node.Parent.parts.IndexOf(node);
                    children[0].Parent = node.Parent;
                    node.Parent.parts[index] = children[0];
                    node.Parent = null;
                    node.parts = null;
                }

                nodes.RemoveAt(0);
                nodes.AddRange(children);
            }
        }

        #endregion

        #region . SetChild .
        /// <summary>
        /// Replaces the child at the specified index with a new child with the specified label.
        /// </summary>
        /// <param name="index">The index of the child to be replaced.</param>
        /// <param name="label">The label to be assigned to the new child.</param>
        public void SetChild(int index, string label) {

            // TODO: Why replace ?

            var child = (Parse)parts[index].Clone();
            child.Label = label;
            parts[index] = child;
        }
        #endregion

        #region . Remove .

        public void Remove(int index) {
            parts.RemoveAt(index);
            if (parts.Count > 0) {
                if (index == 0 || index == parts.Count) { // size is orig last element
                    Span = new Span((parts[0]).Span.Start, parts[parts.Count - 1].Span.End);
                }
            }
        }

        #endregion

        #region + Show .

        /// <summary>
        /// Appends the specified string buffer with a string representation of this parse.
        /// </summary>
        /// <param name="sb">A string buffer into which the parse string can be appended.</param>
        protected void Show(StringBuilder sb) {
            var start = Span.Start;
            if (Type != AbstractBottomUpParser.TOK_NODE) {
                sb.AppendFormat("({0} ", Type);
            }
            foreach (var part in parts) {
                if (start < part.Span.Start) {
                    sb.Append(EncodeToken(Text.Substring(start, part.Span.Start - start)));
                }
                part.Show(sb);
                start = part.Span.End;
            }
            if (start < Span.End) {
                sb.Append(EncodeToken(Text.Substring(start, Span.End - start)));
            }
            if (Type != AbstractBottomUpParser.TOK_NODE) {
                sb.Append(")");
            }
        }

        #endregion

        #region . UpdateHeads .

        /// <summary>
        /// Computes the head parses for this parse and its sub-parses and stores this information 
        /// in the parse data structure.
        /// </summary>
        /// <param name="rules">The head rules which determine how the head of the parse is computed.</param>
        public void UpdateHeads(IHeadRules rules) {
            if (parts != null && parts.Count != 0) {
                for (int pi = 0, pn = parts.Count; pi < pn; pi++) {
                    parts[pi].UpdateHeads(rules);
                }
                Head = rules.GetHead(parts.ToArray(), Type);
                if (Head == null) {
                    Head = this;
                } else {
                    HeadIndex = Head.HeadIndex;
                }
            } else {
                Head = this;
            }
        }

        #endregion

        #region . UpdateSpan .

        /// <summary>
        /// Updates the span object using the current parts.
        /// </summary>
        public void UpdateSpan() {
            Span = new Span(parts[0].Span.Start, parts[parts.Count - 1].Span.End);
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
            Show(sb);
            return sb.ToString();
        }
        #endregion

    }
}