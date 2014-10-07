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
using System.Text;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Provides Java compatible methods to read binary files.
    /// </summary>
    public class BinaryFileDataReader : IDataReader {

        private readonly Stream stream;
        private readonly byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileDataReader"/> class.
        /// </summary>
        /// <param name="inStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException">inStream</exception>
        /// <exception cref="System.ArgumentException">Stream was not readable.</exception>
        public BinaryFileDataReader(Stream inStream) {
            if (inStream == null)
                throw new ArgumentNullException("inStream");

            if (!inStream.CanRead)
                throw new ArgumentException(@"Stream was not readable.", "inStream");

            stream = inStream;
            buffer = new byte[8];
        }

        #region . ReadDouble .
        /// <summary>
        /// Reads an 8-byte floating point value from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>An 8-byte floating point value read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        public double ReadDouble() {
            return BitConverter.Int64BitsToDouble(ReadLong());
        }
        #endregion

        #region . ReadInt .
        /// <summary>
        /// Reads a 4-byte signed integer from the current stream and advances the current position of the stream by four bytes.
        /// </summary>
        /// <returns>A 4-byte signed integer read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        public int ReadInt() {
            if (ReadBytes(4) < 0) {
                throw new EndOfStreamException("The end of the stream is reached.");
            }
            return (((((buffer[0] & 0xff) << 0x18) |
                      ((buffer[1] & 0xff) << 0x10)) | 
                      ((buffer[2] & 0xff) << 8)) |
                       (buffer[3] & 0xff));
        }
        #endregion

        #region . ReadString .
        /// <summary>
        /// Reads a string from the current stream. 
        /// The string is prefixed with the length, encoded as an ushort.
        /// </summary>
        /// <returns>The string being read.</returns>
        /// <remarks>This method is compatible with Java ReadUTF method.</remarks>
        /// <exception cref="System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        public string ReadString() {
            var size = ReadUShort();
            var data = new byte[size];
            ReadBytes(data, 0, data.Length);
            return Encoding.UTF8.GetString(data);
        }
        #endregion

        #region . ReadUShort .
        /// <summary>
        /// Reads a 2-byte unsigned integer from the current stream using little-endian encoding and advances the position of the stream by two bytes.
        /// </summary>
        /// <returns>A 2-byte unsigned integer read from this stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        private int ReadUShort() {
            if (ReadBytes(2) < 0) {
                throw new EndOfStreamException("The end of the stream is reached.");
            }
            return (ushort)(((buffer[0] & 0xff) << 8) | (buffer[1] & 0xff));
        }
        #endregion

        #region + Read(buffers) .

        private void ReadBytes(byte[] data, int offset, int length) {
            if (length < 0) {
                throw new IndexOutOfRangeException();
            }
            if (length != 0) {
                if (data == null) {
                    throw new ArgumentNullException("data");
                }
                if ((offset < 0) || (offset > (data.Length - length))) {
                    throw new ArgumentOutOfRangeException("offset");
                }
                while (length > 0) {
                    var num = stream.Read(data, offset, length);
                    if (num == 0) {
                        throw new EndOfStreamException();
                    }
                    offset += num;
                    length -= num;
                }
            }
        }
        private int ReadBytes(int count) {
            var offset = 0;
            while (offset < count) {
                var num2 = stream.Read(buffer, offset, count);
                if (num2 == 0) {
                    return num2;
                }
                offset += num2;
            }
            return offset;
        }

        #endregion

        #region . ReadLong .
        /// <summary>
        /// Reads an 8-byte signed integer from the current stream and advances the current position of the stream by eight bytes.
        /// </summary>
        /// <returns>An 8-byte signed integer read from the current stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">The end of the stream is reached.</exception>
        private long ReadLong() {
            if (ReadBytes(8) < 0) {
                throw new EndOfStreamException("The end of the stream is reached.");
            }
            var num = ((((buffer[0] & 0xff) << 0x18) | 
                        ((buffer[1] & 0xff) << 0x10)) | 
                        ((buffer[2] & 0xff) << 8)) |
                         (buffer[3] & 0xff);

            var num2 = ((((buffer[4] & 0xff) << 0x18) | 
                         ((buffer[5] & 0xff) << 0x10)) | 
                         ((buffer[6] & 0xff) << 8)) |
                          (buffer[7] & 0xff);

            return ((num & 0xffffffffL) << 0x20) | (num2 & 0xffffffffL);
        }
        #endregion

    }
}