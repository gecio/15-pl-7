using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    public class ConfigProvider : IConfigProvider
    {
        public int Port
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["Port"]);
            }
        }

        public IPAddress IP
        {
            get
            {
                return IPAddress.Parse(ConfigurationManager.AppSettings["IP"]);
            }
        }
    }
}
