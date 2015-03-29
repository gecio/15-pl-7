using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ComputationalCluster.CommunicationServer.Queueing;
namespace ComputationalCluster.CommunicationServer.Models
{
    public class Problem : IQueueableTask
    {
        public ulong Id { get; set; }
        public string InputData { get; set; }
        public string OutputData { get; set; }
        public ProblemDefinition ProblemType { get; set; }
        public ulong Timeout { get; set; }

        public DateTime RequestDate { get; set; }

        public ProblemDefinition ProblemDefinition { get; set; }

        public bool IsAwaiting { get; set; }
        public bool IsDone { get; set; }

    }
}
