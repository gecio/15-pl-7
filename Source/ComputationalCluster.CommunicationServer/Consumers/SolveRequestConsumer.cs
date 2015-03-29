using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Database;
using ComputationalCluster.NetModule;
using ComputationalCluster.CommunicationServer.Database.Entities;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.Common;
using log4net;
using ComputationalCluster.CommunicationServer.Models;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolveRequestConsumer : IMessageConsumer<SolveRequest>
    {
        private readonly IProblemsRepository _problemsRepository;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;

        public SolveRequestConsumer(IProblemsRepository problemRepository, ITimeProvider timeProvider,
            ILog log)
        {
            _problemsRepository = problemRepository;
            _timeProvider = timeProvider;
            _log = log;
        }


        public IMessage Consume(SolveRequest message)
        {
            _log.InfoFormat("Consuming {0} = [{1}]", message.GetType().Name, message.ToString());
            ulong unqueId = SaveData(message);
            SolveRequestResponse response = new SolveRequestResponse
            {
                Id = unqueId
            };
            return response;
        }

        public IMessage Consume(IMessage message)
        {
            var solveRequest = message as SolveRequest;
            if (solveRequest == null)
            {
                throw new NotSupportedException("SolverRequestConsumer consumes SolveRequest ony.\n");
            }
            return Consume(solveRequest);
        }

        //todo: opakować w transakcje
        private ulong SaveData(SolveRequest solveRequest)
        {
            var task = new OrderedProblem
            {
                Data = solveRequest.Data,
                Timeout = solveRequest.SolvingTimeout
                //TODO: ProblemDefinitio??? 
            };
            return _problemsRepository.Add(task);
        }

    }
}
