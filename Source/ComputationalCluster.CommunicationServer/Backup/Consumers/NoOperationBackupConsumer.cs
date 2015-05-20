using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class NoOperationBackupConsumer : IMessageConsumer<NoOperation>
    {
        private ILog _log;
        private ISynchronizationQueue _synchronizationQueue;

        public NoOperationBackupConsumer(ISynchronizationQueue synchronizationQueue, ILog log)
        {
            _synchronizationQueue = synchronizationQueue;
            _log = log;
        }

        public ICollection<IMessage> Consume(NoOperation message, ConnectionInfo connectionInfo = null)
        {
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
          //  _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            _synchronizationQueue.Enqueue(message);
            return new List<IMessage>();
        }
    }
}
