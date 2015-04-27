using ComputationalCluster.CommunicationServer.Models;
using ComputationalCluster.CommunicationServer.Queueing;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ComputationalCluster.CommunicationServer.Repositories
{
    public class PartialProblemsInMemoryRepository : IQueuableTasksRepository<OrderedPartialProblem>, IPartialProblemsRepository
    {
        private List<OrderedPartialProblem> _orderedPartialProblems;

        public PartialProblemsInMemoryRepository(IProblemDefinitionsRepository repository)
        {
            _orderedPartialProblems = new List<OrderedPartialProblem>();
        }

        public ulong Add(OrderedPartialProblem problem)
        {
            problem.RequestDate = DateTime.Now;
            _orderedPartialProblems.Add(problem);
            return problem.TaskId;
        }


        public ICollection<OrderedPartialProblem> GetFinishedProblem(ICollection<ProblemDefinition> problemDefinitions)
        {
            var res2 =
                _orderedPartialProblems.Where(tmp => problemDefinitions.Contains(tmp.ProblemDefinition))
                    .OrderBy(tmp => tmp.RequestDate)
                    .GroupBy(tmp => tmp.Id)
                    .Select(g => g);
            
            IEnumerable<IGrouping<ulong, OrderedPartialProblem>> res3;
            res3 = res2.Where(a => a.All(x => x.Done && x.AssignedTo == null));

            IGrouping<ulong, OrderedPartialProblem> first = res3.FirstOrDefault();
            return first != null ? first.ToArray() : null;
        }

        public ICollection<IQueueableTask> GetQueuableTasks()
        {
            return _orderedPartialProblems
                .Where(t => t.ProblemDefinition.AvailableComputationalNodes > 0)
                .ToArray();
        }


        public void RemoveFinishedProblems(ulong problemId)
        {

            foreach (var partialProblem in _orderedPartialProblems.Where(t => t.Id == problemId))
            {
                partialProblem.AssignedTo = null;
            }
            _orderedPartialProblems = _orderedPartialProblems
                .Where(t => t.Id != problemId).ToList();
        }

        public OrderedPartialProblem Find(ulong Problemid, ulong TaskId)
        {
            return _orderedPartialProblems.FirstOrDefault(item => item.Id == Problemid && item.TaskId == TaskId);
        }
    }
}
