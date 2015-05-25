using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class SolutionsBackupConsumer : IMessageConsumer<Solutions>
    {
        private readonly ISynchronizationQueue _synchronizationQueue;
        private readonly IMessageConsumer<Solutions> _solutionMessageConsumer; 

        public SolutionsBackupConsumer(ISynchronizationQueue synchronizationQueue, IMessageConsumer<Solutions> solutionsMessageConsumer )
        {
            _synchronizationQueue = synchronizationQueue;
            _solutionMessageConsumer = solutionsMessageConsumer;
        }

        public ICollection<IMessage> Consume(Solutions message, ConnectionInfo connectionInfo = null)
        {
            _solutionMessageConsumer.Consume(message);
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _synchronizationQueue.Enqueue(message);
            var solutions = message as Solutions;
            if (solutions == null)
            {
                throw new NotSupportedException("RegisterConsumer consumes Register messages only.\n");
            }

            return Consume(solutions);
        }
    }
}
