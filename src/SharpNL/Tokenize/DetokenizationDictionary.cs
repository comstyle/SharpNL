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
using SharpNL.Dictionary;
using SharpNL.Utility;

namespace SharpNL.Tokenize {
    /// <summary>
    /// Class DetokenizationDictionary.
    /// </summary>
    public class DetokenizationDictionary : IEnumerable<string> {

        /// <summary>
        /// Enumerates the dictionary detokenization operations.
        /// </summary>
        public enum Operation {
            /// <summary>
            /// Attaches the token to the token on the right side.
            /// </summary>
            MoveRight,
            /// <summary>
            /// Attaches the token to the token on the left side.
            /// </summary>
            MoveLeft,
            /// <summary>
            /// Attaches the token to the token on the left and right sides.
            /// </summary>
            MoveBoth,
            /// <summary>
            /// Attaches the token token to the right token on first occurrence, and
            /// to the token on the left side on the second occurrence.
            /// </summary>
            RightLeftMatching
        }

        #region . ParseOperation .
        /// <summary>
        /// Parses the operation string into a valid <see cref="Operation"/> value. Return null if the string cannot be parsed.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns><see cref="Nullable{Operation}"/></returns>
        public static Operation? ParseOperation(string operation) {
            switch (operation.ToUpperInvariant()) {
                case "MOVE_RIGHT":
                    return Operation.MoveRight;
                case "MOVE_LEFT":
                    return Operation.MoveLeft;
                case "MOVE_BOTH":
                    return Operation.MoveBoth;
                case "RIGHT_LEFT_MATCHING":
                    return Operation.RightLeftMatching;
                default:
                    Operation result;
                    if (Enum.TryParse(operation, true, out result)) {
                        return result;
                    }
                    return null;
            }
        }
        #endregion

        #region . OperationString .
        /// <summary>
        /// Gets the equivalent/compatible string value from the operation enum.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>The equivalent/compatible string value.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">operation</exception>
        private static string GetOperationString(Operation operation) {
            switch (operation) {
                case Operation.MoveRight:
                    return "MOVE_RIGHT";
                case Operation.MoveLeft:
                    return "MOVE_LEFT";
                case Operation.MoveBoth:
                    return "MOVE_BOTH";
                case Operation.RightLeftMatching:
                    return "RIGHT_LEFT_MATCHING";
                default:
                    throw new ArgumentOutOfRangeException("operation");
            }
        }
        #endregion

        private readonly Dictionary<string, Operation> operationTable;

        #region . Add .
        /// <summary>
        /// Adds the specified token and its operation value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="operation">The operation.</param>
        /// <exception cref="System.ArgumentNullException">token</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">operation</exception>
        public void Add(string token, Operation operation) {
            if (token == null)
                throw new ArgumentNullException("token");

            if (!Enum.IsDefined(typeof(Operation), operation)) 
                throw new ArgumentOutOfRangeException("operation");

            operationTable.Add(token, operation);
        }
        #endregion

        #region . Contains .

        /// <summary>
        /// Determines whether the dictionary contains the specified token operation.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns><c>true</c> if  the dictionary contains the specified token operation; otherwise, <c>false</c>.</returns>
        public bool Contains(string token) {
            return operationTable.ContainsKey(token);
        }
        #endregion

        #region + Constructors .
        public DetokenizationDictionary() {
            operationTable = new Dictionary<string, Operation>();
        }
        public DetokenizationDictionary(string[] tokens, Operation[] operations) {
            if (tokens == null)
                throw new ArgumentNullException("tokens");

            if (operations == null)
                throw new ArgumentNullException("operations");

            if (tokens.Length != operations.Length)
                throw new InvalidOperationException("tokens and operations must have the same length!");

            operationTable = new Dictionary<string, Operation>();

            for (var i = 0; i < tokens.Length; i++) {
                if (tokens[i] == null)
                    throw new ArgumentException("The token " + i + " is null");

                operationTable.Add(tokens[i], operations[i]);
            }
        }

        public DetokenizationDictionary(Stream inputStream) {
            var dict = (Dictionary.Dictionary) Dictionary.Dictionary.Deserialize(inputStream);

            operationTable = new Dictionary<string, Operation>();

            foreach (var entry in dict) {
                if (entry.Tokens.Count != 1)
                    throw new InvalidFormatException("Each entry must have exactly one token!");

                if (!entry.Attributes.Contains("operation"))
                    throw new InvalidFormatException("The dictionary entry does not have the operation attribute.");

                var operation = ParseOperation(entry.Attributes["operation"]);

                if (operation == null)
                    throw new InvalidFormatException("Unknown operation type: " + entry.Attributes["operation"]);


                operationTable[entry.Tokens[0]] = operation.Value;
            }
        }

        #endregion

        #region + Properties .

        #region . Count .

        /// <summary>
        /// Gets the number of operations contained in this dictionary.
        /// </summary>
        /// <value>The number of operations contained in this dictionary.</value>
        public int Count {
            get { return operationTable.Count; }
        }

        #endregion

        #region . this .

        /// <summary>
        /// Gets the <see cref="Operation"/> for the specified token.
        /// </summary>
        /// <param name="token">The key.</param>
        /// <returns>DetokenizationOperation.</returns>
        public Operation? this[string token] {
            get {
                if (operationTable.ContainsKey(token)) {
                    return operationTable[token];
                }
                return null;
            }
        }

        #endregion

        #endregion

        #region . GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:String"/> key that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<string> GetEnumerator() {
            return operationTable.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        #region . Serialize .

        /// <summary>
        /// Serializes this dictionary to the specified output stream.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        public void Serialize(Stream outputStream) {
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");

            if (!outputStream.CanWrite)
                throw new ArgumentException(@"The input stream was not writable.", "outputStream");

            var dict = new Dictionary.Dictionary();

            foreach (var op in operationTable) {
                dict.Add(new Entry(new[] {op.Key}, new Attributes {
                    {"operation", GetOperationString(op.Value)}
                }));
            }

            dict.Serialize(outputStream);
        }

        #endregion

    }
}