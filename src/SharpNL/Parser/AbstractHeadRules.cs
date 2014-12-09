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
using System.IO;
using SharpNL.Extensions;


namespace SharpNL.Parser {
    /// <summary>
    /// Abstract class for encoding the head rules associated with parsing.
    /// </summary>
    public abstract class AbstractHeadRules : IHeadRules {

        protected AbstractHeadRules() {
            HeadRules = new Dictionary<string, AbstractHeadRule>();
        }

        #region + Properties .

        #region . HeadRules .
        protected Dictionary<string, AbstractHeadRule> HeadRules { get; private set; }
        #endregion

        #region . PunctuationTags .
        /// <summary>
        /// Gets the set of punctuation tags.
        /// Attachment decisions for these tags will not be modeled.
        /// </summary>
        /// <returns>The list of punctuation tags.</returns>
        public List<string> PunctuationTags { get; protected set; }
        #endregion

        #endregion

        #region . GetHead .
        /// <summary>
        /// Returns the head constituent for the specified constituents of the specified type.
        /// </summary>
        /// <param name="constituents">The constituents which make up a constituent of the specified type.</param>
        /// <param name="type">The type of a constituent which is made up of the specified constituents.</param>
        /// <returns>The constituent which is the head.</returns>
        public abstract Parse GetHead(Parse[] constituents, string type);
        #endregion

        #region . Serialize .
        /// <summary>
        /// Writes the head rules to the writer in a format suitable for loading
        /// the head rules again with the constructor. The encoding must be
        /// taken into account while working with the writer and reader.
        /// </summary>
        /// <param name="writer">The stream writer.</param>
        /// <remarks>
        /// After the entries have been written, the writer is flushed. 
        /// The writer remains open after this method returns.
        /// </remarks>
        public virtual void Serialize(StreamWriter writer) {
            foreach (var rule in HeadRules) {
                // num of tags
                writer.Write((rule.Value.Tags.Length + 2).ToString(CultureInfo.InvariantCulture));
                writer.Write(' ');

                // type
                writer.Write("{0} ", rule.Key);

                // leftToRight
                writer.Write(rule.Value.LeftToRight ? "1" : "0");

                foreach (var tag in rule.Value.Tags) {
                    writer.Write(" {0}", tag);
                }

                writer.WriteLine();
            }
            writer.Flush();
        }

        #endregion

        #region . Equals .

        protected bool Equals(AbstractHeadRules other) {
            return HeadRules.Keys.SequenceEqual(other.HeadRules.Keys) &&
                   PunctuationTags.SequenceEqual(other.PunctuationTags);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:AbstractHeadRules"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AbstractHeadRules)obj);
        }

        #endregion

        #region . GetHashCode .

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode() {
            unchecked {
                return ((HeadRules != null ? HeadRules.GetHashCode() : 0) * 397) ^
                       (PunctuationTags != null ? PunctuationTags.GetHashCode() : 0);
            }
        }

        #endregion

    }
}