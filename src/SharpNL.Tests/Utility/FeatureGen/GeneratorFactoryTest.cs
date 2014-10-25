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

using NUnit.Framework;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Tests.Utility.FeatureGen {
    [TestFixture]
    internal class GeneratorFactoryTest {


        [Test]
        public void TestCreationWithSimpleDescriptor() {

            using (var config = Tests.OpenFile("/opennlp/tools/util/featuregen/TestFeatureGeneratorConfig.xml")) {
                
                Assert.NotNull(config);


                var aggregatedGenerator = (AggregatedFeatureGenerator)GeneratorFactory.Create(config, null);

                Assert.NotNull(aggregatedGenerator);

                Assert.AreEqual(1, aggregatedGenerator.Generators.Count);

                Assert.AreEqual(typeof(OutcomePriorFeatureGenerator), aggregatedGenerator.Generators[0].GetType());
            }
           
        }

        [Test]
        public void testCreationWithCustomGenerator() {
            using (var config = Tests.OpenFile("/opennlp/tools/util/featuregen/CustomClassLoading.xml")) {

                var aggregatedGenerator = (AggregatedFeatureGenerator)GeneratorFactory.Create(config, null);

                Assert.NotNull(aggregatedGenerator);
                Assert.AreEqual(1, aggregatedGenerator.Generators.Count);

                Assert.AreEqual(typeof(TokenFeatureGenerator), aggregatedGenerator.Generators[0].GetType());
                
            }
        }

        [Test, ExpectedException(typeof(InvalidFormatException))]
        public void TestCreationWithUnknownElement() {
            using (var config = Tests.OpenFile("/opennlp/tools/util/featuregen/FeatureGeneratorConfigWithUnkownElement.xml")) {

                var never = GeneratorFactory.Create(config, null);

                Assert.Fail("No exception :(");
            }
        }
    }
}