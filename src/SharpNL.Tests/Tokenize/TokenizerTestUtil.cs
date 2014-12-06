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

using System.Collections.Generic;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Tests.Tokenize {
    internal static class TokenizerTestUtil {
        public static TokenizerModel CreateSimpleMaxentTokenModel() {
            var samples = new List<TokenSample> {
                new TokenSample("year", new[] {new Span(0, 4)}),
                new TokenSample("year,", new[] {new Span(0, 4), new Span(4, 5)}),
                new TokenSample("it,", new[] {new Span(0, 2), new Span(2, 3)}),
                new TokenSample("it", new[] {new Span(0, 2)}),
                new TokenSample("yes", new[] {new Span(0, 3)}),
                new TokenSample("yes,", new[] {new Span(0, 3), new Span(3, 4)})
            };

            var mlParams = new TrainingParameters();
            mlParams.Set(Parameters.Iterations, "100");
            mlParams.Set(Parameters.Cutoff, "0");

            return TokenizerME.Train(
                new CollectionObjectStream<TokenSample>(samples),
                new TokenizerFactory("en", null, true),
                mlParams);
        }

        public static TokenizerModel CreateMaxentTokenModel() {
            using (var data = Tests.OpenFile("/opennlp/tools/tokenize/token.train")) {
                var samples = new TokenSampleStream(new PlainTextByLineStream(data));
                var mlParams = new TrainingParameters();
                mlParams.Set(Parameters.Iterations, "100");
                mlParams.Set(Parameters.Cutoff, "0");
                return TokenizerME.Train(samples, new TokenizerFactory("en", null, true), mlParams);
            }
        }
    }
}