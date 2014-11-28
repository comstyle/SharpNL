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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using SharpNL.Text;

namespace SharpNL {
    /// <summary>
    /// Utility class that makes my life easier. =D
    /// </summary>
    internal static class Extensions {

        #region . Add .

        // does not support ref in the array argument :(

        public static T[] Add<T>(this T[] array, T value) {           
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = value;
            return array;
        }
        #endregion

        #region . AddRange .

        /// <summary>
        /// Copies the contents of another <see cref="IDictionary{K, V}" /> object to the end of the collection.
        /// </summary>
        /// <typeparam name="K">The dictionary key type.</typeparam>
        /// <typeparam name="V">The dictionary value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="values">A <see cref="IDictionary{K, V}" /> that contains the objects to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="dictionary" />
        /// or
        /// <paramref name="values" /></exception>
        /// <exception cref="System.ArgumentException">The dictionary is read-only.</exception>
        public static void AddRange<K, V>(this IDictionary<K, V> dictionary, IDictionary<K, V> values) {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            if (dictionary.IsReadOnly)
                throw new ArgumentException("The dictionary is read-only.");

            if (values == null)
                throw new ArgumentNullException("values");

            if (values.Count == 0)
                return;

            

            foreach (var pair in values) {
                dictionary.Add(pair.Key, pair.Value);
            }
        }

        #endregion

        #region . AreEqual .

        /// <summary>
        /// Verifies that the all the specified objects are equal.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="others">The others.</param>
        /// <returns><c>true</c> if the all the specified objects are equal, <c>false</c> otherwise.</returns>
        public static bool AreEqual<T>(this T input, params T[] others) {
            return others.All(other => input.Equals(other));
        }

        /// <summary>
        /// Verifies that the all the specified objects are equal.
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="others">The others.</param>
        /// <returns><c>true</c> if the all the specified objects are equal, <c>false</c> otherwise.</returns>
        public static bool AreEqual<T>(this T[] input, params T[][] others) {
            return others.All(other => input.SequenceEqual(other));
        }
        #endregion

        #region . Contains .
        internal static bool Contains(this string[] array, string value) {
            return array.Any(item => item == value);
        }
        #endregion

