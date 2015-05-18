using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule.Tests.Fakes
{
    public class TestTextMessage : IMessage
    {
        public TestTextMessage(string content)
        {
            Content = content;
        }

        public string Content { get; set; }
    }
}
