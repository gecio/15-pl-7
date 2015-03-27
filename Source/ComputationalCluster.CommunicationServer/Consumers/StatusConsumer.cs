using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.IO;
using System.Text;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class StatusConsumer : IMessageConsumer<Status>
    {
        private readonly IComponentsRepository _componentsRepository;

        public StatusConsumer(IComponentsRepository componentsRepository)
        {
            _componentsRepository = componentsRepository;
        }

        public IMessage Consume(Status message)
        {
            var noOperationResponse = new NoOperation();
            _componentsRepository.UpdateLastStatusTimestamp(message.Id);
            return noOperationResponse;
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
    }
}
