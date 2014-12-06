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

namespace SharpNL.ML.Model {
    using Utility;
    /// <summary>
    /// Represents a abstract event trainer.
    /// </summary>
    public abstract class AbstractEventTrainer : AbstractTrainer, IEventTrainer {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractEventTrainer"/> class.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="isSortAndMerge">if set to <c>true</c> [is sort and merge].</param>
        protected AbstractEventTrainer(Monitor monitor, bool isSortAndMerge) : base(monitor) {
            IsSortAndMerge = isSortAndMerge;
        }


        #region + Properties .

        #region . DataIndexerName .
        /// <summary>
        /// Gets the data indexer name.
        /// </summary>
        /// <value>The name of the data indexer.</value>
        public string DataIndexerName {
            get { return GetStringParam(Parameters.DataIndexer, Parameters.DataIndexers.TwoPass); }
        }

        #endregion

        #region . IsSortAndMerge .
        /// <summary>
        /// Gets a value indicating whether this instance is sort and merge.
        /// </summary>
        /// <value><c>true</c> if this instance is sort and merge; otherwise, <c>false</c>.</value>
        public bool IsSortAndMerge { get; private set; }
        #endregion

        #endregion

        #region . IsValid .
        protected override bool IsValid() {
            if (!base.IsValid()) {
                return false;
            }

            var dataIndexer = GetStringParam(Parameters.DataIndexer, Parameters.DataIndexers.TwoPass);
            if (dataIndexer != Parameters.DataIndexers.OnePass && dataIndexer != Parameters.DataIndexers.TwoPass) {
                return false;
            }

            // TODO: check data indexing...

            return true;
        }
        #endregion

        #region . GetDataIndexer .
        /// <summary>
        /// Creates a new data indexer for the given event stream.
        /// </summary>
        /// <param name="events">The event stream.</param>
        /// <returns>IDataIndexer.</returns>
        /// <exception cref="System.InvalidOperationException">Unexpected data indexer name: Name</exception>
        public IDataIndexer GetDataIndexer(IObjectStream<Event> events) {
            switch (DataIndexerName) {
                case Parameters.DataIndexers.OnePass:
                    return new OnePassDataIndexer(events, Cutoff, IsSortAndMerge, Monitor);
                case Parameters.DataIndexers.TwoPass:
                    return new TwoPassDataIndexer(events, Cutoff, IsSortAndMerge, Monitor);
                default:
                    throw new InvalidOperationException("Unexpected data indexer name: " + DataIndexerName);
            }
        }
        #endregion

        #region . DoTrain .
        /// <summary>
        /// Execute the training operation.
        /// </summary>
        /// <param name="indexer">The data indexer.</param>
        /// <returns>The trained <see cref="IMaxentModel"/> model.</returns>
        protected abstract IMaxentModel DoTrain(IDataIndexer indexer);
        #endregion


        /// <summary>
        /// Trains the maximum entropy model using the specified <paramref name="events"/>.
        /// </summary>
        /// <param name="events">The training events.</param>
        /// <returns>The trained <see cref="IMaxentModel"/> model.</returns>
        public IMaxentModel Train(IObjectStream<Event> events) {

            if (!IsValid()) {
                throw new InvalidOperationException("The train parameters are not valid!");
            }
            var hses = new HashSumEventStream(events);
            var indexer = GetDataIndexer(hses);

            var model = DoTrain(indexer);

            AddToReport(Parameters.TrainingEventhash, hses.CalculateHashSum());
            AddToReport(Parameters.TrainerType, "Event");

            return model;
        }
    }
}