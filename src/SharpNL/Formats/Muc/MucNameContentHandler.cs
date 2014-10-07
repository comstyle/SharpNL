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
using System.Globalization;
using SharpNL.NameFind;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Formats.Muc {
    public class MucNameContentHandler : SgmlParser.ContentHandler {
        private const string ENTITY_ELEMENT_NAME = "ENAMEX";
        private const string TIME_ELEMENT_NAME = "TIMEX";
        private const string NUM_ELEMENT_NAME = "NUMEX";

        private static readonly List<string> NAME_ELEMENT_NAMES;

        private static readonly List<string> EXPECTED_TYPES;
        private readonly Stack<Span> incompleteNames = new Stack<Span>();

        private readonly List<Span> names = new List<Span>();

        private readonly List<NameSample> storedSamples;

        private readonly List<string> text = new List<string>();
        private readonly ITokenizer tokenizer;
        private bool isClearAdaptiveData;
        private bool isInsideContentElement;

        static MucNameContentHandler() {
            EXPECTED_TYPES = new List<string> {
                "PERSON",
                "ORGANIZATION",
                "LOCATION",
                "DATE",
                "TIME",
                "MONEY",
                "PERCENT"
            };

            NAME_ELEMENT_NAMES = new List<string> {
                ENTITY_ELEMENT_NAME,
                TIME_ELEMENT_NAME,
                NUM_ELEMENT_NAME
            };
        }

        public MucNameContentHandler(ITokenizer tokenizer,
            List<NameSample> storedSamples) {
            this.tokenizer = tokenizer;
            this.storedSamples = storedSamples;
        }


        public override void StartElement(string name, Dictionary<string, string> attributes) {
            if (MucElementNames.DOC_ELEMENT.Equals(name)) {
                isClearAdaptiveData = true;
            }

            if (MucElementNames.CONTENT_ELEMENTS.Contains(name)) {
                isInsideContentElement = true;
            }

            if (NAME_ELEMENT_NAMES.Contains(name)) {
                var nameType = attributes["TYPE"];

                if (!EXPECTED_TYPES.Contains(nameType)) {
                    throw new InvalidFormatException("Unknown timex, numex or namex type: "
                                                     + nameType + ", expected one of " + EXPECTED_TYPES);
                }

                incompleteNames.Push(new Span(text.Count, text.Count, nameType.ToLower(CultureInfo.GetCultureInfo("en"))));
            }
        }

        public override void Characters(string chars) {
            if (isInsideContentElement) {
                text.AddRange(tokenizer.Tokenize(chars));
            }
        }

        public override void EndElement(string name) {
            if (NAME_ELEMENT_NAMES.Contains(name)) {
                var nameSpan = incompleteNames.Pop();
                nameSpan = new Span(nameSpan.Start, text.Count, nameSpan.Type);
                names.Add(nameSpan);
            }

            if (MucElementNames.CONTENT_ELEMENTS.Contains(name)) {
                storedSamples.Add(new NameSample(text.ToArray(), names.ToArray(), isClearAdaptiveData));

                if (isClearAdaptiveData) {
                    isClearAdaptiveData = false;
                }

                text.Clear();
                names.Clear();
                isInsideContentElement = false;
            }
        }
    }
}