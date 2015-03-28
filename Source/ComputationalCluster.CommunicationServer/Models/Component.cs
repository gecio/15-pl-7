using ComputationalCluster.Communication.Messages;
using System;
using System.Collections.Generic;

namespace ComputationalCluster.CommunicationServer.Models
{
    public class Component
    {
        public Component()
        {
        }

        public ulong Id { get; set; }
        public DateTime LastStatusTimestamp { get; set; }
        public RegisterType Type { get; set; }

        public ICollection<ProblemDefinition> SolvableProblems { get; set; }
    }
}
