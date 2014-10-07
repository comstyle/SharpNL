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
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    public class PlainTextFileDataReader : IDataReader {
        private readonly StreamReader reader;


        public PlainTextFileDataReader(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException(@"Stream was not readable.", "inputStream");

            reader = new StreamReader(inputStream);
        }

        public PlainTextFileDataReader(StreamReader streamReader) {
            if (streamReader == null)
                throw new ArgumentNullException("streamReader");

            reader = streamReader;
        }

        public double ReadDouble() {
            var line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line)) {
                double value;
                if (!double.TryParse(line, out value)) {
                    throw new InvalidFormatException("Unable to convert to double the following line: " + line);
                }
                return value;
            }
            throw new InvalidFormatException("Unable to read a double value.");
        }

        public int ReadInt() {
            var line = reader.ReadLine();
            if (!string.IsNullOrEmpty(line)) {
                int value;
                if (!int.TryParse(line, out value)) {
                    throw new InvalidFormatException("Unable to convert to int the following line: " + line);
                }
                return value;
            }
            throw new InvalidFormatException("Unable to read a int value.");
        }

        public string ReadString() {
            return reader.ReadLine();
        }
    }
}