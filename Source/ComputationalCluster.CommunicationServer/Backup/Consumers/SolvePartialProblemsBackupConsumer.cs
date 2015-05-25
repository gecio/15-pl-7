using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;

namespace ComputationalCluster.CommunicationServer.Backup.Consumers
{
    public class SolvePartialProblemsBackupConsumer : IMessageConsumer<SolvePartialProblems>
    {
        private readonly ISynchronizationQueue _synchronizationQueue;
        private readonly ILog _log;
        private readonly IPartialProblemsRepository _partialProblemsRepository;
        private readonly IComponentsRepository _componentsRepository;
        private readonly IProblemDefinitionsRepository _problemsRepository;

        public SolvePartialProblemsBackupConsumer(ISynchronizationQueue synchronizationQueue, IPartialProblemsRepository partialProblemsRepository, ILog log,
            IComponentsRepository componentsRepository, IProblemDefinitionsRepository problemsRepository)
        {
            _synchronizationQueue = synchronizationQueue;
            _log = log;
            _partialProblemsRepository = partialProblemsRepository;
            _componentsRepository = componentsRepository;
            _problemsRepository = problemsRepository;
        }

        public ICollection<IMessage> Consume(SolvePartialProblems message, ConnectionInfo connectionInfo = null)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());

            foreach (var partial in message.PartialProblems)
            {
                //jeżeli od TM
                if (partial.NodeID == 0)
                {
                    ProblemDefinition problemDefinition = _problemsRepository.FindByName(message.ProblemType);
                    var partialProblem = new OrderedPartialProblem()
                    {
                        Id = message.Id,
                        TaskId = partial.TaskId,
                        ProblemDefinition = problemDefinition,
                        CommonData = message.CommonData,
                        Data = partial.Data,
                        NodeId = partial.NodeID,
                        Timeout = message.SolvingTimeoutSpecified ? message.SolvingTimeout : 0,
                        Done = false,
                        IsAwaiting = true
                    };
                    _partialProblemsRepository.Add(partialProblem);
                    continue;;
                }
                //jeżeli od Node
                var subTask = _partialProblemsRepository.Find(message.Id, partial.TaskId);
                var component = _componentsRepository.GetById(partial.NodeID);
                subTask.IsAwaiting = false;
                subTask.AssignedTo = component;
            }

            return new List<IMessage>();
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            _synchronizationQueue.Enqueue(message);
            var solvePartialProblems = message as SolvePartialProblems;
            if (solvePartialProblems == null)
            {
                throw new NotSupportedException("RegisterConsumer consumes Register messages only.\n");
            }

            return Consume(solvePartialProblems);
        }
    }
}
