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
    [TestFixture]
    public class DivideProblemValidation
    {

        private IMessageTranslator _messageTranslator;
        private DivideProblem _divideProblem;
        private string _data;

        [SetUp]
        public void SetUp()
        {
            _messageTranslator = new MessageTranslator();
            _data = "Test Data";
            _divideProblem = new DivideProblem
            {
                ProblemType = "TestProblemType",
                Id = 10,
                ComputationalNodes = 10,
                NodeID = 11,
                Data = Convert.ToBase64String(Encoding.UTF8.GetBytes( _data))
            };

        }

        [Test]
        public void StringifyMessage_DivideProblemMessage_XMLString()
        {
            var XMLresult = _messageTranslator.Stringify(_divideProblem);
            var validator = new ComputationalCluster.Communication.Tests.XmlSchemaValidator(@"..\..\xsd\DivideProblem.xsd");
            var isValid = validator.IsValid(XMLresult);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void CreateObjectFromString_DivideProblemXML_DivideProblemObject()
        {
            var xmlMessage = _messageTranslator.Stringify(_divideProblem);
            IMessage result = _messageTranslator.CreateObject(xmlMessage);

            Assert.IsInstanceOf<DivideProblem>(result);
            DivideProblem tmp = result as DivideProblem;
            Assert.AreEqual(_divideProblem.ComputationalNodes,tmp.ComputationalNodes);
            Assert.AreEqual(_divideProblem.Id,tmp.Id);
            Assert.AreEqual(_divideProblem.NodeID, tmp.NodeID);
            Assert.AreEqual(_divideProblem.ProblemType, tmp.ProblemType);
            Assert.AreEqual(_data, Encoding.UTF8.GetString(Convert.FromBase64String(tmp.Data)));
        }
    }
}
