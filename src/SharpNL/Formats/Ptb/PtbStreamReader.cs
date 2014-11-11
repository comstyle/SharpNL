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
using SharpNL.Utility;

namespace SharpNL.Formats.Ptb {
    public sealed class PtbStreamReader : IObjectStream<PtbNode> {

        private readonly Monitor monitor;
        private readonly bool useFunctionTags;
        private readonly IResolver resolver;
        private readonly IObjectStream<string> lineStream;
        public PtbStreamReader(string language, IObjectStream<string> lineStream, bool useFunctionTags, Monitor monitor) {
            if (lineStream == null)
                throw new ArgumentNullException("lineStream");

            switch (language) {
                case "pt":
                case "pt-BR":
                    resolver = Lang.pt.Resolver.Instance;
                    break;
                default:
                    resolver = Lang.DefaultResolver.Instance;
                    break;
            }

            this.lineStream = lineStream;
            this.useFunctionTags = useFunctionTags;
            this.monitor = monitor;
        }

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            lineStream.Dispose();
        }
        #endregion

        /// <summary>
        /// Returns the next <see cref="PtbNode"/> object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next <see cref="PtbNode"/> object or <c>null</c> to signal that the stream is exhausted.
        /// </returns>
        public PtbNode Read() {

            retry:

            if (monitor != null && monitor.Token.CanBeCanceled)
                monitor.Token.ThrowIfCancellationRequested();

            PtbNode root = null;

            var pos = 0; // text position
            var stack = new Stack<PtbNode>();
            var invalid = false;
            string line;
            while ((line = lineStream.Read()) != null) {
                line = line.TrimStart(' ', '\t', '\u00A0'); // \u00A0 = NO-BREAK SPACE

                if (invalid) {
                    if (line.Trim().Length == 0) // end of sentence
                        goto retry;

                    for (var i = 0; i < line.Length; i++) {
                        switch (line[i]) {
                            case '#':
                                if (i == 0)
                                    goto next; // ignore comment

                                break;
                            case '(':
                                stack.Push(null);
                                continue;
                            case ')':
                                stack.Pop();

                                if (stack.Count == 0)
                                    goto done;

                                continue;
                            default:
                                continue;
                        }
                    }

                    continue; // ignore invalid data
                }

                if (line.Length == 0) {
                    if (root != null)
                        goto done;

                    continue;
                }

                
                for (var i = 0; i < line.Length; i++) {
                    switch (line[i]) {
                        case '#':
                            if (i == 0)
                                goto next; // ignore comment, if the line starts with '#'

                            continue;
                        case '(':
                            var rest = line.Substring(i + 1);
                            var type = resolver.GetType(rest, useFunctionTags);
                            var token = resolver.GetToken(rest);

                            if (type == null) {
                                if (monitor != null)
                                    monitor.OnWarning("Penn treebank node without type: " + line);

                                stack.Push(null);

                                invalid = true;
                                goto next;
                            }

                            /* skip a few chars to improve performance (if possible)... */
                            int skip;
                            if (token != null && (skip = rest.IndexOf(')')) != -1) {
                                i += skip;
                            }

                            var child = token != null
                                ? new PtbNode {Type = type, Token = token, Span = new Span(pos, pos + token.Length)}
                                : new PtbNode {Type = type};

                            if (token != null)
                                pos += token.Length + 1;

                            if (root == null)
                                root = child;

                            if (stack.Count > 0) {
                                var parent = stack.Peek();
                                // check if the parent node is a gap
                                if (parent != null)
                                    parent.Children.Add(child);
                                else {
                                    // search for the parent node that is not a gap
                                    var array = stack.ToArray();
                                    foreach (var p in array) {
                                        if (p == null) continue;
                                        p.Children.Add(child);
                                        break;
                                    }
                                }
                            }

                            stack.Push(child);

                            continue;
                        case ')':
                            var pop = stack.Pop();
                            
                            // adjust span
                            if (pop != null) {
                                if (pop.HasChildren) {
                                    var s = GetStartPos(pop);
                                    var e = GetEndPos(pop);
                                    if (s.HasValue && e.HasValue) {
                                        pop.Span = new Span(s.Value, e.Value);
                                    }
                                }

                                if (pop.Span == null) {
                                    pop.Span = null;
                                }

                            }



                            if (stack.Count == 0)
                                goto done;                          

                            continue;
                    }
                }
            next:
                ;
            }

            done:

            // check if invalid.
            if (invalid || stack.Count != 0) {
                if (monitor != null)
                    monitor.OnWarning("A invalid Penn Treebank sentence was skipped.");

                goto retry;
            }

            // End of stream
            if (root == null)
                return null; 

            var rs = GetStartPos(root);
            var re = GetEndPos(root);
            
            root.Span = new Span(rs.Value, re.Value);

            // if the stack is not empty, the sentence is incomplete/invalid.
            return stack.Count == 0 
                ? root 
                : null;
        }

        private static int? GetStartPos(PtbNode node) {
            if (node.Span != null)
                return node.Span.Start;

            if (node.HasChildren) {
                foreach (var child in node.Children) {
                    var pos = GetStartPos(child);
                    if (pos.HasValue)
                        return pos;
                }
            }

            return null;
        }

        private static int? GetEndPos(PtbNode node) {
            if (node.Span != null)
                return node.Span.End;

            if (!node.HasChildren)
                return null;

            for (var i = node.Children.Count - 1; i >= 0; i--) {
                var pos = GetEndPos(node.Children[i]);
                if (pos.HasValue)
                    return pos;
            }

            return null;
        }

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            lineStream.Reset();
        }
        #endregion

    }
}