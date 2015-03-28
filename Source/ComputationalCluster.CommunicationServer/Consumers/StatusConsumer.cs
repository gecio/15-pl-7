using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using ComputationalCluster.TaskSolver.ArithmeticProgressionSum;
using System;
using System.IO;
using System.Text;

namespace ComputationalCluster.CommunicationServer.Consumers
{
    public class StatusConsumer : IMessageConsumer<Status>
    {
        private readonly IComponentsRepository _componentsRepository;

        public StatusConsumer(IComponentsRepository componentsRepository)
        {
            _componentsRepository = componentsRepository;
        }

        public IMessage Consume(Status message)
        {
            /* // przykład, żeby przetestować jak node'y odbierają podproblemy
            if (new Random().Next(2) == 1)
            {
                int firstMember = 2;
                int difference = 3;
                int amount = 10;
                int threadsCount = 3;

                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(firstMember);
                    writer.Write(difference);
                    writer.Write(amount);

                    var solver = new ArithmeticProgressionSumSolver(ms.GetBuffer());
                    var partialProblems = solver.DivideProblem(threadsCount);
                    byte[][] partialSolution = new byte[threadsCount][];
                    for (int i = 0; i < threadsCount; i++)
                        partialSolution[i] = solver.Solve(partialProblems[i], TimeSpan.Zero);
                    var mergedSolution = solver.MergeSolution(partialSolution);
                    var finalSolution = BitConverter.ToInt32(mergedSolution, 0);

                    int expectedSum = (amount * (2 * firstMember + difference * (amount - 1))) / 2;

                    for (int i = 0; i < threadsCount; i++)
                        Console.WriteLine("expected result of partial solution {0}: {1}", i, Convert.ToBase64String(partialSolution[i]));

                    var sppPartialProblems = new SolvePartialProblemsPartialProblem[threadsCount];
                    for (int i=0; i<threadsCount; i++)
                        sppPartialProblems[i] = new SolvePartialProblemsPartialProblem()
                        {
                            TaskId = (ulong)i,
                            Data = Convert.ToBase64String(partialProblems[i])
                        };

                    var partialProblemsResponse = new SolvePartialProblems()
                    {
                        ProblemType = "Arithmetic progression sum",
                        Id = (ulong)(new Random().Next(1000)),
                        CommonData = Convert.ToBase64String(new UTF8Encoding().GetBytes("commondata")),
                        PartialProblems = sppPartialProblems
                    };
                    
                    _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                    return partialProblemsResponse;
                }
            }
            else
            {
                var noOperationResponse = new NoOperation();
                _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                return noOperationResponse;
            }
            */

            /* // przykład, żeby przetestować jak task manager dzieli na podproblemy
            if (new Random().Next(2) == 1)
            {
                int firstMember = 2;
                int difference = 3;
                int amount = 10;
                int threadsCount = 3;

                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(firstMember);
                    writer.Write(difference);
                    writer.Write(amount);

                    var solver = new ArithmeticProgressionSumSolver(ms.GetBuffer());

                    var divideProblemResponse = new DivideProblem()
                    {
                        ProblemType = "Arithmetic progression sum",
                        Id = (ulong)(new Random().Next(1000)),
                        Data = Convert.ToBase64String(ms.GetBuffer()),
                        ComputationalNodes = (ulong)threadsCount
                    };
                    _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                    return divideProblemResponse;
                }
            }
            else
            {
                var noOperationResponse = new NoOperation();
                _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                return noOperationResponse;
            }
            */
            
            
            /*// przykład, testujący czy task manager poprawnie łączy rozwiązania częściowe
            if (new Random().Next(2) == 0)
            {
                int firstMember = 2;
                int difference = 3;
                int amount = 10;
                int threadsCount = 3;

                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(firstMember);
                    writer.Write(difference);
                    writer.Write(amount);

                    var solver = new ArithmeticProgressionSumSolver(ms.GetBuffer());
                    var partialProblems = solver.DivideProblem(threadsCount);
                    byte[][] partialSolutions = new byte[threadsCount][];
                    for (int i = 0; i < threadsCount; i++)
                        partialSolutions[i] = solver.Solve(partialProblems[i], TimeSpan.Zero);
                    var mergedSolution = solver.MergeSolution(partialSolutions);
                    var finalSolution = BitConverter.ToInt32(mergedSolution, 0);
                    int expectedSum = (amount * (2 * firstMember + difference * (amount - 1))) / 2;

                    Console.WriteLine("expected solution: {0} {1}", finalSolution, expectedSum);

                    var partialSolutionsMessage = new Solutions()
                    {
                        ProblemType = "Arithmetic progression sum",
                        Id = (ulong)(new Random().Next(1000)),
                        Solutions1 = new SolutionsSolution[partialSolutions.Length]
                    };
                    for (int i = 0; i < partialSolutions.Length; i++)
                        partialSolutionsMessage.Solutions1[i] = new SolutionsSolution()
                        {
                            Data = Convert.ToBase64String(partialSolutions[i]),
                            TaskId = (ulong)i,
                            TaskIdSpecified = true,
                            Type = SolutionsSolutionType.Partial
                        };
                    _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                    return partialSolutionsMessage;
                }
            }
            else
            {
                var noOperationResponse = new NoOperation();
                _componentsRepository.UpdateLastStatusTimestamp(message.Id);
                return noOperationResponse;
            }
            */

           
            var noOperationResponse = new NoOperation();
            _componentsRepository.UpdateLastStatusTimestamp(message.Id);
            return noOperationResponse;
            
        }

        public IMessage Consume(IMessage message)
        {
            var status = message as Status;
            if (status == null)
            {
                throw new NotSupportedException("StatusConsumer consumes Status messages only.\n");
            }

            return Consume(status);
        }
    }
}
