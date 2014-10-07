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
using System.IO;
using System.Text;
using SharpNL.Utility;

namespace SharpNL.Formats.Muc {
    /// <summary>
    /// SAX style SGML parser.
    /// </summary>
    /// <remarks>
    /// The implementation is very limited, but good enough to
    /// parse the MUC corpora. Its must very likely be extended/improved/fixed to parse
    /// a different SGML corpora.
    /// </remarks>
    public class SgmlParser {
        private static string extractTagName(StringBuilder tagChars) {
            var fromOffset = 1;

            if (tagChars.Length > 1 && tagChars[1] == '/') {
                fromOffset = 2;
            }

            for (var ci = 1; ci < tagChars.Length; ci++) {
                if (tagChars[ci] == '>' || char.IsWhiteSpace(tagChars[ci])) {
                    return tagChars.ToString(fromOffset, ci - fromOffset);
                }
            }

            throw new InvalidFormatException("Failed to extract tag name!");
        }

        private static Dictionary<string, string> getAttributes(StringBuilder tagChars) {
            // format:
            // space
            // key
            // =
            // " <- begin
            // value chars
            // " <- end

            var attributes = new Dictionary<string, string>();

            var key = new StringBuilder();
            var value = new StringBuilder();

            var extractKey = false;
            var extractValue = false;

            for (var i = 0; i < tagChars.Length; i++) {
                // White space indicates begin of new key name
                if (char.IsWhiteSpace(tagChars[i]) && !extractValue) {
                    extractKey = true;
                }
                    // Equals sign indicated end of key name
                else if (extractKey && ('=' == tagChars[i] || char.IsWhiteSpace(tagChars[i]))) {
                    extractKey = false;
                }
                    // Inside key name, extract all chars
                else if (extractKey) {
                    key.Append(tagChars[i]);
                }
                    // " Indicates begin or end of value chars
                else if ('"' == tagChars[i]) {
                    if (extractValue) {
                        attributes[key.ToString()] = value.ToString();

                        // clear key and value buffers
                        key = new StringBuilder();
                        value = new StringBuilder();
                    }

                    extractValue = !extractValue;
                }
                    // Inside value, extract all chars
                else if (extractValue) {
                    value.Append(tagChars[i]);
                }
            }

            return attributes;
        }

        public void Parse(StreamReader input, ContentHandler handler) {
            var buffer = new StringBuilder();

            var isInsideTag = false;
            var isStartTag = true;

            var lastChar = -1;
            int c;
            while ((c = input.Read()) != -1) {
                if ('<' == c) {
                    if (isInsideTag) {
                        throw new InvalidFormatException("Did not expect < char!");
                    }

                    if (buffer.ToString().Trim().Length > 0) {
                        handler.Characters(buffer.ToString().Trim());
                    }

                    buffer = new StringBuilder();

                    isInsideTag = true;
                    isStartTag = true;
                }

                // buffer.AppendCodePoint(c); <-- java
                buffer.Append(char.ConvertFromUtf32(c));


                if ('/' == c && lastChar == '<') {
                    isStartTag = false;
                }

                if ('>' == c) {
                    if (!isInsideTag) {
                        throw new InvalidFormatException("Did not expect > char!");
                    }

                    if (isStartTag) {
                        handler.StartElement(extractTagName(buffer), getAttributes(buffer));
                    } else {
                        handler.EndElement(extractTagName(buffer));
                    }

                    buffer = new StringBuilder();

                    isInsideTag = false;
                }

                lastChar = c;
            }

            if (isInsideTag) {
                throw new InvalidFormatException("Did not find matching > char!");
            }
        }

        public abstract class ContentHandler {
            public virtual void StartElement(string name, Dictionary<string, string> attributes) { }

            public virtual void Characters(string chars) { }

            public virtual void EndElement(string name) { }
        }
    }
}