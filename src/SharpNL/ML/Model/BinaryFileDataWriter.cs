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
    /// Provides Java compatible methods to write binary files.
    /// </summary>
    public class BinaryFileDataWriter : IDataWriter {
        private readonly sbyte[] buffer;
        private readonly Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFileDataWriter"/> class.
        /// </summary>
        /// <param name="outStream">The output stream.</param>
        /// <exception cref="System.ArgumentNullException">outStream</exception>
        /// <exception cref="System.ArgumentException">Stream was not writable.</exception>
        public BinaryFileDataWriter(Stream outStream) {
            if (outStream == null)
                throw new ArgumentNullException("outStream");

            if (!outStream.CanWrite)
                throw new ArgumentException(@"Stream was not writable.", "outStream");

            stream = outStream;
            buffer = new sbyte[8];
        }

        #region . Write(string) .
        /// <summary>
        /// Writes the specified string value.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">String too long.</exception>
        /// <remarks>The string length cannot exceed <see cref="ushort.MaxValue"/> (65535).</remarks>
        public void Write(string value) {
            var data = Encoding.UTF8.GetBytes(value);
            if (data.Length > ushort.MaxValue) {
                throw new ArgumentOutOfRangeException("value", @"String too long.");
            }
            Write((ushort) data.Length);
            Write(data, 0, data.Length);
        }
        #endregion

        #region . Write(double) .
        /// <summary>
        /// Writes the specified double value.
        /// </summary>
        /// <param name="value">The double value.</param>
        public void Write(double value) {
            Write(BitConverter.DoubleToInt64Bits(value));
        }
        #endregion

        #region . Write(int) .
        /// <summary>
        /// Writes the specified int value.
        /// </summary>
        /// <param name="value">The int value.</param>
        public void Write(int value) {
            buffer[0] = (sbyte) (value >> 0x18);
            buffer[1] = (sbyte) (value >> 0x10);
            buffer[2] = (sbyte) (value >> 8);
            buffer[3] = (sbyte) value;
            Write(buffer, 0, 4);
        }
        #endregion

        #region . Write(ushort) .
        private void Write(ushort value) {
            buffer[0] = (sbyte) (value >> 8);
            buffer[1] = (sbyte) value;
            Write(buffer, 0, 2);
        }
        #endregion

        #region . Write(long) .
        private void Write(long value) {
            buffer[0] = (sbyte)(value >> 0x38);
            buffer[1] = (sbyte)(value >> 0x30);
            buffer[2] = (sbyte)(value >> 40);
            buffer[3] = (sbyte)(value >> 0x20);
            buffer[4] = (sbyte)(value >> 0x18);
            buffer[5] = (sbyte)(value >> 0x10);
            buffer[6] = (sbyte)(value >> 8);
            buffer[7] = (sbyte)value;
            Write(buffer, 0, 8);
        }
        #endregion

        #region + Write(buffers) .
        private void Write(byte[] data, int offset, int count) {
            if (data == null) {
                throw new ArgumentNullException("data");
            }
            stream.Write(data, offset, count);
        }
        private void Write(sbyte[] data, int offset, int count) {
            if (data == null) {
                throw new ArgumentNullException("data");
            }
            var buffer2 = new byte[data.Length];
            for (var i = 0; i < data.Length; i++) {
                buffer2[i] = (byte)data[i];
            }
            stream.Write(buffer2, offset, count);
        }
        #endregion

        #region . Flush .
        /// <summary>
        /// Clears all buffers for the current <see cref="Stream"/> object and causes any buffered data to be written to the underlying device.
        /// </summary>
        public void Flush() {
            stream.Flush();
        }
        #endregion

        #region . Close .
        /// <summary>
        /// Closes the current <see cref="Stream"/> object and releases any resources (such as sockets and file handles) associated with it.
        /// </summary>
        public void Close() {
            stream.Close();
        }
        #endregion

    }
}