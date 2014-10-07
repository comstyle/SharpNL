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

namespace SharpNL.Java {
    /// <summary>
    /// Implements a StringTokenizer class for splitting a string into substrings using a set of delimiters.
    /// </summary>
    internal class StringTokenizer : IEnumerable<string> {
        public const string DefaultDelimiters = " \t\n\r\f";

        private readonly string delimiters = DefaultDelimiters;
        private readonly string empty = String.Empty;
        private int index;
        private string[] tokens;

        #region + Constructors .

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the <see cref="F:DefaultDelimiters">default delimiters</see>.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <exception cref="System.NullReferenceException">Thrown when the passed string is <c>null</c></exception>
        public StringTokenizer(string str) {
            Tokenize(str, false, false);
        }

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the given delimiters.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiters">The delimiters used to tokenize the string (each <see cref="!:char"/> will be used as a delimiter).</param>
        /// <exception cref="System.NullReferenceException">Thrown when the passed string is <c>null</c></exception>
        public StringTokenizer(string str, string delimiters) {
            if (delimiters != null) this.delimiters = delimiters;
            Tokenize(str, false, false);
        }

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the given delimiters.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiters">The delimiters used to tokenize the string.</param>
        public StringTokenizer(string str, params char[] delimiters) {
            if (delimiters != null) this.delimiters = new string(delimiters);
            Tokenize(str, false, false);
        }

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the given delimiters and optionally returning them as tokens.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiters">The delimiters used to tokenize the string (each <see cref="!:char"/> will be used as a delimiter).</param>
        /// <param name="returnDelimiters">If set to <c>true</c> the encountered delimiters will also be returned as tokens.</param>
        /// <exception cref="System.NullReferenceException">Thrown when the passed string is <c>null</c></exception>
        public StringTokenizer(string str, string delimiters, bool returnDelimiters) {
            if (delimiters != null) this.delimiters = delimiters;
            Tokenize(str, returnDelimiters, false);
        }

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the given delimiters,
        /// optionally returning them as tokens. Also empty tokens may be returned using the <see cref="!:String.Empty"/> string.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiters">The delimiters used to tokenize the string (each <see cref="!:char"/> will be used as a delimiter).</param>
        /// <param name="returnDelimiters">If set to <c>true</c> the encountered delimiters will also be returned as tokens.</param>
        /// <param name="returnEmpty">If set to <c>true</c> empty tokens will also be returned.</param>
        /// <exception cref="System.NullReferenceException">Thrown when the passed string is <c>null</c></exception>
        public StringTokenizer(string str, string delimiters, bool returnDelimiters, bool returnEmpty) {
            if (delimiters != null) this.delimiters = delimiters;
            Tokenize(str, returnDelimiters, returnEmpty);
        }

        /// <summary>
        /// Constructs a string tokenizer for the specified string using the given delimiters,
        /// optionally returning them as tokens. Also empty tokens may be returned using the <paramref name="empty"/> string.
        /// </summary>
        /// <param name="str">The string to be tokenized.</param>
        /// <param name="delimiters">The delimiters used to tokenize the string (each <see cref="!:char"/> will be used as a delimiter).</param>
        /// <param name="returnDelimiters">If set to <c>true</c> the encountered delimiters will also be returned as tokens.</param>
        /// <param name="returnEmpty">If set to <c>true</c> empty tokens will also be returned.</param>
        /// <param name="empty">The string to be returned as an empty token.</param>
        /// <exception cref="System.NullReferenceException">Thrown when the passed string is <c>null</c></exception>
        public StringTokenizer(string str, string delimiters, bool returnDelimiters, bool returnEmpty, string empty) {
            if (delimiters != null) this.delimiters = delimiters;
            this.empty = empty;
            Tokenize(str, returnDelimiters, returnEmpty);
        }

        #endregion

        #region + Properties .

        #region . Count .
        /// <summary>
        /// Gets the total number of tokens extracted.
        /// </summary>
        /// <remarks>
        /// <see cref="!:Equivalent not available in Java!"/>
        /// This property returns the total number of extracted tokens,
        /// contrary to <see cref="P:CountTokens"/>.
        /// </remarks>
        /// <value>The number of tokens extracted.</value>
        /// <seealso cref="P:StringTokenizer.CountTokens"/>
        public int Count {
            get { return tokens.Length; }
        }
        #endregion

