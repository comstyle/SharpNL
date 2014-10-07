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

namespace SharpNL.Parser {
    public static class ParserFactory {
        public static IParser Create(ParserModel model) {
            if (model == null)
                throw new ArgumentNullException("model");

            return Create(model, AbstractBottomUpParser.DefaultBeamSize, AbstractBottomUpParser.DefaultAdvancePercentage);
        }

        public static IParser Create(ParserModel model, int beamSize, double advancePercentage) {
            if (model == null)
                throw new ArgumentNullException("model");

            switch (model.ParserType) {
                case ParserType.Chunking:
                    return new Chunking.Parser(model, beamSize, advancePercentage);
                case ParserType.TreeInsert:
                    return new TreeInsert.Parser(model, beamSize, advancePercentage);
                default:
                    throw new InvalidOperationException("Unexpected model parser.");
            }
        }
    }
}