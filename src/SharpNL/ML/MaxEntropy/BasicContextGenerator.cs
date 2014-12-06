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

namespace SharpNL.ML.MaxEntropy {
    /// <summary>
    /// Generate contexts for maxent decisions, assuming that the input given to 
    /// the <see cref="GetContext"/> method is a string containing contextual
    /// predicates separated by spaces.
    /// </summary>
    public class BasicContextGenerator : IContextGenerator {

        private readonly string separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicContextGenerator"/> class using a space as separator.
        /// </summary>
        public BasicContextGenerator() : this(" ") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicContextGenerator"/> class using a specified <paramref name="separator"/> value.
        /// </summary>
        /// <param name="separator">The separator.</param>
        public BasicContextGenerator(string separator) {
            this.separator = separator;
        }

        /// <summary>
        /// Builds up the list of contextual predicates given an Object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A string array with the contextual predicates.</returns>
        /// <remarks>The obj type must be a string, otherwise this method will throw a exception.</remarks>
        /// <exception cref="System.NotSupportedException">The object type X is not supported.</exception>
        public string[] GetContext(object obj) {
            var str = obj as string;
            if (str != null)
                return str.Split(new [] {separator}, StringSplitOptions.RemoveEmptyEntries);

            throw new NotSupportedException("The object type " + obj.GetType().Name + " is not supported.");
        }
    }
}