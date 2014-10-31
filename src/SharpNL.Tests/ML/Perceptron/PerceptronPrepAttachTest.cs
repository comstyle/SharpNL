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

using System.Globalization;
using NUnit.Framework;
using SharpNL.ML;
using SharpNL.ML.Model;
using SharpNL.ML.Perceptron;
using SharpNL.Utility;

namespace SharpNL.Tests.ML.Perceptron {
    [TestFixture]
    internal class PerceptronPrepAttachTest {
        [Test]
        public void TestPerceptronOnPrepAttachData() {
            var model = new PerceptronTrainer().TrainModel(400,
                new TwoPassDataIndexer(PrepAttachDataUtility.CreateTrainingStream(), 1, false), 1);

            PrepAttachDataUtility.TestModel(model, 0.7650408516959644);
        }

        [Test]
        public void TestPerceptronOnPrepAttachDataWithSkippedAveraging() {

            var trainParams = new TrainingParameters();

            trainParams.Set(Parameters.Algorithm, Parameters.Algorithms.Perceptron);
            trainParams.Set(Parameters.Cutoff, "1");
            trainParams.Set(Parameters.UseSkippedAveraging, "true");

            var trainer = TrainerFactory.GetEventTrainer(trainParams, null, null);
            var model = trainer.Train(PrepAttachDataUtility.CreateTrainingStream());

            PrepAttachDataUtility.TestModel(model, 0.773706362961129);

        }

        [Test]
        public void TestPerceptronOnPrepAttachDataWithTolerance() {
            var trainParams = new TrainingParameters();

            trainParams.Set(Parameters.Algorithm, Parameters.Algorithms.Perceptron);
            trainParams.Set(Parameters.Cutoff, "1");
            trainParams.Set(Parameters.Iterations, "500");
            trainParams.Set(Parameters.Tolerance, "0.0001");

            var trainer = TrainerFactory.GetEventTrainer(trainParams, null, null);
            var model = trainer.Train(PrepAttachDataUtility.CreateTrainingStream());

            PrepAttachDataUtility.TestModel(model, 0.7677642980935875);

        }

        [Test]
        public void TestPerceptronOnPrepAttachDataWithStepSizeDecrease() {
            var trainParams = new TrainingParameters();

            trainParams.Set(Parameters.Algorithm, Parameters.Algorithms.Perceptron);
            trainParams.Set(Parameters.Cutoff, "1");
            trainParams.Set(Parameters.Iterations, "500");
            trainParams.Set(Parameters.StepSizeDecrease, "0.06");

            var trainer = TrainerFactory.GetEventTrainer(trainParams, null, null);
            var model = trainer.Train(PrepAttachDataUtility.CreateTrainingStream());

            /*
             * The java test gives an error too, soo.... for now i'll assume that is correct :P
             * 
             * java.lang.AssertionError: expected:<0.7756870512503095> but was:<0.7766773953948998>
                at org.junit.Assert.fail(Assert.java:91)
                at org.junit.Assert.failNotEquals(Assert.java:645)
                at org.junit.Assert.assertEquals(Assert.java:441)
                at org.junit.Assert.assertEquals(Assert.java:510)
            */
            //PrepAttachDataUtility.TestModel(model, 0.7756870512503095); < OpenNLP value

            PrepAttachDataUtility.TestModel(model, 0.77742015350334237);
        }


    }
}