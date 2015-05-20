using Autofac;
using ComputationalCluster.Common;
using ComputationalCluster.Communication.Messages;
using ComputationalCluster.CommunicationServer.Repositories;
using ComputationalCluster.NetModule;
using ComputationalCluster.PluginManager;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCCTaskSolver;

namespace ComputationalCluster.TaskManager
{
    public class TaskManagerRunner
    {
        private ITaskSolversRepository _taskSolversRepository;
        private INetClient _client;

        private LinkedList<SolvePartialProblems> _parialProblems;
        private Semaphore _semaphorePartialProblems;
        private LinkedList<Solutions> _finalSolutions;
        private Semaphore _semaphoreFinalSolutions;
        private LinkedList<Error> _errors;

        private ulong _componentId;
        private uint _timeout;

        private ConfigProviderThreads _configProvider;
        private int _numberOfThreads;
        private int _numberOfBusyThreads;

        public TaskManagerRunner(string[] args)
        {
            BasicConfigurator.Configure();

            var builder = new ContainerBuilder();
            builder.RegisterModule<TaskManagerModule>();
            var container = builder.Build();

            var configurator = container.Resolve<ClientConfigurator>();
            configurator.Apply(args);

            _taskSolversRepository = container.Resolve<ITaskSolversRepository>();
            _client = container.Resolve<INetClient>();

            _parialProblems = new LinkedList<SolvePartialProblems>();
            _semaphorePartialProblems = new Semaphore(1, 1);
            _finalSolutions = new LinkedList<Solutions>();
            _semaphoreFinalSolutions = new Semaphore(1, 1);
            _errors = new LinkedList<Error>();

            _configProvider = container.Resolve<ConfigProviderThreads>();
        }

        public void Start()
        {
            _numberOfThreads = (_configProvider as ConfigProviderThreads).ThreadsCount;
            _numberOfBusyThreads = 0;

            SendRegisterMessage();

            while (true)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, (int)(_timeout / 2)));

                var threads = new StatusThread[_numberOfThreads];
                for (int i=0; i<_numberOfBusyThreads; i++)
                    threads[i] = new StatusThread()
                    {
                        State = StatusThreadState.Busy
                    };
                for (int i=_numberOfBusyThreads; i<_numberOfThreads; i++)
                    threads[i] = new StatusThread()
                    {
                        State = StatusThreadState.Idle
                    };
                try
                {
                    var receivedMessages = _client.Send_ManyResponses(new Status()
                    {
                        Id = _componentId,
                        Threads = threads
                    });

                    foreach (var receivedMessage in receivedMessages)
                    {
                        Consume(receivedMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Server unavailable...");
                    return;
                }
                SendPartialProblems();
                SendSolution();
                SendErrorMessages();
            }
        }

        public void Stop()
        {
        }

        public void SendRegisterMessage()
        {
            var response = _client.Send(new Register()
            {
                Type = RegisterType.TaskManager,
                ParallelThreads = (byte)_numberOfThreads,
                SolvableProblems = _taskSolversRepository.GetSolversNames().ToArray(),
            }) as RegisterResponse;
            Console.WriteLine("Register response: ID={0}", _componentId);

            _componentId = response.Id;
            _timeout = response.Timeout;
        }

        public void Consume(IMessage receivedMessage)
        {
            Console.WriteLine("Received message: {0}", receivedMessage.GetType().Name);
            if (receivedMessage.GetType() == typeof(DivideProblem))
            {
                Console.WriteLine("Received a problem to divide: ID={0}", (receivedMessage as DivideProblem).Id);
                Thread thread = new Thread(new ParameterizedThreadStart(Divide));
                _numberOfBusyThreads++;
                thread.Start(receivedMessage);
            }
            else if (receivedMessage.GetType() == typeof(Solutions))
            {
                Console.WriteLine("Received partial problems to merge: ID={0}", (receivedMessage as Solutions).Id);
                Thread thread = new Thread(new ParameterizedThreadStart(Merge));
                _numberOfBusyThreads++;
                thread.Start(receivedMessage);
            }
            else if (receivedMessage.GetType() == typeof(Error))
            {
                Console.WriteLine("Error: type={0}, message={1}", (receivedMessage as Error).ErrorType, (receivedMessage as Error).ErrorMessage);
                if ((receivedMessage as Error).ErrorType == ErrorErrorType.UnknownSender)
                    SendRegisterMessage();
            }
        }

