using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.NetModule.Tests
{
    [TestFixture]
    public class NetModuleTests
    {
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(8)]
        public void NetModule_Connect_ShouldConnect(int p)
        {
            var netModule = new NetModule();
            netModule.Connect("127.0.0.1");
            Assert.IsTrue(netModule.IsConnected);
        }
    }
}
