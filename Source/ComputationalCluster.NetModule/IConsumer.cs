using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    public interface IMessageConsumer
    {
        IMessage Consume(IMessage message);
    }

    public interface IMessageConsumer<TMessage> : IMessageConsumer
        where TMessage: IMessage
    {
        IMessage Consume(TMessage message);
    }
}
