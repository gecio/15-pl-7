using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class StatusBackupConsumer : IMessageConsumer<Status>
    {
        IComponentsRepository _componentRepository;

        public StatusBackupConsumer(IComponentsRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public ICollection<IMessage> Consume(Status message, ConnectionInfo connectionInfo = null)
        {
            _componentRepository.UpdateLastStatusTimestamp(message.Id);
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            var status = message as Status;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }
            return Consume(status);
        }
    }
}
