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
using SharpNL.Utility;

namespace SharpNL.Parser {
    public class ParserTool {
        private static readonly Regex untokenizedParentPattern1;
        private static readonly Regex untokenizedParentPattern2;

        static ParserTool() {
            untokenizedParentPattern1 = new Regex("([^ ])([({)}])", RegexOptions.Compiled);
            untokenizedParentPattern2 = new Regex("([({)}])([^ ])", RegexOptions.Compiled);
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