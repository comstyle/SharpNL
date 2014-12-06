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
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

using SharpNL.Utility;

namespace SharpNL.ML.Model {
    /// <summary>
    /// Represents a stream that wraps a <see cref="T:IObjectStream{Event}"/>, 
    /// and maintains a hash of the events readed from it.
    /// </summary>
    public class HashSumEventStream : AbstractObjectStream<Event> {

        private readonly MD5 digest;
        private Event previous;
        private bool done;

        #region . Constructor .
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSumEventStream"/> class.
        /// </summary>
        /// <param name="eventStream">The event stream.</param>
        public HashSumEventStream(IObjectStream<Event> eventStream)
            : base(eventStream) {

            digest = MD5.Create();
        }
        #endregion

        #region . CalculateHashSum .
        /// <summary>
        /// Calculates the hash sum of the stream. 
        /// The method must be called after the stream is completely consumed.
        /// </summary>
        /// <returns>The hash string value.</returns>
        public string CalculateHashSum() {
            if (done) {
                var buff = Encoding.UTF8.GetBytes(previous.ToString());
                digest.TransformFinalBlock(buff, 0, buff.Length);
                previous = null;

                var sb = new StringBuilder(32);
                foreach (var b in digest.Hash) {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
            throw new InvalidOperationException("If the stream is not consumed completely.");
        }
        #endregion

        #region . Read .
        /// <summary>
        /// Returns the next object. Calling this method repeatedly until it returns, null will return each object from the underlying source exactly once.
        /// </summary>
        /// <returns>The next object or null to signal that the stream is exhausted.</returns>
        /// <exception cref="System.InvalidOperationException">The CalculateHashSum was called! Theoretically this stream should be completely consumed.</exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override Event Read() {
            if (done) {
                throw new InvalidOperationException("The stream was already completely consumed.");
            }
            var ev = base.Read();
            if (ev != null) {
                if (previous != null) {
                    var buff = Encoding.UTF8.GetBytes(previous.ToString());
                    digest.TransformBlock(buff, 0, buff.Length, null, 0);
                }

                previous = ev;
            } else {
                done = true;
            }
            return ev;
        }
        #endregion
        
    }
}