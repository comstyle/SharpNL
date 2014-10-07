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
using System.IO;
using System.Text;

namespace SharpNL.Formats.Brat {
    public class AnnotationConfiguration {
        public const string SPAN_TYPE = "Span";
        public const string ENTITY_TYPE = "Entity";
        public const string RELATION_TYPE = "Relation";

        private readonly Dictionary<string, string> mapping;

        public AnnotationConfiguration(Dictionary<string, string> mapping) {
            this.mapping = mapping;
        }

        #region . this .

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        public string this[string type] {
            get {
                if (mapping.ContainsKey(type))
                    return null;

                return mapping[type];
            }
        }

        #endregion

        #region . GetTypeClass .

#if DEBUG
        [Obsolete("Use the default indexer property.")]
        public string GetTypeClass(string type) {
            return mapping[type];
        }
#endif

        #endregion

        #region . Parse .

        public static AnnotationConfiguration Parse(Stream inputStream) {
            var typeToClassMap = new Dictionary<string, string>();

            using (var reader = new StreamReader(inputStream, Encoding.UTF8)) {
                // Note: This only supports entities and relations section

                string line;
                string sectionType = null;
                while ((line = reader.ReadLine()) != null) {
                    line = line.Trim();

                    if (line.Length == 0 || line.StartsWith("#")) {
                        continue;
                    }

                    if (line.StartsWith("[") && line.EndsWith("]")) {
                        sectionType = line.TrimStart('[').TrimEnd(']');
                    } else {
                        switch (sectionType) {
                            case "entities":
                                typeToClassMap.Add(line, ENTITY_TYPE);
                                continue;
                            case "relations":
                                typeToClassMap.Add(line.Substring(0, line.IndexOf(' ')), RELATION_TYPE);
                                continue;
                        }
                    }
                }


                return new AnnotationConfiguration(typeToClassMap);
            }
        }

        #endregion
    }
}