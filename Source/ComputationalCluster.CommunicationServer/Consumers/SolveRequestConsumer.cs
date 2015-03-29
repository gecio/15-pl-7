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
using log4net;
using ComputationalCluster.CommunicationServer.Models;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolveRequestConsumer : IMessageConsumer<SolveRequest>
    {
        private readonly IProblemsRepository _problemsRepository;
        private readonly IProblemDefinitionsRepository _problemDefinitionsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;

        public SolveRequestConsumer(IProblemsRepository problemRepository, IProblemDefinitionsRepository problemDefinitionsRepository, ITimeProvider timeProvider,
            ILog log)
        {
            _problemsRepository = problemRepository;
            _timeProvider       = timeProvider;
            _log                = log;
            _problemDefinitionsRepository = problemDefinitionsRepository;
        }


        public ICollection<IMessage> Consume(SolveRequest message)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            ulong unqueId = SaveData(message);
            SolveRequestResponse response = new SolveRequestResponse
            {
                Id = unqueId
            };
            return new IMessage[] { response };
        }

        public ICollection<IMessage> Consume(IMessage message)
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
                InputData = solveRequest.Data,
                Timeout = solveRequest.SolvingTimeout,
                ProblemDefinition = problemDefinition,
                RequestDate = _timeProvider.Now,
                IsAwaiting = true,
            };
            return _problemsRepository.Add(orderedProblem);
        }

    }
}
