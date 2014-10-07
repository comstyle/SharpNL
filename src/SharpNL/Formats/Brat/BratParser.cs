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

using SharpNL.Utility;

namespace SharpNL.Formats.Brat {

    internal delegate BratAnnotation BratParse(Span[] tokens, string line);

    internal static class BratParser {

        public const int ID_OFFSET = 0;
        public const int TYPE_OFFSET = 1;
        public const int BEGIN_OFFSET = 2;
        public const int END_OFFSET = 3;

        public const int ARG1_OFFSET = 2;
        public const int ARG2_OFFSET = 3;

        private static int ParseInt(string value) {
            int i;
            if (!int.TryParse(value, out i)) {
                throw new InvalidFormatException("Unable to parse the integer value.");
            }

            return i;
        }

        private static string ParseArg(string value) {

            if (value.Length > 4) {
                return value.Substring(5).Trim();
            }

            throw new InvalidFormatException("Failed to parse argument.");
        }

        public static BratAnnotation ParseSpanAnnotation(Span[] tokens, string line) {
            if (tokens.Length > 4) {
                string type = tokens[TYPE_OFFSET].GetCoveredText(line);

                int endOffset = -1;

                int firstTextTokenIndex = -1;

                for (int i = END_OFFSET; i < tokens.Length; i++) {
                    if (!tokens[i].GetCoveredText(line).Contains(";")) {

                        endOffset = ParseInt(tokens[i].GetCoveredText(line));
                        firstTextTokenIndex = i + 1;
                        break;
                    }
                }

                var id = tokens[ID_OFFSET].GetCoveredText(line);

                var coveredText = line.Substring(tokens[firstTextTokenIndex].Start, tokens[endOffset].End);

                return new SpanAnnotation(
                    id, 
                    type,
                    new Span(ParseInt(tokens[BEGIN_OFFSET].GetCoveredText(line)), endOffset, type), 
                    coveredText);

            }
            throw new InvalidFormatException("Line must have at least 5 fields.");
        }

        public static BratAnnotation ParseRelationAnnotation(Span[] tokens, string line) {
            return new RelationAnnotation(
                tokens[ID_OFFSET].GetCoveredText(line),
                tokens[TYPE_OFFSET].GetCoveredText(line),
                ParseArg(tokens[ARG1_OFFSET].GetCoveredText(line)),
                ParseArg(tokens[ARG2_OFFSET].GetCoveredText(line)));
        }
    }
}