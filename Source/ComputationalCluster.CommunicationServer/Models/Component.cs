using ComputationalCluster.Communication.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
