using Autofac;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using log4net;
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
        private LinkedList<Solutions> _partialSolutions;
        private Semaphore _semaphorePartialSolutions;

        private ITaskSolversRepository _taskSolversRepository;
        private INetClient _client;
        
        public ComputationalNodeRunner()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalNodeModule>();
            var container = builder.Build();

            _partialSolutions = new LinkedList<Solutions>();
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
            Console.WriteLine("Register response ID={0}", response.Id);

            while(true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, (int)(response.Timeout/2)));
                var receivedMessage = _client.Send(new Status()
                {
                    Id = response.Id,
                });

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
            Console.WriteLine("Received message: {0}", receivedMessage.GetType().Name);
            if (receivedMessage.GetType() == typeof(SolvePartialProblems))
            {
                Console.WriteLine("Received partial problems: ID={0}", (receivedMessage as SolvePartialProblems).Id);
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
                        Data = partialProblems[i].Data,
                        Timeout = ((receivedMessage as SolvePartialProblems).SolvingTimeoutSpecified == true)? (receivedMessage as SolvePartialProblems).SolvingTimeout : 0,
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
            byte[] solution = solver.Solve(data, TimeSpan.FromMilliseconds((problem as PartialProblem).Timeout));

            if (solver.State == TaskSolver.TaskSolverState.Error)
                Console.WriteLine("An error occured during solving partial problem: ID={0}, TaskID={1}", (problem as PartialProblem).ProblemId, (problem as PartialProblem).TaskId);
            else if (solver.State == TaskSolver.TaskSolverState.Timeout)
                Console.WriteLine("Timeout occured during solving partial problem: ID={0}, TaskID={1}", (problem as PartialProblem).ProblemId, (problem as PartialProblem).TaskId);

            var solutionMessage = new Solutions()
            {
                ProblemType = (problem as PartialProblem).ProblemType,
                Id = (problem as PartialProblem).ProblemId,
                Solutions1 = new SolutionsSolution[]
                {
                    new SolutionsSolution()
                    {
                        TaskId = (problem as PartialProblem).TaskId,
                        TaskIdSpecified = true,
                        Data = Convert.ToBase64String(solution),
                        Type = SolutionsSolutionType.Partial,
                        TimeoutOccured = (solver.State == TaskSolver.TaskSolverState.Timeout)
                    }
                }
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
                Console.WriteLine("Sending partial solutions: ID={0}, TaskID={1}", _partialSolutions.First.Value.Id, _partialSolutions.First.Value.Solutions1[0].TaskId);
                var response = _client.Send(_partialSolutions.First.Value);
                _partialSolutions.RemoveFirst();
                Consume(response);
            }
            _semaphorePartialSolutions.Release();
        }
    }
}
