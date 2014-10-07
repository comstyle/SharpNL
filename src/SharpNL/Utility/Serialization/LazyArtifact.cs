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
using ICSharpCode.SharpZipLib.Zip;

namespace SharpNL.Utility.Serialization {
    /// <summary>
    /// Represents a artifact loaded into a temporary file or memory.
    /// </summary>
    /// <remarks>
    /// When the manifest file is not the first compressed file in the zip and the stream is not seekable,
    /// the artifacts need to be stored somewhere for later to be loaded. This class manages this possibility
    /// avoiding the seekable exception from the ZipInputStream.
    /// Note: Generally speaking, this only happens when the model is created using a compressor like WinRAR, 7Zip, etc.
    /// </remarks>
    public sealed class LazyArtifact : IDisposable {

        #region . MaxSizeInMemory .
        /// <summary>
        /// Gets or sets the maximum size of the artifact loaded into memory.
        /// The artifacts that exceed this value will be stored in a temporary file.
        /// </summary>
        /// <value>The maximum size of the artifact loaded into memory.</value>
        /// <remarks>The default value is 3145728 bytes (3mb).</remarks>
        public static uint MaxSizeInMemory { get; set; }

        static LazyArtifact() {
            MaxSizeInMemory = 3145728; // (1024 ^ 3) * 3 = 3mb
        }
        #endregion

        private readonly Stream dataStream;
        private bool disposed;

        #region . Constructor .
        internal LazyArtifact(ZipEntry entry, ZipInputStream inputStream) {

            if (entry.Size > MaxSizeInMemory) {
                var fileName = Path.GetTempPath() + Guid.NewGuid() + ".tmp";

                dataStream = new FileStream(
                    fileName,
                    FileMode.CreateNew,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    4096, // The default buffer size is 4096.
                    FileOptions.DeleteOnClose);

            } else {
                dataStream = new MemoryStream();
            }

            inputStream.CopyTo(dataStream);

            dataStream.Seek(0, SeekOrigin.Begin);

            Name = entry.Name;
        }
        #endregion

        #region + Properties .

        #region . Name .
        /// <summary>
        /// Gets the artifact name.
        /// </summary>
        /// <value>The artifact name.</value>
        public string Name { get; private set; }
        #endregion

        #region . Data .
        /// <summary>
        /// Gets the data stream.
        /// </summary>
        /// <value>The data stream.</value>
        public Stream Data {
            get {
                if (!disposed)
                    return dataStream;

                throw new ObjectDisposedException("LazyArtifact");
            }
        }
        #endregion

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            if (!disposed) {
                dataStream.Dispose();
                disposed = true;
            }
        }
        #endregion

    }
}