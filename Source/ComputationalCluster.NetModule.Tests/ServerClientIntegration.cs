using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ComputationalCluster.NetModule;
using ComputationalCluster.NetModule.Tests.Fakes;
using Moq;

namespace ComputationalCluster.NetModule.Tests
{
    [TestFixture]
    public class ServerClientIntegration
    {
        [Test]
        public void IntergationServerClient_ClientSendsOneRequest_ResponseReceived()
        {
            // todo: przez ten test leci wyjątek na sam koniec po sprawdzeniu,
            //       test przechodzi, ale problem trzeba rozwiązać, wynika on z nieco
            //       olewczego podejścia do wątków i zarządzania nimi

            var translator = new TestTextTranslator();

            var responseMessage = "Response.";
            var receiverMock = new Mock<IMessageReceiver>();
            receiverMock.Setup(t => t.Dispatch(It.IsAny<string>())).Returns(responseMessage);

            var encoding = new UTF8Encoding();

            var server = new NetServer(receiverMock.Object, encoding);
            var client = new NetClient(translator, encoding);
            
            server.Start();
            var request = new TestTextMessage("Request.");
            var response = client.Send(new TestTextMessage("Request."));
            server.Stop();

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<TestTextMessage>(response);
            Assert.AreEqual(responseMessage, ((TestTextMessage)response).Content);
        }
    }
}
