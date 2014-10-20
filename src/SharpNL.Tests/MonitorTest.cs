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
using System.Threading;
using NUnit.Framework;

namespace SharpNL.Tests {
    [TestFixture]
    internal class MonitorTest {

        [Test]
        public void DefaultTest() {

            var messages = 0;
            var warnings = 0;
            var exceptions = 0;
            var task = new Monitor();

            task.Message += (sender, args) => {
                messages++;
            };
            task.Exception += (sender, args) => {
                exceptions++;
            };
            task.Warning += (sender, args) => {
                warnings++;
            };

            task.Execute(token => {
                task.OnMessage("m1");
                task.OnMessage("m2");
                task.OnMessage("m3");

                Thread.Sleep(200);

                task.OnWarning("w1");
                task.OnWarning("w2");

                task.OnException(new Exception());

            });

            Assert.AreEqual(true, task.IsRunning);
            task.Wait();

            Assert.AreEqual(3, messages);
            Assert.AreEqual(2, warnings);
            Assert.AreEqual(1, exceptions);
        }

        [Test]
        public void CancelTest() {

            var task = new Monitor();
            var count = 0;
            task.Execute(token => {              
                while (count != 100) {
                    count++;
                    token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            });

            Assert.True(task.IsRunning);

            task.Cancel();

            Assert.True(task.IsCanceled);
            Assert.False(task.IsRunning);

            Assert.AreNotEqual(100, count);
        }

    }
}