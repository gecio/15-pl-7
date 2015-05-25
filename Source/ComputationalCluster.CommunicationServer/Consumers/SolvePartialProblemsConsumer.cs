using Autofac;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class SolvePartialProblemsConsumer : IMessageConsumer<SolvePartialProblems>
    {
        private readonly IPartialProblemsRepository _partialProblemsRepository;
        private readonly IProblemDefinitionsRepository _problemDefinitionsRepository;
        private readonly IProblemsRepository _problemsRepository;
        private readonly ILog _log;

        public SolvePartialProblemsConsumer(IPartialProblemsRepository partialProblemsRepository, IProblemDefinitionsRepository problemDefinitionsRepository, IProblemsRepository problemsRepository,
            ILog log)
        {
            _partialProblemsRepository = partialProblemsRepository;
            _problemDefinitionsRepository = problemDefinitionsRepository;
            _problemsRepository = problemsRepository;
            _log = log;
        }

        public ICollection<IMessage> Consume(SolvePartialProblems message, ConnectionInfo connectionInfo = null)
        {
            if (_problemsRepository.FindById((int)message.Id) == null)
                return new IMessage[] {new Error()
                {
                    ErrorType = ErrorErrorType.InvalidOperation,
                    ErrorMessage = "Problem with Id="+message.Id+" doesn't exist."
                }};

            SaveData(message);
            var noOperationResponse = new NoOperation();
            //Console.WriteLine("Received partial problem: number of tasks={0}, ID={1}", message.PartialProblems.Count(), message.Id);
            return new IMessage[] { noOperationResponse };
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            var status = message as SolvePartialProblems;
            if (status == null)
            {
                throw new NotSupportedException("SolvePartialProblemsConsumer consumes SolvePartialProblems messages only.\n");
            }

            return Consume(status);
        }

        public void SaveData(SolvePartialProblems message)
        {
            ProblemDefinition problemDefinition = _problemDefinitionsRepository.FindByName(message.ProblemType);
            var problem = _problemsRepository.FindById((int)message.Id);
            problem.AssignedTo = null; //zadanie zostało zakończone przez TM
            for (int i = 0; i < message.PartialProblems.Length; i++)
            {
                var partialProblem = new OrderedPartialProblem()
                {
                    Id = message.Id,
                    TaskId = message.PartialProblems[i].TaskId,
                    ProblemDefinition = problemDefinition,
                    CommonData = message.CommonData,
                    Data = message.PartialProblems[i].Data,
                //    NodeId = message.PartialProblems[i].NodeID,
                    Timeout = problem.Timeout,
                    Done = false,
                    IsAwaiting = true
                };
                _partialProblemsRepository.Add(partialProblem);
            }
            _problemsRepository.StopSolvingTimedOutProblems();
        }
    }
}
