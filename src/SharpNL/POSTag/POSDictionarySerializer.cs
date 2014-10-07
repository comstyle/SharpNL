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
using SharpNL.Utility.Serialization;

namespace SharpNL.POSTag {
    internal class POSDictionarySerializer : IArtifactSerializer {
        /// <summary>
        /// Deserializes the artifact using the specified input stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <exception cref="ArgumentNullException">inputStream</exception>
        /// <exception cref="ArgumentException">Stream was not readable.</exception>
        public object Deserialize(Stream inputStream) {
            //return POSDictionary.Create()
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the the artifact into the specified stream.
        /// </summary>
        /// <param name="artifact">The artifact.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <exception cref="ArgumentNullException">outputStream</exception>
        /// <exception cref="ArgumentException">Stream was not writable.</exception>
        public void Serialize(object artifact, Stream outputStream) {
            throw new NotImplementedException();
        }
    }
}