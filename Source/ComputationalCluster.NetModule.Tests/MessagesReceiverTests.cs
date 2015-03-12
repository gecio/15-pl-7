using Autofac;
using NUnit.Framework;
using System;
using Moq;

namespace ComputationalCluster.NetModule.Tests
{
    public class DummyMessage : IMessage
    {
        public DummyMessage()
        {
        }

        public DummyMessage(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
    }

    public class DummyModule : Module
    {
        IMessageConsumer _dummyConsumer;

        public DummyModule(IMessageConsumer dummyConsumer)
        {
            _dummyConsumer = dummyConsumer;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance<IMessageConsumer>(_dummyConsumer)
                .As<IMessageConsumer<DummyMessage>>();
        } 
    }

    [TestFixture]
    public class MessagesReceiverTests
    {
        readonly string dummyRequestMessageContent = "Dummy message request.";
        readonly string dummyResponseMessageContent = "Dummy message response.";
        IMessage dummyRequestMessage;
        IMessage dummyResponseMessage;

        Mock<IMessageConsumer<DummyMessage>> dummyConsumerMock;
        Mock<IMessageTranslator> dummyTranslatorMock;
        MessagesReceiver receiver;

        [SetUp]
        public void SetUp()
        {
            dummyRequestMessage = new DummyMessage(dummyRequestMessageContent);
            dummyResponseMessage = new DummyMessage(dummyResponseMessageContent);

            dummyTranslatorMock = new Mock<IMessageTranslator>();
            dummyTranslatorMock.Setup(t => t.CreateObject(dummyRequestMessageContent))
                .Returns(dummyRequestMessage);
            dummyTranslatorMock.Setup(t => t.Stringify(dummyResponseMessage))
                .Returns(dummyResponseMessageContent);

            dummyConsumerMock = new Mock<IMessageConsumer<DummyMessage>>();
            dummyConsumerMock.As<IMessageConsumer>()
                .Setup(t => t.Consume(dummyRequestMessage))
                .Returns(dummyResponseMessage);

            receiver = new MessagesReceiver(dummyTranslatorMock.Object, 
                new DummyModule(dummyConsumerMock.Object));
        }

        [Test]
        public void Dispatch_DummyMessageConsumerRegistered_ConsumedSuccessfully()
        {
            string response = receiver.Dispatch(dummyRequestMessageContent);

            dummyTranslatorMock.Verify(t => t.CreateObject(dummyRequestMessageContent), Times.Once);
            dummyTranslatorMock.Verify(t => t.Stringify(dummyResponseMessage), Times.Once);
            dummyConsumerMock.Verify(t => t.Consume(dummyRequestMessage), Times.Once);

            Assert.AreEqual(dummyResponseMessageContent, response);
        }
    }
}
