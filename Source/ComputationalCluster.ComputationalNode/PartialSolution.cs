using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.ComputationalNode
{
    public class PartialSolution
    {
        public string ProblemType { get; set; }
        public ulong ProblemId { get; set; }
        public ulong TaskId { get; set; }
        public string Data { get; set; }
    }
}
