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
using NUnit.Framework;
using SharpNL.ML;
using SharpNL.ML.MaxEntropy;
using SharpNL.ML.Model;
using SharpNL.Utility;

namespace SharpNL.Tests.ML {
    [TestFixture]
    public class TrainerFactoryTest {

        private class DummyTrainer : IEventTrainer {
            public IMaxentModel Train(IObjectStream<Event> events) {
                return null;
            }

            public IMaxentModel Train(Monitor monitor, IObjectStream<Event> events) {
                return null;
            }

            public void Init(TrainingParameters parameters, Dictionary<string, string> reportMap) { }
        }
        
        private TrainingParameters mlParams;
        
        [TestFixtureSetUp]
        public void Setup() {
            mlParams = new TrainingParameters();
            mlParams.Set(TrainingParameters.AlgorithmParam, GIS.MaxEntropy);
            mlParams.Set(TrainingParameters.IterationsParam, "10");
            mlParams.Set(TrainingParameters.CutoffParam, "5");

            TrainerFactory.RegisterTrainer("Dummy", typeof(DummyTrainer));
        }

        [Test]
        public void testBuiltInvalid() {
            Assert.True(TrainerFactory.IsValid(mlParams));
        }

        [Test]
        public void testCustomTrainer() {
            mlParams.Set(TrainingParameters.AlgorithmParam, "Dummy");
            Assert.True(TrainerFactory.IsValid(mlParams));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void testCustomTrainer2() {
            TrainerFactory.RegisterTrainer("Dummy", typeof(DummyTrainer));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void testCustomTrainer3() {
            TrainerFactory.RegisterTrainer("Nothing", typeof(object));
        }

        [Test]
        public void testInvalidTrainer() {
            mlParams.Set(TrainingParameters.AlgorithmParam, "Nop");
            Assert.False(TrainerFactory.IsValid(mlParams));
        }


    }
}