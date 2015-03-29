using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ComputationalCluster.CommunicationServer.Queueing;
namespace ComputationalCluster.CommunicationServer.Models
{
    public class OrderedProblem : IQueueableTask
    {
        public ulong Id { get; set; }
        public string Data { get; set; }
        public ProblemDefinition ProblemType { get; set; }
        public ulong Timeout { get; set; }

        public DateTime RequestDate { get; set; }

        public ProblemDefinition ProblemDefinition { get; set; }

        public bool IsAwaiting { get; set; }
    }
}
