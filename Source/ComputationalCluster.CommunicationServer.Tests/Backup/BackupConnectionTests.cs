using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Communication;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Backup;
using ComputationalCluster.NetModule;
using log4net;
using log4net.Config;
using Moq;
using NUnit.Framework;

namespace ComputationalCluster.CommunicationServer.Tests.Backup
{
    [TestFixture]
    public class BackupConnectionTests
    {
        private Mock<ILog> _logMock;
        private NetServer _server;
        private NetClient _client;
        private Mock<IConfigProvider> _configProviderMock;
        private Mock<IConfigProviderBackup> _configProviderBackupMock;
        private Mock<IMessageReceiver> _receiverMock;
        private UTF8Encoding _encoding;
        private IMessageTranslator _messageTranslator;
        private BackupClient _backupClient;

            
        [SetUp]
        public void SetUp()
        {
            _encoding = new UTF8Encoding();
            _messageTranslator = new MessageTranslator();
            //main Server
            _configProviderMock = new Mock<IConfigProvider>();
            _configProviderMock.Setup(t => t.IP).Returns(IPAddress.Parse("127.0.0.1"));
            _configProviderMock.Setup(t => t.Port).Returns(3000);

            //serverBackup
            _configProviderBackupMock = new Mock<IConfigProviderBackup>();
            _configProviderBackupMock.Setup(t => t.BackupMode).Returns(true);
            _configProviderBackupMock.Setup(t => t.IP).Returns(IPAddress.Parse("127.0.0.1"));
            _configProviderBackupMock.Setup(t => t.Port).Returns(3001);
            _configProviderBackupMock.Setup(t => t.MasterIP).Returns(IPAddress.Parse("127.0.0.1"));
            _configProviderBackupMock.Setup(t => t.MasterPort).Returns(3000);

            
            _receiverMock = new Mock<IMessageReceiver>();
            _logMock = new Mock<ILog>();
        }

        [Test]
        public void ConnectionTest_ConnectAsLevel0Backup_RegisteredSuccesfully()
        {
            var registerResponse = new RegisterResponse
            {
                Id = 1,
                Timeout = 20
            };

            _receiverMock.Setup(t => t.Dispatch(It.IsAny<String>(),It.IsAny<ConnectionInfo>())).Returns(_messageTranslator.Stringify(registerResponse));   

            _server = new NetServer(_receiverMock.Object, _encoding, _configProviderMock.Object, _logMock.Object);
            _client = new NetClient(_messageTranslator, _encoding, _configProviderBackupMock.Object);
            _backupClient = new BackupClient(_client,_logMock.Object,_configProviderBackupMock.Object);

            _server.Start();
            _backupClient.Start();
            _server.Start();
            
            Assert.AreEqual(registerResponse.Id,_backupClient.Id);
            Assert.AreEqual(_configProviderMock.Object.IP, _configProviderBackupMock.Object.MasterIP);
            Assert.AreEqual(_configProviderMock.Object.Port, _configProviderBackupMock.Object.MasterPort);
        }

        
    }
}
