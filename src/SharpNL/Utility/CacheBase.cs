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
using System.Runtime.Caching;

namespace SharpNL.Utility {
    /// <summary>
    /// Provides cache support.
    /// </summary>
    public abstract class CacheBase<T> {

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        protected MemoryCache Cache { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBase{T}"/> class.
        /// </summary>
        protected CacheBase(bool cache = true) {
            if (cache)
                Cache = new MemoryCache(GetType().Name);
        }

        #region . CacheEnabled .
        /// <summary>
        /// Gets a value indicating whether the cache is enabled.
        /// </summary>
        /// <value><c>true</c> if the cache is enabled; otherwise, <c>false</c>.</value>
        protected bool CacheEnabled {
            get { return Cache != null; }
        }
        #endregion

        #region . Set .
        /// <summary>
        /// Inserts a cache entry into the cache by using a key and a value and specifies time-based expiration details.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented.</param>
        protected void Set(string key, T value, string regionName = null) {
            if (Cache == null)
                throw new NotSupportedException("The cache is not enabled in the constructor.");

            key = Key(key, regionName);
            Cache.Set(key, value, CreateTimeOffset());
        }
        #endregion

        #region . Get .
        /// <summary>
        /// Returns an entry from the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">A named region in the cache to which a cache entry was added. Do not pass a value for this parameter.</param>
        /// <returns>A reference to the cache entry that is identified by key, if the entry exists; otherwise, <c>null</c>.</returns>
        protected T Get(string key, string regionName = null) {
            if (Cache == null)
                throw new NotSupportedException("The cache is not enabled in the constructor.");

            key = Key(key, regionName);
            return (T)Cache.Get(key);
        }
        #endregion

        #region . IsCached .
        /// <summary>
        /// Determines whether a cache entry exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to search for.</param>
        /// <param name="regionName">A named region in the cache to which a cache entry was added. Do not pass a value for this parameter.</param>
        /// <returns><c>true</c> if the cache contains a cache entry whose key matches key; otherwise, <c>false</c>.</returns>
        protected bool IsCached(string key, string regionName = null) {
            if (Cache == null)
                throw new NotSupportedException("The cache is not enabled in the constructor.");

            key = Key(key, regionName);
            return Cache.Contains(key);
        }
        #endregion

        #region . Key .
        // hack for regions because... MemoryCache class does not implement regions.
        private static string Key(string key, string regionName) {
            return regionName != null
                ? string.Format("{0}-{1}", regionName, key)
                : key;
        }
        #endregion

        #region . CreateTimeOffset .
        /// <summary>
        /// Creates the fixed date and time at which the cache entry will expire.
        /// </summary>
        /// <returns>The fixed date and time at which the cache entry will expire.</returns>
        protected virtual DateTimeOffset CreateTimeOffset() {
            return new DateTimeOffset(DateTime.Now.AddMinutes(15));
        }
        #endregion

    }
}