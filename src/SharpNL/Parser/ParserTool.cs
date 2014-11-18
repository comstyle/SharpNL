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
using SharpNL.Java;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Parser {
    /// <summary>
    /// A helper tool to create the Parse objects that will be parsed by a implemented <see cref="IParser"/> object.
    /// </summary>
    public static class ParserTool {
        private static readonly Regex untokenizedParentPattern1;
        private static readonly Regex untokenizedParentPattern2;

        static ParserTool() {
            untokenizedParentPattern1 = new Regex("([^ ])([({)}])", RegexOptions.Compiled);
            untokenizedParentPattern2 = new Regex("([({)}])([^ ])", RegexOptions.Compiled);
        }

        /// <summary>
        /// Parses the specified <see cref="ISentence"/> object using a given <paramref name="parser"/>.
        /// </summary>
        /// <param name="sentence">The sentence to be parsed.</param>
        /// <param name="parser">The parser.</param>
        /// <param name="numParses">The number parses. Usually 1.</param>
        /// <returns>An array with the parsed results.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sentence"/>
        /// or
        /// <paramref name="parser"/>
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">numParses</exception>
        /// <exception cref="System.InvalidOperationException">The sentence is not tokenized.</exception>
        public static Parse[] ParseLine(ISentence sentence, IParser parser, int numParses) {
            if (sentence == null)
                throw new ArgumentNullException("sentence");

            if (parser == null)
                throw new ArgumentNullException("parser");

            if (numParses < 0)
                throw new ArgumentOutOfRangeException("numParses");

            if (sentence.Tokens == null || sentence.Tokens.Count == 0)
                throw new InvalidOperationException("The sentence is not tokenized.");

            var sb = new StringBuilder(sentence.Length);
            for (var i = 0; i < sentence.Tokens.Count; i++) {
                sb.Append(sentence.Tokens[i].Lexeme).Append(' ');
            }
            sb.Remove(sb.Length - 1, 1);

            var start = 0;
            var p = new Parse(sb.ToString(), new Span(0, sb.Length), AbstractBottomUpParser.INC_NODE, 0, 0);

            for (var i = 0; i < sentence.Tokens.Count; i++) {
                p.Insert(
                    new Parse(
                        sb.ToString(), 
                        new Span(start, start + sentence.Tokens[i].Lexeme.Length),
                        AbstractBottomUpParser.TOK_NODE, 0, i));

                start += sentence.Tokens[i].Lexeme.Length + 1;
            }

            return numParses == 1 ? new[] { parser.Parse(p) } : parser.Parse(p, numParses);
        }

        public static Parse[] ParseLine(string line, IParser parser, int numParses) {
            line = untokenizedParentPattern1.Replace(line, "$1 $2");
            line = untokenizedParentPattern2.Replace(line, "$1 $2");

            var str = new StringTokenizer(line);
            var sb = new StringBuilder();
            var tokens = new List<String>();
            while (str.HasMoreTokens) {
                var tok = str.NextToken;
                tokens.Add(tok);
                sb.Append(tok).Append(" ");
            }
            var text = sb.ToString(0, sb.Length - 1);
            var p = new Parse(text, new Span(0, text.Length), AbstractBottomUpParser.INC_NODE, 0, 0);
            var start = 0;

            for (var i = 0; i < tokens.Count; i++) {
                p.Insert(new Parse(text, new Span(start, start + tokens[i].Length), AbstractBottomUpParser.TOK_NODE, 0, i));
                start += tokens[i].Length + 1;
            }

            return numParses == 1 ? new[] { parser.Parse(p) } : parser.Parse(p, numParses);
        }
    }
}