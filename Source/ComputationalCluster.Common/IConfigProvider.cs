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
        int Port { get; }
        IPAddress IP { get; }
    }
}
