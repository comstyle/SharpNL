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
using NUnit.Framework;
using SharpNL.ML;
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tests.ML.MaxEnt {
    [TestFixture]
    internal class MaxentPrepAttachTest {


        [Test]
        public void TestMaxentOnPrepAttachData() {
            var model = new GISTrainer().TrainModel(100,
                new TwoPassDataIndexer(PrepAttachDataUtility.CreateTrainingStream(), 1), 1);

            PrepAttachDataUtility.TestModel(model, 0.7997028967566229d);
        }

        [Test]
        public void TestMaxentOnPrepAttachData2Threads() {

            var trainer = new GISTrainer();
            var di = new TwoPassDataIndexer(PrepAttachDataUtility.CreateTrainingStream(), 1);

            var model = trainer.TrainModel(100, di, new UniformPrior(), 1, 2);

            PrepAttachDataUtility.TestModel(model, 0.7997028967566229d);
        }

        [Test]
        public void TestMaxentOnPrepAttachDataWithParams() {
            var reportMap = new Dictionary<string, string>();
            var trainParams = new TrainingParameters();

            trainParams.Set(Parameters.Algorithm, Parameters.Algorithms.MaxEnt);
            trainParams.Set(Parameters.DataIndexer, Parameters.DataIndexers.TwoPass);
            trainParams.Set(Parameters.Cutoff, "1");

            var trainer = TrainerFactory.GetEventTrainer(trainParams, reportMap, null);
            var model = trainer.Train(PrepAttachDataUtility.CreateTrainingStream());

            PrepAttachDataUtility.TestModel(model, 0.7997028967566229d);
        }

        [Test]
        public void TestMaxentOnPrepAttachDataWithParamsDefault() {
            var reportMap = new Dictionary<string, string>();
            var trainParams = new TrainingParameters();

            trainParams.Set(Parameters.Algorithm, Parameters.Algorithms.MaxEnt);

            var trainer = TrainerFactory.GetEventTrainer(trainParams, reportMap, null);
            var model = trainer.Train(PrepAttachDataUtility.CreateTrainingStream());

            PrepAttachDataUtility.TestModel(model, 0.8086159940579352d);
        }

    }
}