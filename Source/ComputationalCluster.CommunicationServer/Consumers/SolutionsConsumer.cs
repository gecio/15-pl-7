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
            //Console.WriteLine("partial solution {0}/{1}: {2}", message.Solutions1[0].TaskId, message.Id, message.Solutions1[0].Data);
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
