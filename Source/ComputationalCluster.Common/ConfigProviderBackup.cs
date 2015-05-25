using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalCluster.Common
{
    /// <summary>
    /// Uzupełnienie konfiguracji dla trybu backup.
    /// </summary>
    public interface IConfigProviderBackup : IConfigProvider
    {
        IPAddress MasterIP { get; set; }
        int MasterPort { get; set; }
    }

    /// <summary>
    /// Implementacja konfiguracji dla trybu backup.
    /// </summary>
    public class ConfigProviderBackup : ConfigProvider, IConfigProviderBackup
    {
        public IPAddress MasterIP { get; set; }
        public int MasterPort { get; set; }
    }
}
