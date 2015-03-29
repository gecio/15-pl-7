using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Queueing;
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

        public ProblemsInMemoryRepository(IProblemDefinitionsRepository problemDefinitionsRepository)
        {
            _problems = new Dictionary<ulong, Problem>();
        }

        public ulong Add(Problem problem)
        {
            problem.Id = _nextvalidId++;
            problem.RequestDate = DateTime.Now;
            _problems.Add(problem.Id, problem);
            return problem.Id;
        }

        public Problem FindById(int id)
        {
            return _problems.ContainsKey((ulong) id) ? _problems[(ulong) id] : null;
        }

        public ICollection<IQueueableTask> GetQueuableTasks()
        {
            return _problems.Values.ToArray();
        }

        public void DequeueTask(IQueueableTask task)
        {
            return;
        }
    }
}
