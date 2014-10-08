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

using NUnit.Framework;
using SharpNL.Utility;
using SharpNL.Utility.FeatureGen;

namespace SharpNL.Tests.Utility {
    /// <summary>
    /// This class test the <see cref="TypeResolver"/>.
    /// </summary>
    [TestFixture]
    internal class TypeResolverTest {

        class DummyObjectOne { }
        class DummyObjectTwo { }

        [Test]
        public void TestResolverFromLibrary() {
            Assert.False(Library.TypeResolver.IsRegistered("Wot!"));

            Assert.AreEqual(
                typeof(FeatureGeneratorAdapter), Library.TypeResolver.ResolveType("opennlp.tools.util.featuregen.FeatureGeneratorAdapter")
            );
        }

        [Test]
        public void TestResolver() {
            var typeResolver = new TypeResolver();

            typeResolver.Register("DummyType", typeof(DummyObjectOne));

            Assert.False(typeResolver.IsRegistered("DummyUnknown"));
            Assert.IsNull(typeResolver.ResolveType("DummyUnknown"));

            Assert.True(typeResolver.IsRegistered("DummyType"));
            Assert.AreEqual("DummyType", typeResolver.ResolveName(typeof(DummyObjectOne)));
            Assert.AreEqual(typeof(DummyObjectOne), typeResolver.ResolveType("DummyType"));

            typeResolver.Overwrite("DummyType", typeof(DummyObjectTwo));
            Assert.True(typeResolver.IsRegistered("DummyType"));
            Assert.AreEqual("DummyType", typeResolver.ResolveName(typeof(DummyObjectTwo)));
            Assert.AreEqual(typeof(DummyObjectTwo), typeResolver.ResolveType("DummyType"));
        }
    }
}