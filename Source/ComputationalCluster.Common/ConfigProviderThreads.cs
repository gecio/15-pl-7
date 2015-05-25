using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    /// <summary>
    /// Implementacja konfiguracji dla węzłów i managerów zadań.
    /// </summary>
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
