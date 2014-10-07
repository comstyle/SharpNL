using System;
using System.Collections.Generic;

namespace SharpNL.Utility {
    /// <summary>
    /// Represents a cool cache dictionary.
    /// </summary>
    /// <remarks>The other methods have not been ported to this class, because they were badly implemented in the OpenNLP.</remarks>
    public class Cache {
        /// <summary>
        /// The element in the linked list which was most recently used.
        /// </summary>
        private DoubleLinkedListElement first;

        /// <summary>
        /// The element in the linked list which was least recently used.
        /// </summary>
        private DoubleLinkedListElement last;

        /// <summary>
        /// Temporary holder of the key of the least-recently-used element.
        /// </summary>
        private object lastKey;

        /// <summary>
        /// Temporary value used in swap.
        /// </summary>
        private ObjectWrapper temp;

        /// <summary>
        /// Holds the object wrappers which the keys are mapped to.
        /// </summary>
        private readonly ObjectWrapper[] wrappers;

        /// <summary>
        /// Map which stores the keys and values of the cache.
        /// </summary>
        private readonly Dictionary<object, ObjectWrapper> map;

        /// <summary>
        /// The size of the cache.
        /// </summary>
        private readonly int size;

        #region . Constructor .

        /// <summary>
        /// Initializes a new instance of the <see cref="Cache"/> of the specified size.
        /// </summary>
        /// <param name="size">The size of the cache.</param>
        public Cache(int size) {

            map = new Dictionary<object, ObjectWrapper>();
            wrappers = new ObjectWrapper[size];

            var dummy = new object();

            first = new DoubleLinkedListElement(null, null, dummy);

            wrappers[0] = new ObjectWrapper(null, first);
            map.Add(dummy, wrappers[0]);

            DoubleLinkedListElement element = first;
            for (int i = 1; i < size; i++) {
                dummy = new object();
                element = new DoubleLinkedListElement(element, null, dummy);
                element.Prev.Next = element;

                wrappers[i] = new ObjectWrapper(null, element);

                map.Add(dummy, wrappers[i]);
            }
            last = element;



            this.size = size;

        }

        #endregion

        #region . Clear .
        /// <summary>
        /// Removes all items from the <see cref="T:Cache"/>.
        /// </summary>
        public void Clear() {
            map.Clear();
            DoubleLinkedListElement element = first;
            for (int i = 0; i < size; i++) {
                var value = new object();
                wrappers[i].Value = null;
                map.Add(value, wrappers[i]);
                element.Value = value;
                element = element.Next;
            }
        }
        #endregion

        #region . Get .
        /// <summary>
        /// Gets the value associated with the specified key in this cache object.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The cached object.</returns>
        public object Get(object key) {
            

            if (map.ContainsKey(key)) {
                var wrapper = map[key];

                DoubleLinkedListElement element = wrapper.Element;

                if (element != first) {
                    element.Prev.Next = element.Next;

                    if (element.Next != null) {
                        element.Next.Prev = element.Prev;
                    } else {
                        last = element.Prev;
                    }

                    element.Next = first;

                    first.Prev = element;

                    element.Prev = null;

                    first = element;
                }

                return wrapper.Value;
            }

            return null;
        }
        #endregion

        #region . Put .
        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:Cache"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public object Put(object key, object value) {
            if (key == null) {
                throw new ArgumentNullException("key");
            }

            if (map.ContainsKey(key)) {
                /*
                 * this should never be the case, we only do a put on a cache miss which
                 * means the current value wasn't in the cache. However if the user screws
                 * up or wants to use this as a fixed size hash and puts the same thing in
                 * the list twice then we update the value and more the key to the front of the
                 * most recently used list.
                 */

                var wrapper = map[key];

                var e = wrapper.Element;

                if (e != first) {
                    //remove list item
                    e.Prev.Next = e.Next;
                    if (e.Next != null) {
                        e.Next.Prev = e.Prev;
                    } else {
                        //were moving last
                        last = e.Prev;
                    }

                    //put list item in front
                    e.Next = first;
                    first.Prev = e;
                    e.Prev = null;

                    //update first
                    first = e;
                }
                return wrapper.Value;
            }

            // Put o in the front and remove the last one
            lastKey = last.Value; // store key to remove from hash later
            last.Value = key; //update list element with new key

            // connect list item to front of list
            last.Next = first;
            first.Prev = last;

            // update first and last value
            first = last;
            last = last.Prev;
            first.Prev = null;
            last.Next = null;

            // remove old value from cache
            temp = map[lastKey];
            map.Remove(lastKey);

            //update wrapper
            temp.Value = value;
            temp.Element = first;

            map[key] = temp;

            return null;
        }

        #endregion

        #region . ContainsKey .
        /// <summary>
        /// Determines whether the <see cref="T:Cache"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:Cache"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:Cache"/>.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(object key) {
            return map.ContainsKey(key);
        }
        #endregion

        #region @ DoubleLinkedListElement .
        internal class DoubleLinkedListElement {

            internal DoubleLinkedListElement Prev;
            internal DoubleLinkedListElement Next;
            internal object Value;

            internal DoubleLinkedListElement(
                DoubleLinkedListElement prev,
                DoubleLinkedListElement next,
                object value) {

                Prev = prev;
                Next = next;
                Value = value;

                if (Prev != null) {
                    Prev.Next = this;
                }

                if (Next != null) {
                    Next.Prev = this;
                }
            }
        }
        #endregion

        #region @ ObjectWrapper .
        internal class ObjectWrapper {
            internal DoubleLinkedListElement Element { get; set; }
            internal object Value { get; set; }
            internal ObjectWrapper(object value, DoubleLinkedListElement element) {
                Value = value;
                Element = element;
            }
        }

        #endregion

    }
}
