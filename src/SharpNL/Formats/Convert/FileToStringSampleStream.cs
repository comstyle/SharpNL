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
using SharpNL.Utility;

namespace SharpNL.Formats.Convert {
    public class FileToStringSampleStream : FilterObjectStream<FileInfo, string> {
        private readonly Encoding encoding;


        public FileToStringSampleStream(IObjectStream<FileInfo> samples) : this(samples, Encoding.UTF8) { }

        public FileToStringSampleStream(IObjectStream<FileInfo> samples, Encoding encoding) : base(samples) {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            this.encoding = encoding;
        }

        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns ,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public override string Read() {

            var fileInfo = Samples.Read();

            if (fileInfo != null) {
                if (fileInfo.Exists)
                    return File.ReadAllText(fileInfo.FullName, encoding);

                throw new FileNotFoundException("File not found!", fileInfo.FullName);
            }

            return null;
        }
    }
}