using ComputationalCluster.CommunicationServer.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Queueing
{
    public interface ITaskQueue<T> where T : class, IQueueableTask
    {
        T GetNextTask(); 
    }

    /// <summary>
    /// Kolejka zadań danego typu.
    /// </summary>
    /// <typeparam name="T">Typ zadania</typeparam>
    public class TaskQueue<T> where T : class, IQueueableTask
    {
        public delegate int AvailabilityChecker(ProblemDefinition problemDefinition);

        private readonly IQueuableTasksRepository<T> _queuableTasksRepository;
        //private readonly AvailabilityChecker _availabilityChecker;
        private readonly ILog _log;

        /// <summary
        /// </summary>
        /// <param name="queuableTasksRepository">
        /// Obsługiwane repozytorium zadań.
        /// </param>
        /// <param name="availabilityChecker">
        /// Wyrażenie zwracające ilość zasobów obsługujących dany typ zadania.
        /// </param>
        /// <param name="log">Log</param>
        public TaskQueue(IQueuableTasksRepository<T> queuableTasksRepository, 
            ILog log)
        {
            _queuableTasksRepository = queuableTasksRepository;
            //_availabilityChecker     = availabilityChecker;
            _log                     = log;
        }

        public T GetNextTask()
        {
            T task =  (T) _queuableTasksRepository.GetQueuableTasks()
                .Where(t => t.IsAwaiting)
                //.Where(t => _availabilityChecker(t.ProblemDefinition) > 0)
                .OrderBy(t => t.RequestDate).FirstOrDefault();

            if (task == null)
            {
                return null;
            }

            _queuableTasksRepository.DequeueTask(task);

            return task;
        }

        public T GetNextTask(ICollection<ProblemDefinition> problemDefinitions)
        {
            T task = (T)_queuableTasksRepository.GetQueuableTasks()
                .Where(t => t.IsAwaiting)
                .Where(t => problemDefinitions.Contains(t.ProblemDefinition))
                //.Where(t => _availabilityChecker(t.ProblemDefinition) > 0)
                .OrderBy(t => t.RequestDate).FirstOrDefault();

            if (task == null)
            {
                return null;
            }

            _queuableTasksRepository.DequeueTask(task);

            return task;
        }
    }
}
