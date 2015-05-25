using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.NetModule
{
    /// <summary>
    /// Informacje o połączeniu.
    /// </summary>
    public class ConnectionInfo
    {
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
    }
}
