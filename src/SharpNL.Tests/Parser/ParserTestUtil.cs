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

using NUnit.Framework;
using SharpNL.Parser;
using SharpNL.Parser.Lang;
using SharpNL.Utility;

namespace SharpNL.Tests.Parser {
    internal static class ParserTestUtil {


        internal static IObjectStream<Parse> CreateParseTestStream() {
            return new ParseSampleStream(new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/parser/test.parse")));
        }

        internal static IObjectStream<Parse> CreateParseTrainStream() {
            return new ParseSampleStream(new PlainTextByLineStream(Tests.OpenFile("/opennlp/tools/parser/parser.train")));
        }

        internal static AbstractHeadRules CreateTestHeadRules() {

            AbstractHeadRules rules;

            using (var file = Tests.OpenFile("/opennlp/tools/parser/en_head_rules")) {

                Assert.NotNull(file);

                rules = HeadRulesManager.Deserialize("en", file);

                Assert.NotNull(rules);
            }
            return rules;
        }

    }
}