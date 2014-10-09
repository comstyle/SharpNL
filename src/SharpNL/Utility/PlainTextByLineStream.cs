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
using System.Runtime.CompilerServices;
using System.Text;

namespace SharpNL.Utility {
    /// <summary>
    /// Reads a plain text file and return each line as a <see cref="T:string"/> object.
    /// </summary>
    internal class PlainTextByLineStream : IObjectStream<string> {
        private readonly IInputStreamFactory streamFactory;
        private readonly Encoding encoding;

        private int currentLine;
        private StreamReader reader;
        private StringReader stringReader;

        public PlainTextByLineStream(StringReader reader) {
            currentLine = 0;
            stringReader = reader;
        }

        public PlainTextByLineStream(Stream inputStream) : this(inputStream, Encoding.UTF8) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="PlainTextByLineStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoderName">The code page name of the preferred encoding.</param>
        /// <exception cref="System.ArgumentNullException">
        /// inputStream
        /// or
        /// encoderName
        /// </exception>
        /// <exception cref="System.ArgumentException">The specified stream is not readable.</exception>
        /// <exception cref="System.ArgumentException">
        /// <code>encoderName</code> is not a valid code page name.
        /// or
        /// The code page indicated by name is not supported by the underlying platform.
        /// </exception>
        public PlainTextByLineStream(Stream inputStream, string encoderName) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (string.IsNullOrEmpty(encoderName)) {
                throw new ArgumentNullException("encoderName");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException("The specified stream is not readable.");
            }
            currentLine = 0;
            reader = new StreamReader(inputStream, Encoding.GetEncoding(encoderName));
        }


        public PlainTextByLineStream(Stream inputStream, Encoding inputEncoding) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (inputEncoding == null) {
                throw new ArgumentNullException("inputEncoding");
            }

            if (!inputStream.CanRead) {
                throw new ArgumentException("The specified stream is not readable.");
            }

            currentLine = 0;
            reader = new StreamReader(inputStream, inputEncoding);
            encoding = inputEncoding;
        }


        public PlainTextByLineStream(IInputStreamFactory streamFactory) : this(streamFactory, Encoding.UTF8) { }
        public PlainTextByLineStream(IInputStreamFactory streamFactory, Encoding encoding) {
            if (streamFactory == null)
                throw new ArgumentNullException("streamFactory");

            currentLine = 0;
            this.streamFactory = streamFactory;
            this.encoding = encoding;

            reader = encoding != null ? 
                new StreamReader(streamFactory.CreateInputStream(), encoding) :
                new StreamReader(streamFactory.CreateInputStream());

        }

        #region . CurrentLine .

        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <value>The current line.</value>
        public int CurrentLine {
            get { return currentLine; }
        }

        #endregion

        #region . Progress .

        /// <summary>
        /// Gets the current progress on the stream.
        /// </summary>
        /// <value>The progress.</value>
        /// <remarks>The progress depends by the buffer size of the <see cref="T:StreamReader"/>.</remarks>
        public double Progress {
            get {
                if (reader != null && reader.BaseStream.Length > 0) {
                    return reader.BaseStream.Position / (double)reader.BaseStream.Length;
                }
                return -1;
            }
        }

        #endregion

        #region . Dispose .

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (reader != null)
                reader.Dispose();
           
            if (stringReader != null)
                stringReader.Dispose();
        }

        #endregion

        #region . Read .

        /// <summary>
        /// Returns the next line. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public string Read() {

            var line = stringReader != null
                ? stringReader.ReadLine()
                : reader.ReadLine();

            if (line != null) {
                currentLine++;
            }

            return line;
        }

        #endregion

        #region . Reset .

        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public virtual void Reset() {
            if (streamFactory != null) {
                try {
                    if (reader != null)
                        reader.Dispose();
                } catch (ObjectDisposedException) { }

                reader = encoding != null ? 
                    new StreamReader(streamFactory.CreateInputStream(), encoding) : 
                    new StreamReader(streamFactory.CreateInputStream());

                return;
            }

            if (reader != null && reader.BaseStream.CanSeek) {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.DiscardBufferedData();
                return;
            }

            throw new NotSupportedException();
        }

        #endregion

    }
}