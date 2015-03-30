using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public ICollection<IMessage> Consume(Register message)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());

            // Null object - dlaczego nie jest length=0 od razu?
            if (message.SolvableProblems == null)
            {
                message.SolvableProblems = new string[] { };
            }

            /* Backup feature
            if (message.DeregisterSpecified && message.Deregister)
            {
                if (!message.IdSpecified)
                {
                    _log.Error("Deregister requested without specified component Id.");
                    return null;
                }

                _componentsRepository.Deregister(message.Id);
                return null;
            }
            */

            if (message.SolvableProblems.Length == 0)
            {
                _log.Warn("Registering component with no solvable problems.");
            }

            var component = new Component()
            {
                LastStatusTimestamp = DateTime.Now,
                Type = message.Type,
                MaxThreads = message.ParallelThreads,
                SolvableProblems = message.SolvableProblems.Select(t => new ProblemDefinition { Name = t }).ToList(),
            };

            var guid = _componentsRepository.Register(component);

            var response = new RegisterResponse()
            {
                Id = guid,
                Timeout = 30, // todo: config
            };

            return new IMessage[] { response };
        }

        public ICollection<IMessage> Consume(IMessage message)
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
