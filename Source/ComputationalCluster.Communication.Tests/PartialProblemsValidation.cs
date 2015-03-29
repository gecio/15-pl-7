using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using NUnit.Core;
using NUnit.Framework;

namespace ComputationalCluster.Communication.Tests
{
    [TestFixture]
    public class PartialProblemsValidation
    {
        private IMessageTranslator _messageTranslator;
        private SolvePartialProblems _partialProblems;
        private string _data;

        [SetUp]
        public void SetUp()
        {
            _messageTranslator = new MessageTranslator();
            _data = "Test Data";
            _partialProblems = new SolvePartialProblems
            {
                CommonData = Convert.ToBase64String(Encoding.UTF8.GetBytes(_data)),
                Id = 1,
                ProblemType = "asd",
                SolvingTimeout = 100,
                SolvingTimeoutSpecified = true,
                PartialProblems = new[]
                {
                    new SolvePartialProblemsPartialProblem
                    {
                        Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(_data)),
                        NodeID = 1,
                        TaskId = 2
                    }
                }
            };

        }

        [Test]
        public void StringifyMessage_PartialProblemMessage_XMLString()
        {
            var XMLresult = _messageTranslator.Stringify(_partialProblems);
            var validator = new ComputationalCluster.Communication.Tests.XmlSchemaValidator(@"..\..\xsd\PartialProblems.xsd");
            var isValid = validator.IsValid(XMLresult);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void CreateObjectFromString_PartialProblemXML_PartialProblemObject()
        {
            var xmlMessage = _messageTranslator.Stringify(_partialProblems);
            IMessage result = _messageTranslator.CreateObject(xmlMessage);

            Assert.IsInstanceOf<SolvePartialProblems>(result);
            SolvePartialProblems tmp = result as SolvePartialProblems;
            Assert.AreEqual(_partialProblems.Id, tmp.Id);
            Assert.AreEqual(_partialProblems.ProblemType, tmp.ProblemType);
            Assert.AreEqual(_partialProblems.SolvingTimeout, tmp.SolvingTimeout);
            Assert.AreEqual(_data, Encoding.UTF8.GetString(Convert.FromBase64String(tmp.CommonData)));
            Assert.AreEqual(_data, Encoding.UTF8.GetString(Convert.FromBase64String(tmp.PartialProblems[0].Data)));
            Assert.AreEqual(_partialProblems.PartialProblems[0].NodeID, tmp.PartialProblems[0].NodeID);
            Assert.AreEqual(_partialProblems.PartialProblems[0].TaskId, tmp.PartialProblems[0].TaskId);
      
        }
    }
}
