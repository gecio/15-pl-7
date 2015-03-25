using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;
using System;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class RegisterConsumer : IMessageConsumer<Register>
    {
        private readonly IComponentsRepository _componentsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;

        public RegisterConsumer(IComponentsRepository componentsRepository, ITimeProvider timeProvider,
            ILog log)
        {
            _componentsRepository = componentsRepository;
            _timeProvider         = timeProvider;
            _log                  = log;
        }

        public IMessage Consume(Register message)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());

            var component = new Component()
            {
                LastStatusTimestamp = DateTime.Now,
                Type = message.Type,
            };

            var guid = _componentsRepository.Register(component);

            var response = new RegisterResponse()
            {
                Id = guid,
                Timeout = 30, // todo setup
            };

            return response;
        }

        public IMessage Consume(IMessage message)
        {
            var status = message as Register;
            if (status == null)
            {
                throw new NotSupportedException("RegisterConsumer consumes Register messages only.\n");
            }

            return Consume(status);
        }
    }
}
