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
    public abstract class AbstractEventTrainer : AbstractTrainer, IEventTrainer {

        public const string DataIndexerParam = "DataIndexer";
        public const string DataIndexerOnePass = "OnePass";
        public const string DataIndexerTwoPass = "TwoPass";

        protected AbstractEventTrainer(bool isSortAndMerge) {
            IsSortAndMerge = isSortAndMerge;
        }


        #region + Properties .

        #region . DataIndexerName .

        /// <summary>
        /// Gets the data indexer name.
        /// </summary>
        /// <value>The name of the data indexer.</value>
        public string DataIndexerName {
            get { return GetStringParam(DataIndexerParam, DataIndexerTwoPass); }
        }

        #endregion

        #region . IsSortAndMerge .
        /// <summary>
        /// Gets a value indicating whether this instance is sort and merge.
        /// </summary>
        /// <value><c>true</c> if this instance is sort and merge; otherwise, <c>false</c>.</value>
        public bool IsSortAndMerge { get; private set; }
        #endregion

        #region . PrintMessages .
        public bool PrintMessages { get; set; }
        #endregion

        #endregion

        #region . Display .
        protected void Display(string message) {
            if (PrintMessages)
                Console.Out.WriteLine(message);
        }
        #endregion

        #region . IsValid .
        protected override bool IsValid() {
            if (!base.IsValid()) {
                return false;
            }

            var dataIndexer = GetStringParam(DataIndexerParam, DataIndexerTwoPass);
            if (dataIndexer != DataIndexerOnePass && dataIndexer != DataIndexerTwoPass) {
                return false;
            }

            // TODO: check data indexing...

            return true;
        }
        #endregion

        #region . GetDataIndexer .
        public IDataIndexer GetDataIndexer(IObjectStream<Event> events) {
            switch (DataIndexerName) {
                case DataIndexerOnePass:
                    return new OnePassDataIndexer(events, Cutoff, IsSortAndMerge);
                case DataIndexerTwoPass:
                    return new TwoPassDataIndexer(events, Cutoff, IsSortAndMerge);
                default:
                    throw new InvalidOperationException("Unexpected data indexer name: " + DataIndexerName);
            }
        }
        #endregion



        protected abstract IMaxentModel DoTrain(IDataIndexer indexer);

        public IMaxentModel Train(IObjectStream<Event> events) {

            if (!IsValid()) {
                throw new InvalidOperationException("The train parameters are not valid!");
            }
            var hses = new HashSumEventStream(events);
            var indexer = GetDataIndexer(hses);

            var model = DoTrain(indexer);

            AddToReport("Training-Eventhash", hses.CalculateHashSum());
            AddToReport(TRAINER_TYPE_PARAM, "Event");

            return model;
        }
    }
}