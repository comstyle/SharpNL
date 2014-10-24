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
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Parser.TreeInsert {
    public class ParserEventStream : AbstractParserEventStream {
        protected AttachContextGenerator attachContextGenerator;
        protected BuildContextGenerator buildContextGenerator;
        protected CheckContextGenerator checkContextGenerator;

        #region + Constructors .


        public ParserEventStream(IObjectStream<Parse> d, AbstractHeadRules rules, ParserEventTypeEnum type)
            : base(d, rules, type) {

            buildContextGenerator = new BuildContextGenerator();
            attachContextGenerator = new AttachContextGenerator(Punctuation);
            checkContextGenerator = new CheckContextGenerator(Punctuation);
        }

        public ParserEventStream(IObjectStream<Parse> d, AbstractHeadRules rules, ParserEventTypeEnum type,
            Dictionary.Dictionary dict)
            : base(d, rules, type, dict) {

            buildContextGenerator = new BuildContextGenerator();
            attachContextGenerator = new AttachContextGenerator(Punctuation);
            checkContextGenerator = new CheckContextGenerator(Punctuation);
        }

        #endregion

        #region . AddParseEvents .

        /// <summary>
        /// Produces all events for the specified sentence chunks and adds them to the specified list.
        /// </summary>
        /// <param name="newEvents">A list of events to be added to.</param>
        /// <param name="chunks">Pre-chunked constituents of a sentence.</param>
        protected override void AddParseEvents(List<Event> newEvents, Parse[] chunks) {
            /** Frontier nodes built from node in a completed parse.  Specifically,
              * they have all their children regardless of the stage of parsing.*/

            var rightFrontier = new List<Parse>();
            var builtNodes = new List<Parse>();

            /** Nodes which characterize what the parse looks like to the parser as its being built.
             * Specifically, these nodes don't have all their children attached like the parents of
             * the chunk nodes do.*/
            var currentChunks = new Parse[chunks.Length];
            for (var ci = 0; ci < chunks.Length; ci++) {
                currentChunks[ci] = (Parse) chunks[ci].Clone();
                currentChunks[ci].PreviousPunctuationSet = chunks[ci].PreviousPunctuationSet;
                currentChunks[ci].NextPunctuationSet = chunks[ci].NextPunctuationSet;
                currentChunks[ci].Label = AbstractBottomUpParser.COMPLETE;
                chunks[ci].Label = AbstractBottomUpParser.COMPLETE;
            }
            for (var ci = 0; ci < chunks.Length; ci++) {
                //System.err.println("parserEventStream.addParseEvents: chunks="+Arrays.asList(chunks));
                var parent = chunks[ci].Parent;
                var prevParent = chunks[ci];
                
                //var off = 0;
                //build un-built parents
                if (!chunks[ci].IsPosTag) {
                    builtNodes.Add(chunks[ci]);
                    //builtNodes[off++] = chunks[ci];
                }

                //perform build stages
                while (parent.Type != AbstractBottomUpParser.TOP_NODE && parent.Label == null) {
                    if (parent.Label == null && prevParent.Type != parent.Type) {
                        //build level
                        // if (debug) System.err.println("Build: " + parent.Type + " for: " + currentChunks[ci]);
                        if (Type == ParserEventTypeEnum.Build) {
                            newEvents.Add(new Event(parent.Type, buildContextGenerator.GetContext(currentChunks, ci)));
                        }
                        builtNodes.Add(parent);
                        //builtNodes[off++] = parent;
                        var newParent = new Parse(currentChunks[ci].Text, currentChunks[ci].Span, parent.Type, 1, 0);
                        newParent.Add(currentChunks[ci], Rules);
                        newParent.PreviousPunctuationSet = currentChunks[ci].PreviousPunctuationSet;
                        newParent.NextPunctuationSet = currentChunks[ci].NextPunctuationSet;
                        currentChunks[ci].Parent = newParent;
                        currentChunks[ci] = newParent;
                        newParent.Label = Parser.BUILT;

                        //see if chunk is complete
                        if (LastChild(chunks[ci], parent)) {
                            if (Type == ParserEventTypeEnum.Check) {
                                newEvents.Add(new Event(AbstractBottomUpParser.COMPLETE,
                                    checkContextGenerator.GetContext(currentChunks[ci], currentChunks, ci, false)));
                            }
                            currentChunks[ci].Label = AbstractBottomUpParser.COMPLETE;
                            parent.Label = AbstractBottomUpParser.COMPLETE;
                        } else {
                            if (Type == ParserEventTypeEnum.Check) {
                                newEvents.Add(new Event(AbstractBottomUpParser.INCOMPLETE,
                                    checkContextGenerator.GetContext(currentChunks[ci], currentChunks, ci, false)));
                            }
                            currentChunks[ci].Label = AbstractBottomUpParser.INCOMPLETE;
                            parent.Label = AbstractBottomUpParser.COMPLETE;
                        }

                        chunks[ci] = parent;
                        //System.err.println("build: "+newParent+" for "+parent);
                    }
                    //TODO: Consider whether we need to set this label or train parses at all.

                    parent.Label = Parser.BUILT;
                    prevParent = parent;
                    parent = parent.Parent;
                }
                //decide to attach
                if (Type == ParserEventTypeEnum.Build) {
                    newEvents.Add(new Event(Parser.DONE, buildContextGenerator.GetContext(currentChunks, ci)));
                }
                //attach node
                string attachType = null;
                /** Node selected for attachment. */
                Parse attachNode = null;
                var attachNodeIndex = -1;
                if (ci == 0) {
                    var top = new Parse(currentChunks[ci].Text, new Span(0, currentChunks[ci].Text.Length),
                        AbstractBottomUpParser.TOP_NODE, 1, 0);
                    top.Insert(currentChunks[ci]);
                } else {
                    /** Right frontier consisting of partially-built nodes based on current state of the parse.*/
                    var currentRightFrontier = Parser.GetRightFrontier(currentChunks[0], Punctuation);
                    if (currentRightFrontier.Count != rightFrontier.Count) {
                        throw new InvalidOperationException("frontiers mis-aligned: " + currentRightFrontier.Count +
                                                            " != " + rightFrontier.Count + " " + currentRightFrontier +
                                                            " " + rightFrontier);
                        //System.exit(1);
                    }
                    var parents = GetNonAdjoinedParent(chunks[ci]);
                    //try daughters first.
                    for (var cfi = 0; cfi < currentRightFrontier.Count; cfi++) {
                        var frontierNode = rightFrontier[cfi];
                        var cfn = currentRightFrontier[cfi];
                        if (!Parser.checkComplete || cfn.Label != AbstractBottomUpParser.COMPLETE) {
                            
                            //if (debug) System.err.println("Looking at attachment site (" + cfi + "): " + cfn.Type + " ci=" + i + " cs=" + nonPunctChildCount(cfn) + ", " + cfn + " :for " + currentChunks[ci].Type + " " + currentChunks[ci] + " -> " + parents);
                            if (parents.ContainsKey(frontierNode) && attachNode == null && parents[frontierNode] == NonPunctChildCount(cfn)) {
                                attachType = Parser.ATTACH_DAUGHTER;
                                attachNodeIndex = cfi;
                                attachNode = cfn;
                                if (Type == ParserEventTypeEnum.Attach) {
                                    newEvents.Add(new Event(attachType,
                                        attachContextGenerator.GetContext(currentChunks, ci, currentRightFrontier,
                                            attachNodeIndex)));
                                }
                                //System.err.println("daughter attach "+attachNode+" at "+fi);
                            }
                        } /* else {
                            if (debug) System.err.println("Skipping (" + cfi + "): " + cfn.Type + "," + cfn.getPreviousPunctuationSet() + " " + cfn + " :for " + currentChunks[ci].Type + " " + currentChunks[ci] + " -> " + parents);
                        }
                        // Can't attach past first incomplete node.
                        if (Parser.checkComplete && cfn.getLabel().equals(Parser.INCOMPLETE)) {
                            if (debug) System.err.println("breaking on incomplete:" + cfn.Type + " " + cfn);
                            break;
                        }
                        */
                    }
                    //try sisters, and generate non-attach events.
                    for (var cfi = 0; cfi < currentRightFrontier.Count; cfi++) {
                        var frontierNode = rightFrontier[cfi];
                        var cfn = currentRightFrontier[cfi];
                        if (attachNode == null && parents.ContainsKey(frontierNode.Parent)
                            && frontierNode.Type.Equals(frontierNode.Parent.Type)
                            ) {
                            //&& frontierNode.Parent.getLabel() == null) {
                            attachType = Parser.ATTACH_SISTER;
                            attachNode = cfn;
                            attachNodeIndex = cfi;
                            if (Type == ParserEventTypeEnum.Attach) {
                                newEvents.Add(new Event(Parser.ATTACH_SISTER,
                                    attachContextGenerator.GetContext(currentChunks, ci, currentRightFrontier, cfi)));
                            }
                            chunks[ci].Parent.Label = Parser.BUILT;
                            //System.err.println("in search sister attach "+attachNode+" at "+cfi);
                        } else if (cfi == attachNodeIndex) {
                            //skip over previously attached daughter.
                        } else {
                            if (Type == ParserEventTypeEnum.Attach) {
                                newEvents.Add(new Event(Parser.NON_ATTACH,
                                    attachContextGenerator.GetContext(currentChunks, ci, currentRightFrontier, cfi)));
                            }
                        }
                        //Can't attach past first incomplete node.
                        if (Parser.checkComplete && cfn.Label.Equals(AbstractBottomUpParser.INCOMPLETE)) {
                            //if (debug) System.err.println("breaking on incomplete:" + cfn.Type + " " + cfn);
                            break;
                        }
                    }
                    //attach Node
                    if (attachNode != null) {
                        if (attachType == Parser.ATTACH_DAUGHTER) {
                            var daughter = currentChunks[ci];
                            //if (debug) System.err.println("daughter attach a=" + attachNode.Type + ":" + attachNode + " d=" + daughter + " com=" + lastChild(chunks[ci], rightFrontier.get(attachNodeIndex)));
                            attachNode.Add(daughter, Rules);
                            daughter.Parent = attachNode;
                            if (LastChild(chunks[ci], rightFrontier[attachNodeIndex])) {
                                if (Type == ParserEventTypeEnum.Check) {
                                    newEvents.Add(new Event(AbstractBottomUpParser.COMPLETE,
                                        checkContextGenerator.GetContext(attachNode, currentChunks, ci, true)));
                                }
                                attachNode.Label = AbstractBottomUpParser.COMPLETE;
                            } else {
                                if (Type == ParserEventTypeEnum.Check) {
                                    newEvents.Add(new Event(AbstractBottomUpParser.INCOMPLETE,
                                        checkContextGenerator.GetContext(attachNode, currentChunks, ci, true)));
                                }
                            }
                        } else if (attachType == Parser.ATTACH_SISTER) {
                            var frontierNode = rightFrontier[attachNodeIndex];
                            rightFrontier[attachNodeIndex] = frontierNode.Parent;
                            var sister = currentChunks[ci];
                            //if (debug) System.err.println("sister attach a=" + attachNode.Type + ":" + attachNode + " s=" + sister + " ap=" + attachNode.Parent + " com=" + lastChild(chunks[ci], rightFrontier.get(attachNodeIndex)));
                            var newParent = attachNode.Parent.AdJoin(sister, Rules);

                            newParent.Parent = attachNode.Parent;
                            attachNode.Parent = newParent;
                            sister.Parent = newParent;
                            if (Equals(attachNode, currentChunks[0])) {
                                currentChunks[0] = newParent;
                            }
                            if (LastChild(chunks[ci], rightFrontier[attachNodeIndex])) {
                                if (Type == ParserEventTypeEnum.Check) {
                                    newEvents.Add(new Event(AbstractBottomUpParser.COMPLETE,
                                        checkContextGenerator.GetContext(newParent, currentChunks, ci, true)));
                                }
                                newParent.Label = AbstractBottomUpParser.COMPLETE;
                            } else {
                                if (Type == ParserEventTypeEnum.Check) {
                                    newEvents.Add(new Event(AbstractBottomUpParser.INCOMPLETE,
                                        checkContextGenerator.GetContext(newParent, currentChunks, ci, true)));
                                }
                                newParent.Label = AbstractBottomUpParser.INCOMPLETE;
                            }
                        }
                        //update right frontier
                        for (var ni = 0; ni < attachNodeIndex; ni++) {
                            //System.err.println("removing: "+rightFrontier.get(0));
                            rightFrontier.RemoveAt(0);
                        }
                    } else {
                        //System.err.println("No attachment!");
                        throw new InvalidOperationException("No Attachment: " + chunks[ci]);
                    }
                }
                rightFrontier.InsertRange(0, builtNodes);
                builtNodes.Clear();
            }
        }

        #endregion

        #region . GetNonAdjoinedParent .

        private Dictionary<Parse, int> GetNonAdjoinedParent(Parse node) {
            var parents = new Dictionary<Parse, int>();
            var parent = node.Parent;
            var index = IndexOf(node, parent);
            parents[parent] = index;
            while (parent.Type == node.Type) {
                node = parent;
                parent = parent.Parent;
                index = IndexOf(node, parent);
                parents[parent] = index;
            }
            return parents;
        }

        #endregion

        #region . IndexOf .
        private int IndexOf(Parse child, Parse parent) {
            var kids = AbstractBottomUpParser.CollapsePunctuation(parent.Children, Punctuation);
            for (var ki = 0; ki < kids.Length; ki++) {
                if (child.Equals(kids[ki])) {
                    return ki;
                }
            }
            return -1;
        }
        #endregion

        #region . NonPunctChildCount .
        private int NonPunctChildCount(Parse node) {
            return AbstractBottomUpParser.CollapsePunctuation(node.Children, Punctuation).Length;
        }
        #endregion

    }
}