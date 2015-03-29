using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using NUnit.Framework;

namespace ComputationalCluster.Communication.Tests
{
    public class MessageValidation

    {
        private IMessageTranslator _messageTranslator;
        private NoOperation _noOperation;

        [SetUp]
        public void SetUp()
        {
            _messageTranslator = new MessageTranslator();

            #region NoOperation

            _noOperation = new NoOperation
            {
                BackupCommunicationServers = new NoOperationBackupCommunicationServers
                {
                    BackupCommunicationServer = new NoOperationBackupCommunicationServersBackupCommunicationServer
                    {
                        address = "127.0.0.1",
                        port = 1111,
                        portSpecified = true
                    }
                }
            };

            #endregion
        }

        [Test]
        public void StringifyMessage_NoOperationMessage_XMLString()
        {
            var XMLresult = _messageTranslator.Stringify(_noOperation);
            var validator = new ComputationalCluster.Communication.Tests.XmlSchemaValidator(@"..\..\xsd\NoOperation.xsd");
            var isValid = validator.IsValid(XMLresult);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void CreateObjectFromString_NoOperationXML_NoOperationObject()
        {
            var xmlMessage = _messageTranslator.Stringify(_noOperation);
            IMessage result = _messageTranslator.CreateObject(xmlMessage);

            Assert.IsInstanceOf<NoOperation>(result);
            NoOperation tmp = result as NoOperation;
            Assert.AreEqual(_noOperation.BackupCommunicationServers.BackupCommunicationServer.address, tmp.BackupCommunicationServers.BackupCommunicationServer.address);
            Assert.AreEqual(_noOperation.BackupCommunicationServers.BackupCommunicationServer.port,
                tmp.BackupCommunicationServers.BackupCommunicationServer.port);
        }
    }
}
