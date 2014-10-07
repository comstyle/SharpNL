using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    class ToolFactoryManagerTest {

        class DummyToolFactory : BaseToolFactory {
            
        }
        class DummyToolFactory2 : BaseToolFactory {
            
        }

        [Test]
        public void TestManager() {
            Assert.True(ToolFactoryManager.IsRegistered("DummyToolFactory"));

            // tool factories without default constructor shall not be loaded.
            Assert.False(ToolFactoryManager.IsRegistered("DummyToolFactory3"));
        }

    }
}
