using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpNL.Utility {

    /// <summary>
    /// Represents a generic heap list.
    /// </summary>
    /// <typeparam name="T">The value type of this heap list.</typeparam>
    /// <remarks>
    /// This class implements the heap interface using a <see cref="T:List{T}"/> as the underlying
    /// data structure.  This heap allows values which are equals to be inserted.  The heap will
    /// return the top K values which have been added where K is specified by the size passed to
    /// the constructor. K+1 values are not gaurenteed to be kept in the heap or returned in a
    /// particular order.
    /// </remarks>
    public class ListHeap<T> : IHeap<T> where T : IComparable<T> {

        private readonly int size;
        private readonly List<T> list;
        private readonly IComparer<T> comparer;

        private T max;


        #region . Constructors .

        /// <summary>
        /// Creates a new heap with the specified size using the sorted based on the default object comparator.
        /// </summary>
        /// <param name="size">The size of the heap.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The heap size cannot be negative.</exception>
        public ListHeap(int size) : this(size, null) { }

        /// <summary>
        /// Creates a new heap with the specified size using the sorted based on the specified comparator.
        /// </summary>
        /// <param name="size">The size of the heap.</param>
        /// <param name="comparer">The comparer to be used to sort heap elements.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The heap size cannot be negative.</exception>
        public ListHeap(int size, IComparer<T> comparer) {

            if (size < 0) {
                throw new ArgumentOutOfRangeException("size", size, @"The heap size cannot be negative.");
            }

            list = new List<T>();
            this.size = size;
            this.comparer = comparer;
        }


        #endregion

        #region . GetEnumerator .

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator() {
            return list.GetEnumerator();
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


        private static int parent(int i) {
            return (i - 1) / 2;
        }

        private static int left(int i) {
            return (i + 1) * 2 - 1;
        }

        private static int right(int i) {
            return (i + 1) * 2;
        }

        private bool lt(T one, T two) {
            if (comparer != null) {
                return comparer.Compare(one, two) < 0;
            } 
            return one.CompareTo(two) < 0;
        }

        private bool gt(T one, T two) {
            if (comparer != null) {
                return comparer.Compare(one, two) > 0;
            }
            return one.CompareTo(two) > 0;
        }

        private void Swap(int x, int y) {
            T one = list[x];
            T two = list[y];

            list[y] = one;
            list[x] = two;
        }

        private void Heapify(int i) {
            while (true) {
                int l = left(i);
                int r = right(i);
                int smallest;

                if (l < list.Count && lt(list[l], list[i]))
                    smallest = l;
                else
                    smallest = i;

                if (r < list.Count && lt(list[r], list[smallest]))
                    smallest = r;

                if (smallest != i) {
                    Swap(smallest, i);
                    i = smallest;
                } else
                    break;
            }
        }

        #region . Add .

        /// <summary>
        /// Adds the specified object to the heap.
        /// </summary>
        /// <param name="value">The object to add to the heap.</param>
        public void Add(T value) {

            if (ReferenceEquals(max, null)) {
                max = value;
            } else if (gt(value, max)) {

                if (list.Count < size) {
                    max = value;
                } else {
                    return;
                }
            }

            list.Add(value);

            int i = list.Count - 1;

            while (i > 0 && gt(list[parent(i)], value)) {
                list[i] = list[parent(i)];
                i = parent(i);
            }

            list[i] = value;
        }

        #endregion

        #region . Extract .

        /// <summary>
        /// Removes the smallest element from the heap and returns it.
        /// </summary>
        /// <returns>The smallest element from the heap.</returns>
        /// <exception cref="System.InvalidOperationException">Heap underflow.</exception>
        public T Extract() {
            if (list.Count == 0) {
                throw new InvalidOperationException("Heap underflow.");
            }
            T top = list[0];
            int last = list.Count - 1;

            if (last != 0) {
                T value = list[last];
                list.RemoveAt(last);
                list[0] = value;
                Heapify(0);
            } else {
                list.RemoveAt(last);
            }
            return top;
        }

        #endregion

        #region . First .

        /// <summary>
        /// Returns the smallest element of the heap.
        /// </summary>
        /// <returns>The top element of the heap.</returns>
        /// <exception cref="System.InvalidOperationException">Heap underflow.</exception>
        public T First() {
            if (list.Count == 0) {
                throw new InvalidOperationException("Heap underflow.");
            }
            return list[0];
        }

        #endregion

        #region . Last .
        /// <summary>
        /// Returns the largest element of the heap.
        /// </summary>
        /// <returns>The largest element of the heap.</returns>
        /// <exception cref="System.InvalidOperationException">Heap underflow.</exception>
        public T Last() {
            if (list.Count == 0) {
                throw new InvalidOperationException("Heap underflow.");
            }
            return max;
        }
        #endregion

        #region . Size .
        /// <summary>
        /// Returns the size of the heap.
        /// </summary>
        /// <returns>The size of the heap.</returns>
        public int Size() {
            return list.Count;
        }
        #endregion

        #region . IsEmpty .
        /// <summary>
        /// Returns whether the heap is empty.
        /// </summary>
        /// <returns><c>true</c> if the heap is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty() {
            return list.Count == 0;
        }
        #endregion

        #region . Clear .
        /// <summary>
        /// Clears the contents of the heap.
        /// </summary>
        public void Clear() {
            list.Clear();
        }
        #endregion


    }
}
