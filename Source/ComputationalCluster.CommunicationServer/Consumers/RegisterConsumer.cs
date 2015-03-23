using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class RegisterConsumer : IMessageConsumer<Register>
    {
        private readonly IComponentsRepository _componentsRepository;

        public RegisterConsumer(IComponentsRepository componentsRepository)
        {
            _componentsRepository = componentsRepository;
        }

        public IMessage Consume(Register message)
        {
            System.Console.WriteLine("Register {0}", message.Id);

            var component = new Component()
            {
                Id = message.Id,
                LastStatusTimestamp = DateTime.Now, // TODO: PROVIDER
                Type = message.Type,
            };

            _componentsRepository.Add(component);

            var response = new RegisterResponse()
            {
                Id = message.Id,
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
