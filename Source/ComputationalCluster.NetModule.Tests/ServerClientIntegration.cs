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

namespace ComputationalCluster.NetModule.Tests
{
    [TestFixture]
    public class ServerClientIntegration
    {
        private NetServer _server;
        private TestTextTranslator _translator;
        private Mock<IMessageReceiver> _receiverMock;
        private UTF8Encoding _encoding;
        private Mock<IConfigProvider> _configMock;

        [SetUp]
        public void SetUp()
        {
            _translator = new TestTextTranslator();
            _receiverMock = new Mock<IMessageReceiver>();
            _encoding = new UTF8Encoding();
            _configMock = new Mock<IConfigProvider>();
            _configMock.Setup(t => t.IP).Returns(IPAddress.Parse("127.0.0.1"));
            _configMock.Setup(t => t.Port).Returns(3000);
        }

        [Test]
        public void IntergationServerClient_ClientSendsOneRequest_ResponseReceived()
        {
            // todo: przez ten test leci wyjątek na sam koniec po sprawdzeniu,
            //       test przechodzi, ale problem trzeba rozwiązać, wynika on z nieco
            //       olewczego podejścia do wątków i zarządzania nimi
            var responseMessage = "Response.";
            _receiverMock.Setup(t => t.Dispatch(It.IsAny<string>())).Returns(responseMessage);

            var client = new NetClient(_translator, _encoding, _configMock.Object);
            _server = new NetServer(_receiverMock.Object, _encoding, _configMock.Object);
            
            _server.Start();
            var request = new TestTextMessage("Request.");
            var response = client.Send(new TestTextMessage("Request."));
            _server.Stop();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<TestTextMessage>(response);
            Assert.AreEqual(responseMessage, ((TestTextMessage)response).Content);
        }

        [Test]
        public void IntegrationServerClient_TwoClientsSendRequest_ResponsesReceived()
        {
            var requestMessageOne = "Request1.";
            var requestMessageTwo = "Request2.";
            var responseMessageOne = "Response1.";
            var responseMessageTwo = "Response2.";

            var receiverMock = new Mock<IMessageReceiver>();
            _receiverMock.Setup(t => t.Dispatch(It.IsAny<string>())).Returns((string input) => input == requestMessageOne ? responseMessageOne : responseMessageTwo);

            var clientOne = new NetClient(_translator, _encoding, _configMock.Object);
            var clientTwo = new NetClient(_translator, _encoding, _configMock.Object);
            _server = new NetServer(_receiverMock.Object, _encoding, _configMock.Object);

            _server.Start();
            var resposeOne = clientOne.Send(new TestTextMessage(requestMessageOne));
            var responseTwo = clientTwo.Send(new TestTextMessage(requestMessageTwo));
            _server.Stop();

            Assert.IsNotNull(resposeOne);
            Assert.IsNotNull(responseTwo);
            Assert.IsInstanceOf<TestTextMessage>(resposeOne);
            Assert.IsInstanceOf<TestTextMessage>(responseTwo);
            Assert.AreEqual(responseMessageOne, ((TestTextMessage)resposeOne).Content);
            Assert.AreEqual(responseMessageTwo, ((TestTextMessage)responseTwo).Content);
        }

    }
}
