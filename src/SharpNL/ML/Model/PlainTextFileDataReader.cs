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
    /// <summary>
    /// Represents a plain text data reader.
    /// </summary>
    public class PlainTextFileDataReader : IDataReader {
        private readonly StreamReader reader;


        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextFileDataReader"/> class using a input stream as source.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">inputStream</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="inputStream"/> was not readable.</exception>
        public PlainTextFileDataReader(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException(@"Stream was not readable.", "inputStream");

            reader = new StreamReader(inputStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextFileDataReader"/> class using a <paramref name="streamReader"/> as source.
        /// </summary>
        /// <param name="streamReader">The stream reader.</param>
        /// <exception cref="System.ArgumentNullException">streamReader</exception>
        public PlainTextFileDataReader(StreamReader streamReader) {
            if (streamReader == null)
                throw new ArgumentNullException("streamReader");

            reader = streamReader;
        }

        /// <summary>
        /// Reads a double value.
        /// </summary>
        /// <returns>System.Double.</returns>
        /// <exception cref="InvalidFormatException">
        /// Unable to convert to double the following line: Line
        /// or
        /// Unable to read a double value.
        /// </exception>
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

        /// <summary>
        /// Reads a int value.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <exception cref="InvalidFormatException">
        /// Unable to convert to int the following line:  + line
        /// or
        /// Unable to read a int value.
        /// </exception>
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

        /// <summary>
        /// Reads a string value.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ReadString() {
            return reader.ReadLine();
        }
    }
}