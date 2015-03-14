using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication;
using ComputationalCluster.NetModule;
using ComputationalCluster.Communication.Messages;
using System.IO;

namespace ComputationalCluster.Communication.Tests
{
    [TestFixture]
    public class StatusMessageValidation
    {
        private IMessageTranslator _messageTranslator;
        private Status _status;

        [SetUp]
        public void SetUp()
        {
            _messageTranslator = new MessageTranslator();
            _status = new Status()
            {
                Id = 3,
                Threads = new StatusThread[]{
                    new StatusThread
                    {
                        HowLong = 10,
                        HowLongSpecified = true,
                        ProblemInstanceId = 12,
                        ProblemInstanceIdSpecified = true,
                        ProblemType = "testProblemType1",
                        State = StatusThreadState.Busy,
                        TaskId = 14,
                        TaskIdSpecified = false
                    },

                    new StatusThread
                    {
                        HowLong = 15,
                        HowLongSpecified = true,
                        ProblemInstanceId = 17,
                        ProblemInstanceIdSpecified = true,
                        ProblemType = "testProblemType2",
                        State = StatusThreadState.Idle,
                        TaskId = 19,
                        TaskIdSpecified = false
                    },
                }
            };
        }

        [Test]
        public void StringifyMessage_StatusMessage_XMLString()
        {
            var XMLresult = _messageTranslator.Stringify(_status);
            var validator = new ComputationalCluster.Communication.Tests.XmlSchemaValidator(@"..\..\xsd\Status.xsd");
            var isValid = validator.IsValid(XMLresult);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void CreateObjectFromString_StatusXML_StatusObject()
        {
            var xmlMessage = _messageTranslator.Stringify(_status);
            IMessage result = _messageTranslator.CreateObject(xmlMessage);

            Assert.IsInstanceOf<Status>(result);
            Assert.AreEqual(_status.Id, (result as Status).Id);
            Assert.AreEqual(_status.Threads[0].HowLong, (result as Status).Threads[0].HowLong);
            

        }

    }
}
