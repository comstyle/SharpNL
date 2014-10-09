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
using System.Text;
using SharpNL.Tokenize;
using SharpNL.Utility;

namespace SharpNL.Doccat {
    /// <summary>
    /// The factory that provides Doccat default implementations and resources.
    /// </summary>
    public class DoccatFactory : BaseToolFactory {

        private const string GeneratorsEntry = "doccat.featureGenerators";
        private const string TokenizerEntry = "doccat.tokenizer";

        private IFeatureGenerator[] featureGenerators;
        private ITokenizer tokenizer;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="DoccatFactory"/> that provides 
        /// the default implementation of the resources.
        /// </summary>
        public DoccatFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoccatFactory"/> with the specified tokenizer and the feature generatos.
        /// </summary>
        /// <param name="tokenizer">The tokenizer.</param>
        /// <param name="featureGenerators">The feature generators.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="tokenizer"/>
        /// or
        /// <paramref name="featureGenerators"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified tokenizer is not registered in the type resolver.</exception>
        public DoccatFactory(ITokenizer tokenizer, IFeatureGenerator[] featureGenerators) {
            if (tokenizer == null)
                throw new ArgumentNullException("tokenizer");

            if (featureGenerators == null)
                throw new ArgumentNullException("featureGenerators");

            if (!Library.TypeResolver.IsRegistered(tokenizer.GetType()))
                throw new ArgumentException("The specified tokenizer is not registered in the type resolver.");

            foreach (var featureGenerator in featureGenerators) {

                if (featureGenerator == null)
                    throw new ArgumentException("The feature generators must not have any null objects.");

                if (!Library.TypeResolver.IsRegistered(featureGenerator.GetType()))
                    throw new ArgumentException(string.Format("The feature generator type {0} is not registered in the type resolver.", featureGenerator.GetType().Name));                   
                
            }


            this.featureGenerators = featureGenerators;
            this.tokenizer = tokenizer;
        }
        #endregion

        #region + Properties .

        #region . FeatureGeneratos .
        /// <summary>
        /// Gets the feature generators.
        /// </summary>
        /// <value>The feature generators.</value>
        public IFeatureGenerator[] FeatureGenerators {
            get {
                if (featureGenerators != null)
                    return featureGenerators;

                if (ArtifactProvider != null) {
                    var classNames = ArtifactProvider.Manifest[GeneratorsEntry];
                    if (!string.IsNullOrEmpty(classNames))
                        featureGenerators = LoadFeatureGenerators(classNames);

                }

                return featureGenerators ?? (featureGenerators = new IFeatureGenerator[] {
                    new BagOfWordsFeatureGenerator()
                });
            }
        }

        #endregion

        #region . Tokenizer .
        /// <summary>
        /// Gets the tokenizer.
        /// </summary>
        /// <value>The tokenizer.</value>
        public ITokenizer Tokenizer {
            get {
                if (tokenizer != null)
                    return tokenizer;

                if (ArtifactProvider != null) {
                    var className = ArtifactProvider.Manifest[TokenizerEntry];
                    if (className != null) {
                        if (!Library.TypeResolver.IsRegistered(className))
                            throw new NotSupportedException(
                                string.Format("The class {0} is not registered in the type resolver.", className));

                        try {
                            var type = Library.TypeResolver.ResolveType(className);
                            tokenizer = (ITokenizer) Activator.CreateInstance(type);
                        } catch (Exception ex) {
                            throw new InvalidOperationException("Unable to create the tokenizer instance.", ex);
                        }
                    }
                }
                return tokenizer ?? (tokenizer = WhitespaceTokenizer.Instance);
            }
        }
        #endregion

        #endregion

        #region . CreateManifestEntries .
        /// <summary>
        /// Creates the manifest entries that will be added to the model manifest
        /// </summary>
        /// <returns>The manifest entries to added to the model manifest.</returns>
        public override Dictionary<string, string> CreateManifestEntries() {
            var manifestEntries = base.CreateManifestEntries();

            if (Tokenizer != null)
                // We should have no problem here with the type resolver because we ensure 
                // whether the tokenizer is already registered in the type resolver.
                manifestEntries[TokenizerEntry] = Library.TypeResolver.ResolveName(Tokenizer.GetType());

            if (featureGenerators != null)
                manifestEntries[GeneratorsEntry] = FeatureGeneratorsAsString();

            return manifestEntries;
        }
        #endregion

        #region . FeatureGeneratorsAsString .
        private string FeatureGeneratorsAsString() {
            var sb = new StringBuilder(featureGenerators.Length * 15);
            foreach (var featureGenerator in featureGenerators) {
                // We should have no problem here neither we did the validation in the constructor.
                sb.Append(Library.TypeResolver.ResolveName(featureGenerator.GetType())).Append(",");
            }
            return sb.ToString(0, sb.Length - 1);
        } 
        #endregion

        #region . LoadFeatureGenerators .
        /// <summary>
        /// Loads the feature generators from a string representation.
        /// </summary>
        /// <param name="classNames">The class names.</param>
        /// <returns>IFeatureGenerator[].</returns>
        /// <exception cref="System.ArgumentNullException">classNames</exception>
        /// <exception cref="System.NotSupportedException">Unable to resolve the type #CLASS# with the SharpNL type resolver.</exception>
        /// <seealso cref="TypeResolver"/>
        private static IFeatureGenerator[] LoadFeatureGenerators(string classNames) {
            if (string.IsNullOrEmpty(classNames))
                throw new ArgumentNullException("classNames");

            var classes = classNames.Split(',');
            var fgs = new IFeatureGenerator[classes.Length];

            for (int i = 0; i < classes.Length; i++) {
                var type = Library.TypeResolver.ResolveType(classes[i]);
                if (type == null)
                    throw new NotSupportedException(
                        string.Format("Unable to resolve the type {0} with the SharpNL type resolver.", classes[i]));

                fgs[i] = (IFeatureGenerator)Activator.CreateInstance(type);
            }
            return fgs;
        }
        #endregion


    }
}