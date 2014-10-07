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

using SharpNL.Utility;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Chunker {
    /// <summary>
    /// Represents a chunker cross validator.
    /// </summary>
    public class ChunkerCrossValidator {

        private readonly string languageCode;
        private readonly TrainingParameters parameters;
       
        private readonly IEvaluationMonitor<ChunkSample>[] listeners;
        private readonly ChunkerFactory chunkerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkerCrossValidator"/> class.
        /// </summary>
        /// <param name="languageCode">The language code.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="listeners">The listeners.</param>
        public ChunkerCrossValidator(
            string languageCode,
            TrainingParameters parameters, 
            ChunkerFactory factory, 
            params IEvaluationMonitor<ChunkSample>[] listeners) {

            chunkerFactory = factory;
            FMeasure = new FMeasure<Span>();
            
            this.languageCode = languageCode;
            this.parameters = parameters;
            this.listeners = listeners;           
        }

        #region . FMeasure .
        /// <summary>
        /// Gets the f-measure.
        /// </summary>
        /// <value>The f-measure.</value>
        public FMeasure<Span> FMeasure { get; private set; }
        #endregion

        #region . Evaluate .
        /// <summary>
        /// Evaluates the specified chunk samples.
        /// </summary>
        /// <param name="samples">The chunk samples to be evaluated.</param>
        /// <param name="partitions">The partitions (folds).</param>
        public void Evaluate(IObjectStream<ChunkSample> samples, int partitions) {
            var partitioner = new CrossValidationPartitioner<ChunkSample>(samples, partitions);

            while (partitioner.HasNext) {

                var trainingSampleStream = partitioner.Next();

                var model = ChunkerME.Train(languageCode, trainingSampleStream, parameters, chunkerFactory);

                var evaluator = new ChunkerEvaluator(new ChunkerME(model), listeners);

                evaluator.Evaluate(trainingSampleStream.GetTestSampleStream());

                FMeasure.MergeInto(evaluator.FMeasure);
            }
        }
        #endregion


    }
}