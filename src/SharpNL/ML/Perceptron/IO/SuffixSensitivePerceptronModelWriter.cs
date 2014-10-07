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

using System.IO;
using ICSharpCode.SharpZipLib.GZip;
using SharpNL.ML.Model;

namespace SharpNL.ML.Perceptron.IO {
    /// <summary>
    /// A writer for perceptron models which inspects the filename and invokes the
    /// appropriate <see cref="PerceptronModelWriter"/> depending on the filename's suffixes.
    /// </summary>
    /// <remarks>
    ///<para>
    /// The following assumption are made about suffixes:
    /// 
    ///  .gz  - file is GZipped (must be last suffix)
    ///  .bin - file is binary
    ///  .txt - file is plain text (default)
    /// 
    /// </para>
    /// </remarks>
    public class SuffixSensitivePerceptronModelWriter : PerceptronModelWriter {
        private readonly AbstractModelWriter suffixAppropriateWriter;

        public SuffixSensitivePerceptronModelWriter(AbstractModel model, string fileName) : base(model) {
            string file;
            Stream output;
            if (fileName.EndsWith(".gz")) {
                output = new GZipOutputStream(new FileStream(fileName, FileMode.Create));
                file = fileName.Substring(0, fileName.Length - 3);
            } else {
                file = fileName;
                output = new FileStream(fileName, FileMode.Create);
            }

            if (file.EndsWith(".bin")) {
                suffixAppropriateWriter = new BinaryPerceptronModelWriter(model, output);
            } else {
                // default is ".txt"
                suffixAppropriateWriter = new PlainTextPerceptronModelWriter(model, output);
            }
        }

        public override void Close() {
            suffixAppropriateWriter.Close();
        }

        public override void Write(int value) {
            suffixAppropriateWriter.Write(value);
        }

        public override void Write(string value) {
            suffixAppropriateWriter.Write(value);
        }

        public override void Write(double value) {
            suffixAppropriateWriter.Write(value);
        }
    }
}