using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    public interface IConfigProvider
    {
        int Port { get; set; }
        IPAddress IP { get; set; }
        bool BackupMode { get; set; }
        int Timeout { get; set; }
    }
}
