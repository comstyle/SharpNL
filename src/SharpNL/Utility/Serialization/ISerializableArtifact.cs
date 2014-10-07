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

namespace SharpNL.Utility.Serialization {
    /// <summary>
    /// Represents a serializable artifact.
    /// </summary>
    /// <remarks>The artifact object must have a default constructor, otherwise an exception will occur.</remarks>
    public interface ISerializableArtifact {

        /// <summary>
        /// Gets the artifact serializer.
        /// </summary>
        /// <returns>The corresponding artifact serializer type.</returns>
        /// <remarks>The specified type must implement the <see cref="T:IArtifactSerializer{T}"/> interface.</remarks>
        Type GetArtifactSerializer();

    }
}