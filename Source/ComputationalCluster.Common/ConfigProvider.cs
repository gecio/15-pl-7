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
        private static readonly int DefaultPort = 16987;

        private int? _portCache = null;
        public int Port
        {
            get
            {
                if (_portCache != null)
                {
                    var value = ConfigurationManager.AppSettings["Port"];
                    if (value != null)
                    {
                        _portCache = Int32.Parse(value);
                    }
                }
                return _portCache ?? DefaultPort;
            }
        }

        public IPAddress IP
        {
            get
            {
                return IPAddress.Parse(ConfigurationManager.AppSettings["IP"] ?? "127.0.0.1");
            }
        }
    }
}
