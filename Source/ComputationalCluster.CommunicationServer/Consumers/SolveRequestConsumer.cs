using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolveRequestConsumer : IMessageConsumer<SolveRequest>
    {
        public SolveRequestConsumer()
        {
            
        }


        public IMessage Consume(SolveRequest message)
        {
            var noOperationResponse = new NoOperation();
            return noOperationResponse;
        }

        public IMessage Consume(IMessage message)
        {
            var solveRequest = message as SolveRequest;
            if (solveRequest == null)
            {
                throw new NotSupportedException("SolverRequestConsumer consumes SolveRequest ony.\n");
            }
            return Consume(solveRequest);
        }
    }
}
