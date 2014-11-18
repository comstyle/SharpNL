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
using System.Collections.ObjectModel;
using SharpNL.Analyzer;
using SharpNL.Text;
using SharpNL.Utility;

namespace SharpNL.NameFind {
    /// <summary>
    /// Represents a name finder analyzer which allows the easy abstraction of 
    /// the name finder (Named Entity Recognizer). This class is thread-safe.
    /// </summary>
    public class NameFinderAnalyzer : AbstractAnalyzer {

        /// <summary>
        /// Gets the name finder.
        /// </summary>
        /// <value>The name finder.</value>
        protected ITokenNameFinder NameFinder { get; private set; }

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="NameFinderAnalyzer"/> using the default analyzer weight.
        /// </summary>
        /// <param name="nameFinder">The name finder.</param>
        public NameFinderAnalyzer(ITokenNameFinder nameFinder) : this(nameFinder, 3f) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFinderAnalyzer" /> with the specified weight.
        /// </summary>
        /// <param name="nameFinder">The name finder.</param>
        /// <param name="weight">The analyzer weight.</param>
        /// <exception cref="System.ArgumentNullException">nameFinder</exception>
        public NameFinderAnalyzer(ITokenNameFinder nameFinder, float weight)
            : base(weight) {
            if (nameFinder == null)
                throw new ArgumentNullException("nameFinder");

            NameFinder = nameFinder;
        }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified document.
        /// </summary>
        /// <param name="factory">The factory used in this analysis.</param>
        /// <param name="document">The document to be analyzed.</param>
        protected override void Evaluate(ITextFactory factory, IDocument document) {
            if (document.Sentences == null)
                throw new AnalyzerException(this, "The document does not have the sentences detected.");

            foreach (var sentence in document.Sentences) {
                if (sentence.Tokens == null)
                    throw new AnalyzerException(this, "The document have a sentence without the tokenization.");

                Span[] spans;
                lock (NameFinder) {
                    spans = NameFinder.Find(sentence.Tokens.ToTokenArray());
                }

                if (spans == null || spans.Length == 0) {
                    sentence.Entities = new ReadOnlyCollection<IEntity>(new IEntity[] { });
                    continue;
                }

                var list = new List<IEntity>(spans.Length);

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var span in spans) {
                    var entity = factory.CreateEntity(sentence, span);
                    if (entity != null)
                        list.Add(entity);
                }

                sentence.Entities = new ReadOnlyCollection<IEntity>(list);
            }
        }
        #endregion

    }
}