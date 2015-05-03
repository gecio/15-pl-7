using ComputationalCluster.CommunicationServer.Queueing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Models
{
    public class OrderedPartialProblem : QueueableTask
    {
        public ulong Id { get; set; }

        public ulong TaskId { get; set; }

        public string CommonData { get; set; }

        public string Data { get; set; }

        public ulong Timeout { get; set; }

        public ulong NodeId { get; set; }

        public bool Done { get; set; }
    }
}
