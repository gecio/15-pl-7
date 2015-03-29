using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class ProblemsInMemoryRepository : IQueuableTasksRepository<OrderedProblem>, IProblemsRepository
    {
        private Dictionary<ulong, OrderedProblem> _orderedProblems;
        private ulong _nextvalidId = 1;

        public ProblemsInMemoryRepository()
        {
            _orderedProblems = new Dictionary<ulong, OrderedProblem>();
        }

        public ulong Add(OrderedProblem problem)
        {
            problem.Id = _nextvalidId++;
            problem.RequestDate = DateTime.Now;
            _orderedProblems.Add(problem.Id, problem);
            return problem.Id;
        }

        public ICollection<IQueueableTask> GetQueuableTasks()
        {
            return _orderedProblems.Values.ToArray();
        }

        public void DequeueTask(IQueueableTask task)
        {
            throw new NotImplementedException();
        }
    }
}
