using ComputationalCluster.Common;
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
    public class SolutionsConsumer : IMessageConsumer<Solutions>, IMessage
    {
        private readonly IPartialProblemsRepository _partialProblemsRepository;
        private readonly IProblemsRepository _problemsRepository;
        private readonly IProblemDefinitionsRepository _problemDefinitionsRepository;
        private readonly ITimeProvider _timeProvider;
        
        public SolutionsConsumer(IPartialProblemsRepository partialProblemsRepository, IProblemsRepository problemsRepository,
            IProblemDefinitionsRepository problemDefinitionsRepository, ITimeProvider timeProvider)
        {
            _partialProblemsRepository = partialProblemsRepository;
            _problemsRepository = problemsRepository;
            _problemDefinitionsRepository = problemDefinitionsRepository;
            _timeProvider = timeProvider;
        }

        public ICollection<IMessage> Consume(Solutions message, ConnectionInfo connectionInfo = null)
        {
            if (message.Solutions1[0].Type == SolutionsSolutionType.Partial)
            {
                try { SavePartialSolutions(message); }
                catch (Exception exc)
                {
                    return new IMessage[] {new Error()
                    {
                        ErrorType = ErrorErrorType.InvalidOperation,
                        ErrorMessage = exc.Message,
                    }};
                }
            }
            else if (message.Solutions1[0].Type == SolutionsSolutionType.Final)
            {
                try
                {
                    SaveFinalSolution(message);
                }
                catch (Exception exc)
                {
                    return new IMessage[] {new Error()
                    {
                        ErrorType = ErrorErrorType.InvalidOperation,
                        ErrorMessage = exc.Message,
                    }};
                }
            }

            var noOperationResponse = new NoOperation();
            return new IMessage[] { noOperationResponse };
        }

        public ICollection<IMessage> Consume(IMessage message, ConnectionInfo connectionInfo = null)
        {
            var status = message as Solutions;
            if (status == null)
            {
                throw new NotSupportedException("SolutionsConsumer consumes Solutions messages only.\n");
            }

            return Consume(status);
        }

        public void SavePartialSolutions(Solutions message)
        {
            ProblemDefinition problemDefinition = _problemDefinitionsRepository.FindByName(message.ProblemType);

            for (int i = 0; i < message.Solutions1.Length; i++)
            {
                var partialProblem = _partialProblemsRepository.Find(message.Id, message.Solutions1[i].TaskId);
                if (partialProblem == null)
                    throw new Exception("Partial problem with ProblemId=" + message.Id + " and TaskId=" + message.Solutions1[i].TaskId + " doesn't exist.");
                partialProblem.AssignedTo = null; // zakończone obliczenia
                partialProblem.CommonData = message.CommonData;
                partialProblem.Data = message.Solutions1[i].Data;
                partialProblem.Done = true;
                partialProblem.IsAwaiting = false;
            }
            _problemsRepository.StopSolvingTimedOutProblems();
        }

        public void SaveFinalSolution(Solutions message)
        {
            ProblemDefinition problemDefinition = _problemDefinitionsRepository.FindByName(message.ProblemType);
            var solution = _problemsRepository.FindById((int)message.Id);
            if (solution == null)
                throw new Exception("Problem with Id=" + message.Id + " doesn't exist.");

            solution.OutputData = message.Solutions1[0].Data;
            solution.IsAwaiting = false;
            solution.IsDone = true;
            solution.AssignedTo = null;
            solution.TimeoutOccured = false;
            solution.ComputationsTime = (ulong)_timeProvider.Now.Subtract(solution.RequestDate).TotalMilliseconds;
            
            _problemsRepository.Update(solution);

            Console.WriteLine("Solution saved: id={0}, result={1}", solution.Id, BitConverter.ToInt32(Convert.FromBase64String(solution.OutputData), 0));
            _partialProblemsRepository.RemoveFinishedProblems(message.Id);

        }
    }
}
