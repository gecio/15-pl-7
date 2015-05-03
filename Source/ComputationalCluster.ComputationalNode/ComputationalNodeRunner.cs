using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using log4net;
using log4net.Config;
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
        private ConfigProviderThreads _configProvider;

        private int _numberOfThreads;
        private int _numberOfBusyThreads;

        public ComputationalNodeRunner(string[] args)
        {
            // log4net configuration - log on console
            BasicConfigurator.Configure();

            var builder = new ContainerBuilder();
            builder.RegisterModule<ComputationalNodeModule>();
            var container = builder.Build();

            var configurator = container.Resolve<ClientConfigurator>();
            configurator.Apply(args);

            _partialSolutions = new LinkedList<Solutions>();
            _semaphorePartialSolutions = new Semaphore(1, 1);

            _taskSolversRepository = container.Resolve<ITaskSolversRepository>();
            _client = container.Resolve<INetClient>();
            _configProvider = container.Resolve<ConfigProviderThreads>();
        }

        public void Start()
        {
            _numberOfThreads = (_configProvider as ConfigProviderThreads).ThreadsCount;
            _numberOfBusyThreads = 0;

            var response = _client.Send(new Register()
            {
                Type = RegisterType.ComputationalNode,
                ParallelThreads = (byte)(_configProvider as ConfigProviderThreads).ThreadsCount,
                SolvableProblems = _taskSolversRepository.GetSolversNames().ToArray(),
            }) as RegisterResponse;
            Console.WriteLine("Register response ID={0}", response.Id);

            while (true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, (int)(response.Timeout / 2)));

                var threads = new StatusThread[_numberOfThreads];
                for (int i = 0; i < _numberOfBusyThreads; i++)
                    threads[i] = new StatusThread()
                    {
                        State = StatusThreadState.Busy
                    };
                for (int i = _numberOfBusyThreads; i < _numberOfThreads; i++)
                    threads[i] = new StatusThread()
                    {
                        State = StatusThreadState.Idle
                    };
                try
                {
                    var receivedMessages = _client.Send_ManyResponses(new Status()
                    {
                        Id = response.Id,
                        Threads = threads
                    });

                    foreach (var receivedMessage in receivedMessages)
                    {
                        Consume(receivedMessage);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Server unavailable...");
                    return;
                }
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
                var received = receivedMessage as SolvePartialProblems;
                Console.WriteLine("Received partial problems: ID={0}", received.Id);
                var partialProblems = received.PartialProblems;
                for (int i = 0; i < partialProblems.Length; i++)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(SolvePartialProblem));
                    PartialProblem task = new PartialProblem
                    {
                        ProblemType = received.ProblemType,
                        ProblemId = received.Id,
                        CommonData = received.CommonData,
                        TaskId = partialProblems[i].TaskId,
                        Data = partialProblems[i].Data,
                        Timeout = (received.SolvingTimeoutSpecified == true) ? received.SolvingTimeout : 0,
                    };
                    _numberOfBusyThreads++;
                    thread.Start(task);
                }
            }
            else if (receivedMessage.GetType() == typeof(Error))
            {
                Console.WriteLine("Error: type={0}, message={1}", (receivedMessage as Error).ErrorType, (receivedMessage as Error).ErrorMessage);
            }
        }

        /// <summary>
        /// Rozwiązuje pojedynczy podproblem i wstawia rozwiązanie do kolejki do wysłania.
        /// </summary>
        /// <param name="problem">informacje o podproblemie do rozwiązania</param>
        public void SolvePartialProblem(object problem)
        {
            var partialProblem = problem as PartialProblem;
            TaskSolver solver = _taskSolversRepository.GetSolverInstance(partialProblem.ProblemType);
            byte[] data = Convert.FromBase64String(partialProblem.Data);
            byte[] solution = solver.Solve(data, TimeSpan.FromMilliseconds(partialProblem.Timeout));

            if (solver.State == TaskSolver.TaskSolverState.Error)
                Console.WriteLine("An error occured during solving partial problem: ID={0}, TaskID={1}", partialProblem.ProblemId, partialProblem.TaskId);
            else if (solver.State == TaskSolver.TaskSolverState.Timeout)
                Console.WriteLine("Timeout occured during solving partial problem: ID={0}, TaskID={1}", partialProblem.ProblemId, partialProblem.TaskId);

            var solutionMessage = new Solutions()
            {
                ProblemType = partialProblem.ProblemType,
                Id = partialProblem.ProblemId,
                Solutions1 = new []
                {
                    new SolutionsSolution()
                    {
                        TaskId = partialProblem.TaskId,
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
            _numberOfBusyThreads--;
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
