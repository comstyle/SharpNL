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

namespace SharpNL.Utility.Serialization {

    /// <summary>
    /// A delegate to serialize a <paramref name="artifact"/> into the <paramref name="outputStream"/> stream object.
    /// </summary>
    /// <param name="artifact">The artifact to be serialized.</param>
    /// <param name="outputStream">The output stream.</param>
    public delegate void SerializeDelegate(object artifact, Stream outputStream);

    /// <summary>
    /// A delegate to deserialize a artifact from a <see cref="Stream"/> object.
    /// </summary>
    /// <param name="inputStream">The input stream.</param>
    /// <returns>The deserialized artifact.</returns>
    public delegate object DeserializeDelegate(Stream inputStream);

}