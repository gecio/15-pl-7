using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.NetModule;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.Common;
using ComputationalCluster.CommunicationServer.Backup;
using log4net;
using ComputationalCluster.CommunicationServer.Models;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolveRequestConsumer : IMessageConsumer<SolveRequest>
    {
        private readonly ISynchronizationQueue _synchronizationQueue;
        private readonly IProblemsRepository _problemsRepository;
        private readonly IProblemDefinitionsRepository _problemDefinitionsRepository;
        private readonly IComponentsRepository _componentsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;

        public SolveRequestConsumer(IProblemsRepository problemRepository, IProblemDefinitionsRepository problemDefinitionsRepository,
            ISynchronizationQueue synchronizationQueue, IComponentsRepository componentsRepository,  ITimeProvider timeProvider, ILog log)
        {
            _synchronizationQueue = synchronizationQueue;
            _problemsRepository = problemRepository;
            _timeProvider       = timeProvider;
            _log                = log;
            _problemDefinitionsRepository = problemDefinitionsRepository;
            _componentsRepository = componentsRepository;
        }


        public ICollection<IMessage> Consume(SolveRequest message, ConnectionInfo connectionInfo = null)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            ulong unqueId = SaveData(message);
            SolveRequestResponse response = new SolveRequestResponse
            {
                Id = unqueId
            };

            message.Id = unqueId;
            message.IdSpecified = true;
            _synchronizationQueue.Enqueue(message);

            return new IMessage[] { response, PrepareNoOperationMessage() };
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            var solveRequest = message as SolveRequest;
            if (solveRequest == null)
            {
                _log.Error("SolverRequestConsumer consumes SolveRequest ony");
                throw new NotSupportedException("SolverRequestConsumer consumes SolveRequest ony.\n");
            }
            return Consume(solveRequest);
        }

        private ulong SaveData(SolveRequest solveRequest)
        {
            var problemDefinition = _problemDefinitionsRepository.FindByName(solveRequest.ProblemType);
            if (problemDefinition == null)
            {
                problemDefinition = new ProblemDefinition
                {
                    AvailableComputationalNodes = 0,
                    AvailableTaskManagers = 0,
                    Name = solveRequest.ProblemType
                };
                _problemDefinitionsRepository.Add(problemDefinition);
            }

            var orderedProblem = new Problem
            {
                Id = solveRequest.Id > 0 ? solveRequest.Id : 0,
                InputData = solveRequest.Data,
                Timeout = solveRequest.SolvingTimeout,
                ProblemDefinition = problemDefinition,
                RequestDate = _timeProvider.Now,
                IsAwaiting = true,
            };
            return _problemsRepository.Add(orderedProblem);
        }

        private IMessage PrepareNoOperationMessage()
        {
            var backup = _componentsRepository.GetBackupServer();
            if (backup == null)
            {
                return new NoOperation();
            }

            return new NoOperation
            {
                BackupCommunicationServers = new NoOperationBackupCommunicationServers
                {
                    BackupCommunicationServer = new NoOperationBackupCommunicationServersBackupCommunicationServer
                    {
                        address = ((BackupComponent)backup).IpAddress.ToString(),
                        port = (ushort)((BackupComponent)backup).Port,
                        portSpecified = true
                    }
                }
            };
        }

    }
}
