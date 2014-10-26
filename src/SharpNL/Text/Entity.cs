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

namespace SharpNL.Text {
    /// <summary>
    /// Represents a generic entity.
    /// </summary>
    public class Entity : IEntity {

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="tokens">The sentence tokens.</param>
        /// <exception cref="System.ArgumentNullException">span</exception>
        public Entity(Span span, string[] tokens) {
            if (span == null) 
                throw new ArgumentNullException("span");

            CoveredText = span.GetCoveredText(tokens);

            for (var i = 0; i < span.Start; i++)
                Start += tokens[i].Length + 1;           

            Span = span;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="sentence">The sentence.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="span"/>
        /// or
        /// <paramref name="sentence"/>
        /// </exception>
        public Entity(Span span, Sentence sentence) {
            if (span == null)
                throw new ArgumentNullException("span");

            if (sentence == null)
                throw new ArgumentNullException("sentence");

            CoveredText = span.GetCoveredText(sentence.Tokens);

            for (var i = 0; i < span.Start; i++)
                Start += sentence.Tokens[i].Length + 1;

            Span = span;
        }


        #endregion

        #region . Probability .

        /// <summary>
        /// Gets the entity probability.
        /// </summary>
        /// <value>The entity probability.</value>
        public double Probability {
            get { return Span.Probability; }           
        }
        #endregion

        #region . Span .
        /// <summary>
        /// Gets the entity span.
        /// </summary>
        /// <value>The entity span.</value>
        public Span Span { get; private set; }
        #endregion

        #region . Start .
        /// <summary>
        /// Gets the start position in the sentence.
        /// </summary>
        /// <value>The start position in the sentence.</value>
        public int Start { get; private set; }
        #endregion

        #region . Text .
        /// <summary>
        /// Gets the covered text.
        /// </summary>
        /// <value>The covered text.</value>
        public string CoveredText { get; private set; }
        #endregion

        #region . Type .
        /// <summary>
        /// Gets the entity type.
        /// </summary>
        /// <value>The entity type.</value>
        public virtual string Type {
            get { return Span.Type; }
        }
        #endregion

    }
}