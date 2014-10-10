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
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// Base class for CoNLL operations.
    /// </summary>
    public abstract class CoNLL {
        /// <summary>
        /// Enumerates the supported languages for CoNLL
        /// </summary>
        public enum Language {
            De,
            En,
            Es,
            Nl
        }

        /// <summary>
        /// Enumerate the entities in the files.
        /// </summary>
        [Flags]
        public enum Types {
            PersonEntities = 1 << 0,
            OrganizationEntities = 1 << 1,
            LocationEntities = 1 << 2,
            MiscEntities = 1 << 3
        }

        /// <summary>
        /// Extracts a entity based on <paramref name="beginTag"/> tag.
        /// </summary>
        /// <param name="begin">The begin.</param>
        /// <param name="end">The end.</param>
        /// <param name="beginTag">The begin tag.</param>
        /// <returns>The entity <see cref="Span"/> object.</returns>
        /// <exception cref="InvalidFormatException">Unknown type:  + type</exception>
        protected static Span Extract(int begin, int end, string beginTag) {
            var type = beginTag.Substring(2);

            switch (type) {
                case "PER":
                    type = "person";
                    break;
                case "LOC":
                    type = "location";
                    break;
                case "MISC":
                    type = "misc";
                    break;
                case "ORG":
                    type = "organization";
                    break;
                default:
                    throw new InvalidFormatException("Unknown type: " + type);
            }
            return new Span(begin, end, type);
        }

    }
}