        #region . CountTokens .
        /// <summary>
        /// Counts the <see cref="!:remaining"/> tokens - the number of times the
        /// <see cref="P:NextToken"/> property can be used before it throws an exception.
        /// </summary>
        /// <value>The number of remaining tokens.</value>
        /// <seealso cref="P:Count"/>
        public int CountTokens {
            get { return tokens.Length - index; }
        }
        #endregion

        #region . EmptyString .
        /// <summary>
        /// Gets the currently set string for empty tokens.
        /// </summary>
        /// <remarks>Default is <c>System.String.Empty</c></remarks>
        /// <value>The empty token string.</value>
        public string EmptyString {
            get { return empty; }
        }
        #endregion

        #region . HasMoreTokens .
        /// <summary>
        /// Tests if there are more tokens available from this tokenizer's string.
        /// If this method returns <c>true</c>, then a subsequent
        /// use of the <see cref="P:NextToken" /> property will successfully return a token.
        /// </summary>
        /// <value><c>true</c> if more tokens are available; otherwise <c>false</c>.</value>
        public bool HasMoreTokens {
            get { return index < tokens.Length; }
        }
        #endregion

        #region . NextToken .
        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <value>The next token.</value>
        /// <exception cref="System.IndexOutOfRangeException">Thrown when trying to get a token which doesn't exist.
        /// Usually caused by not checking if the <see cref="P:HasMoreTokens"/> property returns <c>true</c> before trying to get the next token.</exception>
        public string NextToken {
            get { return tokens[index++]; }
        }
        #endregion

        #region . this .
        /// <summary>
        /// Gets the token with the specified index from the tokenizer without moving the current position index.
        /// </summary>
        /// <remarks><see cref="!:Equivalent not available in Java!"/></remarks>
        /// <param name="index">The index of the token to get.</param>
        /// <value>The token with the given index</value>
        /// <exception cref="System.IndexOutOfRangeException">Thrown when trying to get a token which doesn't exist, that is when <see cref="!:index"/> is equal or greater then <see cref="!:Count"/> or <see cref="!:index"/> is negative.</exception>
        public string this[int index] {
            get { return tokens[index]; }
        }
        #endregion

        #endregion

        #region . Tokenize .

        private void Tokenize(string str, bool returnDelimiters, bool returnEmpty) {
            if (returnDelimiters) {
                tokens = str.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var tmp = new List<string>(tokens.Length << 1);

                var delimIndex = str.IndexOfAny(delimiters.ToCharArray());
                var tokensIndex = 0;
                int prevDelimIdx;

                if (delimIndex == 0)
                    do {
                        tmp.Add(new string(str[delimIndex], 1));
                        prevDelimIdx = delimIndex++;
                        delimIndex = str.IndexOfAny(delimiters.ToCharArray(), delimIndex);
                        if (returnEmpty && delimIndex == prevDelimIdx + 1)
                            tmp.Add(empty);
                    } while (delimIndex == prevDelimIdx + 1);

                while (delimIndex > -1) {
                    tmp.Add(tokens[tokensIndex++]);

                    do {
                        tmp.Add(new string(str[delimIndex], 1));
                        prevDelimIdx = delimIndex++;
                        delimIndex = str.IndexOfAny(delimiters.ToCharArray(), delimIndex);
                        if (returnEmpty && delimIndex == prevDelimIdx + 1)
                            tmp.Add(empty);
                    } while (delimIndex == prevDelimIdx + 1);
                }
                if (tokensIndex < tokens.Length)
                    tmp.Add(tokens[tokensIndex]);

                tokens = tmp.ToArray();
            } else if (returnEmpty) {
                tokens = str.Split(delimiters.ToCharArray(), StringSplitOptions.None);
                if (empty != String.Empty)
                    for (var i = 0; i < tokens.Length; i++)
                        if (tokens[i] == String.Empty) tokens[i] = empty;
            } else
                tokens = str.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region . Reset .
        /// <summary>
        /// Resets the current position index so that the tokens can be extracted again.
        /// </summary>
        public void Reset() {
            index = 0;
        }
        #endregion

        #region . GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            while (HasMoreTokens)
                yield return NextToken;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }
}