        #region . Each .
        /// <example>
        /// var values = new string[] { "a", "b", "c" };
        /// strings.Each( ( value, index ) => {
        ///     // nice hack ;)
        /// });
        /// </example>
        internal static void Each<T>(this IEnumerable<T> ie, Action<T, int> action) {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
        #endregion

        #region . EnsureContainsKey .
        public static void EnsureContainsKey<K, V>(this Dictionary<K, V> dictionary, K key, Type valueType) {
            dictionary.EnsureContainsKey(key, valueType, null);
        }
        public static void EnsureContainsKey<K, V>(this Dictionary<K, V> dictionary, K key, Type valueType, params object[] constructorParameters) {
            if (!dictionary.ContainsKey(key)) {
                dictionary.Add(key, (V)(Activator.CreateInstance(valueType, constructorParameters)));
            }
        }
        #endregion

        #region . Fill .

        /// <summary>
        /// Fills the specified input with the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The array.</param>
        /// <param name="value">The value.</param>
        internal static void Fill<T>(this T[] input, T value) {
            for (int i = 0; i < input.Length; i++) {
                input[i] = value;
            }
        }
        #endregion

        #region . First .
        /// <summary>
        /// Returns the first element of a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The first element in the specified list.</returns>
        public static T First<T>(this IList<T> list) {
            if (list == null || list.Count == 0)
                return default(T);

            return list[0];
        }
        #endregion

        #region . GetKey .
        /// <summary>
        /// Gets the element key by the first element that are equals to the specified <paramref name="value"/>,
        /// of a <c>default(K)</c> value if no element is found.
        /// </summary>
        /// <typeparam name="K">The type of the key of the <paramref name="dictionary"/>.</typeparam>
        /// <typeparam name="V">The type of the values in the <paramref name="dictionary"/>.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>default(k)</c> if no element is found; otherwise the first element key found in the <paramref name="dictionary"/>.</returns>
        public static K GetKey<K, V>(this IDictionary<K, V> dictionary, V value) {
            return dictionary.FirstOrDefault(pair => Equals(pair.Value, value)).Key;
        }
        #endregion

        #region . IndexOf .
        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of a specified Unicode character in the current <see cref="StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The Unicode character you want to find.</param>
        /// <returns>The character position of the value parameter if the specified character is found, or -1 if it is not found.</returns>
        /// <remarks>
        /// The search for the value parameter is both case-sensitive and culture-sensitive.
        /// </remarks>
        internal static int IndexOf(this StringBuilder sb, char value) {
            return IndexOf(sb, value, 0);
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of a specified Unicode character in the current <see cref="StringBuilder"/> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The Unicode character you want to find.</param>
        /// <param name="startIndex">The starting index number for the search.</param>
        /// <returns>The character position of the value parameter if the specified character is found, or -1 if it is not found.</returns>
        /// <remarks>
        /// <para>Index numbering starts at 0 (zero). The search ranges from <paramref name="startIndex"/> to the end of the string.</para>
        /// <para>The search for the value parameter is both case-sensitive and culture-sensitive.</para>
        /// </remarks>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        internal static int IndexOf(this StringBuilder sb, char value, int startIndex) {           
            if (startIndex < 0 || startIndex > sb.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            for (var i = startIndex; i < sb.Length; i++) {
                if (sb[i] == value)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of the specified String object in the current <see cref="StringBuilder" /> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The <see cref="string" /> object you want to find.</param>
        /// <returns>The character position of the <paramref name="value" /> parameter if the specified string is found, or -1 if it is not found. If value is empty, the return value is 0 (zero).</returns>
        internal static int IndexOf(this StringBuilder sb, string value) {
            if (string.IsNullOrEmpty(value))
                return 0;

            return IndexOf(sb, value, 0, false);
        }

        /// <summary>
        /// Reports the index number, or character position, of the first occurrence of the specified String object in the current <see cref="StringBuilder" /> object.
        /// The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="value">The <see cref="string" /> object you want to find.</param>
        /// <param name="startIndex">The starting index number for the search.</param>
        /// <param name="ignoreCase">if set to <c>true</c> the case will be ignored during the comparison.</param>
        /// <returns>The character position of the <paramref name="value" /> parameter if the specified string is found, or -1 if it is not found. If value is empty, the return value is <paramref name="startIndex" />.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">startIndex</exception>
        internal static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase) {
            if (startIndex > sb.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            if (sb.Length == 0 || value == null)
                return -1;

            if (value == string.Empty)
                return startIndex;

            int index;
            var length = value.Length;
            var count = (sb.Length - length) + 1;
            if (!ignoreCase) {
                for (var i = startIndex; i < count; i++) {
                    if (sb[i] != value[0]) 
                        continue;

                    index = 1;
                    while ((index < length) && (sb[i + index] == value[index])) {
                        index++;
                    }
                    if (index == length) {
                        return i;
                    }
                }
            } else {
                for (var i = startIndex; i < count; i++) {
                    if (char.ToLower(sb[i]) != char.ToLower(value[0])) 
                        continue;

                    index = 1;
                    while ((index < length) && (char.ToLower(sb[i + index]) == char.ToLower(value[index]))) {
                        index++;
                    }
                    if (index == length) {
                        return i;
                    }
                }
            }
            return -1;
        }
        #endregion

        #region . Last .
        /// <summary>
        /// Returns the last element of a list.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The last element in the specified list.</returns>
        public static T Last<T>(this IList<T> list) {
            if (list == null || list.Count == 0)
                return default(T);

            return list[list.Count - 1];
        }
        #endregion

        #region . Pop .
        /// <summary>
        /// Removes the object at the top of the <see cref="IList{T}" />, and returns it.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The object removed from the top of the <see cref="IList{T}" />.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The <paramref name="list"/> is empty.
        /// or
        /// The <paramref name="list"/> is read-only.
        /// </exception>
        internal static T Pop<T>(this IList<T> list) {   
            if (list.Count == 0)
                throw new InvalidOperationException("The list is empty.");

            if (list.IsReadOnly)
                throw new InvalidOperationException("The list is read-only.");

            T item = list[0];
            list.RemoveAt(0);
            return item;
        }
        #endregion

        #region . ReadAllBytes .
        /// <summary>
        /// Reads the contents of the stream into a byte array, and then closes the stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>A byte array containing the contents of the stream.</returns>
        /// <exception cref="System.ArgumentNullException">stream</exception>
        /// <exception cref="System.NotSupportedException">The stream was not readable.</exception>
        public static byte[] ReadAllBytes(this Stream stream) {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream.CanRead)
                throw new NotSupportedException("The stream was not readable.");

            var memory = stream as MemoryStream;
            if (memory != null)
                return memory.ToArray();

            using (memory = new MemoryStream()) {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
        #endregion

        #region . Replace .
        /// <summary>
        /// Returns a new string in which all occurrences of a specified Unicode characters in this instance are replaced with another specified Unicode character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldChars">The Unicode characters to be replaced.</param>
        /// <param name="newChar">The Unicode character to replace all occurrences of <paramref name="oldChars"/>.</param>
        /// <returns>
        /// A string that is equivalent to this instance except that all instances of <paramref name="oldChars"/> are replaced with <paramref name="newChar"/>. 
        /// If any <paramref name="oldChars"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        public static string Replace(this string value, char[] oldChars, char newChar) {
            var sb = new StringBuilder(value);
            for (var i = 0; i < sb.Length; i++) {
                if (sb[i].In(oldChars))
                    sb[i] = newChar;
            }
            return sb.ToString();
        }
        #endregion

        #region . SequenceEqual .
        /// <summary>
        /// Determines whether two sequences are equal by comparing the elements by using the default equality comparer for their type.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
        /// <param name="first">The first enumerable.</param>
        /// <param name="second">The second enumerable.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns><c>true</c> if the two source sequences are of equal length and their corresponding elements are equal according to the default equality comparer for their type, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">first or second.</exception>
        internal static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null) {
            if (first == null && second == null)
                return true;

            if (first == null) {
                throw new ArgumentNullException("first");
            }
            if (second == null) {
                throw new ArgumentNullException("second");
            }
            if (comparer == null) {
                comparer = EqualityComparer<T>.Default;
            }

            using (IEnumerator<T> enumerator = first.GetEnumerator())
            using (IEnumerator<T> enumerator2 = second.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    if (!enumerator2.MoveNext() || !comparer.Equals(enumerator.Current, enumerator2.Current)) {
                        return false;
                    }
                }
                if (enumerator2.MoveNext()) {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region . SubArray .
        public static T[] SubArray<T>(this T[] data, int index, int length) {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        #endregion

        #region + SubList .

        internal static ReadOnlyCollection<T> SubList<T>(this IReadOnlyList<T> list, int start, int count) {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            if (start > list.Count || start + count > list.Count)
                throw new ArgumentOutOfRangeException("start");

            var sub = new List<T>();
            for (int i = start; i < start + count; i++) {
                sub.Add(list[i]);
            }
            return new ReadOnlyCollection<T>(sub);
        }

        internal static List<T> SubList<T>(this IList<T> list, int start, int count) {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            if (start > list.Count || start + count > list.Count)
                throw new ArgumentOutOfRangeException("start");

            var sub = new List<T>();
            for (int i = start; i < start + count; i++) {
                sub.Add(list[i]);
            }
            return sub;
        }

        #endregion

        #region . ToArray .
        /// <summary>
        /// Copies a specific number of elements in the current stack into a new array.
        /// </summary>
        /// <typeparam name="T">Specifies the type of elements in the stack.</typeparam>
        /// <param name="stack">The stack.</param>
        /// <param name="count">The number of elements to be copied into a new array.</param>
        /// <returns>A new array containing copies of the elements of the stack.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">count</exception>
        public static T[] ToArray<T>(this Stack<T> stack, int count) {

            if (stack.Count < count)
                throw new ArgumentOutOfRangeException("count");

            int index = 0;
            var array = new T[count];
            IEnumerator<T> e;
            for (e = stack.GetEnumerator(); index < count && e.MoveNext(); index++) {
                array[index] = e.Current;
            }
            return array;
        }

        public static T[] ToArray<T>(this IEnumerable<T> enumerable) {
            return new List<T>(enumerable).ToArray();
        }

        #endregion

        #region . ToDisplay .
        internal static string ToDisplay(this string[] list) {
            return string.Format("[{0}]", string.Join(", ", list));
        }
        internal static string ToDisplay(this List<string> list) {
            return list.ToArray().ToDisplay();
        }
        #endregion

        #region . ToTokenArray .
        /// <summary>
        /// Gets a array of tokens from the token collection.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <returns>The token array.</returns>
        public static string[] ToTokenArray(this IReadOnlyList<IToken> tokens) {
            var items = new string[tokens.Count];
            for (var i = 0; i < tokens.Count; i++) {
                items[i] = tokens[i].Lexeme;
            }
            return items;
        }
        #endregion

        #region . IEnumerator.Cast .
        public static IEnumerator<T> Cast<T>(this IEnumerator iterator) {
            while (iterator.MoveNext()) {
                yield return (T)iterator.Current;
            }
        }
        #endregion

        #region . In .
        /// <summary>
        /// Determines if the instance is equal to any of the specified values
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="values">The values to be compared.</param>
        /// <returns><c>true</c> if the instance is equal to any of the specified values, <c>false</c> otherwise.</returns>
        public static bool In<T>(this T input, params T[] values) {
            return values.Contains(input);

        }

        /// <summary>
        /// Determines if the instance is equal to any of the specified values
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="values">The values to be compared.</param>
        /// <returns><c>true</c> if the instance is equal to any of the specified values, <c>false</c> otherwise.</returns>
        public static bool In<T>(this T input, IEnumerable<T> values) {
            return values.Any(arg => input.Equals(arg));
        }
        #endregion

        #region . HasDefaultConstructor .
        public static bool HasDefaultConstructor(this Type t) {
            return t.IsValueType || t.GetConstructor(Type.EmptyTypes) != null;
        }
        #endregion

    }
}