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
using System.Globalization;

namespace SharpNL.Utility.FeatureGen {
    /// <summary>
    /// Recognizes predefined patterns in strings.
    /// </summary>
    public class StringPattern {
        private readonly int digits;
        private readonly Patterns pattern;

        private StringPattern(Patterns pattern, int digits) {
            this.pattern = pattern;
            this.digits = digits;
        }

        #region + Properties .

        #region . AllCapitalLetter .
        /// <summary>
        /// Gets a value indicating whether if all letters are capital.
        /// </summary>
        /// <value><c>true</c> if all letters are capital; otherwise, <c>false</c>.</value>
        public bool AllCapitalLetter {
            get { return (pattern & Patterns.AllCapitalLetter) > 0; }
        }
        #endregion

        #region . AllDigit .

        /// <summary>
        /// Gets a value indicating whether if all chars are digits.
        /// </summary>
        /// <value><c>true</c> if all chars are digits; otherwise, <c>false</c>.</value>
        public bool AllDigit {
            get { return (pattern & Patterns.AllDigit) > 0; }
        }

        #endregion

        #region . AllLetter .
        /// <summary>
        /// Gets a value indicating whether the characters are all letters.
        /// </summary>
        /// <value><c>true</c> if all characters are letters.; otherwise, <c>false</c>.</value>
        public bool AllLetter {
            get { return (pattern & Patterns.AllLetters) > 0; }
        }
        #endregion

        #region . AllLowerCaseLetter .
        /// <summary>
        /// Gets a value indicating whether if all letters are lower case.
        /// </summary>
        /// <value><c>true</c> if all letters are lower case; otherwise, <c>false</c>.</value>
        public bool AllLowerCaseLetter {
            get { return (pattern & Patterns.AllLowerCaseLetter) > 0; }
        }
        #endregion

        #region . ContainsComma .

        /// <summary>
        /// Gets a value indicating whether this pattern contains an comma.
        /// </summary>
        /// <value><c>true</c> if this pattern contains comma; otherwise, <c>false</c>.</value>
        public bool ContainsComma {
            get {
                return (pattern & Patterns.ContainsComma) > 0;
            }
        }

        #endregion

        #region . ContainsDigit .

        /// <summary>
        /// Gets a value indicating whether this pattern contains an digit.
        /// </summary>
        /// <value><c>true</c> if this pattern contains an digit; otherwise, <c>false</c>.</value>
        public bool ContainsDigit {
            get {
                return (pattern & Patterns.ContainsDigit) > 0;
            }
        }

        #endregion
        
        #region . ContainsHyphen .

        /// <summary>
        /// Gets a value indicating whether this pattern contains an hyphen.
        /// </summary>
        /// <value><c>true</c> if this pattern contains an hyphen; otherwise, <c>false</c>.</value>
        public bool ContainsHyphen {
            get {
                return (pattern & Patterns.ContainsHyphen) > 0;
            }
        }

        #endregion
        
        #region . ContainsLetters .

        /// <summary>
        /// Gets a value indicating whether this pattern contains letters.
        /// </summary>
        /// <value><c>true</c> if this pattern contains letters; otherwise, <c>false</c>.</value>
        public bool ContainsLetters {
            get {
                return (pattern & Patterns.ContainsLetters) > 0;
            }
        }

        #endregion

        #region . ContainsPeriod .
        /// <summary>
        /// Gets a value indicating whether this pattern contains period.
        /// </summary>
        /// <value><c>true</c> if this pattern contains period; otherwise, <c>false</c>.</value>
        public bool ContainsPeriod {
            get {
                return (pattern & Patterns.ContainsPeriod) > 0;
            }
        }
        #endregion

        #region . ContainsSlash .
        /// <summary>
        /// Gets a value indicating whether this pattern contains slash.
        /// </summary>
        /// <value><c>true</c> if this pattern contains slash; otherwise, <c>false</c>.</value>
        public bool ContainsSlash {
            get {
                return (pattern & Patterns.ContainsSlash) > 0;
            }
        }
        #endregion

        #region . Digits .
        /// <summary>
        /// Gets the number of digits.
        /// </summary>
        /// <value>The number of digits.</value>
        public int Digits {
            get { return digits; }
        }

        #endregion

        #region . InitialCapitalLetter .
        /// <summary>
        /// Gets a value indicating whether the first letter is capital.
        /// </summary>
        /// <value><c>true</c> if first letter is capital; otherwise, <c>false</c>.</value>
        public bool InitialCapitalLetter {
            get { return (pattern & Patterns.InitialCapitalLetter) > 0; }
        }
        #endregion

        #endregion

        #region . Recognize .

        /// <summary>
        /// Recognizes the <see cref="StringPattern"/> of the specified string.
        /// </summary>
        /// <param name="value">The string to be recognized.</param>
        /// <returns>The recognized <see cref="StringPattern"/> object.</returns>
        public static StringPattern Recognize(string value) {
            var pattern = Patterns.AllCapitalLetter |
                          Patterns.AllLowerCaseLetter |
                          Patterns.AllDigit |
                          Patterns.AllLetters;

            var digits = 0;
            for (var i = 0; i < value.Length; i++) {
                var c = char.GetUnicodeCategory(value[i]);

                var isLetter = c == UnicodeCategory.UppercaseLetter ||
                               c == UnicodeCategory.LowercaseLetter ||
                               c == UnicodeCategory.TitlecaseLetter ||
                               c == UnicodeCategory.ModifierLetter ||
                               c == UnicodeCategory.OtherLetter;

                if (isLetter) {
                    pattern |= Patterns.ContainsLetters;
                    pattern &= ~Patterns.AllDigit;


                    if (c == UnicodeCategory.UppercaseLetter) {
                        if (i == 0) {
                            pattern |= Patterns.InitialCapitalLetter;
                        }
                        pattern |= Patterns.ContainsUpperCase;
                        pattern &= ~Patterns.AllLowerCaseLetter;
                    } else {
                        pattern &= ~Patterns.AllCapitalLetter;
                    }
                } else {
                    // contains chars other than letter, this means
                    // it can not be one of these:

                    pattern &= ~Patterns.AllLetters;
                    pattern &= ~Patterns.AllCapitalLetter;
                    pattern &= ~Patterns.AllLowerCaseLetter;

                    if (c == UnicodeCategory.DecimalDigitNumber) {
                        pattern |= Patterns.ContainsDigit;
                        digits++;
                    } else {
                        pattern &= ~Patterns.AllDigit;
                    }

                    switch (value[i]) {
                        case ',':
                            pattern |= Patterns.ContainsComma;
                            break;
                        case '.':
                            pattern |= Patterns.ContainsPeriod;
                            break;
                        case '/':
                            pattern |= Patterns.ContainsSlash;
                            break;
                        case '-':
                            pattern |= Patterns.ContainsHyphen;
                            break;
                    }
                }
            }
            return new StringPattern(pattern, digits);
        }

        #endregion

        [Flags]
        private enum Patterns {
            InitialCapitalLetter = 0x1,
            AllCapitalLetter = 0x1 << 1,
            AllLowerCaseLetter = 0x1 << 2,
            AllLetters = 0x1 << 3,
            AllDigit = 0x1 << 4,
            ContainsPeriod = 0x1 << 5,
            ContainsComma = 0x1 << 6,
            ContainsSlash = 0x1 << 7,
            ContainsDigit = 0x1 << 8,
            ContainsHyphen = 0x1 << 9,
            ContainsLetters = 0x1 << 10,
            ContainsUpperCase = 0x1 << 11
        }
    }
}