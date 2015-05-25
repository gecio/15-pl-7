using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.CommunicationServer.Models
{
    public class BackupComponent : Component
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
    }
}
