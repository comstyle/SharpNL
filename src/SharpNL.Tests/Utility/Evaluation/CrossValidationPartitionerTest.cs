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
using NUnit.Framework;
using SharpNL.Utility.Evaluation;

namespace SharpNL.Tests.Utility.Evaluation {
    [TestFixture]
    public class CrossValidationPartitionerTest {
        [Test]
        public void Test3FoldCV() {
            var partitioner = new CrossValidationPartitioner<string>(new[] {
                "01", "02", "03", "04", "05", "06", "07", "08", "09", "10"
            }, 3);

            // first partition
            Assert.True(partitioner.HasNext);

            var firstTraining = partitioner.Next();
            Assert.AreEqual("02", firstTraining.Read());
            Assert.AreEqual("03", firstTraining.Read());
            Assert.AreEqual("05", firstTraining.Read());
            Assert.AreEqual("06", firstTraining.Read());
            Assert.AreEqual("08", firstTraining.Read());
            Assert.AreEqual("09", firstTraining.Read());
            Assert.Null(firstTraining.Read());

            var firstTest = firstTraining.GetTestSampleStream();
            Assert.AreEqual("01", firstTest.Read());
            Assert.AreEqual("04", firstTest.Read());
            Assert.AreEqual("07", firstTest.Read());
            Assert.AreEqual("10", firstTest.Read());
            Assert.Null(firstTest.Read());

            // second partition
            Assert.True(partitioner.HasNext);
            var secondTraining = partitioner.Next();

            Assert.AreEqual("01", secondTraining.Read());
            Assert.AreEqual("03", secondTraining.Read());
            Assert.AreEqual("04", secondTraining.Read());
            Assert.AreEqual("06", secondTraining.Read());
            Assert.AreEqual("07", secondTraining.Read());
            Assert.AreEqual("09", secondTraining.Read());
            Assert.AreEqual("10", secondTraining.Read());

            Assert.Null(secondTraining.Read());

            var secondTest = secondTraining.GetTestSampleStream();

            Assert.AreEqual("02", secondTest.Read());
            Assert.AreEqual("05", secondTest.Read());
            Assert.AreEqual("08", secondTest.Read());
            Assert.Null(secondTest.Read());

            // third partition
            Assert.True(partitioner.HasNext);
            var thirdTraining = partitioner.Next();

            Assert.AreEqual("01", thirdTraining.Read());
            Assert.AreEqual("02", thirdTraining.Read());
            Assert.AreEqual("04", thirdTraining.Read());
            Assert.AreEqual("05", thirdTraining.Read());
            Assert.AreEqual("07", thirdTraining.Read());
            Assert.AreEqual("08", thirdTraining.Read());
            Assert.AreEqual("10", thirdTraining.Read());
            Assert.Null(thirdTraining.Read());

            var thirdTest = thirdTraining.GetTestSampleStream();

            Assert.AreEqual("03", thirdTest.Read());
            Assert.AreEqual("06", thirdTest.Read());
            Assert.AreEqual("09", thirdTest.Read());
            Assert.Null(thirdTest.Read());

            Assert.False(partitioner.HasNext);
        }

        [Test]
        public void TestEmptyDataSet() {
            var partitioner = new CrossValidationPartitioner<string>(new string[] {}, 2);

            Assert.True(partitioner.HasNext);
            Assert.Null(partitioner.Next().Read());

            Assert.True(partitioner.HasNext);
            Assert.Null(partitioner.Next().Read());

            Assert.False(partitioner.HasNext);

            try {
                partitioner.Next();

                Assert.Fail("Ups, hasn't thrown one!");
            } catch (Exception) {
                // expected
            }
        }

        [Test]
        public void TestFailSafety() {
            var partitioner = new CrossValidationPartitioner<String>(new[] {
                "01", "02", "03", "04"
            }, 4);

            // Test that iterator from previous partition fails
            // if it is accessed
            var firstTraining = partitioner.Next();
            Assert.AreEqual("02", firstTraining.Read());

            var secondTraining = partitioner.Next();

            try {
                firstTraining.Read();
                Assert.Fail();
            } catch (Exception) {}

            try {
                firstTraining.GetTestSampleStream();
                Assert.Fail();
            } catch (Exception) {}

            // Test that training iterator fails if there is a test iterator
            secondTraining.GetTestSampleStream();

            try {
                secondTraining.Read();
                Assert.Fail();
            } catch (Exception) {}

            // Test that test iterator from previous partition fails
            // if there is a new partition
            var thirdTraining = partitioner.Next();
            var thirdTest = thirdTraining.GetTestSampleStream();

            Assert.True(partitioner.HasNext);
            partitioner.Next();

            try {
                thirdTest.Read();
                Assert.Fail();
            } catch (Exception) {}
        }

        [Test]
        public void ToStringTest() {
            var value = new CrossValidationPartitioner<string>(new string[] {}, 10).ToString();
            Assert.AreEqual("At partition 1 of 10.", value);
        }
    }
}