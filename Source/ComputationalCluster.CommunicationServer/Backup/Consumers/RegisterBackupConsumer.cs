using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class RegisterBackupConsumer : IMessageConsumer<Register>
    {
        private readonly ILog _log;
        private readonly IComponentsRepository _componentsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ISynchronizationQueue _synchronizationQueue;

        public RegisterBackupConsumer(IComponentsRepository componentsRepository,ISynchronizationQueue synchronizationQueue, ITimeProvider timeProvider, ILog log)
        {
            _componentsRepository = componentsRepository;
            _synchronizationQueue = synchronizationQueue;
            _timeProvider = timeProvider;
            _log = log;
        }

        public ICollection<IMessage> Consume(Register message, ConnectionInfo connectionInfo = null)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            if (message.DeregisterSpecified && message.Deregister)
            {
                if (!message.IdSpecified)
                {
                    _log.Error("Deregister requested without specified component Id.");
                    return null;
                }

                _componentsRepository.Deregister(message.Id);
                return new List<IMessage>();
            }
            var component = new Component
            {
                Id = message.Id,
                LastStatusTimestamp = _timeProvider.Now,
                Type = message.Type,
                MaxThreads = message.ParallelThreads,
                SolvableProblems = message.SolvableProblems.Select(t => new ProblemDefinition {Name = t}).ToList()
            };
            _componentsRepository.Register(component);
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _synchronizationQueue.Enqueue(message);
            var status = message as Register;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }
            return Consume(status);
        }
    }
}
