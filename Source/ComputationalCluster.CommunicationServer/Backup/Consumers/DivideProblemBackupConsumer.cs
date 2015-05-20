using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class DivideProblemBackupConsumer : IMessageConsumer<DivideProblem>
    {
        private readonly ISynchronizationQueue _synchronizationQueue;
        private readonly ILog _log;
        private readonly IProblemsRepository _problemsRepository;
        private readonly IComponentsRepository _componentsRepository;

        public DivideProblemBackupConsumer(ISynchronizationQueue synchronizationQueue, IProblemsRepository problemsRepository,
            IComponentsRepository componentsRepository, ILog log)
        {
            _synchronizationQueue = synchronizationQueue;
            _log = log;
            _problemsRepository = problemsRepository;
            _componentsRepository = componentsRepository;
        }

        public ICollection<IMessage> Consume(DivideProblem message, ConnectionInfo connectionInfo = null)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            var component = _componentsRepository.GetById(message.NodeID);
            var problem = _problemsRepository.FindById((int)message.Id);
            if (problem != null)
            {
                problem.IsAwaiting = false;
                problem.AssignedTo = component;
            }
            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _synchronizationQueue.Enqueue(message);
            var status = message as DivideProblem;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }

            return Consume(status);
        }
    }
}
