using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    public interface IMessageTranslator
    {
        IMessage CreateObject(string message);
        string Stringify(IMessage message);
    }
}
