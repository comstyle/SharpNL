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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SharpNL.Utility;

namespace SharpNL.Tokenize.Language.en {
    /// <summary>
    /// Class which produces an <see cref="IEnumerator{T}"/> from a file of space delimited token.
    /// This class uses a number of English-specific heuristics to un-separate tokens which
    /// are typically found together in text.
    /// </summary>
    public class TokenSampleStream : IEnumerator<TokenSample> {
        private static readonly Regex alphaNumeric;
        private readonly StreamReader reader;
        private readonly long startPos;
        private bool evenq = true;
        private string line;


        static TokenSampleStream() {
            alphaNumeric = new Regex("[A-Za-z0-9]", RegexOptions.Compiled);
        }

        public TokenSampleStream(Stream inputStream) {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");

            if (!inputStream.CanRead)
                throw new ArgumentException(@"The stream is not readable.", "inputStream");

            startPos = inputStream.Position;

            reader = new StreamReader(inputStream, Encoding.UTF8);

            line = reader.ReadLine();
        }

        #region + Properties .

        #region . Current .

        /// <summary>
        /// Gets the <see cref="TokenSample"/> in the collection at the current position of the <see cref="TokenSampleStream"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="TokenSample"/> in the collection at the current position of the <see cref="TokenSampleStream"/>.
        /// </returns>
        public TokenSample Current { get; private set; }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        object IEnumerator.Current {
            get { return Current; }
        }

        #endregion

        #region . HasNext .
        /// <summary>
        /// Gets a value indicating whether this instance has a next <see cref="TokenSample"/>.
        /// </summary>
        /// <value><c>true</c> if this instance has a next <see cref="TokenSample"/>; otherwise, <c>false</c>.</value>
        public bool HasNext {
            get { return line != null; }
        }
        #endregion

        #endregion

        #region . Dispose .
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            reader.Dispose();
        }

        #endregion

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public bool MoveNext() {
            if (line != null) {

                var tokens = Regex.Split(line, "\\s+");
                if (tokens.Length == 0)
                    evenq = true;

                var sb = new StringBuilder(line.Length);
                var spans = new List<Span>();

                var length = 0;
                for (var ti = 0; ti < tokens.Length; ti++) {
                    var token = tokens[ti];
                    var lastToken = ti - 1 >= 0 ? tokens[ti - 1] : "";
                    if (token == "-LRB-") {
                        token = "(";
                    } else if (token == "-LCB-") {
                        token = "{";
                    } else if (token == "-RRB-") {
                        token = ")";
                    } else if (token == "-RCB-") {
                        token = "}";
                    }
                    if (sb.Length == 0) {} else if (!alphaNumeric.IsMatch(token) || token.StartsWith("'") ||
                                                    token.Equals("n't", StringComparison.InvariantCultureIgnoreCase)) {
                        if ((token.Equals("``") || token.Equals("--") || token.Equals("$") ||
                             token.Equals("(") || token.Equals("&") || token.Equals("#") ||
                             (token.Equals("\"") && (evenq && ti != tokens.Length - 1)))
                            && (!lastToken.Equals("(") || !lastToken.Equals("{"))) {
                            //System.out.print(" "+token);
                            length++;
                        }
                    } else {
                        if (lastToken.Equals("``") || (lastToken.Equals("\"") && !evenq) || lastToken.Equals("(") ||
                            lastToken.Equals("{")
                            || lastToken.Equals("$") || lastToken.Equals("#")) {
                            //System.out.print(token);
                        } else {
                            //System.out.print(" "+token);
                            length++;
                        }
                    }
                    if (token.Equals("\"")) {
                        if (ti == tokens.Length - 1) {
                            evenq = true;
                        } else {
                            evenq = !evenq;
                        }
                    }
                    if (sb.Length < length) {
                        sb.Append(" ");
                    }
                    sb.Append(token);
                    spans.Add(new Span(length, length + token.Length));
                    length += token.Length;
                }
                //System.out.println();


                try {
                    line = reader.ReadLine();
                } catch {
                    line = null;
                }
                Current = new TokenSample(sb.ToString(), spans.ToArray());
                return true;
            }

            Current = null;
            return false;
        }

        #region . Reset .

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
        public void Reset() {
            if (!reader.BaseStream.CanSeek)
                throw new NotSupportedException("The input stream does not support seeking.");

            reader.BaseStream.Seek(startPos, SeekOrigin.Begin);
        }

        #endregion
    }
}