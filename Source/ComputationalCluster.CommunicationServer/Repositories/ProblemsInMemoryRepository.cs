using ComputationalCluster.Common;
using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Queueing;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ProblemsInMemoryRepository : IQueuableTasksRepository<Problem>, IProblemsRepository
    {
        private Dictionary<ulong, Problem> _problems;
        private ulong _nextvalidId = 1;
        private readonly ITimeProvider _timeProvider;
        private readonly ILog _log;
        private readonly IPartialProblemsRepository _partialProblems;

        public ProblemsInMemoryRepository(ITimeProvider timeProvider, ILog log, IPartialProblemsRepository partialProblems)
        {
            _problems = new Dictionary<ulong, Problem>();
            _timeProvider = timeProvider;
            _log = log;
            _partialProblems = partialProblems;
        }

        public ulong Add(Problem problem)
        {
            if (problem.Id == 0)
            {
                problem.Id = _nextvalidId++;
            }
            else
            {
                _nextvalidId = Math.Max(_nextvalidId, problem.Id) + 1;
            }
            _problems.Add(problem.Id, problem);
            return problem.Id;
        }

        public Problem FindById(int id)
        {
            return _problems.ContainsKey((ulong) id) ? _problems[(ulong) id] : null;
        }

        public void Update(Problem problem)
        {
            //TODO: logi
        }

        public ICollection<IQueueableTask> GetQueuableTasks()
        {
            return _problems.Values
                .Where(t => t.ProblemDefinition.AvailableTaskManagers > 0)
                .ToArray();
        }

        public void StopSolvingTimedOutProblems()
        {
            var timedOutProblems = _problems.Values
                .Where(p => !p.IsDone && p.AssignedTo == null && !_partialProblems.IsProblemComputed(p.Id))
                .Where(p => p.Timeout != 0)
                .Where(p => p.RequestDate < _timeProvider.Now.Subtract(new TimeSpan(0, 0, 0, 0, (int)p.Timeout)))
                .ToList();

            foreach (var problem in timedOutProblems)
            {
                _log.InfoFormat("Problem timed out (Problem ID={0}).", problem.Id);
                problem.IsDone = true;
                problem.IsAwaiting = false;
                problem.TimeoutOccured = true;
                problem.ComputationsTime = problem.Timeout;
                _partialProblems.RemoveFinishedProblems(problem.Id);
            }
        }
    }
}
