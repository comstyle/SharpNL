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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SharpNL.Chunker;
using SharpNL.DocumentCategorizer;
using SharpNL.NameFind;
using SharpNL.Parser;
using SharpNL.POSTag;
using SharpNL.SentenceDetector;
using SharpNL.Text;
using SharpNL.Tokenize;
using SharpNL.Utility.Model;

namespace SharpNL.Analyzer {
    /// <summary>
    /// Represents a analyzer that aggregates one or more analyzers.
    /// </summary>
    public class AggregateAnalyzer : IAnalyzer, IEnumerable<IAnalyzer> {

        /// <summary>
        /// The analyzers in this instance.
        /// </summary>
        protected readonly SortedSet<IAnalyzer> Analyzers;

        #region + Constructors .
        /// <summary>
        /// Initializes static members of the <see cref="AggregateAnalyzer"/>.
        /// </summary>
        static AggregateAnalyzer() {
            DefaultWeight = 5f;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateAnalyzer"/> class 
        /// weighted by the <see cref="DefaultWeight"/> value.
        /// </summary>
        public AggregateAnalyzer() : this(5f) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateAnalyzer"/> using
        /// a specific <paramref name="weight"/> value.
        /// </summary>
        /// <param name="weight">A specific weight.</param>
        public AggregateAnalyzer(float weight) {
            Analyzers = new SortedSet<IAnalyzer>(Comparer<IAnalyzer>.Create((x, y) => x.Weight.CompareTo(y.Weight)));
            Weight = weight;           
        }
        #endregion

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the number of analyzers contained in this instance.
        /// </summary>
        /// <value>The number of analyzers contained in this instance.</value>
        public int Count {
            get { return Analyzers.Count; }
        }
        #endregion

        #region . DefaultWeight .
        /// <summary>
        /// Gets or sets the default weight of the aggreated analyzer. The default value is <c>5f</c>;
        /// </summary>
        /// <value>The default weight.</value>
        public static float DefaultWeight { get; set; }
        #endregion

        #region . Weight .
        /// <summary>
        /// Gets the analyzer weight which control the influence of the aggregated analyzer during the execution. 
        /// The lower values will be executed first.
        /// </summary>
        /// <value>Returns a floating point value indicating the relative weight a task.</value>
        public float Weight { get; private set; }
        #endregion

        #endregion

        #region . Add .
        /// <summary>
        /// Adds the specified analyzer.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="analyzer"/>
        /// </exception>
        public void Add(IAnalyzer analyzer) {
            if (analyzer == null)
                throw new ArgumentNullException("analyzer");

            Analyzers.Add(analyzer);
        }


        /// <summary>
        /// Adds the corresponding analyzer for the given model file.
        /// </summary>
        /// <param name="modelFile">The model.</param>
        /// <remarks>
        /// This method automatically detects the model type, then a corresponding analyzer 
        /// is added to this instance.
        /// </remarks>
        public void Add(string modelFile) {
            Add(new FileInfo(modelFile));          
        }

        /// <summary>
        /// Adds the corresponding analyzer for the given model file.
        /// </summary>
        /// <param name="modelFile">The model.</param>
        /// <remarks>
        /// <para>This method automatically detects the model type, then a corresponding analyzer is added to this instance.</para>
        /// <para>The parser model will be added using the <see cref="Parser.Chunking.Parser"/>.</para>
        /// </remarks>
        public void Add(FileInfo modelFile) {
            if (modelFile == null)
                throw new ArgumentNullException("modelFile");

            if (!modelFile.Exists)
                throw new FileNotFoundException("The model file does not exist.", modelFile.FullName);

            var info = new ModelInfo(modelFile.FullName);
            switch (info.ModelType) {
                case Models.Chunker:
                    Analyzers.Add(
                        new ChunkerAnalyzer(
                            new ChunkerME((ChunkerModel) info.OpenModel())));
                    return;
                case Models.DocumentCategorizer:
                    Analyzers.Add(
                        new DocumentCategorizerAnalyzer(
                            new DocumentCategorizerME((DocumentCategorizerModel) info.OpenModel())));
                    break;
                case Models.Tokenizer:
                    Analyzers.Add(
                        new TokenizerAnalyzer(
                            new TokenizerME((TokenizerModel) info.OpenModel())));
                    break;
                case Models.NameFind:
                    Analyzers.Add(
                        new NameFinderAnalyzer(
                            new NameFinderME((TokenNameFinderModel) info.OpenModel())));
                    break;
                case Models.Parser:
                    Analyzers.Add(
                        new ParserAnalyzer(
                            new Parser.Chunking.Parser((ParserModel) info.OpenModel())));
                    break;
                case Models.POSTag:
                    Analyzers.Add(
                        new POSTaggerAnalyzer(
                            new POSTaggerME((POSModel)info.OpenModel())));
                    break;
                case Models.SentenceDetector:
                    Analyzers.Add(
                        new SentenceDetectorAnalyzer(
                            new SentenceDetectorME((SentenceModel)info.OpenModel())));
                    break;
                default:
                    throw new NotSupportedException("The specified model type is not supported.");
            }
        }

        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all the analyzers from this instance.
        /// </summary>
        public void Clear() {
            Analyzers.Clear();
        }
        #endregion

        #region . Analyze .

        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// The <see cref="DefaultTextFactory"/> will be used to create the objects in the <see cref="IDocument"/>.
        /// </summary>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="document"/>
        /// </exception>
        public void Analyze(IDocument document) {
            Analyze(DefaultTextFactory.Instance, document);
        }

        /// <summary>
        /// Analyzes the specified document which can be several sentences, a sentence or even a single word.
        /// </summary>
        /// <param name="factory">The text factory. if this argument is <c>null</c> the <see cref="DefaultTextFactory"/> must 
        /// be used during the analysis.</param>
        /// <param name="document">The <see cref="IDocument" /> to be analyzed.</param>
        public void Analyze(ITextFactory factory, IDocument document) {
            lock (document) {
                foreach (var analyzer in Analyzers) {
                    try {
                        analyzer.Analyze(factory, document);
                    } catch (Exception ex) {
                        throw new AnalyzerException(analyzer, "The analyzer raised an exception.", ex);
                    }
                }
            }
        }
        #endregion

        #region . Remove .

        /// <summary>
        /// Removes the specified analyzer.
        /// </summary>
        /// <param name="analyzer">The analyzer.</param>
        public void Remove(IAnalyzer analyzer) {
            if (Analyzers.Contains(analyzer)) {
                Analyzers.Remove(analyzer);
            }
        }
        #endregion

        #region . GetEnumerator .
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:IEnumerator{IAnalyzer}"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IAnalyzer> GetEnumerator() {
            return Analyzers.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

    }
}