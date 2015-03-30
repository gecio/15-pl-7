using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    public class ConfigProviderThreads : ConfigProvider
    {
        public int ThreadsCount
        {
            get
            {
                return Int32.Parse(ConfigurationManager.AppSettings["ThreadsCount"] ?? "1");
            }
        }
    }
}
