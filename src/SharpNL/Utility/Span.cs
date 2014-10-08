/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License. You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents an span object.
    /// </summary>
    public class Span : IEquatable<Span>, IComparable<Span> {

        #region + Properties .

        #region . End .

        /// <summary>
        /// Gets the end position of the span.
        /// </summary>
        /// <value>The end position of the span.</value>
        public int End { get; private set; }
        #endregion

        #region . Length .
        /// <summary>
        /// Gets the length of the span.
        /// </summary>
        /// <value>The length of the span.</value>
        public int Length {
            get { return End - Start; }
        }
        #endregion

        #region . Start .

        /// <summary>
        /// Gets the start position of the span.
        /// </summary>
        /// <value>The start position of the span.</value>
        public int Start { get; private set; }

        #endregion

        #region . Probability .
        /// <summary>
        /// Gets the probability of the span.
        /// </summary>
        public double Probability { get; internal set; }
        #endregion

        #region . Type .
        /// <summary>
        /// Gets the type of the span.
        /// </summary>
        /// <value>The type of the span.</value>
        public string Type { get; private set; }
        #endregion

        #endregion

        #region + Constructors .


        public Span(Span span, double probability) : this (span.Start, span.End, probability) { }

        public Span(Span span, int offset) : this(span.Start + offset, span.End + offset, span.Type, span.Probability) { }

        public Span(int start, int end) : this (start, end, null, 0d) { }

        public Span(int start, int end, double probability) : this (start, end, null, probability) { }

        public Span(int start, int end, string type) : this(start, end, type, 0d) { }

        public Span(int start, int end, string type, double probability) {
            if (start < 0) {
                throw new ArgumentOutOfRangeException("start", @"The start index must be zero or greater.");
            }
            if (start > end) {
                throw new ArgumentException(@"The start index must not be larger than end index.", "start");
            }
            if (end < 0) {
                throw new ArgumentOutOfRangeException("end", @"The end index must be zero or greater.");
            }

            Start = start;
            End = end;
            Type = type;
            Probability = probability;
        }

        #endregion

        #region . CompareTo .

        /// <summary>
        /// Compares the current span with another span
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the spans being compared. 
        /// The return value has the following meanings: Value Meaning Less than zero This span is less than the <paramref name="other"/> parameter. Zero This span is equal to <paramref name="other"/>. Greater than zero This span is greater than <paramref name="other"/>.  
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(Span other) {
            if (Start < other.Start) {
                return -1;
            }

            if (Start == other.Start) {
                if (End > other.End) {
                    return -1;
                }
                if (End < other.End) {
                    return 1;
                }

                if (Type == null && other.Type == null) {
                    return 0;
                }

                if (!string.IsNullOrEmpty(Type) && !string.IsNullOrEmpty(other.Type)) {
                    // use type lexicography order
                    return string.CompareOrdinal(Type, other.Type);
                }

                if (!string.IsNullOrEmpty(Type)) {
                    return -1;
                }
            }
            return 1;
        }

        #endregion

        #region . Contains .

        /// <summary>
        /// Determines whether specified span is contained by this span.
        /// </summary>
        /// <param name="span">The span to compare with this span.</param>
        /// <returns><c>true</c> if specified span is contained by this span; otherwise, <c>false</c>.</returns>
        /// <remarks>Identical spans are considered to contain each other.</remarks>
        public bool Contains(Span span) {
            return Start <= span.Start && span.End <= End;
        }

        /// <summary>
        /// Determines whether specified index is contained inside this span.
        /// </summary>
        /// <param name="index">The index to test with this span.</param>
        /// <returns><c>true</c> if specified index is contained inside this span.; otherwise, <c>false</c>.</returns>
        /// <remarks>An index with the value of end is considered outside the span.</remarks>
        public bool Contains(int index) {
            return Start <= index && index < End;
        }

        #endregion

        #region . Crosses .
        /// <summary>
        /// Determines whether the specified span crosses this span.
        /// </summary>
        /// <param name="span">The span to compare with this span.</param>
        /// <returns><c>true</c> if specified span overlaps this span and contains a non-overlapping section, <c>false</c> otherwise.</returns>
        public bool Crosses(Span span) {
            return 
                !Contains(span) && 
                !span.Contains(this) &&
                (Start <= span.Start && span.Start < End || span.Start <= Start && Start < span.End);
        }

        #endregion

        #region . DropOverlappingSpans .
        /// <summary>
        /// Removes spans with are intersecting or crossing in anyway.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <returns>A array of non-overlapping spans.</returns>
        /// <remarks>
        /// The following rules are used to remove the spans: <br />
        /// Identical spans: The first span in the array after sorting it remains<br />
        /// Intersecting spans: The first span after sorting remains<br />
        /// Contained spans: All spans which are contained by another are removed<br />
        /// </remarks>
        public static Span[] DropOverlappingSpans(Span[] spans) {
            var list = new List<Span>(spans);
            list.Sort();

            var index = 0;
            Span lastSpan = null;

            while (index < list.Count) {
                if (lastSpan != null && lastSpan.Intersects(list[index])) {
                    list.RemoveAt(index);
                } else {
                    lastSpan = list[index];
                    index++;
                }
            }
            return list.ToArray();
        }
        #endregion

        #region . Equals .
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current span is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An span object to compare with this object.</param>
        public bool Equals(Span other) {
            if (other == null) {
                return false;
            }
            return Start == other.Start &&
                   End == other.End &&
                   Type == other.Type;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            if (obj == this)
                return true;

            var span = obj as Span;
            if (span != null)
                return Equals(span);

            return false;

        }

        #endregion

        #region . GetCoveredText .
        /// <summary>
        /// Gets the text covered by the current span.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The substring covered by the current span.</returns>
        /// <exception cref="System.ArgumentException">The span is outside the given text.</exception>
        public string GetCoveredText(string text) {

            if (text.Length < Length) {
                throw new ArgumentException("The span is outside the given text.");
            }

            return text.Substring(Start, Length);
        }
        #endregion

        #region . Intersects .
        /// <summary>
        /// Determines whether the span intersects with the specified span.
        /// </summary>
        /// <param name="span">The span to compare with this span.</param>
        /// <returns><c>true</c> if the span intersects with the specified span, <c>false</c> otherwise.</returns>
        public bool Intersects(Span span) {
            return 
                Contains(span) || 
                span.Contains(this) ||
                (Start <= span.Start && span.Start < End) || 
                (span.Start <= Start && Start < span.End) ;
        }
        #endregion

        #region . StartWith .

        /// <summary>
        /// Determines whether the specified span is the begin of this span and the specified span is contained in this span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <returns><c>true</c> if the specified span starts with this span and is contained in this span, <c>false</c> otherwise.</returns>
        public bool StartsWith(Span span) {
            return Start == span.Start && span.End <= End;

        }
        #endregion

        #region . Trim .
        /// <summary>
        /// Returns a copy of this span with leading and trailing white spaces removed.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The trimmed span.</returns>
        public Span Trim(string text) {

            int start = Start;
            for (int i = Start; i < End && char.IsWhiteSpace(text[i]); i++) {
                start++;
            }

            int end = End;
            for (int i = End; i > Start && char.IsWhiteSpace(text[i - 1]); i--) {
                end--;
            }

            if (Start == start && End == end) {
                return this;
            }

            if (start > end) {
                return new Span(Start, Start, Type, Probability);
            }

            return new Span(start, end, Type, Probability);
        }
        #endregion

        #region . GetHashCode .
        /// <summary>
        /// Generates a hash code of the current span.
        /// </summary>
        /// <returns>
        /// A int hash code for the current <see cref="Span"/>.
        /// </returns>
        public override int GetHashCode() {
            int res = 23;
            res = res * 37 + Start;
            res = res * 37 + End;
            if (Type == null) {
                res = res * 37;
            } else {
                res = res * 37 + Type.GetHashCode();
            }

            return res;
        }
        #endregion

        #region . ToString .
        /// <summary>
        /// Returns a string that represents the current span.
        /// </summary>
        /// <returns>
        /// A string that represents the current span.
        /// </returns>
        public override string ToString() {
            return string.Format("[{0}..{1}]{2}", Start, End, string.IsNullOrEmpty(Type) ? string.Empty : " " + Type);
        }

        #endregion

        #region . SpansToStrings .
        /// <summary>
        /// Converts an array of <see cref="T:Span"/> to an array of <see cref="T:string"/>.
        /// </summary>
        /// <param name="spans">The spans.</param>
        /// <param name="text">The text.</param>
        /// <returns>The strings.</returns>
        public static string[] SpansToStrings(Span[] spans, string text) {
            var items = new string[spans.Length];
            for (int i = 0; i < spans.Length; i++) {
                items[i] = spans[i].GetCoveredText(text);
            }
            return items;
        }

        public static string[] SpansToStrings(Span[] spans, string[] tokens) {

            var items = new string[spans.Length];


            for (int i = 0; i < spans.Length; i++) {

                string value = string.Empty;
                for (int n = spans[i].Start; n < spans[i].End; n++) {

                    // prevent index exception.
                    if (n > tokens.Length) {
                        break;
                    }
                    value += tokens[n] + " ";
                }

                items[i] = value.TrimEnd(' ');
            }
            return items;
        }


        #endregion


    }
}
