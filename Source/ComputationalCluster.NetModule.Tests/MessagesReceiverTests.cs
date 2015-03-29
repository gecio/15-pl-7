using Autofac;
using NUnit.Framework;
using System;
using Moq;
using ComputationalCluster.NetModule.Tests.Fakes;
using log4net;
using System.Collections.Generic;

namespace ComputationalCluster.NetModule.Tests
{
    [TestFixture]
    public class MessagesReceiverTests
    {
        private readonly string requestMessageContent = "Dummy message request.";
        private readonly string responseMessageContent = "Dummy message response.";
        private IMessage requestMessage;
        private IMessage responseMessage;

        private Mock<IMessageConsumer<TestTextMessage>> testTextConsumerMock;
        private Mock<IMessageTranslator> translatorMock;
        private Mock<ILog> _logMock;
        private IMessageReceiver receiver;

        [SetUp]
        public void SetUp()
        {
            requestMessage = new TestTextMessage(requestMessageContent);
            responseMessage = new TestTextMessage(responseMessageContent);

            translatorMock = new Mock<IMessageTranslator>();
            translatorMock.Setup(t => t.CreateObject(requestMessageContent))
                .Returns(requestMessage);
            translatorMock.Setup(t => t.Stringify(responseMessage))
                .Returns(responseMessageContent);

            testTextConsumerMock = new Mock<IMessageConsumer<TestTextMessage>>();
            testTextConsumerMock.As<IMessageConsumer>()
                .Setup(t => t.Consume(requestMessage))
                .Returns(new List<IMessage> { responseMessage });

            _logMock = new Mock<ILog>();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new FakesModule(testTextConsumerMock.Object));
            var container = builder.Build();

            receiver = new MessageReceiver(translatorMock.Object, container, _logMock.Object);
        }

        [Test]
        public void Dispatch_TestTextMessageConsumerRegistered_ConsumedSuccessfully()
        {
            string response = receiver.Dispatch(requestMessageContent);

            translatorMock.Verify(t => t.CreateObject(requestMessageContent), Times.Once);
            translatorMock.Verify(t => t.Stringify(responseMessage), Times.Once);
            testTextConsumerMock.Verify(t => t.Consume(requestMessage), Times.Once);

            Assert.AreEqual(responseMessageContent, response);
        }

    }
}
