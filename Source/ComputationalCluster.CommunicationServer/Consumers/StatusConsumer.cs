using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.NetModule;
using ComputationalCluster.Communication.Messages;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class StatusConsumer : IMessageConsumer<Status>
    {
        public StatusConsumer()
        {
        }

        public IMessage Consume(IMessage message)
        {
            var status = message as Status;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }

            return Consume(status);
        }

        public IMessage Consume(Status message)
        {
            var noOperationResponse = new NoOperation();
            return noOperationResponse;
        }
    }
}
