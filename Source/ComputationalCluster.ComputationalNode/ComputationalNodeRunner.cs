using Autofac;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCCTaskSolver;

namespace ComputationalCluster.ComputationalNode
{
    public class ComputationalNodeRunner
    {
        private LinkedList<PartialSolution> _partialSolutions;
        private Semaphore _semaphorePartialSolutions;

        private ITaskSolversRepository _taskSolversRepository;
        private INetClient _client;
        
        public ComputationalNodeRunner()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalNodeModule>();
            var container = builder.Build();

            _partialSolutions = new LinkedList<PartialSolution>();
            _semaphorePartialSolutions = new Semaphore(1, 1);

            _taskSolversRepository = container.Resolve<ITaskSolversRepository>();
            _client = container.Resolve<INetClient>();
        }

        public void Start()
        {
            var response = _client.Send(new Register()
            {
                Type = RegisterType.ComputationalNode,
            }) as RegisterResponse;

            Console.WriteLine("Response {0}", response.Id);

            while(true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, (int)(response.Timeout/2)));
                var receivedMessage = _client.Send(new Status()
                {
                    Id = response.Id,
                });
                Console.WriteLine("Response {0}", receivedMessage.GetType().Name);

                Consume(receivedMessage);
                SendPartialSolutions();
            }
        }

        public void Stop()
        {
        }

        /// <summary>
        /// Obsługuje odebraną wiadomość typu SolvePartialProblems - tworzy wątki i zleca im rozwiązanie podproblemów.
        /// </summary>
        /// <param name="receivedMessage">wiadomość odebrana od serwera</param>
        public void Consume(IMessage receivedMessage)
        {
            if (receivedMessage.GetType() == typeof(SolvePartialProblems))
            {
                var partialProblems = (receivedMessage as SolvePartialProblems).PartialProblems;
                for (int i = 0; i < partialProblems.Length; i++)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(SolvePartialProblem));
                    PartialProblem task = new PartialProblem
                    {
                        ProblemType = (receivedMessage as SolvePartialProblems).ProblemType,
                        ProblemId = (receivedMessage as SolvePartialProblems).Id,
                        CommonData = (receivedMessage as SolvePartialProblems).CommonData,
                        TaskId = partialProblems[i].TaskId,
                        Data = partialProblems[i].Data
                    };
                    thread.Start(task);
                }
            }
        }

        /// <summary>
        /// Rozwiązuje pojedynczy podproblem i wstawia rozwiązanie do kolejki do wysłania.
        /// </summary>
        /// <param name="problem">informacje o podproblemie do rozwiązania</param>
        public void SolvePartialProblem(object problem)
        {
            TaskSolver solver = _taskSolversRepository.GetSolverByName((problem as PartialProblem).ProblemType);
            byte[] data = Convert.FromBase64String((problem as PartialProblem).Data);
            byte[] solution = solver.Solve(data, new TimeSpan(1, 0, 0));

            Thread.Sleep(new TimeSpan(0,1,30));
            var solutionMessage = new PartialSolution()
            {
                ProblemType = (problem as PartialProblem).ProblemType,
                ProblemId = (problem as PartialProblem).ProblemId,
                TaskId = (problem as PartialProblem).TaskId,
                Data = Convert.ToBase64String(solution)
            };
            _semaphorePartialSolutions.WaitOne();
            _partialSolutions.AddLast(solutionMessage);
            _semaphorePartialSolutions.Release();
        }

        /// <summary>
        /// Wysyła wyniki zakończonych obliczeń do serwera.
        /// </summary>
        public void SendPartialSolutions()
        {
            _semaphorePartialSolutions.WaitOne();
            while (_partialSolutions.Count != 0)
            {
                var solutionMessage = new Solutions()
                {
                    ProblemType = _partialSolutions.First.Value.ProblemType,
                    Id = _partialSolutions.First.Value.ProblemId,
                    Solutions1 = new SolutionsSolution[]
                    {
                        new SolutionsSolution()
                        {
                            TaskId = _partialSolutions.First.Value.TaskId,
                            TaskIdSpecified = true,
                            Data = _partialSolutions.First.Value.Data,
                            Type = SolutionsSolutionType.Partial
                        }
                    }
                };
                _partialSolutions.RemoveFirst();

                var response = _client.Send(solutionMessage);
                Consume(response);
            }
            _semaphorePartialSolutions.Release();
        }
    }
}
