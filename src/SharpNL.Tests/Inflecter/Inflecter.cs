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

using System.Collections.Generic;
using NUnit.Framework;
using SharpNL.Inflecter;

namespace SharpNL.Tests.Inflecter {
    [TestFixture]
    internal class Inflecter {

        private static readonly Dictionary<string, string> words;

        static Inflecter() {
            words = new Dictionary<string, string> {
                { "archive", "archives" },
                { "address", "addresses" },
                { "box", "boxes" },
                { "city", "cities" },
                { "child", "children" },
                { "fish", "fish" },
                { "foot", "feet" },
                { "home", "homes" },
                { "man", "men" },
                { "photo", "photos" }, 
                { "tooth", "teeth" },
                { "woman", "women" },
            };
        }

        [TestFixtureSetUp]
        public void Setup() {
            // forces the loading of the internal inflecter;
            Inflector.GetInfleter("en");
        }

        [Test]
        public void PluralizeTest() {
            foreach (var pair in words) {
                var p = Inflector.Pluralize("en", pair.Key);

                Assert.AreEqual(pair.Value, p);
            }
        }

        [Test]
        public void SingularizeTest() {
            foreach (var pair in words) {
                var p = Inflector.Singularize("en", pair.Value);

                Assert.AreEqual(pair.Key, p);
            }
        }

    }
}