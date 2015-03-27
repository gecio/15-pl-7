using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolutionsConsumer : IMessageConsumer<Solutions>
    {
        public SolutionsConsumer()
        {
        }

        public IMessage Consume(Solutions message)
        {
            var noOperationResponse = new NoOperation();
            return noOperationResponse;
        }

        public IMessage Consume(IMessage message)
        {
            var status = message as Solutions;
            if (status == null)
            {
                throw new NotSupportedException("SolutionsConsumer consumes Solutions messages only.\n");
            }

            return Consume(status);
        }
    }
}
