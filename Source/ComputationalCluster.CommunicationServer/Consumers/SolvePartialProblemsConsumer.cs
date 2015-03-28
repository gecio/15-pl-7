using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolvePartialProblemsConsumer : IMessageConsumer<SolvePartialProblems>
    {
        public SolvePartialProblemsConsumer()
        {

        }

        public IMessage Consume(SolvePartialProblems message)
        {
            var noOperationResponse = new NoOperation();
            //Console.WriteLine("Received partial problem: number of tasks={0}, ID={1}", message.PartialProblems.Count(), message.Id);
            return noOperationResponse;
        }

        public IMessage Consume(IMessage message)
        {
            var status = message as SolvePartialProblems;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes SolvePartialProblems messages only.\n");
            }

            return Consume(status);
        }
    }
}
