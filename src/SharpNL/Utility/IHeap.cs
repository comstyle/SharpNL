using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// Interface for interacting with a Heap data structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// This implementation extract objects from smallest to largest based on either
    /// their natural ordering or the comparator provided to an implementation.
    /// While this is a typical of a heap it allows this objects natural ordering to
    /// match that of other sorted collections.
    /// </remarks>
    public interface IHeap<T> : IEnumerable<T> {

        /// <summary>
        /// Removes the smallest element from the heap and returns it.
        /// </summary>
        /// <returns>The smallest element from the heap.</returns>
        T Extract();

        /// <summary>
        /// Returns the smallest element of the heap.
        /// </summary>
        /// <returns>The top element of the heap.</returns>
        T First();


        /// <summary>
        /// Returns the largest element of the heap.
        /// </summary>
        /// <returns>The largest element of the heap.</returns>
        T Last();

        /// <summary>
        /// Adds the specified object to the heap.
        /// </summary>
        /// <param name="value">The object to add to the heap.</param>
        void Add(T value);

        /// <summary>
        /// Returns the size of the heap.
        /// </summary>
        /// <returns>The size of the heap.</returns>
        int Size();


        /// <summary>
        /// Returns whether the heap is empty.
        /// </summary>
        /// <returns><c>true</c> if the heap is empty; otherwise, <c>false</c>.</returns>
        bool IsEmpty();

        /// <summary>
        /// Clears the contents of the heap.
        /// </summary>
        void Clear();

    }
}