        public void Divide(object problem)
        {
            Type solverType = _taskSolversRepository.GetSolverType((problem as DivideProblem).ProblemType);
            TaskSolver solver = (TaskSolver)Activator.CreateInstance(solverType, Convert.FromBase64String((problem as DivideProblem).Data));
            byte[][] partialProblems = solver.DivideProblem((int)(problem as DivideProblem).ComputationalNodes);

            if (solver.State == TaskSolver.TaskSolverState.Error)
            {
                Console.WriteLine("An error occured during dividing problem: ID={0}", (problem as DivideProblem).Id);
                _errors.AddLast(new Error()
                {
                    ErrorType = ErrorErrorType.ExceptionOccured,
                    ErrorMessage = "Error during dividing problem with Id="+(problem as DivideProblem).Id,
                });
                return;
            }
            else if (solver.State == TaskSolver.TaskSolverState.Timeout)
                Console.WriteLine("Timeout occured during dividing problem: ID={0}", (problem as DivideProblem).Id);

            var partialProblemsMessage = new SolvePartialProblems()
            {
                CommonData = (problem as DivideProblem).Data,
                ProblemType = (problem as DivideProblem).ProblemType,
                Id = (problem as DivideProblem).Id,
                PartialProblems = new SolvePartialProblemsPartialProblem[partialProblems.Length],
            };
            for (int i=0; i<partialProblems.Length; i++)
            {
                partialProblemsMessage.PartialProblems[i] = new SolvePartialProblemsPartialProblem()
                {
                    Data = Convert.ToBase64String(partialProblems[i]),
                    TaskId = (ulong)(i+1),
                };
            }

            _semaphorePartialProblems.WaitOne();
            _parialProblems.AddLast(partialProblemsMessage);
            _semaphorePartialProblems.Release();
            _numberOfBusyThreads--;
        }

        public void SendPartialProblems()
        {
            _semaphorePartialProblems.WaitOne();
            while (_parialProblems.Count != 0)
            {
                Console.WriteLine("Sending partial problems: ID={0}, number of partial problems={1}", _parialProblems.First.Value.Id, _parialProblems.First.Value.PartialProblems.Count());
                var response = _client.Send(_parialProblems.First.Value);
                _parialProblems.RemoveFirst();
                Consume(response);
            }
            _semaphorePartialProblems.Release();
        }

        public void Merge(object problems)
        {
            var solver = _taskSolversRepository.GetSolverInstance((problems as Solutions).ProblemType);
            //TaskSolver solver = (TaskSolver)Activator.CreateInstance(solverType);
            
            byte[][] partialSolutions = new byte[(problems as Solutions).Solutions1.Length][];
            for (int i = 0; i < partialSolutions.Length; i++)
                partialSolutions[i] = Convert.FromBase64String((problems as Solutions).Solutions1[i].Data);
                
            byte[] solution = solver.MergeSolution(partialSolutions);

            if (solver.State == TaskSolver.TaskSolverState.Error)
            {
                Console.WriteLine("An error occured during merging partial solutions: ID={0}", (problems as Solutions).Id);
                _errors.AddLast(new Error()
                {
                    ErrorType = ErrorErrorType.ExceptionOccured,
                    ErrorMessage = "Error during merging partial solutions with ProblemId="+(problems as Solutions).Id,
                });
                return;
            }
            else if (solver.State == TaskSolver.TaskSolverState.Timeout)
                Console.WriteLine("Timeout occured during merging partial solutions: ID={0}", (problems as Solutions).Id);

            var solutionMessage = new Solutions()
            {
                ProblemType = (problems as Solutions).ProblemType,
                Id = (problems as Solutions).Id,
                Solutions1 = new SolutionsSolution[]
                { 
                    new SolutionsSolution()
                    {
                        TaskIdSpecified = false,
                        Data = Convert.ToBase64String(solution),
                        Type = SolutionsSolutionType.Final
                    }
                }
            };

            _semaphoreFinalSolutions.WaitOne();
            _finalSolutions.AddLast(solutionMessage);
            _semaphoreFinalSolutions.Release();
            _numberOfBusyThreads--;
        }

        public void SendSolution()
        {
            _semaphoreFinalSolutions.WaitOne();
            while (_finalSolutions.Count != 0)
            {
                Console.WriteLine("Sending final solution: ID={0}", _finalSolutions.First.Value.Id);
                
                var response = _client.Send(_finalSolutions.First.Value);
                _finalSolutions.RemoveFirst();
                Consume(response);
            }
            _semaphoreFinalSolutions.Release();
        }

        /// <summary>
        /// Wysyła ewentualne informacje o błędach, które wystąpiły w czasie obliczeń
        /// </summary>
        public void SendErrorMessages()
        {
            while (_errors.Count != 0)
            {
                Console.WriteLine("Sending error message: type={0} message={1}", _errors.First.Value.ErrorType, _errors.First.Value.ErrorMessage);
                var response = _client.Send(_errors.First.Value);
                _errors.RemoveFirst();
                Consume(response);
            }
        }
    }
}
