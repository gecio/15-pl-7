using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule.Tests.Fakes
{
    public class TestTextTranslator : IMessageTranslator
    {
        public IMessage CreateObject(string message)
        {
            return new TestTextMessage(message);
        }

        public string Stringify(IMessage message)
        {
            return ((TestTextMessage)message).Content;
        }
    }
}
