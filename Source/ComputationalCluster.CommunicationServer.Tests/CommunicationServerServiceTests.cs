using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Common;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using log4net;
using log4net.Core;
using Moq;
using NUnit.Framework;

namespace ComputationalCluster.CommunicationServer.Tests
{
    [TestFixture]
    class CommunicationServerServiceTests
    {
        [Test]
        [TestCase("", new string[] { "p", "9000", "b", "t", "100" })]
        [TestCase("", new string[] { "-port", "9000", "-backup", "-time", "100" })]
        public void ApplyArgumentsConfiguresServerService(string dummy, string[] parameters)
        {
            var netServer = new Mock<INetServer>();
            var componentsRepository = new Mock<IComponentsRepository>();
            var taskSolversRepository = new Mock<ITaskSolversRepository>();
            var log = new Mock<ILog>();
            var configProvider = new ConfigProvider();
            var problemsRepository = new Mock<IProblemsRepository>();
            var cs = new CommunicationServerService(netServer.Object, componentsRepository.Object, log.Object,
                taskSolversRepository.Object, configProvider, problemsRepository.Object);

            cs.ApplyArguments(parameters);

            Assert.AreEqual(9000, configProvider.Port);
            Assert.AreEqual(true, configProvider.BackupMode);
            Assert.AreEqual(100, configProvider.Timeout);
        }
    }
}
