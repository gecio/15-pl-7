using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class StatusBackupConsumer : IMessageConsumer<Status>
    {
        private readonly IComponentsRepository _componentRepository;
        private readonly ISynchronizationQueue _synchronizationQueue;
        private readonly ILog _log;

        public StatusBackupConsumer(IComponentsRepository componentRepository,ISynchronizationQueue synchronizationQueue, ILog log)
        {
            _componentRepository = componentRepository;
            _synchronizationQueue = synchronizationQueue;
            _log = log;
        }

        public ICollection<IMessage> Consume(Status message, ConnectionInfo connectionInfo = null)
        {
            _componentRepository.UpdateLastStatusTimestamp(message.Id);
           // _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _synchronizationQueue.Enqueue(message);
            var status = message as Status;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }
            return Consume(status);
        }
    }
}
