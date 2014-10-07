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

namespace SharpNL.Utility.Evaluation {
    /// <summary>
    /// Provides access to training and test partitions for n-fold cross validation.
    /// </summary>
    public class CrossValidationPartitioner<T> {
        private readonly IObjectStream<T> sampleStream;
        private TrainingSampleStream lastTrainingSampleStream;
        private int testIndex;

        #region + Properties .

        #region . HasNext .

        /// <summary>
        /// Gets a value indicating whether there are more partitions available.
        /// </summary>
        /// <value><c>true</c> if there are more partitions available; otherwise, <c>false</c>.</value>
        public bool HasNext {
            get { return testIndex < NumberOfPartitions; }
        }

        #endregion

        #region . NumberOfPartitions .
        /// <summary>
        /// Gets the number of partitions.
        /// </summary>
        /// <value>The number of partitions.</value>
        public int NumberOfPartitions { get; private set; }
        #endregion

        #endregion

        #region @ TestSampleStream .

        private class TestSampleStream : IObjectStream<T> {
            private readonly int numberOfPartitions;
            private readonly IObjectStream<T> sampleStream;
            private readonly int testIndex;
            private int index;
            private bool isPoisoned;

            internal TestSampleStream(IObjectStream<T> sampleStream, int numberOfPartitions, int testIndex) {
                this.numberOfPartitions = numberOfPartitions;
                this.sampleStream = sampleStream;
                this.testIndex = testIndex;
            }

            public void Dispose() {
                sampleStream.Dispose();
                isPoisoned = true;
            }

            public T Read() {
                if (isPoisoned)
                    throw new InvalidOperationException();

                // skip training samples
                while (index%numberOfPartitions != testIndex) {
                    sampleStream.Read();
                    index++;
                }

                index++;

                return sampleStream.Read();
            }

            public void Reset() {
                throw new NotSupportedException();
            }

            public void Poison() {
                isPoisoned = true;
            }
        }

        #endregion

        #region @ TrainingSampleStream .

        public class TrainingSampleStream : IObjectStream<T> {
            private readonly int numberOfPartitions;
            private readonly IObjectStream<T> sampleStream;
            private readonly int testIndex;
            private int index;
            private bool isPoisoned;
            private TestSampleStream testSampleStream;

            internal TrainingSampleStream(IObjectStream<T> sampleStream, int numberOfPartitions, int testIndex) {
                this.numberOfPartitions = numberOfPartitions;
                this.sampleStream = sampleStream;
                this.testIndex = testIndex;
            }

            #region . Dispose .

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose() {
                sampleStream.Dispose();
                Poison();
            }

            #endregion

            #region . Read .

            /// <summary>
            /// Returns the next object. Calling this method repeatedly until it returns ,
            /// null will return each object from the underlying source exactly once.
            /// </summary>
            /// <returns>
            /// The next object or null to signal that the stream is exhausted.
            /// </returns>
            public T Read() {
                if (testSampleStream != null || isPoisoned) {
                    throw new InvalidOperationException();
                }

                // If the test element is reached skip over it to not include it in
                // the training data
                if (index%numberOfPartitions == testIndex) {
                    sampleStream.Read();
                    index++;
                }

                index++;

                return sampleStream.Read();
            }

            #endregion

            #region . Reset .

            /// <summary>
            /// Repositions the stream at the beginning and the previously seen object 
            /// sequence will be repeated exactly. This method can be used to re-read the
            /// stream if multiple passes over the objects are required.
            /// </summary>
            public void Reset() {
                if (testSampleStream != null || isPoisoned) {
                    throw new InvalidOperationException();
                }
                index = 0;
                sampleStream.Reset();
            }

            #endregion

            #region . Poison .

            internal void Poison() {
                isPoisoned = true;
                if (testSampleStream != null)
                    testSampleStream.Poison();
            }

            #endregion

            #region . GetTestSampleStream .

            /// <summary>
            /// Retrieves the <see cref="T:IObjectStream{T}"/> over the test/evaluations elements and poisons this <see cref="TrainingSampleStream"/>.
            /// </summary>
            /// <returns>IObjectStream&lt;T&gt;.</returns>
            /// <exception cref="System.InvalidOperationException"></exception>
            /// <remarks>From now on calls to the hasNext and next methods are forbidden and will raise an <see cref="System.InvalidOperationException"/>.</remarks>
            public IObjectStream<T> GetTestSampleStream() {
                if (isPoisoned) {
                    throw new InvalidOperationException();
                }

                if (testSampleStream == null) {
                    sampleStream.Reset();
                    testSampleStream = new TestSampleStream(sampleStream, numberOfPartitions, testIndex);
                }

                return testSampleStream;
            }

            #endregion
        }

        #endregion

        public CrossValidationPartitioner(IObjectStream<T> inElements, int numberOfPartitions) {
            sampleStream = inElements;
            NumberOfPartitions = numberOfPartitions;
        }
        public CrossValidationPartitioner(IEnumerable<T> enumerable, int numberOfPartitions) : this(new CollectionObjectStream<T>(enumerable), numberOfPartitions) { }

        public TrainingSampleStream Next() {
            if (HasNext) {
                if (lastTrainingSampleStream != null)
                    lastTrainingSampleStream.Poison();

                sampleStream.Reset();

                lastTrainingSampleStream = new TrainingSampleStream(sampleStream, NumberOfPartitions, testIndex++);

                return lastTrainingSampleStream;
            }

            throw new InvalidOperationException("No such element.");
        }

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() {
            return string.Format("At partition {0} of {1}.", testIndex + 1, NumberOfPartitions);
        }
        #endregion

    }
}