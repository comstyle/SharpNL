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
using SharpNL.NameFind;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.Analyzer {

    /// <summary>
    /// The <see cref="EntityFinder"/> class searches for entities in the document sentences 
    /// using a <see cref="ITokenNameFinder"/>.
    /// 
    /// Each result is processed by the specified <see cref="IEntityResolver"/> resolver.
    /// </summary>
    public class EntityFinder : IAnalyzer {

        private readonly ITokenNameFinder nameFinder;
        private readonly IEntityResolver entityResolver;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFinder"/> class.
        /// </summary>
        /// <param name="nameFinder">The name finder used to find the entities.</param>
        /// <param name="entityResolver">The entity resolver.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="nameFinder"/>
        /// or
        /// <paramref name="entityResolver"/>
        /// </exception>
        public EntityFinder(ITokenNameFinder nameFinder, IEntityResolver entityResolver) {
            
            if (nameFinder == null)
                throw new ArgumentNullException("nameFinder");

            if (entityResolver == null)
                throw new ArgumentNullException("entityResolver");

            this.nameFinder = nameFinder;
            this.entityResolver = entityResolver;
        }
        #endregion

        /// <summary>
        /// Analyzes the specified document.
        /// </summary>
        /// <param name="document">
        /// The the whole text given by the user. 
        /// After an analysis it can store the text's sentences, words or its tags.
        /// </param>
        public void Analyze(Document document) {
            var sentences = document.Sentences;

            if (sentences == null || sentences.Count == 0)
                throw new InvalidOperationException("The sentences are not detected on the specified document.");

            foreach (var sentence in sentences) {

                Span[] spans;
                lock (nameFinder) {
                    spans = nameFinder.Find(TextUtils.TokensToString(sentence.Tokens));
                }

                var entities = new List<IEntity>(spans.Length);
                foreach (var span in spans) {
                    IEntity entity = null;
                    try {
                        entity = entityResolver.Resolve(document.Language, sentence, span);
                    } catch (Exception) {
                        // TODO: Implement a proper monitor.
                        Console.Error.WriteLine("An error occurred during entity resolution.");
                    }

                    if (entity != null) {
                        entities.Add(entity);
                    }

                }
                sentence.Entities = entities.AsReadOnly();
            }
        }
    }
}