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
using System.Collections.Generic;
using System.IO;
using SharpNL.Utility;

namespace SharpNL.Formats {
    /// <summary>
    /// The directory sample stream scans a directory (recursively) for plain text
    /// files and outputs each file as a <see cref="FileInfo"/> object.
    /// </summary>
    public class DirectorySampleStream : IObjectStream<FileInfo> {

        private readonly DirectoryInfo[] directories;
        private readonly string searchPattern;
        private readonly bool recursive;

        private Stack<DirectoryInfo> stackDirectories;
        private Stack<FileInfo> stackFiles;

        #region + Constructors .
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySampleStream"/> with the given directories.
        /// </summary>
        /// <param name="directories">The directories.</param>
        /// <param name="searchPattern">The search string, such as "*.txt".</param>
        /// <param name="recursive">if set to <c>true</c> searches the directories recursively.</param>
        public DirectorySampleStream(DirectoryInfo[] directories, string searchPattern, bool recursive) {
            if (directories == null)
                throw new ArgumentNullException("directories");

            if (directories.Length == 0)
                throw new ArgumentOutOfRangeException("directories");

            this.directories = directories;
            this.searchPattern = searchPattern;
            this.recursive = recursive;

            stackFiles = new Stack<FileInfo>();
            stackDirectories = new Stack<DirectoryInfo>();

            foreach (var directory in directories) {
                stackDirectories.Push(directory);
            }           
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySampleStream"/> with a given directory
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="recursive">if set to <c>true</c> searches the directory recursively.</param>
        public DirectorySampleStream(DirectoryInfo directory, string searchPattern, bool recursive)
            : this(new[] { directory }, searchPattern, recursive) {

        }
        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {

        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns,
        /// null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>
        /// The next object or null to signal that the stream is exhausted.
        /// </returns>
        public FileInfo Read() {

            while (stackFiles.Count == 0 && stackDirectories.Count > 0) {
                var dir = stackDirectories.Pop();

                if (recursive) {
                    var subDirectories = dir.GetDirectories();
                    foreach (var subDir in subDirectories) {
                        stackDirectories.Push(subDir);
                    }
                }

                var files = string.IsNullOrWhiteSpace(searchPattern)
                    ? dir.GetFiles()
                    : dir.GetFiles(searchPattern);

                foreach (var file in files) {
                    stackFiles.Push(file);
                }
            }

            return stackFiles.Count > 0 ? stackFiles.Pop() : null;
        }
        #endregion

        #region . Reset .
        /// <summary>
        /// Repositions the stream at the beginning and the previously seen object 
        /// sequence will be repeated exactly. This method can be used to re-read the
        /// stream if multiple passes over the objects are required.
        /// </summary>
        public void Reset() {
            stackFiles = new Stack<FileInfo>();
            stackDirectories = new Stack<DirectoryInfo>(directories);
        }
        #endregion

    }
}