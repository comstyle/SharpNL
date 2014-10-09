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
using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.NameFind {
    /// <summary>
    /// Represents a cross validator for <see cref="TokenNameFinderModel"/> models.
    /// </summary>
    public class TokenNameFinderCrossValidator {
        private readonly string type;
        private readonly string languageCode;
        private readonly TrainingParameters parameters;
        private readonly TokenNameFinderFactory factory;
        private readonly IEvaluationMonitor<NameSample>[] listeners;

        #region + Constructors .

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderCrossValidator"/> class.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="type"><c>null</c> or an override type for all types in the training data.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="listeners">The listeners.</param>
        public TokenNameFinderCrossValidator(string languageCode, string type, TrainingParameters parameters, params IEvaluationMonitor<NameSample>[] listeners) {
            this.languageCode = languageCode;
            this.type = type;
            this.parameters = parameters;
            this.listeners = listeners;
            factory = new TokenNameFinderFactory();
            FMeasure = new FMeasure<Span>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenNameFinderCrossValidator"/> class.
        /// </summary>
        /// <param name="languageCode">The language of the training data.</param>
        /// <param name="type"><c>null</c> or an override type for all types in the training data.</param>
        /// <param name="parameters">The machine learning train parameters.</param>
        /// <param name="factory">The name finder factory.</param>
        /// <param name="listeners">The listeners.</param>
        public TokenNameFinderCrossValidator(string languageCode, string type, TrainingParameters parameters, TokenNameFinderFactory factory, params IEvaluationMonitor<NameSample>[] listeners) {
            if (factory == null)
                throw new ArgumentNullException("factory");

            this.languageCode = languageCode;
            this.type = type;
            this.parameters = parameters;
            this.factory = factory;
            this.listeners = listeners;

            FMeasure = new FMeasure<Span>();
        }

        #endregion

        #region + Properties .

        #region . FMesure .
        /// <summary>
        /// Gets the f-measure.
        /// </summary>
        /// <value>The f-measure.</value>
        public FMeasure<Span> FMeasure { get; private set; }
        #endregion

        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the samples with a given number of partitions.
        /// </summary>
        /// <param name="samples">The samples to train and test.</param>
        /// <param name="partitions">The number of folds.</param>
        public void Evaluate(IObjectStream<NameSample> samples, int partitions) {
            // Note: The name samples need to be grouped on a document basis.

            var partitioner = new CrossValidationPartitioner<DocumentSample>(new NameToDocumentSampleStream(samples),
                partitions);

            while (partitioner.HasNext) {
                var trainingSampleStream = partitioner.Next();

                var model = NameFinderME.Train(
                    languageCode,
                    type,
                    factory != null ? samples : new DocumentToNameSampleStream(trainingSampleStream),
                    parameters,
                    factory);

                // do testing
                var evaluator = new TokenNameFinderEvaluator(new NameFinderME(model), listeners);

                evaluator.Evaluate(new DocumentToNameSampleStream(trainingSampleStream.GetTestSampleStream()));

                FMeasure.MergeInto(evaluator.FMeasure);
            }
        }
        #endregion

        #region @ DocumentSample .

        private class DocumentSample {
            internal DocumentSample(NameSample[] samples) {
                Samples = samples;
            }

            public NameSample[] Samples { get; private set; }
        }

        #endregion

        #region @ NameToDocumentSampleStream .

        private class NameToDocumentSampleStream : FilterObjectStream<NameSample, DocumentSample> {
            private NameSample beginSample;

            internal NameToDocumentSampleStream(IObjectStream<NameSample> samples) : base(samples) { }

            public override DocumentSample Read() {
                var document = new List<NameSample>();

                if (beginSample == null) {
                    // Assume that the clear flag is set
                    beginSample = Samples.Read();
                }

                // Underlying stream is exhausted!
                if (beginSample == null) {
                    return null;
                }

                document.Add(beginSample);

                NameSample sample;
                while ((sample = Samples.Read()) != null) {
                    if (sample.ClearAdaptiveData) {
                        beginSample = sample;
                        break;
                    }

                    document.Add(sample);
                }

                // Underlying stream is exhausted,
                // next call must return null
                if (sample == null) {
                    beginSample = null;
                }

                return new DocumentSample(document.ToArray());
            }

            /// <summary>
            /// Repositions the stream at the beginning and the previously seen object 
            /// sequence will be repeated exactly. This method can be used to re-read the
            /// stream if multiple passes over the objects are required.
            /// </summary>
            public override void Reset() {
                base.Reset();

                beginSample = null;
            }
        }

        #endregion

        #region @ DocumentToNameSampleStream .
        private class DocumentToNameSampleStream : FilterObjectStream<DocumentSample, NameSample> {
            private IEnumerator<NameSample> enumerator;

            internal DocumentToNameSampleStream(IObjectStream<DocumentSample> samples)
                : base(samples) {
                enumerator = new List<NameSample>().GetEnumerator();
            }

            public override NameSample Read() {
                if (enumerator.MoveNext())
                    return enumerator.Current;
                var docSample = Samples.Read();
                if (docSample != null) {
                    enumerator = docSample.Samples.GetEnumerator().Cast<NameSample>();
                    return Read();
                }
                return null;
            }
        }
        #endregion

    }
}