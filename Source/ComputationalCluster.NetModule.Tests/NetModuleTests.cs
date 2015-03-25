using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ComputationalCluster.NetModule;
using ComputationalCluster.NetModule.Tests.Fakes;
using Moq;
using ComputationalCluster.Common;
using System.Net;
using Autofac;
using log4net;

namespace ComputationalCluster.NetModule.Tests
{
    [TestFixture]
    public class NetModuleTests
    {
        private NetServer _server;
        private TestTextTranslator _translator;
        private Mock<IMessageReceiver> _receiverMock;
        private UTF8Encoding _encoding;
        private Mock<IConfigProvider> _configMock;
        private Mock<ILog> _logMock;

        [SetUp]
        public void SetUp()
        {
            _translator = new TestTextTranslator();
            _receiverMock = new Mock<IMessageReceiver>();
            _encoding = new UTF8Encoding();

            _configMock = new Mock<IConfigProvider>();
            _configMock.Setup(t => t.IP).Returns(IPAddress.Parse("127.0.0.1"));
            _configMock.Setup(t => t.Port).Returns(3000);

            _logMock = new Mock<ILog>();
        }

        [Test]
        public void SendingMessage_SendsLongRequest_MessageReceived()
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for(int i =0; i < 1024 * 41; i++)
            {
                sb.Append((char)('a' + rnd.Next(0,25)));
            }
            var message = sb.ToString();


            _receiverMock.Setup(t => t.Dispatch(It.IsAny<string>())).Returns((string msg) =>
            {
                Assert.AreEqual(message.Length, msg.Length);
                Assert.AreEqual(message, msg);
                return msg;
            });

            var client = new NetClient(_translator, _encoding, _configMock.Object);
            _server = new NetServer(_receiverMock.Object, _encoding, _configMock.Object, _logMock.Object);
            
            _server.Start();
            var response = client.Send(new TestTextMessage(message));
            _server.Stop();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<TestTextMessage>(response);
            Assert.AreEqual(message, ((TestTextMessage)response).Content);
        }

    }
}
