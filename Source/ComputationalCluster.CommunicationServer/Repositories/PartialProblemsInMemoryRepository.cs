using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class PartialProblemsInMemoryRepository : IQueuableTasksRepository<OrderedPartialProblem>, IPartialProblemsRepository
    {
        private Dictionary<ulong, OrderedPartialProblem> _orderedPartialProblems;
        private ulong _nextValidId = 1;

        public PartialProblemsInMemoryRepository(IProblemDefinitionsRepository repository)
        {
            _orderedPartialProblems = new Dictionary<ulong, OrderedPartialProblem>();
        }

        public ulong Add(OrderedPartialProblem problem)
        {
            problem.Id = _nextValidId++;
            problem.RequestDate = DateTime.Now;
            _orderedPartialProblems.Add(problem.Id, problem);
            return problem.Id;
        }

        public ICollection<IQueueableTask> GetQueuableTasks()
        {
            return _orderedPartialProblems.Values.ToArray();
        }

        public void DequeueTask(IQueueableTask task)
        {
            task.IsAwaiting = false;
        }

    }
}
