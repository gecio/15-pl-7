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
        
        public SolutionsConsumer(IPartialProblemsRepository partialProblemsRepository, IProblemsRepository problemsRepository, IProblemDefinitionsRepository problemDefinitionsRepository)
        {
            _partialProblemsRepository = partialProblemsRepository;
            _problemsRepository = problemsRepository;
            _problemDefinitionsRepository = problemDefinitionsRepository;
        }

        public ICollection<IMessage> Consume(Solutions message)
        {
            //Console.WriteLine("final solution: ID={0}, final solution={1}", message.Id, BitConverter.ToInt32(Convert.FromBase64String(message.Solutions1[0].Data), 0));
            //Console.WriteLine("partial solution {0}/{1}: {2}", message.Solutions1[0].TaskId, message.Id, message.Solutions1[0].Data);

            if (message.Solutions1[0].Type == SolutionsSolutionType.Partial)
                SavePartialSolutions(message);
            else if (message.Solutions1[0].Type == SolutionsSolutionType.Final)
                SaveFinalSolution(message);

            var noOperationResponse = new NoOperation();
            return new IMessage[] { noOperationResponse };
        }

        public ICollection<IMessage> Consume(IMessage message)
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

            for (int i=0; i<message.Solutions1.Length; i++)
            {
                var partialProblem = _partialProblemsRepository.FindById((int)message.Solutions1[i].TaskId);
                partialProblem.CommonData = message.CommonData;
                partialProblem.Data = message.Solutions1[i].Data;
                partialProblem.Done = true;
                partialProblem.IsAwaiting = false;


                //var partialSolution = new OrderedPartialProblem()
                //{
                //    ProblemDefinition = problemDefinition,
                //    Id = message.Id,
                //    CommonData = message.CommonData,
                //    TaskId = message.Solutions1[i].TaskId,
                //    Data = message.Solutions1[i].Data,
                //    IsAwaiting = true,
                //    Done = true,
                //};
                //_partialProblemsRepository.Add(partialSolution);
            }
        }

        public void SaveFinalSolution(Solutions message)
        {
            ProblemDefinition problemDefinition = _problemDefinitionsRepository.FindByName(message.ProblemType);
            var solution = _problemsRepository.FindById((int)message.Id);
            if (solution != null)
            {
                solution.OutputData = message.Solutions1[0].Data;
                solution.IsAwaiting = false;
                solution.IsDone = true;
            }
            //var solution = new Problem()
            //{
            //    Id = message.Id,
            //    OutputData = message.Solutions1[0].Data,
            //    ProblemDefinition = problemDefinition,
            //    ProblemType = problemDefinition,
            //    IsAwaiting = false,
            //    IsDone = true,
            //};
            _problemsRepository.Update(solution);

            Console.WriteLine("Solution saved: id={0}, result={1}", solution.Id, BitConverter.ToInt32(Convert.FromBase64String(solution.OutputData), 0));
            _partialProblemsRepository.RemoveFinishedProblems(message.Id);

        }
    }
}
