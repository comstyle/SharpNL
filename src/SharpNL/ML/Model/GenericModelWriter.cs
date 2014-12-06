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
using System.IO;
using SharpNL.ML.MaxEntropy.IO;
using SharpNL.ML.Perceptron.IO;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a generic model writer.
    /// </summary>
    public class GenericModelWriter : AbstractModelWriter {

        /// <summary>
        /// The model writer
        /// </summary>
        private readonly AbstractModelWriter writer;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericModelWriter" /> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.InvalidOperationException">Invalid model type.</exception>
        public GenericModelWriter(AbstractModel model, Stream outputStream) {
            switch (model.ModelType) {
                case ModelType.Maxent:
                    writer = new BinaryGISModelWriter(model, outputStream);
                    break;
                case ModelType.MaxentQn:
                    writer = new BinaryQNModelWriter(model, outputStream);
                    break;
                case ModelType.Perceptron:
                    writer = new BinaryPerceptronModelWriter(model, outputStream);
                    break;
                default:
                    throw new InvalidOperationException("Invalid model type");
            }
        }
        #endregion

        #region + Write .

        /// <summary>
        /// Writes the specified string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        public override void Write(string value) {
            writer.Write(value);
        }
        /// <summary>
        /// Writes the specified int32 value.
        /// </summary>
        /// <param name="value">The int32 value.</param>
        public override void Write(int value) {
            writer.Write(value);
        }
        /// <summary>
        /// Writes the specified double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public override void Write(double value) {
            writer.Write(value);
        }

        #endregion

        #region . Close .
        /// <summary>
        /// Closes this writer instance.
        /// </summary>
        public override void Close() {
            writer.Close();
        }
        #endregion

        #region . Persist .
        /// <summary>
        /// Persists this instance.
        /// </summary>
        public override void Persist() {
            writer.Persist();
        }
        #endregion

        #region . Writer .
        /// <summary>
        /// Gets the model writer.
        /// </summary>
        /// <value>The model writer.</value>
        protected AbstractModelWriter Writer {
            get { return writer; }
        }
        #endregion

    }
}