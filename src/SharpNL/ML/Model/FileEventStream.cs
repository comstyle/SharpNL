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
using SharpNL.Java;
using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Class for using a file of events as an event stream. The format of the file is one event per line with
    /// each line consisting of outcome followed by contexts (space delimited).
    /// </summary>
    public class FileEventStream : IObjectStream<Event> {

        protected StreamReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="inputStream"/></exception>
        /// <exception cref="System.ArgumentException">The stream is not readable.</exception>
        public FileEventStream(Stream inputStream)
            : this(inputStream, Encoding.UTF8) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileEventStream"/> class.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/>
        /// or
        /// <paramref name="encoding"/>
        /// </exception>
        /// <exception cref="System.ArgumentException">The stream is not readable.</exception>
        public FileEventStream(Stream inputStream, Encoding encoding) {
            if (inputStream == null) {
                throw new ArgumentNullException("inputStream");
            }
            if (!inputStream.CanRead) {
                throw new ArgumentException(@"The stream is not readable.", "inputStream");
            }

            if (encoding == null)
                throw new ArgumentNullException("encoding");

            reader = new StreamReader(inputStream, encoding);
        }

        public FileEventStream(string fileName) : this(fileName, Encoding.UTF8) { }
        public FileEventStream(string fileName, Encoding encoding) {
            reader = new StreamReader(fileName, encoding);
        }


        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose() {
            reader.Close();
            reader.Dispose();
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
        public virtual Event Read() {
            string line = reader.ReadLine();

            if (line == null) {
                return null;
            }

            var st = new StringTokenizer(line);
            string outcome = st.NextToken;
            var count = st.CountTokens;
            var context = new string[count];

            for (int i = 0; i < count; i++) {
                context[i] = st.NextToken;
            }
            return new Event(outcome, context);
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        public virtual void Reset() {
            throw new NotSupportedException();
        }
        #endregion

        #region . ToLine .
        /// <summary>
        /// Generates a string representing the specified event.
        /// </summary>
        /// <param name="ev">The event for which a string representation is needed.</param>
        /// <returns>A string representing the specified event.</returns>
        /// <exception cref="System.ArgumentNullException">ev</exception>
        public static string ToLine(Event ev) {
            if (ev == null) {
                throw new ArgumentNullException("ev");
            }
            return string.Format("{0} {1}{2}", ev.Outcome, string.Join(" ", ev.Context), Environment.NewLine);
        }
        #endregion

    }
}