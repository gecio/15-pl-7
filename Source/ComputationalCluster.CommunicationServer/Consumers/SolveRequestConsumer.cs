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

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolveRequestConsumer : IMessageConsumer<SolveRequest>
    {
        private IRepository<Problem> _repository;
        public SolveRequestConsumer(IRepository<Problem> repository)
        {
            _repository = repository;
        }


        public IMessage Consume(SolveRequest message)
        {
            int unqueId = SaveData(message);
            SolveRequestResponse response = new SolveRequestResponse
            {
                Id = (ulong)unqueId
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
        private int SaveData(SolveRequest solveRequest)
        {
            var task = new Problem
            {
                Data = solveRequest.Data,
                ProblemType = solveRequest.ProblemType,
                UniqueId = GenerateUniquId(),
            };
            if (solveRequest.SolvingTimeoutSpecified)
            {
                task.Timeout = solveRequest.SolvingTimeout;
            }
            _repository.Add(task);
            return task.UniqueId;
        }

        private int GenerateUniquId()
        {
            var lastUid = _repository.GetAll().OrderByDescending(a => a.UniqueId).Select(a => a.UniqueId).FirstOrDefault(x => true);
            return lastUid + 1;
        }
    }
}
