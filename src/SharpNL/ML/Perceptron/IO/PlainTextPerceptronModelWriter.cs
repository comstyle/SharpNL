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
using System.Globalization;
using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using SharpNL.ML.Model;

namespace SharpNL.ML.Perceptron.IO {
    /// <summary>
    /// Model writer that saves models in plain text format.
    /// </summary>
    public class PlainTextPerceptronModelWriter : PerceptronModelWriter {
        public StreamWriter writer;

        /// <summary>
        /// Constructor which takes a <see cref="PerceptronModel"/> and a File and prepares itself to
        /// write the model to that file. Detects whether the file is GZipped or not based on whether
        /// the suffix contains ".gz".
        /// </summary>
        /// <param name="model">The PerceptronModel which is to be persisted.</param>
        /// <param name="fileName">The filename in which the model is to be persisted.</param>
        public PlainTextPerceptronModelWriter(AbstractModel model, string fileName) : base(model) {
            writer = fileName.EndsWith(".gz") ?
                new StreamWriter(new GZipOutputStream(new FileStream(fileName, FileMode.Create))) : 
                new StreamWriter(new FileStream(fileName, FileMode.Create));
        }

        /// <summary>
        /// Constructor which takes a <see cref="PerceptronModel"/> and a <see cref="Stream"/> and prepares
        /// itself to write the model to that writer.
        /// </summary>
        /// <param name="model">The <seealso cref="PerceptronModel"/> which is to be persisted.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">outputStream</exception>
        /// <exception cref="System.ArgumentException">Stream was not writable.</exception>
        public PlainTextPerceptronModelWriter(AbstractModel model, Stream outputStream) : base(model) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");

            if (!outputStream.CanWrite)
                throw new ArgumentException(@"Stream was not writable.", "outputStream");

            writer = new StreamWriter(outputStream);
        }

        public override void Close() {
            writer.Flush();
            writer.Close();
        }

        public override void Write(int value) {
            writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }

        public override void Write(string value) {
            writer.WriteLine(value);
        }

        public override void Write(double value) {
            writer.WriteLine(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}